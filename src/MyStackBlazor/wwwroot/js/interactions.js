// ----------------------------------------------------------------
//  Small browser helpers shared by several components:
//  breakpoints, hotkeys, scroll-to-top, swipe, table of contents
//  (scroll spy).
//  Every "watch" returns an id; call the matching dispose with it.
// ----------------------------------------------------------------

const disposers = new Map();
let nextId = 1;

function register(dispose) {
    const id = nextId++;
    disposers.set(id, dispose);
    return id;
}

export function dispose(id) {
    const d = disposers.get(id);
    if (d) { d(); disposers.delete(id); }
}

// ── Breakpoints ─────────────────────────────────────────────────────
// Tailwind defaults. Reports the largest breakpoint whose min-width matches ("xs" below sm).
const BREAKPOINTS = [['sm', 640], ['md', 768], ['lg', 1024], ['xl', 1280], ['2xl', 1536]];

function currentBreakpoint() {
    let bp = 'xs';
    for (const [name, px] of BREAKPOINTS) {
        if (window.matchMedia(`(min-width: ${px}px)`).matches) bp = name;
    }
    return bp;
}

export function watchBreakpoint(dotnetRef) {
    const queries = BREAKPOINTS.map(([, px]) => window.matchMedia(`(min-width: ${px}px)`));
    let last = currentBreakpoint();
    const onChange = () => {
        const bp = currentBreakpoint();
        if (bp === last) return;
        last = bp;
        dotnetRef.invokeMethodAsync('OnBreakpointChanged', bp).catch(() => {});
    };
    queries.forEach(q => q.addEventListener('change', onChange));
    return { id: register(() => queries.forEach(q => q.removeEventListener('change', onChange))), breakpoint: last };
}

// ── Hotkeys ─────────────────────────────────────────────────────────
// Combos like "ctrl+k", "mod+s" (Cmd on Mac, Ctrl elsewhere), "shift+?", "escape", or sequences "g i".
const isMac = /Mac|iPhone|iPad/.test(navigator.platform || navigator.userAgent);

function normalizeKey(k) {
    const map = { esc: 'escape', del: 'delete', space: ' ', plus: '+', up: 'arrowup', down: 'arrowdown', left: 'arrowleft', right: 'arrowright' };
    k = k.toLowerCase();
    return map[k] || k;
}

function parseCombo(combo) {
    const parts = combo.toLowerCase().split('+').map(p => p.trim()).filter(Boolean);
    const c = { ctrl: false, alt: false, shift: false, meta: false, key: '' };
    for (const p of parts) {
        if (p === 'ctrl' || p === 'control') c.ctrl = true;
        else if (p === 'alt' || p === 'option') c.alt = true;
        else if (p === 'shift') c.shift = true;
        else if (p === 'meta' || p === 'cmd' || p === 'win') c.meta = true;
        else if (p === 'mod') { if (isMac) c.meta = true; else c.ctrl = true; }
        else c.key = normalizeKey(p);
    }
    return c;
}

function matches(e, c) {
    const key = normalizeKey(e.key || '');
    // For printable symbols like "?" the browser already applied Shift, so don't require the flag to match.
    const shiftOk = c.key.length === 1 && !/[a-z0-9]/.test(c.key) ? true : e.shiftKey === c.shift;
    return key === c.key && e.ctrlKey === c.ctrl && e.altKey === c.alt && e.metaKey === c.meta && shiftOk;
}

function isTyping(target) {
    if (!target) return false;
    const tag = target.tagName;
    return target.isContentEditable || tag === 'INPUT' || tag === 'TEXTAREA' || tag === 'SELECT';
}

export function registerHotkey(dotnetRef, keys, opts) {
    const o = opts || {};
    const steps = keys.trim().split(/\s+/).map(parseCombo); // "g i" → two steps
    let progress = 0;
    let timer = 0;

    const handler = e => {
        if (e.repeat) return;
        if (!o.allowInInputs && isTyping(e.target)) return;
        if (matches(e, steps[progress])) {
            progress++;
            clearTimeout(timer);
            if (progress === steps.length) {
                progress = 0;
                if (o.preventDefault !== false) e.preventDefault();
                dotnetRef.invokeMethodAsync('OnHotkey').catch(() => {});
            } else {
                timer = setTimeout(() => { progress = 0; }, 1000);
            }
        } else if (!['shift', 'control', 'alt', 'meta'].includes(normalizeKey(e.key || ''))) {
            progress = matches(e, steps[0]) ? 1 : 0;
        }
    };
    document.addEventListener('keydown', handler);
    return register(() => { document.removeEventListener('keydown', handler); clearTimeout(timer); });
}

// ── Scroll to top ───────────────────────────────────────────────────
function scrollerOf(selector) {
    return selector ? document.querySelector(selector) : null;
}

