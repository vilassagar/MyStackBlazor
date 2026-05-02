window.MyStackBlazor = {

    // ----------------------------------------------------------------
    //  Theme
    // ----------------------------------------------------------------
    THEME_KEY: 'msb-theme',

    initTheme(dotnetRef) {
        const saved = this._getStoredTheme();
        const isDark = saved === 'dark' ||
            (!saved && window.matchMedia('(prefers-color-scheme: dark)').matches);

        // Notify Blazor of the resolved OS preference
        if (dotnetRef) {
            dotnetRef.invokeMethodAsync('NotifySystemDark',
                window.matchMedia('(prefers-color-scheme: dark)').matches);
        }

        // Watch OS changes
        window.matchMedia('(prefers-color-scheme: dark)')
            .addEventListener('change', e => {
                if (dotnetRef) {
                    dotnetRef.invokeMethodAsync('NotifySystemDark', e.matches);
                }
            });

        return isDark;
    },

    setTheme(isDark) {
        try {
            localStorage.setItem(this.THEME_KEY, isDark ? 'dark' : 'light');
        } catch { /* MAUI WebView may restrict localStorage */ }
    },

    _getStoredTheme() {
        try { return localStorage.getItem(this.THEME_KEY); } catch { return null; }
    },

    // ----------------------------------------------------------------
    //  Toast
    // ----------------------------------------------------------------
    showToast(id, duration) {
        const el = document.getElementById(id);
        if (!el) return;
        el.classList.remove('hidden');
        el.classList.add('flex');
        if (duration > 0) {
            setTimeout(() => this.hideToast(id), duration);
        }
    },

    hideToast(id) {
        const el = document.getElementById(id);
        if (!el) return;
        el.classList.add('opacity-0');
        setTimeout(() => {
            el.classList.add('hidden');
            el.classList.remove('flex', 'opacity-0');
        }, 150);
    },

    // ----------------------------------------------------------------
    //  Focus trap
    // ----------------------------------------------------------------
    trapFocus(el) {
        if (!el) return;
        const focusable = el.querySelectorAll(
            'a[href],button:not([disabled]),textarea,input,select,[tabindex]:not([tabindex="-1"])'
        );
        if (focusable.length) focusable[0].focus();
    },

    // ----------------------------------------------------------------
    //  Tooltip positioning
    // ----------------------------------------------------------------
    positionTooltip(triggerId, tooltipId, side) {
        const trigger = document.getElementById(triggerId);
        const tooltip = document.getElementById(tooltipId);
        if (!trigger || !tooltip) return;
        const rect = trigger.getBoundingClientRect();
        const tip  = tooltip.getBoundingClientRect();
        const gap  = 8;
        let top, left;
        switch (side) {
            case 'top':
                top  = rect.top  - tip.height - gap + window.scrollY;
                left = rect.left + (rect.width - tip.width) / 2 + window.scrollX;
                break;
            case 'bottom':
                top  = rect.bottom + gap + window.scrollY;
                left = rect.left + (rect.width - tip.width) / 2 + window.scrollX;
                break;
            case 'left':
                top  = rect.top  + (rect.height - tip.height) / 2 + window.scrollY;
                left = rect.left - tip.width - gap + window.scrollX;
                break;
            default: // right
                top  = rect.top   + (rect.height - tip.height) / 2 + window.scrollY;
                left = rect.right + gap + window.scrollX;
                break;
        }
        tooltip.style.top  = `${top}px`;
        tooltip.style.left = `${left}px`;
    },

    // ----------------------------------------------------------------
    //  Context menu
    // ----------------------------------------------------------------
    positionContextMenu(menuId, x, y) {
        const menu = document.getElementById(menuId);
        if (!menu) return;
        menu.style.top  = `${y + window.scrollY}px`;
        menu.style.left = `${x + window.scrollX}px`;
    },

    // ----------------------------------------------------------------
    //  Click outside
    // ----------------------------------------------------------------
    addClickOutsideListener(dotnetRef, elementId, methodName) {
        const el = document.getElementById(elementId);
        const handler = (e) => {
            if (el && !el.contains(e.target)) {
                dotnetRef.invokeMethodAsync(methodName);
            }
        };
        document.addEventListener('click', handler);
        return handler;
    },

    // ----------------------------------------------------------------
    //  Clipboard
    // ----------------------------------------------------------------
    copyToClipboard(text) {
        if (navigator.clipboard && navigator.clipboard.writeText) {
            return navigator.clipboard.writeText(text);
        }
        // Fallback for MAUI / older browsers
        const ta = document.createElement('textarea');
        ta.value = text;
        ta.style.position = 'fixed';
        ta.style.opacity = '0';
        document.body.appendChild(ta);
        ta.focus();
        ta.select();
        try { document.execCommand('copy'); } catch {}
        document.body.removeChild(ta);
        return Promise.resolve();
    },

    // ----------------------------------------------------------------
    //  Scroll area
    // ----------------------------------------------------------------
    initScrollArea(el) {
        if (el) el.style.overflowY = 'auto';
    }
};
