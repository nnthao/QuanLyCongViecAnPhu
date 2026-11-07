---
alwaysApply: true
---

- nếu có nhiều dòng liên tiếp trống thì xoá hết chỉ để lại 1 dòng trống giữa các method funtion, ở các file có sự thay đổi nội dung
- code theo  nguyên tắc SOLID để clean code: Single responsibility priciple (SRP), Open/Closed principle (OCP), Liskov substitution principe (LSP), Interface segregation principle (ISP), Dependency inversion principle (DIP). Nếu có code dùng lại nhiều hãy viết 1 method dùng chung
- đặt tên biến có ý nghĩa để người review code hiểu được
- Xoá các method không dùng ở các file khi thay đổi để code sạch hơn thêm method vừa tạo nếu class có class interface nữa
- Khi tạo file mới (entity, model, repository, service, controller, views, v.v.) PHẢI thêm file đó vào file .csproj của dự án. Luôn kiểm tra và cập nhật file .csproj khi tạo file mới
- ĐẶC BIỆT: Khi tạo file .sql (trong folder /DataScripts) hoặc file .cshtml (trong folder /Views), BẮT BUỘC phải thêm vào file .csproj ngay lập tức. File .sql thêm vào phần DataScripts, file .cshtml thêm vào phần Views tương ứng trong .csproj
- Không tự sửa các method không liên quan đến chức năng đang làm
- Trả lời nội dùng bằng tiếng việt
- chỉnh lại định dang thụt các dòng ra vào cho đúng định dạng code chuẩn cho dễ nhìn. không cần run dotnet format với các file có thay đổi nội dung. xoá các code bị comment lại cho sạch file
- Tạo file script chnhr sửa database tham khảo cac file trong folder /DataScripts
- Code các chức năng theo chức năng quản lý User ở folder /Views/User/ 
- Thay đổi giá trị các bảng trong CacheManagerHelper nhớ gọi lại hàm init danh sách vào cache để cache luôn mới
- Không bao giờ validate ở Frontend (FE), chỉ validate ở Backend (BE). Frontend chỉ gửi dữ liệu lên server và hiển thị lỗi từ backend trả về
- thay method ajax method GET bằng ajaxGet và ajax method POST bằng ajaxPostFormData nhưu trong /Views/User/Form.cshtml
- Nếu dữ liệu ở Views không đúng hoặc không hiển thị, BẮT BUỘC phải kiểm tra và truy vết luồng dữ liệu từ View lên Backend (Controller -> Service -> Repository -> Data/Cache) xem Server đã trả về đúng dữ liệu chưa, không được tự ý làm fallback tạm thời ở Frontend.
- dùng code jqurery  có thể để gọn code thay vì javascript thuần
- Không dùng $(function () { }) (ready của jQuery); dùng DOMContentLoaded của JavaScript thuần. Ví dụ: document.addEventListener("DOMContentLoaded", function () { ... });
- chỉnh sửa ui nhưung không thay dổi bố cục grid bootstrap 4
- Nếu nhiệm vụ chỉnh sửa ui thì chỉ chỉnh code html thôi, không làm việc khác nhưu chỉnh code js hay code backend
- Không dùng kiểu Dictionary<string, List<string>> hãy dùng kiểu List<TenClass>. TenClass định nghĩa các trường cho rõ nghĩa 
- jquery ghép chuỗi dùng template literal. ví dụ: `Người chuyển giao:</b> ${row.NguoiChuyenGiao}` thay vì 'Người chuyển giao:</b>' + row.NguoiChuyenGiao
- trong Task.Run khi láy dữu liệu từ DB phải dùng ado.net
- lấy trong forach có lấy dữ liệu từ DB thì nên láy dữu liệu 1 lân trước khi foreach
- Khi sửa code của method thì kiểm tra code cũ không liên quan thì xoá cho sạch
- boostrap 4 grid chỉ cùng keieur col-md-.., col-sm-.. và col-xs-12 thôi
- Khi tạo message commit code lên github , hiện tiếng việt giúp tôi
- Các method static cần xem xét có thể đưa vào các file trong folder /Common được không? Nếu được thì di chuyển để file có nội dung ngắn lại, dễ nhìn hơn. Ưu tiên đưa vào file Common phù hợp với chức năng của method
- Nếu một trường int cho phép null (int?) thì dùng .GetValueOrDefault() để kiểm tra và lấy giá trị. Nếu .GetValueOrDefault() khác 0 thì truy vấn theo trường đó, nếu bằng 0 thì bỏ qua điều kiện filter đó
- Khi tạo file script SQL trong folder /DataScripts, tên file phải có format YYYYMMDDHHmmss_mota.sql (ví dụ: 20251124221417_them_IsTuGiao.sql) và phải lấy thời gian hiện tại (Get-Date -Format 'yyyyMMddHHmmss') làm timestamp, không được dùng thời gian cố định. Giá trị trong INSERT INTO DatabaseVersion(Name) cũng phải khớp với timestamp trong tên file
- KHÔNG BAO GIỜ dùng lệnh GO trong file script SQL. Tất cả các câu lệnh SQL phải được viết liên tiếp không có GO. Nếu cần thực thi nhiều câu lệnh riêng biệt, hãy sử dụng BEGIN...END block hoặc viết các câu lệnh tuần tự
- Viết script SQL trong folder /DataScripts phải đơn giản, KHÔNG tạo khóa ngoại (foreign key/REFERENCES), KHÔNG tạo index
- Frontend (FE) chỉ có nhiệm vụ gửi dữ liệu lên Backend (BE) và hiển thị kết quả trả về từ BE. Tất cả validation, kiểm tra logic, xử lý nghiệp vụ đều phải thực hiện ở BE. FE không được thực hiện validation hoặc kiểm tra logic nghiệp vụ (như kiểm tra phòng ban có trưởng phòng không, kiểm tra dữ liệu hợp lệ, tìm thành viên gốc, filter dữ liệu, v.v.). Nếu có lỗi, BE sẽ trả về và FE chỉ hiển thị lỗi đó cho người dùng. FE chỉ map dữ liệu từ BE vào các field trên form để hiển thị, không có logic xử lý nghiệp vụ
- Nếu prompt của tôi có chứa chữ "plan" (hoặc "kế hoạch") thì bạn hãy lên kế hoạch trước (dùng CreatePlan / plan mode), không code ngay; chỉ khi tôi xác nhận hoặc yêu cầu thực hiện thì mới bắt đầu viết code
- Khi lên plan (kế hoạch), luôn tạo file .md (ví dụ trong .cursor/plans/ hoặc thư mục phù hợp) ghi rõ mục tiêu, các bước, file cần sửa/tạo để tôi dễ hình dung và theo dõi
- Sau khi tạo xong file plan .md, luôn ghi rõ đường dẫn đầy đủ tới file đó (ví dụ: .cursor/plans/ten_plan.md hoặc đường dẫn tuyệt đối workspace) để tôi có thể mở nhanh (Ctrl+P rồi dán đường dẫn, hoặc click vào đường dẫn nếu IDE hiển thị link)
- Nếu method (Controller, Service, Repository) có nhiều hơn 1 tham số thì tạo class ViewModel/Request chứa các tham số đó, truyền 1 object vào method để dễ bảo trì và mở rộng sau này
- Khi kế thừa class (inheritance), KHÔNG được khai báo lại các trường (property, field) đã có trong class cha. Phải kiểm tra class cha có những trường nào trước khi khai báo trong class con. Nếu cần override với kiểu dữ liệu khác, chỉ override khi thực sự cần thiết và phải có comment giải thích lý do
- FE không được tự chặn submit vì lỗi nhập liệu. FE chỉ lấy dữ liệu người dùng đã nhập/chọn, gửi lên BE, rồi hiển thị thông báo lỗi hoặc kết quả do BE trả về.
- Khi trả lời sau khi code xong, không cần liệt kê chi tiết code thay đổi ở dòng nào. Chỉ tóm tắt ngắn gọn những gì đã làm, kết quả kiểm tra/build, và lưu ý quan trọng nếu có vì tôi đã có tool để review diff.
- Khi tạo mới hoặc cập nhật source code có chứa chuỗi ký tự tiếng Việt, luôn đảm bảo file được ghi xuống/lưu trữ ở định dạng chuẩn bảng mã UTF-8 (ưu tiên UTF-8 with BOM). Không sử dụng định dạng ANSI.
- Các modal luôn được tạo thành file partial view (.cshtml) riêng biệt, chứa đầy đủ code HTML, CSS và JavaScript liên quan trực tiếp đến modal đó ngay bên trong file partial.
- Quy chuẩn thiết kế giao diện Form / Modal (chuẩn theo hình ảnh /Data/UI/Form.png và /Views/User/FormModal.cshtml):
    + Tuyệt đối KHÔNG màu mè, không lạm dụng các class màu sắc lòe loẹt (text-primary, text-info, text-danger, text-secondary) trên các nhãn text hiển thị thông tin hoặc gắn nhiều icon FontAwesome không cần thiết.
    + Modal Header: Sử dụng nền `bg-primary text-white`, tiêu đề `<h5><b>...</b></h5>` KHÔNG gắn icon, nút đóng màu trắng `<button type="button" class="close text-white" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>`.
    + Phân cách giữa các nhóm thông tin/section: Dùng `<div class="txt-middle-line"><span><b>Tiêu đề section</b></span></div>`, KHÔNG dùng các card bao lồng màu mè (`card card-outline card-info`, `card-secondary`,...).
    + Bố cục bên trong modal-body: Thiết kế phẳng, gọn gàng, sử dụng grid Bootstrap 4 (`col-sm-.. col-xs-12` hoặc `col-md-..`).
    + Modal Footer: Nút đóng bên trái dùng `btn btn-light` với `<i class="fa fa-times"></i> Đóng`; Nút hành động chính (Lưu/Thêm mới/Cập nhật) bên phải dùng `btn btn-success` với `<i class="fa fa-save"></i>` hoặc icon hành động tương ứng.

