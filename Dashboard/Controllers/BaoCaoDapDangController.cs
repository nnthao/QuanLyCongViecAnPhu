using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class BaoCaoDapDangController : Controller
    {
        private readonly IDanhSachCongTrinhService _danhSachCongTrinhService;
        
        public BaoCaoDapDangController(IDanhSachCongTrinhService danhSachCongTrinhService)
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
            // Filter chỉ lấy dữ liệu loại công trình = 2 (Đập Dâng)
            queryParams.loaiCongTrinh = "2";
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
            var queryParams = new QueryParamViewModel { limit = int.MaxValue, offset = 0, loaiCongTrinh = "2" };
            var result = _danhSachCongTrinhService.GetAll(queryParams);

            var bytes = BuildXlsx(result.rows);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
        }

        private static byte[] BuildXlsx(List<DanhSachCongTrinhGridVo> rows)
        {
            using (var ms = new MemoryStream())
            using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                AddEntry(archive, "[Content_Types].xml",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                    "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">" +
                    "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>" +
                    "<Default Extension=\"xml\" ContentType=\"application/xml\"/>" +
                    "<Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>" +
                    "<Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>" +
                    "<Override PartName=\"/xl/styles.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml\"/>" +
                    "</Types>");

                AddEntry(archive, "_rels/.rels",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                    "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                    "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"/xl/workbook.xml\"/>" +
                    "</Relationships>");

                AddEntry(archive, "xl/_rels/workbook.xml.rels",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                    "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                    "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/>" +
                    "<Relationship Id=\"rId2\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" Target=\"styles.xml\"/>" +
                    "</Relationships>");

                AddEntry(archive, "xl/workbook.xml",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                    "<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">" +
                    "<sheets><sheet name=\"Sheet1\" sheetId=\"1\" r:id=\"rId1\"/></sheets>" +
                    "</workbook>");

                AddEntry(archive, "xl/styles.xml",
                    "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                    "<styleSheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">" +
                    "<fonts count=\"1\"><font><sz val=\"11\"/><name val=\"Calibri\"/></font></fonts>" +
                    "<fills count=\"1\"><fill><patternFill patternType=\"none\"/></fill></fills>" +
                    "<borders count=\"2\"><border/><border><left style=\"thin\"/><right style=\"thin\"/><top style=\"thin\"/><bottom style=\"thin\"/></border></borders>" +
                    "<cellStyleXfs count=\"1\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\"/></cellStyleXfs>" +
                    "<cellXfs count=\"2\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\"/><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"1\" xfId=\"0\" applyAlignment=\"1\"><alignment horizontal=\"center\" vertical=\"center\"/></xf></cellXfs>" +
                    "</styleSheet>");

                var sheet = new StringBuilder();
                sheet.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
                sheet.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetData>");

                for (int i = 1; i <= 4; i++) sheet.Append(Row(i));

                var headersGroup = new[] { "TT", "Tên công trình", "Địa điểm", "Năm HT", "Số lần NC, SC lớn", "Năm NC, SC gần nhất", "FLv (km2)", "F tưới (ha)", "", "Đập", "", "Cống lấy nước", "", "Đơn vị trực tiếp quản lý", "Trạng thái", "Ghi chú" };
                sheet.Append(Row(5, headersGroup));

                var headersDetail = new[] { "", "", "", "", "", "", "", "TK", "TT", "Hmax (m)", "L (m)", "Kích thước (m)", "Vật liệu", "", "", "" };
                sheet.Append(Row(6, headersDetail));

                int rowIndex = 7; int stt = 1;
                foreach (var r in rows)
                {
                    var data = new string[] {
                        stt.ToString(),
                        r.TenCongTrinh,
                        r.Diadiem,
                        r.NamHoanThanhCongTrinh?.ToString(),
                        r.SoLanNangCapSuaChua?.ToString(),
                        r.NamNangCapSuaChuaGanNhat?.ToString(),
                        r.DienTichLuuVuc_Km2?.ToString(),
                        r.DienTichTuoiThietKe_Ha?.ToString(), // TK
                        r.DienTichTuoiThucTe_Ha?.ToString(), // TT
                        r.ChieuCaoDapLonNhatTrenCongTrinhPhanKhongTran_M?.ToString(), // Hmax
                        r.ChieuDaiPhanKhongTran_M?.ToString(), // L
                        r.KichThuocCongCapNuoc_Cm?.ToString(), // kích thước (m)
                        r.VatLieuCongCapNuoc,
                        r.DonViQuanLyCongTrinh,
                        r.TrangThai,
                        r.GhiChu
                    };
                    sheet.Append(Row(rowIndex, data));
                    rowIndex++; stt++;
                }

                sheet.Append("</sheetData>");
                sheet.Append("<mergeCells count=\"3\">");
                sheet.Append("<mergeCell ref=\"H5:I5\"/>");
                sheet.Append("<mergeCell ref=\"J5:K5\"/>");
                sheet.Append("<mergeCell ref=\"L5:M5\"/>");
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
