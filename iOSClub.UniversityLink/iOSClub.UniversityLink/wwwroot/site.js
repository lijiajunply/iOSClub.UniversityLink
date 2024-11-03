function jsSaveAsFile(filename, byteBase64) {
    const link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + byteBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

window.localStorageHelper = {
    setItem: function (key, value) {
        localStorage.setItem(key, value);
    },
    getItem: function (key) {
        return localStorage.getItem(key);
    },
    removeItem: function (key) {
        localStorage.removeItem(key);
    },
    clear: function () {
        localStorage.clear();
    }
};

function isWeiXin() {
    const ua = navigator.userAgent
    return !!/MicroMessenger/i.test(ua)
}

function makeCode (text) {
    let a =document.getElementById("qrcode")
    a.innerHTML = ""
    const qrcode = new QRCode(a, {
        width : 150,
        height : 150
    });
    qrcode.makeCode(text);
}

async function copyText(content) {
    try {
        await navigator.clipboard.writeText(content);
        return true;
    } catch (err) {
        console.error('复制失败: ', err);
        return false;
    }
}