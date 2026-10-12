// Content script - chay tren trang web

chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
    if (request.action === 'getAuthToken') {
        const token = localStorage.getItem('authToken')
            || localStorage.getItem('token')
            || sessionStorage.getItem('authToken')
            || sessionStorage.getItem('token');

        if (token) {
            chrome.runtime.sendMessage({
                action: 'setToken',
                token
            });
        }

        sendResponse({ token });
        return true;
    }

    if (request.action === 'injectQuickAction') {
        injectQuickActionButton();
        sendResponse({ success: true });
        return true;
    }
});

function injectQuickActionButton() {
    if (document.getElementById('giao-viec-extension-quick-btn')) {
        return;
    }

    const button = document.createElement('div');
    button.id = 'giao-viec-extension-quick-btn';
    button.innerHTML = `
        <button id="giao-viec-quick-btn" title="Giao Viec Extension">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
                <path d="M19 13h-6v6h-2v-6H5v-2h6V5h2v6h6v2z"/>
            </svg>
        </button>
    `;
    button.style.cssText = `
        position: fixed;
        bottom: 20px;
        right: 20px;
        z-index: 10000;
        background: #007bff;
        border-radius: 50%;
        width: 50px;
        height: 50px;
        display: flex;
        align-items: center;
        justify-content: center;
        box-shadow: 0 2px 10px rgba(0,0,0,0.3);
        cursor: pointer;
    `;

    button.querySelector('button').style.cssText = `
        background: transparent;
        border: none;
        color: white;
        cursor: pointer;
        padding: 0;
    `;

    button.addEventListener('click', () => {
        const giaoViecDomain = GiaoViecExtensionConfig.getGiaoViecDomain();
        const url = `${giaoViecDomain}/GiaoViec`;
        try {
            chrome.runtime.sendMessage({ action: 'openOrFocusTab', url });
        } catch (e) {
            window.open(url, '_blank');
        }
    });

    document.body.appendChild(button);
}

function ensureKDGPStyle() {
    if (document.getElementById('giao-viec-kdgp-style')) {
        return;
    }

    const style = document.createElement('style');
    style.id = 'giao-viec-kdgp-style';
    style.textContent = `
        @keyframes blink {
            0%, 100% {
                opacity: 1;
                box-shadow: 0 0 0 0 rgba(40, 167, 69, 0.7);
            }
            50% {
                opacity: 0.8;
                box-shadow: 0 0 0 8px rgba(40, 167, 69, 0);
            }
        }

        #btn_tao_giaoviec_kdgp {
            position: relative;
            animation: blink 1.5s ease-in-out infinite;
            background-color: #28a745 !important;
            border-color: #28a745 !important;
            color: #ffffff !important;
        }

        #btn_tao_giaoviec_kdgp:hover,
        #btn_tao_giaoviec_kdgp:focus,
        #btn_tao_giaoviec_kdgp:active {
            animation: none;
            background-color: #218838 !important;
            border-color: #1e7e34 !important;
            color: #ffffff !important;
            opacity: 1;
        }

        #giao-viec-ykien-row td {
            vertical-align: top;
        }

        #giao-viec-ykien-cell {
            padding: 10px 12px;
            background: #f8fbff;
            color: #1f2937;
            font-size: 13px;
            line-height: 1.5;
        }

        #giao-viec-ykien-cell .ykien-item + .ykien-item {
            margin-top: 10px;
            padding-top: 10px;
            border-top: 1px solid #d9e2ef;
        }

        #giao-viec-ykien-cell .ykien-meta {
            margin-bottom: 4px;
            font-weight: 600;
        }

        #giao-viec-ykien-cell .ykien-time {
            margin-left: 8px;
            color: #4b5563;
            font-weight: 400;
        }

        #giao-viec-ykien-cell .ykien-content {
            white-space: pre-line;
            color: #111827;
        }

        #giao-viec-ykien-cell .ykien-empty {
            color: #6b7280;
        }
    `;

    document.head.appendChild(style);
}