# 6. Clean Code & Formatting
# QUY TẮC PHÁT TRIỂN MÃ NGUỒN (SOLID PRINCIPLES)
## Mục tiêu: Đảm bảo code dễ bảo trì, dễ mở rộng và có tính linh hoạt cao.

### 1. SRP: Single Responsibility Principle (Nguyên tắc đơn trách nhiệm)
- **Định nghĩa:** Mỗi class, module hoặc function chỉ nên có một lý do duy nhất để thay đổi.
- **Yêu cầu đối với AI:** - Phải tách biệt các logic xử lý khác nhau (ví dụ: tách biệt xử lý dữ liệu và gửi thông báo).
    - Tránh tạo ra các "God Object" (một class làm quá nhiều việc).

### 2. OCP: Open/Closed Principle (Nguyên tắc Đóng/Mở)
- **Định nghĩa:** Thực thể phần mềm nên mở rộng cho việc phát triển thêm, nhưng đóng đối với việc sửa đổi trực tiếp.
- **Yêu cầu đối với AI:** - Ưu tiên sử dụng tính kế thừa (Inheritance) hoặc đa hình (Polymorphism).
    - Khi thêm tính năng mới, hãy tạo class mới thay vì sửa đổi logic bên trong class hiện tại.

### 3. LSP: Liskov Substitution Principle (Nguyên tắc thay thế Liskov)
- **Định nghĩa:** Các đối tượng của lớp con phải có khả năng thay thế lớp cha mà không làm thay đổi tính đúng đắn của chương trình.
- **Yêu cầu đối với AI:** - Đảm bảo các lớp con giữ đúng hành vi mà lớp cha đã định nghĩa.
    - Không được ném ra ngoại lệ (Exception) cho các phương thức mà lớp cha hỗ trợ.

