// site.js

window.openInNewTab = function (url) {
    window.open(url, '_blank');
}

window.scrollToElementById = function (id) {
    const el = document.getElementById(id);
    if (el) {
        el.scrollIntoView({ behavior: 'smooth' });
    }
};

window.validateImage = (base64DataUrl) => {
    return new Promise((resolve) => {
        const img = new Image();
        img.onload = () => resolve(true);   // valid image
        img.onerror = () => resolve(false); // corrupted
        img.src = base64DataUrl;
    });
};

document.addEventListener('contextmenu', function (e) {
    if (e.target.tagName === 'IMG' || e.target.closest('.hero')) {
        e.preventDefault();
    }
});

function downloadFile(url, filename) {
    const link = document.createElement("a");
    link.href = url;
    link.target = '_blank';
    link.download = filename || "image.jpg";
    link.click();
}