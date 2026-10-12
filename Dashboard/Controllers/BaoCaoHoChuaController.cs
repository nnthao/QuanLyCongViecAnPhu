using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class BaoCaoHoChuaController : Controller
    {
        private readonly IDanhSachCongTrinhService _danhSachCongTrinhService;

        public BaoCaoHoChuaController(IDanhSachCongTrinhService danhSachCongTrinhService)
        {
            _danhSachCongTrinhService = danhSachCongTrinhService;
        }

        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult GetAll(QueryParamViewModel queryParams)
        {
            // Chỉ lấy Hồ
            queryParams.loaiCongTrinh = "1";

            // Filter bổ sung
            var xaPhuong = Request.QueryString["xaPhuong"]; if (!string.IsNullOrEmpty(xaPhuong)) queryParams.xaPhuong = xaPhuong;
            var donViQuanLy = Request.QueryString["donViQuanLy"]; if (!string.IsNullOrEmpty(donViQuanLy)) queryParams.donViQuanLy = donViQuanLy;
            var phanLoaiHo = Request.QueryString["phanLoaiHo"]; if (!string.IsNullOrEmpty(phanLoaiHo)) queryParams.phanLoaiHo = phanLoaiHo;

            var data = _danhSachCongTrinhService.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult Form(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        [HttpPost]
        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult ThemOrCapNhat(DanhSachCongTrinhViewModel model)
        {
            var result = _danhSachCongTrinhService.ThemOrCapNhat(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult Xoa(int id)
        {
            var result = _danhSachCongTrinhService.Xoa(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult KhoiPhuc(int id)
        {
            var result = _danhSachCongTrinhService.KhoiPhuc(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult GetById(int id)
        {
            var data = _danhSachCongTrinhService.GetById(id);
            return Json(new { Item = data }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("QuanLyCongTrinhHuHong")]
        public ActionResult Export()
        {
            var queryParams = new QueryParamViewModel
            {
                limit = int.MaxValue,
                offset = 0,
                loaiCongTrinh = "1"
            };

            var xaPhuong = Request.QueryString["xaPhuong"]; if (!string.IsNullOrEmpty(xaPhuong)) queryParams.xaPhuong = xaPhuong;
            var donViQuanLy = Request.QueryString["donViQuanLy"]; if (!string.IsNullOrEmpty(donViQuanLy)) queryParams.donViQuanLy = donViQuanLy;
            var phanLoaiHo = Request.QueryString["phanLoaiHo"]; if (!string.IsNullOrEmpty(phanLoaiHo)) queryParams.phanLoaiHo = phanLoaiHo;

            var result = _danhSachCongTrinhService.GetAll(queryParams);

            // Tạo file .xlsx chuẩn OpenXML, header nhóm (row 5) + chi tiết (row 6), dữ liệu từ row 7
            var xlsxBytes = BuildXlsx(result.rows);
            var fileName = "file.xlsx";
            return File(xlsxBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private static byte[] BuildXlsx(List<DanhSachCongTrinhGridVo> rows)
        {
            using (var ms = new MemoryStream())
            using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                // [Content_Types].xml
                AddEntry(archive, "[Content_Types].xml",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                    "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">" +
                    "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>" +
                    "<Default Extension=\"xml\" ContentType=\"application/xml\"/>" +
                    "<Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>" +
                    "<Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>" +
                    "<Override PartName=\"/xl/styles.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml\"/>" +
                    "</Types>");

                // _rels/.rels
                AddEntry(archive, "_rels/.rels",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                    "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                    "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"/xl/workbook.xml\"/>" +
                    "</Relationships>");

                // xl/_rels/workbook.xml.rels
                AddEntry(archive, "xl/_rels/workbook.xml.rels",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                    "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                    "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/>" +
                    "<Relationship Id=\"rId2\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" Target=\"styles.xml\"/>" +
                    "</Relationships>");

                // xl/workbook.xml
                AddEntry(archive, "xl/workbook.xml",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                    "<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">" +
                    "<sheets><sheet name=\"Sheet1\" sheetId=\"1\" r:id=\"rId1\"/></sheets>" +
                    "</workbook>");

                // xl/styles.xml (border + center)
                AddEntry(archive, "xl/styles.xml",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                    "<styleSheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">" +
                    "<fonts count=\"1\"><font><sz val=\"11\"/><name val=\"Calibri\"/></font></fonts>" +
                    "<fills count=\"1\"><fill><patternFill patternType=\"none\"/></fill></fills>" +
                    "<borders count=\"2\">" +
                    "<border/>" +
                    "<border><left style=\"thin\"/><right style=\"thin\"/><top style=\"thin\"/><bottom style=\"thin\"/></border>" +
                    "</borders>" +
                    "<cellStyleXfs count=\"1\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\"/></cellStyleXfs>" +
                    "<cellXfs count=\"2\">" +
                    "<xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\"/>" +
                    "<xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"1\" xfId=\"0\" applyAlignment=\"1\"><alignment horizontal=\"center\" vertical=\"center\"/></xf>" +
                    "</cellXfs>" +
                    "</styleSheet>");

                // xl/worksheets/sheet1.xml
                var sheet = new StringBuilder();
                sheet.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
                sheet.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetData>");

                // 4 dòng trống
                for (int i = 1; i <= 4; i++) sheet.Append(Row(i));

                // Hàng 5: tiêu đề nhóm (đặt text ở ô đầu của nhóm, các ô còn lại để trống)
                var headersGroup = new[] { "TT", "Tên công trình", "Địa điểm", "Năm HT", "FLv (km2)", "F tưới (ha)", "", "Dung tích ứng với MNDBT (triệu m3)", "Đập chính", "", "", "Cống lấy nước", "", "Tràn xã lũ", "", "", "", "Dung tích lòng hồ hiện tại", "Khả năng đáp ứng nhu cầu nước tháng tới (%)", "Trạng thái" };
                sheet.Append(Row(5, headersGroup));

                // Hàng 6: tiêu đề chi tiết
                var headersDetail = new[] { "", "", "", "", "", "TT", "TK", "", "Hmax (m)", "B (m)", "L (m)", "Kích thước (cm)", "Vật liệu", "Cao trình ngưỡng tràn (m)", "B (m)", "Vật liệu", "Hình thức", "", "", "" };
                sheet.Append(Row(6, headersDetail));

                // Dữ liệu từ hàng 7
                int rowIndex = 7;
                int stt = 1;
                foreach (var r in rows)
                {
                    var data = new string[] {
                        stt.ToString(),
                        r.TenCongTrinh,
                        r.Diadiem,
                        r.NamHoanThanhCongTrinh?.ToString(),
                        r.DienTichLuuVuc_Km2?.ToString(),
                        r.DienTichTuoiThucTe_Ha?.ToString(), // TT
                        r.DienTichTuoiThietKe_Ha?.ToString(), // TK
                        r.DungTichUngVoiMNDBT_TrieuM3?.ToString(),
                        r.DapChinhHmax?.ToString(),
                        r.DapChinhB?.ToString(),
                        r.DapChinhL?.ToString(),
                        r.CongLayNuocKichThuoc,
                        r.CongLayNuocVatLieu,
                        r.TranXaLuuCaoTrinh?.ToString(),
                        r.TranXaLuuB?.ToString(),
                        r.TranXaLuuVatLieu,
                        r.TranXaLuuHinhThuc,
                        r.DungTichLongHoHienTai?.ToString(),
                        r.KhaNangDapUngNhuCauNuocThangToi?.ToString(),
                        r.TrangThai
                    };
                    sheet.Append(Row(rowIndex, data));
                    rowIndex++; stt++;
                }

                sheet.Append("</sheetData>");
                // Merge các ô tiêu đề nhóm hàng 5
                sheet.Append("<mergeCells count=\"4\">");
                sheet.Append("<mergeCell ref=\"F5:G5\"/>");
                sheet.Append("<mergeCell ref=\"I5:K5\"/>");
                sheet.Append("<mergeCell ref=\"L5:M5\"/>");
                sheet.Append("<mergeCell ref=\"N5:Q5\"/>");
                sheet.Append("</mergeCells>");
                sheet.Append("</worksheet>");
                AddEntry(archive, "xl/worksheets/sheet1.xml", sheet.ToString());

                archive.Dispose();
                return ms.ToArray();
            }
        }

        private static void AddEntry(ZipArchive archive, string path, string content)
        {
            var entry = archive.CreateEntry(path, CompressionLevel.Fastest);
            using (var s = entry.Open())
            using (var w = new StreamWriter(s, new UTF8Encoding(false)))
            {
                w.Write(content);
            }
        }

        private static string Row(int index)
        {
            return "<row r=\"" + index + "\"></row>";
        }

        private static string Row(int index, string[] cells)
        {
            var sb = new StringBuilder();
            sb.Append("<row r=\"" + index + "\">");
            int col = 1;
            foreach (var c in cells)
            {
                if (string.IsNullOrEmpty(c)) { sb.Append(Cell(index, col, null)); }
                else if (decimal.TryParse(c, out decimal _)) { sb.Append(CellNumber(index, col, c)); }
                else { sb.Append(CellInline(index, col, c)); }
                col++;
            }
            sb.Append("</row>");
            return sb.ToString();
        }

        private static string CellInline(int r, int c, string text)
        {
            return "<c r=\"" + A1(c, r) + "\" t=\"inlineStr\" s=\"1\"><is><t>" + XmlEscape(text) + "</t></is></c>";
        }

        private static string CellNumber(int r, int c, string num)
        {
            return "<c r=\"" + A1(c, r) + "\" s=\"1\"><v>" + num + "</v></c>";
        }

        private static string A1(int col, int row)
        {
            string colName = string.Empty;
            int dividend = col;
            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                colName = Convert.ToChar(65 + modulo) + colName;
                dividend = (dividend - modulo) / 26;
            }
            return colName + row.ToString();
        }

        private static string Cell(int r, int c, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "<c r=\"" + A1(c, r) + "\" s=\"1\"/>";
            }

            if (decimal.TryParse(value, out decimal _))
            {
                return CellNumber(r, c, value);
            }

            return CellInline(r, c, value);
        }

        private static string XmlEscape(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
        }
    }
}