function getIofficeNumber() {
    const primaryElement = document.querySelector('#dt_thongtin_vanban > tbody > tr:nth-child(1) > td:nth-child(2) > span');
    if (primaryElement) {
        const value = primaryElement.textContent?.trim() || primaryElement.innerText?.trim() || '';
        if (value) {
            return value;
        }
    }

    const urlParams = new URLSearchParams(window.location.search);
    const iofficeNumber = urlParams.get('iofficeNumber') || urlParams.get('id') || '';
    if (iofficeNumber) {
        return iofficeNumber.trim();
    }

    const selectors = [
        '.ioffice_number',
        '#ioffice_number',
        '[class*="ioffice"]',
        '[id*="ioffice"]',
        '[data-ioffice]',
        'input[name*="ioffice"]',
        'input[id*="ioffice"]'
    ];

    for (const selector of selectors) {
        const element = document.querySelector(selector);
        if (!element) {
            continue;
        }

        const value = element.textContent?.trim()
            || element.value?.trim()
            || element.getAttribute('data-ioffice')?.trim()
            || element.innerText?.trim()
            || '';

        if (value) {
            return value;
        }
    }

    return '';
}

function getVanBanId() {
    const idElement = document.getElementById('idVB') || document.querySelector('input#idVB');
    if (idElement) {
        const value = idElement.value?.trim() || idElement.getAttribute('value')?.trim() || '';
        if (value) {
            return value;
        }
    }

    const selectors = [
        '#idVB',
        'input[name="idVB"]',
        'input[id*="idVB"]',
        '[data-vanban-id]',
        '#vanBanId'
    ];

    for (const selector of selectors) {
        const element = document.querySelector(selector);
        if (!element) {
            continue;
        }

        const value = element.value?.trim()
            || element.getAttribute('value')?.trim()
            || element.textContent?.trim()
            || '';

        if (value) {
            return value;
        }
    }

    return '';
}

function getLatestYkienItems() {
    const tables = Array.from(document.querySelectorAll('table#dt_activiti_log'));
    const lastTable = tables[tables.length - 1];
    if (!lastTable) {
        return [];
    }

    const rows = Array.from(lastTable.querySelectorAll('tbody > tr.log_role_type_received'));

    return rows
        .slice(-2)
        .reverse()
        .map((row) => {
            const nameCell = row.querySelector('td[name="name_"]');
            const timeCell = Array.from(row.querySelectorAll('td')).find((td) => {
                const text = (td.textContent || '').trim();
                return td.getAttribute('style')?.includes('text-align:center')
                    && /\d{2}\/\d{2}\/\d{4}\s+\d{2}:\d{2}/.test(text);
            });
            const commentNode = row.querySelector('span[name="comment"][id^="comment_"]');

            const sender = (nameCell?.childNodes[0]?.textContent || nameCell?.textContent || '').trim();
            const time = (timeCell?.textContent || '').trim();
            const content = (commentNode?.textContent || '')
                .replace(/\u00a0/g, ' ')
                .trim();

            return { sender, time, content };
        })
        .filter((item) => item.sender || item.time || item.content);
}

function renderYkienSection() {
    const infoTableBody = document.querySelector('#dt_thongtin_vanban > tbody');
    if (!infoTableBody) {
        return;
    }

    let row = document.getElementById('giao-viec-ykien-row');
    if (!row) {
        row = document.createElement('tr');
        row.id = 'giao-viec-ykien-row';
        row.innerHTML = `
            <td><b>Y kien chi dao</b></td>
            <td id="giao-viec-ykien-cell" colspan="5"></td>
        `;
        infoTableBody.appendChild(row);
    }

    const cell = document.getElementById('giao-viec-ykien-cell');
    if (!cell) {
        return;
    }

    const items = getLatestYkienItems();
    if (!items.length) {
        cell.innerHTML = '<div class="ykien-empty">Chua lay duoc du lieu tu bang y kien.</div>';
        return;
    }

    cell.innerHTML = `
        ${items.map((item) => `
            <div class="ykien-item">
                <div class="ykien-meta">
                    <span class="ykien-name">${item.sender || 'Khong xac dinh'}</span>
                    ${item.time ? `<span class="ykien-time">${item.time}</span>` : ''}
                </div>
                <div class="ykien-content">${item.content || 'Khong co noi dung'}</div>
            </div>
        `).join('')}
    `;
}

