// ----------------------------------------------------------------
//  Camera barcode scanner using the browser BarcodeDetector API.
//  Native on Chrome/Edge for Android, macOS and ChromeOS. Elsewhere,
//  pass a polyfill module URL that defines globalThis.BarcodeDetector
//  (e.g. the "barcode-detector" npm package's polyfill entry).
// ----------------------------------------------------------------

const sessions = new Map();
let nextId = 1;

async function ensureDetector(polyfillUrl) {
    if ('BarcodeDetector' in globalThis) return true;
    if (!polyfillUrl) return false;
    try {
        await import(polyfillUrl);
    } catch (e) {
        console.warn('[MyStackBlazor] Barcode polyfill failed to load:', e);
    }
    return 'BarcodeDetector' in globalThis;
}

export async function isSupported(polyfillUrl) {
    const hasCamera = !!(navigator.mediaDevices && navigator.mediaDevices.getUserMedia);
    return hasCamera && await ensureDetector(polyfillUrl);
}

export async function start(video, dotnetRef, opts) {
    const o = opts || {};
    if (!(await isSupported(o.polyfillUrl))) {
        await dotnetRef.invokeMethodAsync('HandleError', 'unsupported').catch(() => {});
        return 0;
    }

    let detector;
    try {
        let formats = o.formats && o.formats.length ? o.formats : undefined;
        if (formats && BarcodeDetector.getSupportedFormats) {
            const supported = await BarcodeDetector.getSupportedFormats();
            formats = formats.filter(f => supported.includes(f));
            if (!formats.length) formats = undefined;
        }
        detector = new BarcodeDetector(formats ? { formats } : undefined);
    } catch (e) {
        await dotnetRef.invokeMethodAsync('HandleError', 'unsupported').catch(() => {});
        return 0;
    }

    let stream;
    try {
        stream = await navigator.mediaDevices.getUserMedia({
            audio: false,
            video: { facingMode: { ideal: o.facingMode || 'environment' } },
        });
    } catch (e) {
        // NotAllowedError (permission denied), NotFoundError (no camera), NotReadableError (in use)…
        await dotnetRef.invokeMethodAsync('HandleError', (e && e.name) || 'camera-error').catch(() => {});
        return 0;
    }

    video.srcObject = stream;
    video.setAttribute('playsinline', '');
    video.muted = true;
    try { await video.play(); } catch { /* autoplay policies — detection still works once frames arrive */ }

    const id = nextId++;
    const session = { stream, video, stopped: false, last: '', lastAt: 0, timer: 0 };
    sessions.set(id, session);

    const interval = Math.max(50, o.intervalMs || 150);
    const tick = async () => {
        if (session.stopped) return;
        try {
            if (video.readyState >= 2) {
                const codes = await detector.detect(video);
                if (codes && codes.length) {
                    const c = codes[0];
                    const now = Date.now();
                    // Ignore the same code seen again within 1.5s (continuous mode re-reads every frame).
                    if (c.rawValue !== session.last || now - session.lastAt > 1500) {
                        session.last = c.rawValue;
                        session.lastAt = now;
                        if (o.vibrate && navigator.vibrate) navigator.vibrate(60);
                        await dotnetRef.invokeMethodAsync('HandleDetected', c.rawValue, c.format || '').catch(() => {});
                        if (!o.continuous) { stop(id); return; }
                    }
                }
            }
        } catch { /* transient detect errors (e.g. frame not ready) */ }
        if (!session.stopped) session.timer = setTimeout(tick, interval);
    };
    tick();
    return id;
}

export async function setTorch(id, on) {
    const s = sessions.get(id);
    const track = s && s.stream.getVideoTracks()[0];
    if (!track || !track.getCapabilities || !track.getCapabilities().torch) return false;
    try {
        await track.applyConstraints({ advanced: [{ torch: !!on }] });
        return true;
    } catch {
        return false;
    }
}

export function stop(id) {
    const s = sessions.get(id);
    if (!s) return;
    s.stopped = true;
    clearTimeout(s.timer);
    s.stream.getTracks().forEach(t => t.stop());
    s.video.srcObject = null;
    sessions.delete(id);
}
