// Focusable selectors per WCAG 2.2
const FOCUSABLE = [
    'a[href]', 'button:not([disabled])', 'input:not([disabled])',
    'select:not([disabled])', 'textarea:not([disabled])',
    '[tabindex]:not([tabindex="-1"])', 'details > summary'
].join(', ');

let _priorFocus = null;
const _traps = new Map();

export function focusElement(el) {
    el?.focus();
}

export function trapFocus(container) {
    if (!container || _traps.has(container)) return;

    _priorFocus = document.activeElement;

    const handler = (e) => {
        if (e.key !== 'Tab') return;

        const focusable = [...container.querySelectorAll(FOCUSABLE)].filter(
            el => !el.closest('[disabled]') && el.offsetParent !== null
        );
        if (focusable.length === 0) { e.preventDefault(); return; }

        const first = focusable[0];
        const last = focusable[focusable.length - 1];

        if (e.shiftKey) {
            if (document.activeElement === first) {
                e.preventDefault();
                last.focus();
            }
        } else {
            if (document.activeElement === last) {
                e.preventDefault();
                first.focus();
            }
        }
    };

    container.addEventListener('keydown', handler);
    _traps.set(container, handler);

    // Focus first focusable element in container
    const firstFocusable = container.querySelector(FOCUSABLE);
    firstFocusable?.focus();
}

export function releaseFocus(container) {
    const handler = _traps.get(container);
    if (!handler) return;
    container.removeEventListener('keydown', handler);
    _traps.delete(container);
}

export function returnFocus() {
    _priorFocus?.focus();
    _priorFocus = null;
}
