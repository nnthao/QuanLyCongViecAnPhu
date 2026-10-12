using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class QuanLyCongTrinhHuHongController : Controller
    {
        private readonly IQuanLyCongTrinhHuHongService _quanLyCongTrinhHuHongService;
        private readonly IQuanLyCongTrinhHuHongChiTietService _quanLyCongTrinhHuHongChiTietService;
        private readonly IQuanLyCongTrinhHuHongFileService _quanLyCongTrinhHuHongFileService;
        private readonly IDanhSachCongTrinhService _danhSachCongTrinhService;
        
        public QuanLyCongTrinhHuHongController(
            IQuanLyCongTrinhHuHongService quanLyCongTrinhHuHongService,
            IQuanLyCongTrinhHuHongChiTietService quanLyCongTrinhHuHongChiTietService,
            IQuanLyCongTrinhHuHongFileService quanLyCongTrinhHuHongFileService,
            IDanhSachCongTrinhService danhSachCongTrinhService)
        {
            _quanLyCongTrinhHuHongService = quanLyCongTrinhHuHongService;
            _quanLyCongTrinhHuHongChiTietService = quanLyCongTrinhHuHongChiTietService;
            _quanLyCongTrinhHuHongFileService = quanLyCongTrinhHuHongFileService;
            _danhSachCongTrinhService = danhSachCongTrinhService;
        }

        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult GetListQuanLyCongTrinhHuHong(QueryParamViewModel queryParams)
        {
            var data = _quanLyCongTrinhHuHongService.GetAll(queryParams);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult ThemOrCapNhat(QuanLyCongTrinhHuHongViewModel quanLyCongTrinhHuHongViewModel)
        {
            var data = _quanLyCongTrinhHuHongService.ThemOrCapNhat(quanLyCongTrinhHuHongViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult Xoa(int Id)
        {
            var data = _quanLyCongTrinhHuHongService.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult KhoiPhuc(int Id)
        {
            var data = _quanLyCongTrinhHuHongService.KhoiPhuc(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult Form(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public JsonResult GetById(int id)
        {
            var data = _quanLyCongTrinhHuHongService.GetById(id);

            return Json(new
            {
                Item = data,
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public JsonResult QuanLyCongTrinhHuHongLookup()
        {
            var data = _quanLyCongTrinhHuHongService.QuanLyCongTrinhHuHongLookup(new LookupQueryViewModel());

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // Chi tiết công trình hư hỏng
        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult ChiTiet(int congTrinhHuHongId)
        {
            ViewBag.CongTrinhHuHongId = congTrinhHuHongId;
            return View();
        }

        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult GetListChiTiet(QueryParamViewModel queryParams)
        {
            var data = _quanLyCongTrinhHuHongChiTietService.GetAll(queryParams);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult ThemOrCapNhatChiTiet(QuanLyCongTrinhHuHongChiTietViewModel quanLyCongTrinhHuHongChiTietViewModel)
        {
            var data = _quanLyCongTrinhHuHongChiTietService.ThemOrCapNhat(quanLyCongTrinhHuHongChiTietViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult XoaChiTiet(int Id)
        {
            var data = _quanLyCongTrinhHuHongChiTietService.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult FormChiTiet(int id = 0, int congTrinhHuHongId = 0)
        {
            ViewBag.CongTrinhHuHongId = congTrinhHuHongId;
            return View(id);
        }

        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public JsonResult GetChiTietById(int id)
        {
            var data = _quanLyCongTrinhHuHongChiTietService.GetById(id);

            return Json(new
            {
                Item = data,
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public JsonResult GetChiTietByCongTrinhHuHongId(int congTrinhHuHongId)
        {
            var data = _quanLyCongTrinhHuHongChiTietService.GetByCongTrinhHuHongId(congTrinhHuHongId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // File công trình hư hỏng
        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult File(int congTrinhHuHongId)
        {
            ViewBag.CongTrinhHuHongId = congTrinhHuHongId;
            return View();
        }

        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult GetListFile(QueryParamViewModel queryParams)
        {
            var data = _quanLyCongTrinhHuHongFileService.GetAll(queryParams);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult ThemOrCapNhatFile(QuanLyCongTrinhHuHongFileViewModel quanLyCongTrinhHuHongFileViewModel)
        {
            var data = _quanLyCongTrinhHuHongFileService.ThemOrCapNhat(quanLyCongTrinhHuHongFileViewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult XoaFile(int Id)
        {
            var data = _quanLyCongTrinhHuHongFileService.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult FormFile(int id = 0, int congTrinhHuHongId = 0)
        {
            ViewBag.CongTrinhHuHongId = congTrinhHuHongId;
            return View(id);
        }

        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public JsonResult GetFileById(int id)
        {
            var data = _quanLyCongTrinhHuHongFileService.GetById(id);

            return Json(new
            {
                Item = data,
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public JsonResult GetFileByCongTrinhHuHongId(int congTrinhHuHongId)
        {
            var data = _quanLyCongTrinhHuHongFileService.GetByCongTrinhHuHongId(congTrinhHuHongId);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        // Công trình hư hỏng - Damage Management
        public ActionResult CongTrinhHuHong()
        {
            return View();
        }

        public ActionResult GetListCongTrinhHuHong(QueryParamViewModel queryParams)
        {
            var data = _quanLyCongTrinhHuHongService.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FormCongTrinhHuHong(int id = 0)
        {
            return View(id);
        }

        public JsonResult GetCongTrinhHuHongById(int id)
        {
            var data = _quanLyCongTrinhHuHongService.GetById(id);
            return Json(new { Item = data }, JsonRequestBehavior.AllowGet);
        }

        // Thống kê, Báo cáo
        public ActionResult ThongKe()
        {
            return View();
        }

        public JsonResult TestData()
        {
            try
            {
                var queryParams = new QueryParamViewModel();
                var allData = _danhSachCongTrinhService.GetAll(queryParams);
                
                var result = new
                {
                    total = allData.total,
                    rowsCount = allData.rows.Count,
                    data = allData.rows.Select(x => new
                    {
                        Id = x.Id,
                        TenCongTrinh = x.TenCongTrinh,
                        LoaiCongTrinh = x.LoaiCongTrinh,
                        Diadiem = x.Diadiem
                    }).ToList()
                };
                
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetThongKeTheoLoaiCongTrinh()
        {
            try
            {
                // Lấy toàn bộ dữ liệu (bỏ phân trang) để thống kê chính xác
                var queryParams = new QueryParamViewModel
                {
                    offset = 0,
                    limit = int.MaxValue
                };
                var allData = _danhSachCongTrinhService.GetAll(queryParams);
                
                // Debug: Log dữ liệu để kiểm tra
                System.Diagnostics.Debug.WriteLine($"Total records: {allData.total}");
                System.Diagnostics.Debug.WriteLine($"Rows count: {allData.rows.Count}");
                
                if (allData.rows.Count > 0)
                {
                    foreach (var row in allData.rows.Take(5))
                    {
                        System.Diagnostics.Debug.WriteLine($"ID: {row.Id}, TenCongTrinh: {row.TenCongTrinh}, LoaiCongTrinh: {row.LoaiCongTrinh}");
                    }
                }
                
                // Thống kê theo loại công trình
                var loaiCongTrinhStats = allData.rows
                    .GroupBy(x => x.LoaiCongTrinh)
                    .Select(g => new
                    {
                        LoaiCongTrinh = g.Key,
                        Count = g.Count(),
                        Percentage = Math.Round((double)g.Count() / allData.total * 100, 2)
                    })
                    .OrderBy(x => x.LoaiCongTrinh)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"LoaiCongTrinhStats count: {loaiCongTrinhStats.Count}");

                var labels = new List<string>();
                var data = new List<int>();
                var backgroundColor = new List<string>();
                
                var loaiCongTrinhNames = new Dictionary<int?, string>
                {
                    { 1, "Hồ" },
                    { 2, "Đập Dâng" },
                    { 3, "Đập Ngăn Mặn" },
                    { 4, "Trạm Khí Tượng" },
                    { 5, "Trạm Thủy Văn" }
                };

                // Màu sắc cho từng loại công trình
                var colorMap = new Dictionary<int?, string>
                {
                    { 1, "#FF6384" },      // Hồ - Màu đỏ hồng
                    { 2, "#36A2EB" },      // Đập Dâng - Màu xanh dương
                    { 3, "#FFCE56" },      // Đập Ngăn Mặn - Màu vàng
                    { 4, "#4BC0C0" },      // Trạm Khí Tượng - Màu xanh lá
                    { 5, "#9966FF" }       // Trạm Thủy Văn - Màu tím
                };

                for (int i = 0; i < loaiCongTrinhStats.Count; i++)
                {
                    var stat = loaiCongTrinhStats[i];
                    labels.Add(loaiCongTrinhNames.ContainsKey(stat.LoaiCongTrinh) ? 
                        loaiCongTrinhNames[stat.LoaiCongTrinh] : $"Loại {stat.LoaiCongTrinh}");
                    data.Add(stat.Count);
                    backgroundColor.Add(colorMap.ContainsKey(stat.LoaiCongTrinh) ? 
                        colorMap[stat.LoaiCongTrinh] : "#FF9F40");
                    
                    System.Diagnostics.Debug.WriteLine($"Added: {stat.LoaiCongTrinh} -> {stat.Count}");
                }

                // Nếu không có dữ liệu, tạo dữ liệu mẫu với màu sắc
                if (labels.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("No data found, using sample data");
                    labels = new List<string> { "Hồ", "Đập Dâng", "Đập Ngăn Mặn", "Trạm Khí Tượng", "Trạm Thủy Văn" };
                    data = new List<int> { 0, 0, 0, 0, 0 }; // Thay đổi thành 0 để hiển thị không có dữ liệu
                    backgroundColor = new List<string> { "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF" };
                }

                var result = new
                {
                    labels = labels.ToArray(),
                    datasets = new[]
                    {
                        new
                        {
                            label = "Số lượng công trình",
                            data = data.ToArray(),
                            backgroundColor = backgroundColor.ToArray()
                        }
                    }
                };
                
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetThongKeTheoLoaiCongTrinh: {ex.Message}");
                
                // Fallback data nếu có lỗi
                var fallbackResult = new
                {
                    labels = new[] { "Hồ", "Đập Dâng", "Đập Ngăn Mặn", "Trạm Khí Tượng", "Trạm Thủy Văn" },
                    datasets = new[]
                    {
                        new
                        {
                            label = "Số lượng công trình",
                            data = new[] { 0, 0, 0, 0, 0 },
                            backgroundColor = new[] { "#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0", "#9966FF" }
                        }
                    }
                };
                return Json(fallbackResult, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetThongKeTheoMucDoHuHong()
        {
            try
            {
                // Lấy toàn bộ dữ liệu (bỏ phân trang) để thống kê chính xác
                var queryParams = new QueryParamViewModel
                {
                    offset = 0,
                    limit = int.MaxValue
                };
                var allData = _danhSachCongTrinhService.GetAll(queryParams);
                
                // Debug: Log dữ liệu để kiểm tra
                System.Diagnostics.Debug.WriteLine($"Total records for MucDoHuHong: {allData.total}");
                System.Diagnostics.Debug.WriteLine($"Rows with MucDoHuHong: {allData.rows.Count(x => !string.IsNullOrEmpty(x.MucDoHuHong))}");
                
                // Thống kê theo mức độ hư hỏng
                var mucDoHuHongStats = allData.rows
                    .Where(x => !string.IsNullOrEmpty(x.MucDoHuHong))
                    .GroupBy(x => x.MucDoHuHong)
                    .Select(g => new
                    {
                        MucDoHuHong = g.Key,
                        Count = g.Count(),
                        Percentage = Math.Round((double)g.Count() / allData.total * 100, 2)
                    })
                    .OrderBy(x => x.MucDoHuHong)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"MucDoHuHongStats count: {mucDoHuHongStats.Count}");

                var labels = new List<string>();
                var data = new List<int>();
                var backgroundColor = new List<string>();
                
                // Màu sắc cho từng mức độ hư hỏng
                var colorMap = new Dictionary<string, string>
                {
                    { "Nhẹ", "#4BC0C0" },        // Màu xanh lá nhẹ
                    { "Trung bình", "#FFCE56" }, // Màu vàng
                    { "Nặng", "#FF6384" },       // Màu đỏ
                    { "Rất nặng", "#9966FF" },   // Màu tím
                    { "Nghiêm trọng", "#FF6B6B" } // Màu đỏ đậm
                };

                for (int i = 0; i < mucDoHuHongStats.Count; i++)
                {
                    var stat = mucDoHuHongStats[i];
                    labels.Add(stat.MucDoHuHong);
                    data.Add(stat.Count);
                    backgroundColor.Add(colorMap.ContainsKey(stat.MucDoHuHong) ? 
                        colorMap[stat.MucDoHuHong] : "#36A2EB");
                    
                    System.Diagnostics.Debug.WriteLine($"Added MucDoHuHong: {stat.MucDoHuHong} -> {stat.Count}");
                }

                // Nếu không có dữ liệu, tạo dữ liệu mẫu với màu sắc
                if (labels.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("No MucDoHuHong data found, using sample data");
                    labels = new List<string> { "Nhẹ", "Trung bình", "Nặng" };
                    data = new List<int> { 2, 1, 1 }; // Dữ liệu mẫu để test
                    backgroundColor = new List<string> { "#4BC0C0", "#FFCE56", "#FF6384" };
                }

                var result = new
                {
                    labels = labels.ToArray(),
                    datasets = new[]
                    {
                        new
                        {
                            label = "Số lượng hư hỏng",
                            data = data.ToArray(),
                            backgroundColor = backgroundColor.ToArray()
                        }
                    }
                };
                
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetThongKeTheoMucDoHuHong: {ex.Message}");
                
                // Fallback data nếu có lỗi
                var fallbackResult = new
                {
                    labels = new[] { "Nhẹ", "Trung bình", "Nặng" },
                    datasets = new[]
                    {
                        new
                        {
                            label = "Số lượng hư hỏng",
                            data = new[] { 0, 0, 0 },
                            backgroundColor = new[] { "#4BC0C0", "#FFCE56", "#FF6384" }
                        }
                    }
                };
                return Json(fallbackResult, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetThongKeTheoTrangThaiXuLy()
        {
            try
            {
                // Lấy toàn bộ dữ liệu (bỏ phân trang) để thống kê chính xác
                var queryParams = new QueryParamViewModel
                {
                    offset = 0,
                    limit = int.MaxValue
                };
                var allData = _danhSachCongTrinhService.GetAll(queryParams);

                // Debug
                System.Diagnostics.Debug.WriteLine($"Total records for TrangThaiXuLy: {allData.total}");

                // Thống kê theo trạng thái xử lý
                var trangThaiXuLyStats = allData.rows
                    .Where(x => !string.IsNullOrEmpty(x.TrangThaiXuLy))
                    .GroupBy(x => x.TrangThaiXuLy)
                    .Select(g => new
                    {
                        TrangThaiXuLy = g.Key,
                        Count = g.Count(),
                        Percentage = Math.Round((double)g.Count() / allData.total * 100, 2)
                    })
                    .OrderBy(x => x.TrangThaiXuLy)
                    .ToList();

                var labels = new List<string>();
                var data = new List<int>();
                var backgroundColor = new List<string>();

                // Màu sắc cố định cho trạng thái xử lý
                var colorMap = new Dictionary<string, string>
                {
                    { "Chưa xử lý", "#FF6384" },  // Đỏ
                    { "Đang xử lý", "#FFCE56" },  // Vàng
                    { "Đã xử lý",   "#4BC0C0" }   // Xanh
                };

                foreach (var stat in trangThaiXuLyStats)
                {
                    labels.Add(stat.TrangThaiXuLy);
                    data.Add(stat.Count);
                    backgroundColor.Add(colorMap.ContainsKey(stat.TrangThaiXuLy) ? colorMap[stat.TrangThaiXuLy] : "#36A2EB");
                }

                // Nếu không có dữ liệu, tạo dữ liệu mẫu với màu sắc
                if (labels.Count == 0)
                {
                    labels = new List<string> { "Chưa xử lý", "Đang xử lý", "Đã xử lý" };
                    data = new List<int> { 0, 0, 0 };
                    backgroundColor = new List<string> { "#FF6384", "#FFCE56", "#4BC0C0" };
                }

                var result = new
                {
                    labels = labels.ToArray(),
                    datasets = new[]
                    {
                        new
                        {
                            label = "Số lượng xử lý",
                            data = data.ToArray(),
                            backgroundColor = backgroundColor.ToArray()
                        }
                    }
                };
                
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Fallback data nếu có lỗi
                var fallbackResult = new
                {
                    labels = new[] { "Chưa xử lý", "Đang xử lý", "Đã xử lý" },
                    datasets = new[]
                    {
                        new
                        {
                            label = "Số lượng xử lý",
                            data = new[] { 0, 0, 0 },
                            backgroundColor = new[] { "#FF6384", "#FFCE56", "#4BC0C0" }
                        }
                    }
                };
                return Json(fallbackResult, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetDanhSachCongTrinhLoc(QueryParamViewModel queryParams)
        {
            // Thêm các tham số filter từ query string
            if (!string.IsNullOrEmpty(Request.QueryString["tuNgay"]))
            {
                queryParams.TuNgayStr = Request.QueryString["tuNgay"];
            }
            if (!string.IsNullOrEmpty(Request.QueryString["denNgay"]))
            {
                queryParams.DenNgayStr = Request.QueryString["denNgay"];
            }
            if (!string.IsNullOrEmpty(Request.QueryString["loaiCongTrinh"]))
            {
                queryParams.loaiCongTrinh = Request.QueryString["loaiCongTrinh"];
            }
            if (!string.IsNullOrEmpty(Request.QueryString["trangThai"]))
            {
                queryParams.trangThai = Request.QueryString["trangThai"];
            }

            // Sử dụng DanhSachCongTrinhService để lấy dữ liệu từ bảng DanhSachCongTrinh
            var data = _danhSachCongTrinhService.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetChiTietCongTrinh(int id)
        {
            var data = _danhSachCongTrinhService.GetById(id);
            return Json(new { Item = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult XuatBaoCao(string format, string tuNgay, string denNgay, string loaiCongTrinh, string trangThai)
        {
            // Mock export functionality - implement actual export logic
            var fileName = $"BaoCaoCongTrinh_{DateTime.Now:yyyyMMdd}.{format}";
            
            if (format == "pdf")
            {
                // Implement PDF export
                return File(new byte[0], "application/pdf", fileName);
            }
            else if (format == "excel")
            {
                // Implement Excel export
                return File(new byte[0], "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            else if (format == "word")
            {
                // Implement Word export
                return File(new byte[0], "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
            
            return Json(new { success = false, message = "Định dạng không được hỗ trợ" }, JsonRequestBehavior.AllowGet);
        }
    }
}
