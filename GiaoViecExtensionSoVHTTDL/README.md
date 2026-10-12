# Giao Việc Extension

Chrome Extension hỗ trợ quản lý công việc từ trình duyệt.

## Cài đặt

1. Mở Chrome và truy cập `chrome://extensions/`
2. Bật "Chế độ dành cho nhà phát triển" (Developer mode)
3. Click "Tải tiện ích đã giải nén" (Load unpacked)
4. Chọn thư mục `GiaoViecExtension`

## Cấu hình

### 1. Cấu hình Domain

**Lưu ý quan trọng**: Extension chỉ hoạt động trên trang `https://office.quangngai.gov.vn/`. Content script chỉ chạy trên domain này.

Tuy nhiên, bạn có thể cấu hình domain của trang giao việc:

1. Click vào icon extension
2. Click vào nút **⚙️ (Cấu hình)** ở góc dưới bên trái
3. Nhập domain trang giao việc (ví dụ: `https://ttkdgp.vnptquangngai.vn` hoặc `https://eoffice.vnpt.vn`)
4. Click **Lưu cấu hình**

Domain này sẽ được sử dụng khi:
- Click button "Tạo giao việc KDGP" trên trang văn bản
- Click button "Mở ứng dụng" trong popup

### 2. Tạo Icons

Tạo các file icon trong thư mục `icons/`:
- `icon16.png` (16x16 pixels)
- `icon48.png` (48x48 pixels)
- `icon128.png` (128x128 pixels)

Hoặc sử dụng tool online để tạo icons từ một hình ảnh.

### 2. Cấu hình API Endpoints

Các endpoint đã được cấu hình trong `popup.js`:
```javascript
const API_ENDPOINTS = {
    getCongViecCanXuLy: '/GiaoViecCanXuLy/GetAll',
    getCongViecDaGiao: '/GiaoViec/GetListDaGiao'
};
```

Nếu cần thay đổi, chỉnh sửa trong file `popup.js`.

## Tính năng

- **Cấu hình domain linh hoạt**: Có thể cấu hình domain tùy chỉnh (mặc định: https://office.quangngai.gov.vn)
- Xem danh sách công việc cần xử lý
- Xem danh sách công việc đã giao
- Làm mới dữ liệu
- Mở ứng dụng trong tab mới
- Tự động lấy token xác thực từ trang web
- Tự động inject nút "Tạo giao việc KDGP" vào trang văn bản

## Phát triển

### Cấu trúc file

- `manifest.json` - Cấu hình extension
- `popup.html` - Giao diện popup
- `popup.js` - Logic popup
- `background.js` - Service worker
- `content.js` - Script chạy trên trang web
- `styles.css` - CSS cho popup
- `icons/` - Thư mục chứa icons

### Lưu ý

- Extension sử dụng Manifest V3
- Token được lưu trong `chrome.storage.sync`
- Content script tự động lấy token từ localStorage của trang web
- Cần cấu hình CORS trên server để cho phép extension gọi API

## License

Internal use only
