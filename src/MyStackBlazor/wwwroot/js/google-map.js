// ----------------------------------------------------------------
//  Google Maps loader — lazily loads the Maps JS API once per page
//  and initializes a map instance into a target element.
// ----------------------------------------------------------------

export function loadGoogleMaps(apiKey) {
    if (window.google && window.google.maps) {
        return Promise.resolve();
    }

    if (!window.__msbGoogleMapsPromise) {
        window.__msbGoogleMapsPromise = new Promise((resolve, reject) => {
            const script = document.createElement('script');
            script.src = `https://maps.googleapis.com/maps/api/js?key=${encodeURIComponent(apiKey)}`;
            script.async = true;
            script.defer = true;
            script.onload = () => resolve();
            script.onerror = () => reject(new Error('Failed to load the Google Maps script.'));
            document.head.appendChild(script);
        });
    }

    return window.__msbGoogleMapsPromise;
}

export function initMap(element, lat, lng, zoom, markers) {
    if (!element || !window.google || !window.google.maps) return null;

    const center = { lat, lng };
    const map = new google.maps.Map(element, { center, zoom });

    if (Array.isArray(markers)) {
        for (const m of markers) {
            new google.maps.Marker({
                position: { lat: m.lat, lng: m.lng },
                map,
                title: m.title || undefined,
            });
        }
    }

    return map;
}
