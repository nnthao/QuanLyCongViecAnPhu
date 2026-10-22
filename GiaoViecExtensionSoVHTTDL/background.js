importScripts('config.js');

// Background service worker cho Chrome Extension

// Lắng nghe khi extension được cài đặt
chrome.runtime.onInstalled.addListener((details) => {
    console.log('GiaoViecExtension đã được cài đặt:', details.reason);

    if (details.reason === 'install') {
        chrome.storage.sync.set({
            [GiaoViecExtensionConfig.storageKeys.autoRefresh]: GiaoViecExtensionConfig.autoRefresh,
            [GiaoViecExtensionConfig.storageKeys.refreshInterval]: GiaoViecExtensionConfig.refreshInterval
        });
    }
});

// Hàm mở hoặc chuyển đến tab đã có của ứng dụng Giao Việc
function openOrFocusTab(targetUrl, sendResponse) {
    const domain = GiaoViecExtensionConfig.getGiaoViecDomain();
    let hostname = 'demo5.vnptquangngai.vn';
    try {
        hostname = new URL(domain).hostname;
    } catch (e) {
        console.error('Lỗi parse hostname domain:', e);
    }
    const matchPattern = `*://${hostname}/*`;

    chrome.tabs.query({ url: matchPattern }, (tabs) => {
        if (tabs && tabs.length > 0) {
            const existingTab = tabs[0];
            chrome.tabs.update(existingTab.id, { url: targetUrl, active: true }, () => {
                if (existingTab.windowId) {
                    chrome.windows.update(existingTab.windowId, { focused: true });
                }
                if (sendResponse) sendResponse({ success: true, mode: 'focused' });
            });
        } else {
            chrome.tabs.create({ url: targetUrl }, () => {
                if (sendResponse) sendResponse({ success: true, mode: 'created' });
            });
        }
    });
}

// Lắng nghe message từ content script hoặc popup
chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
    if (request.action === 'getToken') {
        chrome.storage.sync.get(['authToken'], (result) => {
            sendResponse({ token: result.authToken });
        });
        return true;
    }

    if (request.action === 'setToken') {
        chrome.storage.sync.set({ authToken: request.token }, () => {
            sendResponse({ success: true });
        });
        return true;
    }

    if (request.action === 'clearToken') {
        chrome.storage.sync.remove('authToken', () => {
            sendResponse({ success: true });
        });
        return true;
    }

    if (request.action === 'openOrFocusTab') {
        const targetUrl = request.url || `${GiaoViecExtensionConfig.getGiaoViecDomain()}/GiaoViec`;
        openOrFocusTab(targetUrl, sendResponse);
        return true;
    }

    if (request.action === 'openApp' || request.action === 'openPopup') {
        const targetUrl = `${GiaoViecExtensionConfig.getGiaoViecDomain()}/GiaoViec`;
        openOrFocusTab(targetUrl, sendResponse);
        return true;
    }
});

// Badge notification (hiển thị số công việc cần xử lý)
async function updateBadge() {
    try {
        const result = await chrome.storage.sync.get([GiaoViecExtensionConfig.storageKeys.authToken]);
        if (!result[GiaoViecExtensionConfig.storageKeys.authToken]) {
            chrome.action.setBadgeText({ text: '' });
            return;
        }
    } catch (error) {
        console.error('Lỗi khi cập nhật badge:', error);
    }
}

// Cập nhật badge định kỳ
chrome.storage.sync.get([
    GiaoViecExtensionConfig.storageKeys.autoRefresh,
    GiaoViecExtensionConfig.storageKeys.refreshInterval
], (result) => {
    const autoRefresh = result[GiaoViecExtensionConfig.storageKeys.autoRefresh];
    if (autoRefresh !== false) {
        const interval = result[GiaoViecExtensionConfig.storageKeys.refreshInterval]
            || GiaoViecExtensionConfig.refreshInterval;
        setInterval(updateBadge, interval);
    }
});
