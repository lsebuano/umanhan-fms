
window.loadGoogleMaps = function (apiKey) {
    if (document.getElementById('map')) {
        console.log('map element exists');
        return;
    }

    var script = document.createElement('script');
    script.id = 'google-maps';
    script.src = `https://maps.googleapis.com/maps/api/js?key=${apiKey}&libraries=drawing,places,geometry`;
    script.async = true;
    script.defer = true;
    document.head.appendChild(script);

    loadScript('js/google-maps-interop.js');
}

window.loadGoogleMapsFromProxy = async function () {
    const interopUrl = 'js/google-maps-interop.js';
    const res = await fetch("https://localhost:7038/api/secrets/google-maps-api-key");
    const data = await res.json();
    console.log({ data });

    if (!data.apiKey) throw new Error("API key missing in response.");

    const proxyUrl = `https://maps.googleapis.com/maps/api/js?key=${data.apiKey}=drawing,places,geometry`;
    if (!window.google || !window.google.maps)
        await loadScript(proxyUrl);
    else
        console.log("Google Maps API already loaded.");

    // then load the interop script
    if (!window.googleMapInteropLoaded) {
        await loadScript(interopUrl);
        window.googleMapInteropLoaded = true;
    }
    else
        console.log("Interop already loaded.");

    console.log("Google Maps and interop are ready.");

    await loadScript('_framework/blazor.webassembly.js');
    await loadScript('js/auth-settings-override.js');
    await loadScript('_content/Radzen.Blazor/Radzen.Blazor.js');
}

function loadScript(src) {
    const separator = src.includes('?') ? '&' : '?';
    src += `${separator}v=${Date.now()}`; // append cache buster

    return new Promise((resolve, reject) => {
        //if (document.querySelector(`script[src="${src}"]`)) {
        if (document.querySelector(`script[src^="${src.split('?')[0]}"]`)) {
            resolve(); // Already loaded
            return;
        }

        const script = document.createElement("script");
        script.src = src;
        //script.async = true;
        script.onload = resolve;
        script.onerror = () => reject(new Error(`Failed to load script: ${src}`));
        document.head.appendChild(script);
    });
}