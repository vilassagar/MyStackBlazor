// Modal focus ownership is tracked per dialog, including nested dialogs.
const dialogs = [];
const selector = 'a[href],button,input,select,textarea,[tabindex],summary';
function focusable(panel) {
    return [...panel.querySelectorAll(selector)].filter(el =>
        el.tabIndex >= 0 && !el.disabled && !el.closest('[inert]') && el.getClientRects().length > 0);
}
export function isTop(id) { return dialogs.at(-1)?.id === id; }
export function activate(id, panel) {
    if (!panel || dialogs.some(d => d.id === id)) return;
    const entry = { id, panel, previous: document.activeElement };
    entry.keydown = e => {
        if (!isTop(id) || e.key !== 'Tab') return;
        const items = focusable(panel);
        const first = items[0] ?? panel;
        const last = items.at(-1) ?? panel;
        if (!items.length || !panel.contains(document.activeElement) ||
            (e.shiftKey && (document.activeElement === first || document.activeElement === panel)) ||
            (!e.shiftKey && document.activeElement === last)) {
            e.preventDefault();
            (e.shiftKey ? last : first).focus();
        }
    };
    entry.focusin = e => {
        if (isTop(id) && !panel.contains(e.target)) (focusable(panel)[0] ?? panel).focus();
    };
    dialogs.push(entry);
    document.addEventListener('keydown', entry.keydown);
    document.addEventListener('focusin', entry.focusin);
    (focusable(panel)[0] ?? panel).focus();
}
export function deactivate(id) {
    const index = dialogs.findIndex(d => d.id === id);
    if (index < 0) return;
    const top = isTop(id);
    const [entry] = dialogs.splice(index, 1);
    document.removeEventListener('keydown', entry.keydown);
    document.removeEventListener('focusin', entry.focusin);
    // If an underlying dialog unmounts first, preserve the original return target.
    for (const child of dialogs) {
        if (entry.panel.contains(child.previous)) child.previous = entry.previous;
    }
    if (top) {
        const parent = dialogs.at(-1)?.panel;
        if (entry.previous?.isConnected && (!parent || parent.contains(entry.previous))) entry.previous.focus();
        else if (parent) (focusable(parent)[0] ?? parent).focus();
    }
}
