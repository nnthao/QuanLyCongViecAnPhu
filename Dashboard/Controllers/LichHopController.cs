using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class LichHopController : Controller
    {
        private readonly ICuocHopService _cuocHopService;
        private readonly IUserService _userService;

        public LichHopController(ICuocHopService cuocHopService,
            IUserService userService)
        {
            _cuocHopService = cuocHopService;
            _userService = userService;
        }

        [JwtAuthorize("LichHop")]
        public ActionResult Index()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var coQuanCurrentId = UserClaimHelper.GetPhongBanId(identity);
            var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
            var isQuyenGiaoViecTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenGiaoViecTatCaPhongBan.ToString());

            var coQuanNew = new DataLookupViewModel();
            coQuanNew.Data = new List<LookupViewModel>();

            if (isSystemUser || isQuyenGiaoViecTatCaPhongBan)
            {
                var phongBans = UltiHelper.GetPhongBanTreeSorted(0);
                var phongBanList = phongBans.Select(pb => new LookupViewModel
                {
                    id = pb.Id,
                    text = pb.Level > 0 ? $"{new string('—', pb.Level)} {pb.Ten}" : pb.Ten,
                    phongBanId = pb.PhongBanChaId
                }).ToList();

                phongBanList.Insert(0, new LookupViewModel
                {
                    id = 0,
                    text = "--- Tất cả phòng ban ---"
                });

                coQuanNew.Data = phongBanList;
            }
            else if (coQuanCurrentId != 0)
            {
                var phongBans = CacheManagerHelper.GetPhongBanCache();
                var phongBanCurrent = phongBans.FirstOrDefault(pb => pb.Id == coQuanCurrentId && pb.IsDeleted != true);

                var phongBanList = new List<LookupViewModel>();

                // Thêm phòng ban hiện tại của người đăng nhập trước
                if (phongBanCurrent != null)
                {
                    phongBanList.Add(new LookupViewModel
                    {
                        id = phongBanCurrent.Id,
                        text = phongBanCurrent.Ten,
                        phongBanId = phongBanCurrent.PhongBanChaId
                    });
                }

                // Lấy danh sách phòng ban con và sắp xếp
                var phongBanChildren = UltiHelper.GetPhongBanChildrenSorted(coQuanCurrentId);
                phongBanList.AddRange(phongBanChildren);

                if (phongBanList.Count > 1)
                {
                    phongBanList.Insert(0, new LookupViewModel
                    {
                        id = 0,
                        text = "--- Chọn ---"
                    });
                }

                coQuanNew.Data = phongBanList;
            }

            return View(new ParamControllerToView
            {
                CoQuanCurrentId = (isSystemUser || isQuyenGiaoViecTatCaPhongBan) ? 0 : coQuanCurrentId,
                CoQuanLookups = coQuanNew
            });
        }

        [JwtAuthorize("Public")]
        [HttpPost]
        public ActionResult GetListLichHop(QueryParamViewModel queryParams)
        {
            var identity = (ClaimsIdentity)User.Identity;
            queryParams.CurrentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var phongBanCurrentId = UserClaimHelper.GetPhongBanId(identity);
            var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
            var isQuyenGiaoViecTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenGiaoViecTatCaPhongBan.ToString());

            List<int?> allPhongBanIds = new List<int?>();
            if (queryParams.CoQuanCurrentId != 0)
            {
                allPhongBanIds = new List<int?> { queryParams.CoQuanCurrentId };
            }
            else if (isSystemUser || isQuyenGiaoViecTatCaPhongBan)
            {
                allPhongBanIds = null;
            }
            else if (phongBanCurrentId != 0)
            {
                allPhongBanIds = GetPhongBanAndChildrenIds(phongBanCurrentId);
            }

            queryParams.PhongBanIds = allPhongBanIds;

            var data = _cuocHopService.GetLichHopTheoTuan(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<int?> GetPhongBanAndChildrenIds(int phongBanId)
        {
            var result = new List<int?> { phongBanId };
            var phongBans = CacheManagerHelper.GetPhongBanCache();
            var phongBanCon = phongBans.Where(o => o.PhongBanChaId == phongBanId && o.IsDeleted != true).ToList();

            foreach (var phongBan in phongBanCon)
            {
                result.AddRange(GetPhongBanAndChildrenIds(phongBan.Id));
            }

            return result;
        }

        [JwtAuthorize("Public")]
        [HttpPost]
        public ActionResult GetListLichHopTheoThang(QueryParamViewModel queryParams)
        {
            var identity = (ClaimsIdentity)User.Identity;
            queryParams.CurrentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var phongBanCurrentId = UserClaimHelper.GetPhongBanId(identity);
            var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
            var isQuyenGiaoViecTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenGiaoViecTatCaPhongBan.ToString());

            List<int?> allPhongBanIds = new List<int?>();
            if (queryParams.CoQuanCurrentId != 0)
            {
                allPhongBanIds = new List<int?> { queryParams.CoQuanCurrentId };
            }
            else if (isSystemUser || isQuyenGiaoViecTatCaPhongBan)
            {
                allPhongBanIds = null;
            }
            else if (phongBanCurrentId != 0)
            {
                allPhongBanIds = GetPhongBanAndChildrenIds(phongBanCurrentId);
            }

            queryParams.PhongBanIds = allPhongBanIds;

            var data = _cuocHopService.GetLichHopTheoThang(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public JsonResult GetTaiLieuCuocHop(int id)
        {
            return Json(_cuocHopService.GetTaiLieuCuocHop(id),
                JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public ActionResult DownloadTaiLieuCuocHop(int id)
        {
            var data = _cuocHopService.DownloadTaiLieuCuocHop(id);

            if (data != null)
            {
                return Json(data.DuongDan, JsonRequestBehavior.AllowGet);
            }

            throw new Exception("File đã bị xóa");
        }

        [JwtAuthorize("Public")]
        public JsonResult GetThongTinCuocHop(int id)
        {
            var cuocHop = _cuocHopService.GetCuocHopInfoFromId(id);
            var cuocHopTaiLieu = _cuocHopService.GetTaiLieuCuocHop(id);

            return Json(new { CuocHop = cuocHop, TaiLieus = cuocHopTaiLieu }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public ActionResult ChiTiet(int id = 0)
        {
            return View(id);
        }

        [JwtAuthorize("Public")]
        public JsonResult XuatFile(QueryParamViewModel queryParams)
        {
            var identity = (ClaimsIdentity)User.Identity;
            queryParams.CurrentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var phongBanCurrentId = UserClaimHelper.GetPhongBanId(identity);
            var isSystemUser = UserClaimHelper.GetIsSystemUser(identity);
            var isQuyenGiaoViecTatCaPhongBan = UserClaimHelper.IsQuyenAllow(identity, QuyenMoRong.QuyenGiaoViecTatCaPhongBan.ToString());

            List<int?> allPhongBanIds = new List<int?>();
            if (queryParams.CoQuanCurrentId != 0)
            {
                allPhongBanIds = new List<int?> { queryParams.CoQuanCurrentId };
            }
            else if (isSystemUser || isQuyenGiaoViecTatCaPhongBan)
            {
                allPhongBanIds = null;
            }
            else if (phongBanCurrentId != 0)
            {
                allPhongBanIds = GetPhongBanAndChildrenIds(phongBanCurrentId);
            }

            queryParams.PhongBanIds = allPhongBanIds;

            var datas = new List<NgaySscheduler>();
            var isTuan = queryParams.LoaiLich == BienDungChung.LOAI_LICH_TUAN;

            if (queryParams.LoaiLich == BienDungChung.LOAI_LICH_TUAN)
            {
                queryParams.ThangHienTai = "";
                datas = _cuocHopService.GetLichHopTheoTuan(queryParams);
            }
            else if (queryParams.LoaiLich == BienDungChung.LOAI_LICH_THANG)
            {
                queryParams.TuanHienTai = "";
                queryParams.TuanTimKiem = "";
                datas = _cuocHopService.GetLichHopTheoThang(queryParams);
            }

            var folderSaveTemp = Server.MapPath(BienDungChung.FolderSavePath);
            if (!System.IO.Directory.Exists(folderSaveTemp))
            {
                System.IO.Directory.CreateDirectory(folderSaveTemp);
            }

            var file = _cuocHopService.XuatFile(new FileExportVm
            {
                Lichs = datas,
                FolderSave = folderSaveTemp,
                FileTemplate = Server.MapPath(BienDungChung.LichHopTuanExportTemplate),
                IsTuan = isTuan
            });

            if (queryParams.LoaiFile == BienDungChung.LOAI_FILE_PDF)
            {
                var pdfFile = _cuocHopService.XuatFilePdf(file.DuongDan);
                return Json(new { Url = pdfFile.DuongDan, Name = pdfFile.TenFile }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Url = file.DuongDan, Name = file.TenFile }, JsonRequestBehavior.AllowGet);
        }
    }
}