// ----------------------------------------------------------------
//  Count-up — animates a number in an element from one value to
//  another, then shows the exact .NET-formatted final text.
// ----------------------------------------------------------------

const running = new WeakMap();

export function animate(element, from, to, durationMs, decimals, locale, prefix, suffix, finalText) {
    if (!element) return;
    cancel(element);

    const reduce = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    if (reduce || durationMs <= 0 || from === to) {
        element.textContent = finalText;
        return;
    }

    let fmt;
    try {
        fmt = new Intl.NumberFormat(locale || undefined, {
            minimumFractionDigits: decimals,
            maximumFractionDigits: decimals,
        });
    } catch {
        fmt = new Intl.NumberFormat(undefined, { minimumFractionDigits: decimals, maximumFractionDigits: decimals });
    }

    const start = performance.now();
    const easeOutCubic = t => 1 - Math.pow(1 - t, 3);

    const step = now => {
        const t = Math.min(1, (now - start) / durationMs);
        if (t >= 1) {
            element.textContent = finalText;
            running.delete(element);
            return;
        }
        const v = from + (to - from) * easeOutCubic(t);
        element.textContent = prefix + fmt.format(v) + suffix;
        running.set(element, requestAnimationFrame(step));
    };
    running.set(element, requestAnimationFrame(step));
}

export function cancel(element) {
    const id = element && running.get(element);
    if (id) cancelAnimationFrame(id);
    if (element) running.delete(element);
}
