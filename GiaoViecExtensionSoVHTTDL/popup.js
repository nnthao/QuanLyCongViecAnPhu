// Mở ứng dụng trong tab mới
function openApp() {
    const giaoViecDomain = GiaoViecExtensionConfig.getGiaoViecDomain();
    chrome.tabs.create({ url: `${giaoViecDomain}/GiaoViec` });
}

// Khởi tạo
document.addEventListener('DOMContentLoaded', () => {
    const btnOpenApp = document.getElementById('btnOpenApp');
    if (btnOpenApp) {
        btnOpenApp.addEventListener('click', openApp);
    }
});