export function watchScroll(dotnetRef, selector, threshold) {
    const el = scrollerOf(selector);
    const target = el || window;
    const pos = () => el ? el.scrollTop : (window.scrollY || document.documentElement.scrollTop);
    let visible = null;
    const onScroll = () => {
        const v = pos() > threshold;
        if (v === visible) return;
        visible = v;
        dotnetRef.invokeMethodAsync('OnVisibilityChanged', v).catch(() => {});
    };
    target.addEventListener('scroll', onScroll, { passive: true });
    onScroll();
    return register(() => target.removeEventListener('scroll', onScroll));
}

export function scrollToTop(selector, smooth) {
    const reduce = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    const behavior = smooth && !reduce ? 'smooth' : 'auto';
    const el = scrollerOf(selector);
    (el || window).scrollTo({ top: 0, behavior });
}

// ── Swipe ───────────────────────────────────────────────────────────
export function watchSwipe(element, dotnetRef, opts) {
    const o = Object.assign({ threshold: 50, maxTime: 600 }, opts || {});
    let sx = 0, sy = 0, st = 0, tracking = false;

    const start = (x, y) => { sx = x; sy = y; st = Date.now(); tracking = true; };
    const end = (x, y) => {
        if (!tracking) return;
        tracking = false;
        const dx = x - sx, dy = y - sy, dt = Date.now() - st;
        if (dt > o.maxTime) return;
        const ax = Math.abs(dx), ay = Math.abs(dy);
        if (Math.max(ax, ay) < o.threshold) return;
        const dir = ax > ay ? (dx > 0 ? 'Right' : 'Left') : (dy > 0 ? 'Down' : 'Up');
        dotnetRef.invokeMethodAsync('HandleSwipe', dir, ax > ay ? dx : dy).catch(() => {});
    };

    const ts = e => { if (e.touches.length === 1) start(e.touches[0].clientX, e.touches[0].clientY); };
    const te = e => { const t = e.changedTouches[0]; if (t) end(t.clientX, t.clientY); };
    // Pointer events cover mouse/pen; touch is handled above so a finger doesn't fire twice.
    const pd = e => { if (e.pointerType !== 'touch') start(e.clientX, e.clientY); };
    const pu = e => { if (e.pointerType !== 'touch') end(e.clientX, e.clientY); };

    element.addEventListener('touchstart', ts, { passive: true });
    element.addEventListener('touchend', te, { passive: true });
    if (o.mouse) {
        element.addEventListener('pointerdown', pd);
        element.addEventListener('pointerup', pu);
    }
    return register(() => {
        element.removeEventListener('touchstart', ts);
        element.removeEventListener('touchend', te);
        element.removeEventListener('pointerdown', pd);
        element.removeEventListener('pointerup', pu);
    });
}

// ── Table of contents / scroll spy ──────────────────────────────────
export function scanHeadings(selector) {
    const root = document.querySelector(selector || 'main') || document.body;
    const heads = [...root.querySelectorAll('h2, h3, h4')].filter(h => !h.closest('[data-toc-ignore]'));
    let n = 0;
    return heads.map(h => {
        if (!h.id) {
            const slug = (h.textContent || '').trim().toLowerCase().replace(/[^\w]+/g, '-').replace(/^-|-$/g, '') || 'section';
            let id = slug;
            while (document.getElementById(id)) id = `${slug}-${++n}`;
            h.id = id;
        }
        return { id: h.id, text: (h.textContent || '').trim(), level: Number(h.tagName.substring(1)) };
    });
}

export function watchSections(dotnetRef, ids, offset) {
    const els = ids.map(id => document.getElementById(id)).filter(Boolean);
    if (!els.length || !('IntersectionObserver' in window)) return 0;
    const visible = new Set();
    let active = null;
    const io = new IntersectionObserver(entries => {
        for (const e of entries) {
            if (e.isIntersecting) visible.add(e.target.id); else visible.delete(e.target.id);
        }
        // Active = first heading (in document order) currently in the top part of the viewport.
        const first = els.find(el => visible.has(el.id));
        const next = first ? first.id : active;
        if (next && next !== active) {
            active = next;
            dotnetRef.invokeMethodAsync('OnActiveSection', active).catch(() => {});
        }
    }, { rootMargin: `-${offset || 0}px 0px -60% 0px` });
    els.forEach(el => io.observe(el));
    return register(() => io.disconnect());
}

export function scrollToId(id, offset, smooth) {
    const el = document.getElementById(id);
    if (!el) return;
    const reduce = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    const top = el.getBoundingClientRect().top + window.scrollY - (offset || 0);
    window.scrollTo({ top, behavior: smooth && !reduce ? 'smooth' : 'auto' });
    history.replaceState(null, '', '#' + id);
}
