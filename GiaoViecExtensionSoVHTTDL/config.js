(function initializeExtensionConfig(globalScope) {
    const eOfficeDomain = 'https://office.quangngai.gov.vn/';
    const giaoViecDomain = 'https://demo5.vnptquangngai.vn';
    const storageKeys = Object.freeze({
        authToken: 'authToken',
        autoRefresh: 'autoRefresh',
        refreshInterval: 'refreshInterval'
    });

    function getGiaoViecDomain() {
        return giaoViecDomain.replace(/\/+$/, '');
    }

    globalScope.GiaoViecExtensionConfig = Object.freeze({
        eOfficeDomain,
        eOfficeHostname: new URL(eOfficeDomain).hostname,
        giaoViecDomain,
        autoRefresh: true,
        refreshInterval: 300000,
        storageKeys,
        getGiaoViecDomain
    });
})(globalThis);