### 4. ISP: Interface Segregation Principle (Nguyên tắc phân tách Interface)
- **Định nghĩa:** Thay vì một Interface lớn, hãy chia thành nhiều Interface nhỏ với các mục đích cụ thể.
- **Yêu cầu đối với AI:** - Không ép buộc một class phải implement những phương thức mà nó không sử dụng.
    - Thiết kế các Interface tinh gọn, tập trung vào một nhóm chức năng nhất định.

### 5. DIP: Dependency Inversion Principle (Nguyên tắc đảo ngược phụ thuộc)
- **Định nghĩa:** Module cấp cao không được phụ thuộc vào module cấp thấp. Cả hai phải phụ thuộc vào sự trừu tượng (Abstraction).
- **Yêu cầu đối với AI:** - Sử dụng Dependency Injection (DI).
    - Luôn lập trình thông qua Interface/Abstract class thay vì các lớp thực thi (Concrete class) cụ thể.
- **Cleanup:**
    - Xoá các method không còn sử dụng.
    - Xoá code đã bị comment (commented-out code).
    - Chỉ để lại **1 dòng trống** giữa các method. Xoá các dòng trống thừa liên tiếp.
- **Naming:** Đặt tên biến/hàm rõ nghĩa, tiếng Anh chuẩn.
- **Formatting:** Thụt đầu dòng (indentation) chuẩn xác, dễ nhìn. Không cần chạy lệnh `dotnet format`.
