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
    },

    // ----------------------------------------------------------------
    //  DropDownList — stop navigation keys from scrolling the page
    // ----------------------------------------------------------------
    dropDownPreventKeys(el, keys) {
        if (!el || el._msDdlKeys) return;
        el._msDdlKeys = (e) => { if (keys.includes(e.key)) e.preventDefault(); };
        el.addEventListener('keydown', el._msDdlKeys);
    },

    // ----------------------------------------------------------------
    //  TabStrip — scrollable headers
    // ----------------------------------------------------------------
    tabStripScroll(el, direction, vertical) {
        if (!el) return;
        const step = (vertical ? el.clientHeight : el.clientWidth) * 0.8 * direction;
        el.scrollBy({ [vertical ? 'top' : 'left']: step, behavior: 'smooth' });
    },

    tabStripHasOverflow(el, vertical) {
        if (!el) return false;
        return vertical ? el.scrollHeight > el.clientHeight + 1 : el.scrollWidth > el.clientWidth + 1;
    },

    tabStripScrollIntoView(id) {
        document.getElementById(id)?.scrollIntoView({ block: 'nearest', inline: 'nearest', behavior: 'smooth' });
    },

    // ----------------------------------------------------------------
    //  Signature pad: pointer drawing on a canvas, exported as a PNG data URL.
    // ----------------------------------------------------------------
    signatureInit(canvas, dotnet, options) {
        if (!canvas) return;
        const state = canvas._msSig = { dotnet, options, strokes: [], current: null };
        const ctx = canvas.getContext('2d');

        const resize = () => {
            const o = state.options, scale = o.exportScale || 1;
            canvas.width = Math.max(1, canvas.clientWidth * scale);
            canvas.height = Math.max(1, canvas.clientHeight * scale);
            ctx.setTransform(scale, 0, 0, scale, 0, 0);
            this._signatureRedraw(canvas);
        };
        const point = (e) => {
            const r = canvas.getBoundingClientRect();
            return { x: e.clientX - r.left, y: e.clientY - r.top };
        };
        const down = (e) => {
            if (state.options.disabled) return;
            canvas.setPointerCapture(e.pointerId);
            state.current = [point(e)];
            state.strokes.push(state.current);
            this._signatureRedraw(canvas);
            e.preventDefault();
        };
        const move = (e) => {
            if (!state.current) return;
            state.current.push(point(e));
            this._signatureRedraw(canvas);
            e.preventDefault();
        };
        const up = () => {
            if (!state.current) return;
            state.current = null;
            state.background = null;
            dotnet.invokeMethodAsync('OnSignatureChanged', canvas.toDataURL('image/png'));
        };

        canvas.addEventListener('pointerdown', down);
        canvas.addEventListener('pointermove', move);
        canvas.addEventListener('pointerup', up);
        canvas.addEventListener('pointercancel', up);
        state.observer = new ResizeObserver(resize);
        state.observer.observe(canvas);
        state.dispose = () => {
            canvas.removeEventListener('pointerdown', down);
            canvas.removeEventListener('pointermove', move);
            canvas.removeEventListener('pointerup', up);
            canvas.removeEventListener('pointercancel', up);
            state.observer.disconnect();
        };
        resize();
    },

    _signatureRedraw(canvas) {
        const state = canvas._msSig, o = state.options, ctx = canvas.getContext('2d');
        const w = canvas.clientWidth, h = canvas.clientHeight;
        ctx.save();
        ctx.fillStyle = o.background || '#ffffff';
        ctx.fillRect(0, 0, w, h);
        ctx.restore();
        if (state.background) ctx.drawImage(state.background, 0, 0, w, h);
        ctx.strokeStyle = o.color || '#000000';
        ctx.lineWidth = o.strokeWidth || 1;
        ctx.lineCap = 'round';
        ctx.lineJoin = 'round';
        for (const s of state.strokes) {
            if (s.length === 0) continue;
            ctx.beginPath();
            ctx.moveTo(s[0].x, s[0].y);
            if (s.length === 1) { ctx.lineTo(s[0].x + 0.1, s[0].y + 0.1); }
            else if (o.smooth) {
                for (let i = 1; i < s.length - 1; i++) {
                    const mx = (s[i].x + s[i + 1].x) / 2, my = (s[i].y + s[i + 1].y) / 2;
                    ctx.quadraticCurveTo(s[i].x, s[i].y, mx, my);
                }
                ctx.lineTo(s[s.length - 1].x, s[s.length - 1].y);
            } else {
                for (let i = 1; i < s.length; i++) ctx.lineTo(s[i].x, s[i].y);
            }
            ctx.stroke();
        }
    },

    signatureSetOptions(canvas, options) {
        if (!canvas?._msSig) return;
        const scaleChanged = canvas._msSig.options.exportScale !== options.exportScale;
        canvas._msSig.options = options;
        if (scaleChanged) {
            const s = options.exportScale || 1;
            canvas.width = canvas.clientWidth * s; canvas.height = canvas.clientHeight * s;
            canvas.getContext('2d').setTransform(s, 0, 0, s, 0, 0);
        }
        this._signatureRedraw(canvas);
    },

    signatureLoad(canvas, dataUrl) {
        const state = canvas?._msSig;
        if (!state) return;
        state.strokes = [];
        if (!dataUrl) { state.background = null; this._signatureRedraw(canvas); return; }
        const img = new Image();
        img.onload = () => { state.background = img; this._signatureRedraw(canvas); };
        img.src = dataUrl;
    },

    signatureDispose(canvas) { canvas?._msSig?.dispose?.(); },

    clickElement(id) { document.getElementById(id)?.click(); },

    // ----------------------------------------------------------------
    //  Rich-text editor (contenteditable). HTML is sanitized before it enters the editable area:
    //  inline event handlers, scripts and javascript: URLs would otherwise run.
    // ----------------------------------------------------------------
    _editorSanitize(html) {
        const doc = new DOMParser().parseFromString(`<body>${html || ''}</body>`, 'text/html');
        doc.querySelectorAll('script,style,iframe,object,embed,link,meta,base,form,input,button,textarea,select').forEach(n => n.remove());
        doc.body.querySelectorAll('*').forEach(el => {
            for (const attr of [...el.attributes]) {
                const name = attr.name.toLowerCase(), value = attr.value.trim().toLowerCase();
                if (name.startsWith('on') || name === 'srcdoc') el.removeAttribute(attr.name);
                else if (['href', 'src', 'xlink:href', 'action', 'formaction'].includes(name)
                         && (value.startsWith('javascript:') || value.startsWith('vbscript:') || (value.startsWith('data:') && !value.startsWith('data:image/'))))
                    el.removeAttribute(attr.name);
            }
        });
        return doc.body.innerHTML;
    },

    editorInit(el, dotnet, html, debounce) {
        if (!el || el._msEditor) return;
        const state = el._msEditor = { dotnet, timer: 0, lastState: '' };
        el.innerHTML = this._editorSanitize(html);
        try { document.execCommand('styleWithCSS', false, true); } catch { }
        const empty = () => el.classList.toggle('ms-empty', el.textContent.trim() === '' && !el.querySelector('img,table,hr'));
        empty();
        state.publish = () => {
            clearTimeout(state.timer);
            state.timer = setTimeout(() => dotnet.invokeMethodAsync('OnEditorInput', el.innerHTML), debounce || 0);
        };
        const onInput = () => { empty(); state.publish(); };
        const onPaste = (e) => {
            const html = e.clipboardData?.getData('text/html');
            if (!html) return;
            e.preventDefault();
            document.execCommand('insertHTML', false, this._editorSanitize(html));
        };
        const report = () => {
            const sel = document.getSelection();
            if (!sel || !sel.anchorNode || !el.contains(sel.anchorNode)) return;
            if (sel.rangeCount) state.range = sel.getRangeAt(0).cloneRange();
            const s = {};
            for (const c of ['bold', 'italic', 'underline', 'strikeThrough', 'subscript', 'superscript',
                             'insertUnorderedList', 'insertOrderedList', 'justifyLeft', 'justifyCenter', 'justifyRight', 'justifyFull']) {
                try { s[c] = document.queryCommandState(c); } catch { s[c] = false; }
            }
            try { s.formatBlock = (document.queryCommandValue('formatBlock') || 'p').toLowerCase(); } catch { s.formatBlock = 'p'; }
            const json = JSON.stringify(s);
            if (json !== state.lastState) { state.lastState = json; dotnet.invokeMethodAsync('OnEditorState', s); }
        };
        state.onSelection = () => requestAnimationFrame(report);
        el.addEventListener('input', onInput);
        el.addEventListener('paste', onPaste);
        document.addEventListener('selectionchange', state.onSelection);
        state.dispose = () => {
            el.removeEventListener('input', onInput);
            el.removeEventListener('paste', onPaste);
            document.removeEventListener('selectionchange', state.onSelection);
            clearTimeout(state.timer);
        };
    },

    editorSetHtml(el, html) {
        if (!el?._msEditor) return;
        el.innerHTML = this._editorSanitize(html);
        el.classList.toggle('ms-empty', el.textContent.trim() === '' && !el.querySelector('img,table,hr'));
    },

    editorSaveSelection(el) {
        const sel = document.getSelection();
        if (el?._msEditor && sel?.rangeCount && el.contains(sel.anchorNode)) el._msEditor.range = sel.getRangeAt(0).cloneRange();
        return sel ? sel.toString() : '';
    },

    editorExec(el, command, value) {
        const state = el?._msEditor;
        if (!state) return;
        el.focus();
        if (state.range) {
            const sel = document.getSelection();
            sel.removeAllRanges();
            sel.addRange(state.range);
        }
        if (command === 'insertHTML') value = this._editorSanitize(value);
        if (command === 'createLink' && /^\s*(javascript|vbscript):/i.test(value || '')) return;
        document.execCommand(command, false, value ?? null);
        el.classList.toggle('ms-empty', el.textContent.trim() === '' && !el.querySelector('img,table,hr'));
        state.publish();
        state.onSelection();
    },

    editorDispose(el) { el?._msEditor?.dispose?.(); if (el) el._msEditor = null; },

    capturePointer(el, pointerId) { try { el?.setPointerCapture(pointerId); } catch { } },

    // Keeps a fixed-position element (validation tooltip) next to the element matching a CSS selector.
    anchorTo(selector, el, side) {
        if (!el) return;
        const place = () => {
            const target = document.querySelector(selector);
            if (!target) { el.style.visibility = 'hidden'; return; }
            const t = target.getBoundingClientRect(), r = el.getBoundingClientRect(), gap = 6;
            let top = t.bottom + gap, left = t.left;
            if (side === 'top') top = t.top - r.height - gap;
            if (side === 'left') { top = t.top + (t.height - r.height) / 2; left = t.left - r.width - gap; }
            if (side === 'right') { top = t.top + (t.height - r.height) / 2; left = t.right + gap; }
            el.style.top = `${top}px`; el.style.left = `${left}px`; el.style.visibility = 'visible';
        };
        if (!el._msAnchor) {
            el._msAnchor = place;
            window.addEventListener('scroll', place, true);
            window.addEventListener('resize', place);
        }
        place();
    },

    unanchor(el) {
        if (!el?._msAnchor) return;
        window.removeEventListener('scroll', el._msAnchor, true);
        window.removeEventListener('resize', el._msAnchor);
        el._msAnchor = null;
    },

    // ----------------------------------------------------------------
    //  Upload: files stay in the browser and are sent with XHR (progress, cancel).
    // ----------------------------------------------------------------
    _uploads: {},

    uploadInit(uid, input, dropzoneIds, dotnet) {
        const reg = this._uploads[uid] = { files: {}, xhrs: {}, counter: 0, dotnet, input };
        const add = (fileList) => {
            const meta = [];
            for (const f of fileList) {
                const id = `${uid}-${++reg.counter}`;
                reg.files[id] = f;
                meta.push({ id, name: f.name, size: f.size, type: f.type || '' });
            }
            if (meta.length) dotnet.invokeMethodAsync('OnJsFilesSelected', meta);
        };
        input.addEventListener('change', () => { add(input.files); input.value = ''; });
        for (const dzId of dropzoneIds || []) {
            const dz = document.getElementById(dzId);
            if (!dz) continue;
            dz.addEventListener('dragover', (e) => e.preventDefault());
            dz.addEventListener('drop', (e) => {
                e.preventDefault();
                if (input.disabled || !e.dataTransfer?.files?.length) return;
                add(input.multiple ? e.dataTransfer.files : [e.dataTransfer.files[0]]);
            });
        }
    },

    uploadOpenDialog(uid) { this._uploads[uid]?.input?.click(); },

    uploadSend(uid, fileId, url, field, headers, data, withCredentials) {
        const reg = this._uploads[uid], file = reg?.files[fileId];
        if (!file) return;
        const form = new FormData();
        form.append(field || 'files', file, file.name);
        for (const [k, v] of Object.entries(data || {})) form.append(k, v);
        const xhr = new XMLHttpRequest();
        reg.xhrs[fileId] = xhr;
        xhr.open('POST', url, true);
        xhr.withCredentials = !!withCredentials;
        for (const [k, v] of Object.entries(headers || {})) xhr.setRequestHeader(k, v);
        xhr.upload.onprogress = (e) => {
            if (e.lengthComputable) reg.dotnet.invokeMethodAsync('OnJsProgress', fileId, Math.round(e.loaded * 100 / e.total));
        };
        const done = (ok) => {
            delete reg.xhrs[fileId];
            reg.dotnet.invokeMethodAsync('OnJsCompleted', fileId, ok, xhr.status, xhr.statusText || '', xhr.responseText || '');
        };
        xhr.onload = () => done(xhr.status >= 200 && xhr.status < 300);
        xhr.onerror = () => done(false);
        xhr.send(form);
    },

    uploadCancel(uid, fileId) {
        const reg = this._uploads[uid];
        reg?.xhrs[fileId]?.abort();
        if (reg) delete reg.xhrs[fileId];
    },

    async uploadRemove(url, field, names, headers, data, withCredentials) {
        const form = new FormData();
        for (const n of names) form.append(field || 'files', n);
        for (const [k, v] of Object.entries(data || {})) form.append(k, v);
        try {
            const res = await fetch(url, { method: 'POST', body: form, headers: headers || {}, credentials: withCredentials ? 'include' : 'same-origin' });
            return { ok: res.ok, status: res.status, statusText: res.statusText || '', responseText: await res.text() };
        } catch (e) {
            return { ok: false, status: 0, statusText: String(e), responseText: '' };
        }
    },

    uploadForget(uid, fileId) { const reg = this._uploads[uid]; if (reg) delete reg.files[fileId]; },

    uploadDispose(uid) {
        const reg = this._uploads[uid];
        if (!reg) return;
        for (const xhr of Object.values(reg.xhrs)) xhr.abort();
        delete this._uploads[uid];
    },

    // TextArea AutoSize: grow/shrink with the content.
    textAreaAutoSize(el) {
        if (!el) return;
        const fit = () => { el.style.height = 'auto'; el.style.height = el.scrollHeight + 'px'; };
        if (!el._msAutoSize) { el._msAutoSize = fit; el.addEventListener('input', fit); }
        fit();
    },

    selectAll(el) { if (el && el.select) el.select(); },

    // Caret helpers for the masked text box.
    getCaret(el) { return el ? el.selectionStart ?? 0 : 0; },
    setCaret(el, pos) { if (el && document.activeElement === el) el.setSelectionRange(pos, pos); },

    // AutoComplete: arrows never move the caret; Enter only selects (instead of submitting) while the list is open.
    autoCompletePreventKeys(el) {
        if (!el || el._msAcKeys) return;
        el._msAcKeys = (e) => {
            if (['ArrowUp', 'ArrowDown', 'PageUp', 'PageDown'].includes(e.key)) e.preventDefault();
            else if (e.key === 'Enter' && el.getAttribute('aria-expanded') === 'true' && el.hasAttribute('aria-activedescendant')) e.preventDefault();
        };
        el.addEventListener('keydown', el._msAcKeys);
    },

    dropDownScrollIntoView(listId, index) {
        const list = document.getElementById(listId);
        const item = list?.querySelector(`[data-index="${index}"]`);
        item?.scrollIntoView({ block: 'nearest' });
    }
};