function findButtonContainer() {
    const selectors = [
        '//*[@id="div_thongtin_chitiet"]/article/div[1]',
    ];

    for (const selector of selectors) {
        let container = null;
        if (selector.startsWith('/')) {
            const result = document.evaluate(selector, document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null);
            container = result.singleNodeValue;
        } else {
            container = document.querySelector(selector);
        }

        if (container) {
            return container;
        }
    }
    return null;
}

function injectTaoGiaoViecKDGPButton() {
    const listButtonDiv = findButtonContainer();
    if (!listButtonDiv) {
        return;
    }

    ensureKDGPStyle();

    let button = document.getElementById('btn_tao_giaoviec_kdgp');
    if (!button || !listButtonDiv.contains(button)) {
        if (!button) {
            button = document.createElement('button');
            button.id = 'btn_tao_giaoviec_kdgp';
            button.className = 'btn btn-success btn-sm class_xuly_uyquyen';
            button.style.setProperty('background-color', '#28a745', 'important');
            button.style.setProperty('border-color', '#28a745', 'important');
            button.style.setProperty('color', '#ffffff', 'important');
            button.style.marginRight = '5px';
            button.innerHTML = '<span class="fa fa-paper-plane"></span> Tạo nhiệm vụ';
            button.title = 'Tạo nhiệm vụ';

            button.addEventListener('click', () => {
                const giaoViecDomain = GiaoViecExtensionConfig.getGiaoViecDomain();
                const soKyHieu = document.getElementById('detailSoKyHieu')?.textContent?.trim()
                    || document.querySelector('[id*="SoKyHieu"]')?.textContent?.trim()
                    || document.querySelector('.so-ky-hieu')?.textContent?.trim()
                    || '';
                const trichYeu = document.getElementById('detailTrichYeu')?.textContent?.trim()
                    || document.querySelector('[id*="TrichYeu"]')?.textContent?.trim()
                    || document.querySelector('.trich-yeu')?.textContent?.trim()
                    || '';
                const iofficeNumber = getIofficeNumber();
                const vanBanId = getVanBanId();

                const params = new URLSearchParams();
                if (soKyHieu) params.append('soKyHieu', soKyHieu);
                if (trichYeu) params.append('trichYeu', trichYeu);
                if (iofficeNumber) params.append('iofficeNumber', iofficeNumber);
                if (vanBanId) params.append('vanBanId', vanBanId);

                const url = `${giaoViecDomain}/GiaoViec?id=0&${params.toString()}`;
                try {
                    chrome.runtime.sendMessage({ action: 'openOrFocusTab', url });
                } catch (e) {
                    window.open(url, '_blank');
                }
            });
        }

        listButtonDiv.appendChild(button);
    }

    renderYkienSection();
}

if (window.location.hostname.includes(GiaoViecExtensionConfig.eOfficeHostname) || window.location.hostname === GiaoViecExtensionConfig.eOfficeHostname) {
    const initExtension = () => {
        setTimeout(() => {
            chrome.runtime.sendMessage({ action: 'getAuthToken' });
            injectTaoGiaoViecKDGPButton();
        }, 800);
    };

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initExtension);
    } else {
        initExtension();
    }

    setInterval(() => {
        injectTaoGiaoViecKDGPButton();
    }, 1500);

    const observer = new MutationObserver((mutations) => {
        let shouldRefresh = false;

        for (const mutation of mutations) {
            if (!mutation.addedNodes.length) {
                continue;
            }

            const hasRelevantNode = Array.from(mutation.addedNodes).some((node) => {
                if (node.nodeType !== 1) {
                    return false;
                }

                return node.id === 'div_thongtin_chitiet'
                    || node.classList?.contains('list-button')
                    || node.matches?.('#div_thongtin_chitiet, .list-button, #dt_thongtin_vanban, table#dt_activiti_log, tr.log_role_type_received')
                    || node.querySelector?.('#div_thongtin_chitiet, .list-button, #dt_thongtin_vanban, table#dt_activiti_log, tr.log_role_type_received');
            });

            if (hasRelevantNode) {
                shouldRefresh = true;
                break;
            }
        }

        if (shouldRefresh) {
            setTimeout(() => {
                injectTaoGiaoViecKDGPButton();
            }, 300);
        }
    });

    observer.observe(document.body, {
        childList: true,
        subtree: true
    });
}
