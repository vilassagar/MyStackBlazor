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
    },

    // ----------------------------------------------------------------
    //  Infinite scroll — IntersectionObserver helpers
    // ----------------------------------------------------------------
    _observers: {},

    observeInfiniteScroll(sentinelId, containerId, dotnetRef) {
        const sentinel  = document.getElementById(sentinelId);
        const container = document.getElementById(containerId);
        if (!sentinel) return;

        // Tear down any existing observer for this sentinel
        if (this._observers[sentinelId]) {
            this._observers[sentinelId].disconnect();
        }

        const observer = new IntersectionObserver(
            (entries) => {
                if (entries[0].isIntersecting) {
                    dotnetRef.invokeMethodAsync('TriggerLoadMore');
                }
            },
            { root: container || null, threshold: 0.1 }
        );
        observer.observe(sentinel);
        this._observers[sentinelId] = observer;
    },

    disconnectInfiniteScroll(sentinelId) {
        if (this._observers[sentinelId]) {
            this._observers[sentinelId].disconnect();
            delete this._observers[sentinelId];
        }
    },

    // ----------------------------------------------------------------
    //  File download — triggers browser save-as from bytes or URL
    // ----------------------------------------------------------------
    downloadFile(fileName, contentType, base64) {
        const bytes = atob(base64);
        const buf = new Uint8Array(bytes.length);
        for (let i = 0; i < bytes.length; i++) buf[i] = bytes.charCodeAt(i);
        const blob = new Blob([buf], { type: contentType });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    },

    downloadFromUrl(url, fileName) {
        const a = document.createElement('a');
        a.href = url;
        a.download = fileName || '';
        a.target = '_blank';
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
    },

    // ----------------------------------------------------------------
    //  Markdown editor — cursor-aware text insertion
    // ----------------------------------------------------------------
    insertAtCursor(el, before, after, defaultText) {
        if (!el) return '';
        const start    = el.selectionStart ?? el.value.length;
        const end      = el.selectionEnd   ?? el.value.length;
        const selected = el.value.substring(start, end) || (defaultText ?? '');
        const insert   = before + selected + after;
        el.value = el.value.substring(0, start) + insert + el.value.substring(end);
        const cur = start + before.length + selected.length;
        el.selectionStart = el.selectionEnd = cur;
        el.focus();
        el.dispatchEvent(new InputEvent('input', { bubbles: true }));
        return el.value;
    },

    // ----------------------------------------------------------------
    //  Resizable panels — measure container dimension
    // ----------------------------------------------------------------
    getElementDimension(el, dimension) {
        if (!el) return 800;
        const rect = el.getBoundingClientRect();
        return dimension === 'height' ? rect.height : rect.width;
    },

    // ----------------------------------------------------------------
    //  File dropzone — bridges drag-drop onto <InputFile>
    // ----------------------------------------------------------------
    setupDropzone(dropzoneId, inputId) {
        const dz    = document.getElementById(dropzoneId);
        const input = document.getElementById(inputId);
        if (!dz || !input) return;
        dz.addEventListener('drop', (e) => {
            e.preventDefault();
            if (!e.dataTransfer?.files?.length) return;
            const dt = new DataTransfer();
            for (const f of e.dataTransfer.files) dt.items.add(f);
            input.files = dt.files;
            input.dispatchEvent(new Event('change', { bubbles: true }));
        });
    },

    // ----------------------------------------------------------------
    //  Media player — imperative HTMLMediaElement control
    // ----------------------------------------------------------------
    mediaPlay(el) { return el?.play?.(); },

    mediaPause(el) { el?.pause?.(); },

    mediaSeek(el, seconds) { if (el) el.currentTime = seconds; },

    mediaSetVolume(el, volume) { if (el) el.volume = volume; },

    mediaSetMuted(el, muted) { if (el) el.muted = muted; },

    mediaSetPlaybackRate(el, rate) { if (el) el.playbackRate = rate; },

    mediaGetBufferedEnd(el) {
        if (!el || !el.buffered || el.buffered.length === 0) return 0;
        return el.buffered.end(el.buffered.length - 1);
    },

    mediaGetPlaybackState(el) {
        if (!el) return { currentTime: 0, duration: 0, bufferedEnd: 0, paused: true, ended: false };
        const d = el.duration;
        return {
            currentTime: el.currentTime || 0,
            duration: isFinite(d) ? d : 0,
            bufferedEnd: this.mediaGetBufferedEnd(el),
            paused: el.paused,
            ended: el.ended
        };
    },

    mediaSetCaptionsEnabled(el, enabled) {
        if (!el || !el.textTracks) return;
        for (let i = 0; i < el.textTracks.length; i++) {
            el.textTracks[i].mode = enabled ? 'showing' : 'hidden';
        }
    },

    mediaCanPlayNativeHls(el) {
        return !!(el && el.canPlayType && el.canPlayType('application/vnd.apple.mpegurl'));
    },

    // Extensibility hook: consumers can set window.MyStackBlazorMediaEngine = (videoEl, src) => {...}
    // to attach hls.js/dash.js for browsers without native HLS/DASH support, without this
    // library bundling those dependencies itself.
    mediaAttachCustomEngine(el, src) {
        if (typeof window.MyStackBlazorMediaEngine === 'function') {
            window.MyStackBlazorMediaEngine(el, src);
            return true;
        }
        return false;
    },

    mediaRequestFullscreen(el) {
        const target = el?.parentElement ?? el;
        return target?.requestFullscreen?.();
    },

    mediaExitFullscreen() {
        if (document.fullscreenElement) return document.exitFullscreen();
    },

    mediaIsFullscreen(el) {
        const target = el?.parentElement ?? el;
        return !!document.fullscreenElement && document.fullscreenElement === target;
    },

    mediaRequestPip(el) { return el?.requestPictureInPicture?.(); },

    mediaExitPip() {
        if (document.pictureInPictureElement) return document.exitPictureInPicture();
    },

    mediaSupportsPip(el) { return !!(document.pictureInPictureEnabled && el && !el.disablePictureInPicture); },

    // ----------------------------------------------------------------
    //  Chat message list — stick-to-bottom auto-scroll
    // ----------------------------------------------------------------
    chatIsNearBottom(el, threshold) {
        if (!el) return true;
        return (el.scrollHeight - el.scrollTop - el.clientHeight) <= (threshold ?? 80);
    },

    chatScrollToBottom(el, smooth) {
        if (!el) return;
        el.scrollTo({ top: el.scrollHeight, behavior: smooth ? 'smooth' : 'auto' });
    }
};
