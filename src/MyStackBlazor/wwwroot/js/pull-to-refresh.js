// ----------------------------------------------------------------
//  Pull-to-refresh — touch gesture on a container. Only starts when
//  the container (and the page) is scrolled to the very top. Calls
//  .NET `OnPulledAsync` when released past the threshold.
// ----------------------------------------------------------------

const instances = new WeakMap();

function scrolledToTop(el) {
    if (el.scrollTop > 0) return false;
    // Any scrollable ancestor (or the page) that isn't at the top blocks the gesture.
    for (let p = el.parentElement; p; p = p.parentElement) {
        if (p.scrollTop > 0) return false;
    }
    return (window.scrollY || document.documentElement.scrollTop || 0) <= 0;
}

export function init(root, indicator, label, dotnetRef, opts) {
    if (!root || instances.has(root)) return;
    const o = Object.assign({ threshold: 70, max: 120, resistance: 0.5 }, opts || {});
    const state = { startY: 0, pull: 0, active: false, refreshing: false, disabled: !!o.disabled };

    const setPull = (px, animate) => {
        state.pull = px;
        indicator.style.transition = animate ? 'height .2s ease' : 'none';
        indicator.style.height = px + 'px';
        const ready = px >= o.threshold;
        root.dataset.ptrState = state.refreshing ? 'refreshing' : (px <= 0 ? 'idle' : (ready ? 'ready' : 'pulling'));
        if (label) {
            label.textContent = state.refreshing ? o.refreshingText : (ready ? o.releaseText : o.pullText);
        }
        const icon = indicator.querySelector('[data-ptr-icon]');
        if (icon) icon.style.transform = state.refreshing ? '' : `rotate(${Math.min(1, px / o.threshold) * 180}deg)`;
    };

    const onStart = e => {
        if (state.disabled || state.refreshing || e.touches.length !== 1) return;
        if (!scrolledToTop(root)) return;
        state.startY = e.touches[0].clientY;
        state.active = true;
    };

    const onMove = e => {
        if (!state.active) return;
        const dy = e.touches[0].clientY - state.startY;
        if (dy <= 0) { setPull(0, false); return; }
        if (e.cancelable) e.preventDefault(); // stop native overscroll/bounce while pulling
        setPull(Math.min(o.max, dy * o.resistance), false);
    };

    const onEnd = async () => {
        if (!state.active) return;
        state.active = false;
        if (state.pull < o.threshold) { setPull(0, true); return; }
        state.refreshing = true;
        setPull(o.threshold, true);
        try { await dotnetRef.invokeMethodAsync('OnPulledAsync'); }
        catch { /* circuit gone or handler threw — just reset */ }
        finally {
            state.refreshing = false;
            setPull(0, true);
        }
    };

    root.addEventListener('touchstart', onStart, { passive: true });
    root.addEventListener('touchmove', onMove, { passive: false });
    root.addEventListener('touchend', onEnd, { passive: true });
    root.addEventListener('touchcancel', onEnd, { passive: true });

    instances.set(root, { state, setPull, dispose: () => {
        root.removeEventListener('touchstart', onStart);
        root.removeEventListener('touchmove', onMove);
        root.removeEventListener('touchend', onEnd);
        root.removeEventListener('touchcancel', onEnd);
    }});
    setPull(0, false);
}

export function setDisabled(root, disabled) {
    const inst = root && instances.get(root);
    if (inst) inst.state.disabled = !!disabled;
}

export function dispose(root) {
    const inst = root && instances.get(root);
    if (inst) { inst.dispose(); instances.delete(root); }
}
