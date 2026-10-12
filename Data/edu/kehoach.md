# KẾ HOẠCH THỰC HIỆN LOGIC MAP CỘT CHUYỂN ĐỔI DỮ LIỆU VNEDU

## I. KIẾN TRÚC VÀ QUY TRÌNH XỬ LÝ (WORKFLOW)
Hệ thống hoạt động dựa trên cơ chế cấu hình động. Người dùng chỉ cần cung cấp 1 file đầu vào và chọn tham số, hệ thống sẽ tự động áp dụng bộ quy tắc tương ứng.

1. **Đầu vào (Input):** Tải lên **01 file Excel** duy nhất chứa danh sách học sinh (file nguồn).
2. **Cấu hình tham số (Select Option):** Người dùng chọn Khối lớp tương ứng với file nguồn thông qua Dropdown menu: **Khối 1**, **Khối 6**, hoặc **Khối 10**.
3. **Xác định vùng dữ liệu (Data Boundary):** Hệ thống đọc file từ trên xuống, dò tìm tại **Cột B**. Dòng nào chứa text chính xác là `"STT"` sẽ được đánh dấu là dòng Tiêu đề (Header). Hệ thống chỉ bắt đầu lấy dữ liệu từ **dòng ngay sau dòng "STT" này**.
4. **Xử lý (Processing):** Dựa vào lựa chọn ở bước 2, hệ thống kích hoạt điều kiện lọc (trạng thái duyệt) và logic map cột tương ứng để trích xuất dữ liệu.
5. **Ghi dữ liệu (Writing):** Hệ thống sẽ mở file mẫu có sẵn tại đường dẫn cố định `\Temp\File_Import_hocsinh_vnEdu.xls`. Dữ liệu sau khi trích xuất và biến đổi sẽ được điền vào file này, **bắt đầu từ dòng số 9**.
6. **Đầu ra (Output):** File sau khi ghi xong sẽ được lưu thành một bản sao tạm (file temp) và hệ thống trả về đường dẫn để người dùng tải xuống, đảm bảo không ghi đè làm hỏng file mẫu gốc.

---

## II. CHI TIẾT LOGIC XỬ LÝ THEO TỪNG KHỐI LỚP

### 1. Nếu Select = "Khối 1"
- **Điều kiện tiền xử lý:** Quét **Cột AZ** của các dòng dữ liệu (sau dòng STT). Chỉ lấy các bản ghi có giá trị chính xác là `"Hồ sơ đã được xét duyệt"`.
- **Logic Map Cột:**

| STT | Cột ở File Excel Nguồn | Cột tương ứng ở File vnEdu Đích (`D://Import_to_edu.xlsx`) |
|---|---|---|
| 1 | Cột C | Cột G |
| 2 | Cột D | Cột J |
| 3 | Cột E | Cột H |
| 4 | Cột F | Cột V |
| 5 | Cột G | Cột T |
| 6 | Cột H | Cột Y |
| 7 | Cột I | Cột K |
| 8 | Cột J | Cột AE |
| 9 | Cột K | Cột AF |
| 10 | Cột M | Cột AU và Cột AY (Điền đồng thời) |
| 11 | Cột P | Cột O |
| 12 | Cột Q | Cột N |
| 13 | Cột T | Cột L |
| 14 | Cột U | Cột S |
| 15 | Cột V | Cột R |
| 16 | Cột Y | Cột P |
| 17 | Cột Z | Cột AK |
| 18 | Cột AA | Cột AW |
| 19 | Cột AC | Cột AM |
| 20 | Cột AD | Cột AL |
| 21 | Cột AF | Cột AP |
| 22 | Cột AG | Cột AX |
| 23 | Cột AI | Cột AR |
| 24 | Cột AJ | Cột AQ |
| 25 | Cột AW | Cột E |
| 26 | Cột AX | Cột AB |

---

### 2. Nếu Select = "Khối 6"
- **Điều kiện tiền xử lý:** Quét **Cột CX** của các dòng dữ liệu (sau dòng STT). Chỉ lấy các bản ghi có giá trị chính xác là `"Hồ sơ đã được xét duyệt"`.
- **Logic Map Cột:**

| STT | Cột ở File Excel Nguồn | Cột tương ứng ở File vnEdu Đích (`D://Import_to_edu.xlsx`) |
|---|---|---|
| 1 | Cột C | Cột G |
| 2 | Cột D | Cột J |
| 3 | Cột E | Cột H |
| 4 | Cột F | Cột V |
| 5 | Cột G | Cột T |
| 6 | Cột H | Cột Y |
| 7 | Cột I | Cột K |
| 8 | Cột J | Cột AE |
| 9 | Cột K | Cột AF |
| 10 | Cột M | Cột AU và Cột AY (Điền đồng thời) |
| 11 | Cột P | Cột O |
| 12 | Cột Q | Cột N |
| 13 | Cột T | Cột L |
| 14 | Cột U | Cột S |
| 15 | Cột V | Cột R |
| 16 | Cột Y | Cột P |
| 17 | Cột Z | Cột AK |
| 18 | Cột AA | Cột AW |
| 19 | Cột AC | Cột AM |
| 20 | Cột AD | Cột AL |
| 21 | Cột AF | Cột AP |
| 22 | Cột AG | Cột AX |
| 23 | Cột AI | Cột AR |
| 24 | Cột AJ | Cột AQ |
| 25 | Cột AW | Cột E |
| 26 | Cột AX | Cột AB |

---

### 3. Nếu Select = "Khối 10"
- **Điều kiện tiền xử lý:** Quét **Cột DN** của các dòng dữ liệu (sau dòng STT). Chỉ lấy các bản ghi có giá trị chính xác là `"Hồ sơ đã được xét duyệt"`.
- **Logic Map Cột:**

| STT | Cột ở File Excel Nguồn | Cột tương ứng ở File vnEdu Đích (`D://Import_to_edu.xlsx`) |
|---|---|---|
| 1 | Cột C | Cột E |
| 2 | Cột D | Cột AB |
| 3 | Cột E | Cột G |
| 4 | Cột F | Cột J |
| 5 | Cột G | Cột H |
| 6 | Cột H | Cột V |
| 7 | Cột I | Cột T |
| 8 | Cột J | Cột Y |
| 9 | Cột K | Cột K |
| 10 | Cột L | Cột AE |
| 11 | Cột M | Cột AF |
| 12 | Cột O | Cột AU và Cột AY (Điền đồng thời) |
| 13 | Cột R | Cột O |
| 14 | Cột S | Cột N |
| 15 | Cột X | Cột L |
| 16 | Cột Y | Cột S |
| 17 | Cột Z | Cột R |
| 18 | Cột AE | Cột P |
| 19 | Cột AF | Cột AK |
| 20 | Cột AH | Cột AW |
| 21 | Cột AI | Cột AL |
| 22 | Cột AL | Cột AP |
| 23 | Cột AN | Cột AX |
| 24 | Cột AO | Cột AQ |