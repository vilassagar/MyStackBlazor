// ----------------------------------------------------------------
//  Speech recognition (Web Speech API) for voice search.
//  Supported in Chromium-based browsers and Safari; elsewhere
//  isSupported() returns false.
// ----------------------------------------------------------------

const Recognition = window.SpeechRecognition || window.webkitSpeechRecognition;
const sessions = new Map();
let nextId = 1;

export function isSupported() {
    return !!Recognition;
}

export function start(dotnetRef, opts) {
    if (!Recognition) return 0;
    const o = opts || {};
    const rec = new Recognition();
    rec.lang = o.lang || navigator.language || 'en-US';
    rec.interimResults = !!o.interimResults;
    rec.continuous = !!o.continuous;
    rec.maxAlternatives = 1;

    const id = nextId++;
    sessions.set(id, rec);
    let finalText = '';

    rec.onresult = e => {
        let interim = '';
        for (let i = e.resultIndex; i < e.results.length; i++) {
            const r = e.results[i];
            if (r.isFinal) finalText += r[0].transcript;
            else interim += r[0].transcript;
        }
        if (interim) dotnetRef.invokeMethodAsync('HandleInterim', (finalText + interim).trim()).catch(() => {});
        if (!rec.continuous && finalText) dotnetRef.invokeMethodAsync('HandleFinal', finalText.trim()).catch(() => {});
    };
    rec.onerror = e => dotnetRef.invokeMethodAsync('HandleError', e.error || 'error').catch(() => {});
    rec.onend = () => {
        sessions.delete(id);
        if (rec.continuous && finalText) dotnetRef.invokeMethodAsync('HandleFinal', finalText.trim()).catch(() => {});
        dotnetRef.invokeMethodAsync('HandleEnded').catch(() => {});
    };

    try {
        rec.start();
    } catch (err) {
        sessions.delete(id);
        dotnetRef.invokeMethodAsync('HandleError', String(err && err.name || 'start-failed')).catch(() => {});
        return 0;
    }
    return id;
}

export function stop(id) {
    const rec = sessions.get(id);
    if (rec) rec.stop();
}

export function abort(id) {
    const rec = sessions.get(id);
    if (rec) { rec.abort(); sessions.delete(id); }
}
