// Dynamically loads an external <script> tag, deduping by src, and resolves once it fires
// onload (or immediately if a matching <script> is already present in the document).
// Attached to the shared window.MyStackBlazor namespace to match mystackblazor.js's
// convention of exposing functions as MyStackBlazor.xxx for JS.InvokeVoidAsync/InvokeAsync calls.
window.MyStackBlazor = window.MyStackBlazor || {};

window.MyStackBlazor.loadScript = function (src, async, defer) {
    const existing = document.querySelector(`script[src="${src}"]`);
    if (existing) {
        return existing.dataset.msbLoaded === 'true'
            ? Promise.resolve()
            : new Promise((resolve, reject) => {
                existing.addEventListener('load', () => resolve(), { once: true });
                existing.addEventListener('error', () => reject(new Error(`Failed to load script: ${src}`)), { once: true });
            });
    }

    return new Promise((resolve, reject) => {
        const script = document.createElement('script');
        script.src = src;
        script.async = async !== false;
        script.defer = defer === true;
        script.onload = () => {
            script.dataset.msbLoaded = 'true';
            resolve();
        };
        script.onerror = () => reject(new Error(`Failed to load script: ${src}`));
        document.head.appendChild(script);
    });
};
