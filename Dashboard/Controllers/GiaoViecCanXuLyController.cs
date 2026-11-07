using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Linq;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class GiaoViecCanXuLyController : Controller
    {
        private readonly IGiaoViecService _service;
        public GiaoViecCanXuLyController(IGiaoViecService service)
        {
            _service = service;
        }

        [JwtAuthorize("GiaoViecCanXuLy")]
        public ActionResult Index()
        {
            var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var hasQuyenChuyenGiao = UserClaimHelper.IsQuyenAllow(identity, "GiaoViecDaChuyenGiao") || UserClaimHelper.GetIsSystemUser(identity);
            ViewBag.HasQuyenChuyenGiao = hasQuyenChuyenGiao;
            return View();
        }

        [JwtAuthorize("GiaoViecCanXuLy")]
        public ActionResult Form(int id = 0, int giaoViecId = 0)
        {
            return View(new ParamControllerToView
            {
                GiaoViecThanhVienId = id,
                GiaoViecId = giaoViecId
            });
        }

        [JwtAuthorize("GiaoViecCanXuLy")]
        public ActionResult XemThongTin(int id = 0, int giaoViecId = 0)
        {
            return View(new ParamControllerToView
            {
                GiaoViecThanhVienId = id,
                GiaoViecId = giaoViecId,
                IsXemThongTinDaXuLy = true
            });
        }

        [JwtAuthorize("GiaoViecCanXuLy")]
        public ActionResult GiaoViecTiepTuc(int id = 0, int giaoViecId = 0)
        {
            var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var hasQuyenChuyenGiao = UserClaimHelper.IsQuyenAllow(identity, "GiaoViecDaChuyenGiao") || UserClaimHelper.GetIsSystemUser(identity);
            if (!hasQuyenChuyenGiao)
            {
                return new HttpStatusCodeResult(403, "Bạn không có quyền thực hiện chức năng này.");
            }

            var currentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var data = _service.GetById(giaoViecId);
            var thanhVien = data.GiaoViecThanhViens.FirstOrDefault(o => o.ThanhVienId == currentUserId && o.Id == id);
            if (thanhVien == null)
            {
                throw new System.Exception("Không tìm thấy dữ liệu");
            }

            return View(new ParamControllerToView
            {
                GiaoViecThanhVienId = id,
                GiaoViecId = giaoViecId
            });
        }

        [HttpPost]
        [JwtAuthorize("GiaoViecCanXuLy")]
        public ActionResult ChuyenGiaoViecTiepTuc(ChuyenGiaoViecTiepTucViewModel viewModel)
        {
            var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var hasQuyenChuyenGiao = UserClaimHelper.IsQuyenAllow(identity, "GiaoViecDaChuyenGiao") || UserClaimHelper.GetIsSystemUser(identity);
            if (!hasQuyenChuyenGiao)
            {
                return Json(new
                {
                    Errors = new System.Collections.Generic.List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "Permission",
                            Description = "Bạn không có quyền thực hiện chức năng này."
                        }
                    }
                }, JsonRequestBehavior.AllowGet);
            }

            viewModel.CurrentUserId = UserClaimHelper.GetCurrentUserId(identity);
            var data = _service.ChuyenGiaoViecTiepTuc(viewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("GiaoViecCanXuLy")]
        public ActionResult GetListCongViecCanXuLy(GiaoViecQueryViewModel queryParams)
        {
            queryParams.NguoiDuocGiaoId = UserClaimHelper.GetCurrentUserId((System.Security.Claims.ClaimsIdentity)User.Identity);
            var data = _service.GetListCongViecCanXuLy(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("GiaoViecCanXuLy")]
        public JsonResult GetById(int id = 0)
        {
            var item = GetByIdCommon(id);
            return Json(new { Item = item }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("GiaoViecCanXuLy")]
        public JsonResult XemThongTinGetById(int id = 0)
        {
            var item = GetByIdCommon(id, isXemThongTin: true);
            return Json(new { Item = item }, JsonRequestBehavior.AllowGet);
        }

        private GiaoViecGridVo GetByIdCommon(int id, bool isXemThongTin = false)
        {
            var currentUserId = UserClaimHelper.GetCurrentUserId((System.Security.Claims.ClaimsIdentity)User.Identity);
            var data = _service.GetById(id);

            var thanhVien = data.GiaoViecThanhViens.FirstOrDefault(o => o.ThanhVienId == currentUserId);
            if (thanhVien == null)
            {
                throw new System.Exception("Không tìm thấy dữ liệu");
            }

            data.HanXuLyThanhVienCanXuLy = thanhVien.HanXuLy;
            data.NoiDungThanhVienCanXuLy = thanhVien.NoiDungNhiemVu;
            data.SoGioUocTinhThanhVienCanXuLy = thanhVien.SoGioUocTinh;
            if (thanhVien.VaiTroXuLyId.HasValue)
            {
                var vaiTroXuLy = CacheManagerHelper.GetVaiTroXuLyByIdFromCache(thanhVien.VaiTroXuLyId.Value);
                if (vaiTroXuLy != null)
                {
                    data.TenVaiTroXuLyThanhVienCanXuLy = vaiTroXuLy.Ten;
                    data.IsCanDinhKemFileKhiXacNhanHoanThanhThanhVienCanXuLy = vaiTroXuLy.IsCanDinhKemFileKhiXacNhanHoanThanh;
                }
            }
            if (thanhVien.NguoiTheoDoiChiDaoId.GetValueOrDefault() > 0)
            {
                data.NguoiTheoDoiChiDaoThanhVienCanXuLyId = thanhVien.NguoiTheoDoiChiDaoId;
                var nguoiTheoDoi = CacheManagerHelper.GetUserByIdFromCache(thanhVien.NguoiTheoDoiChiDaoId.Value);
                if (nguoiTheoDoi != null)
                {
                    data.TenNguoiTheoDoiChiDaoThanhVienCanXuLy = nguoiTheoDoi.Name;
                    data.NguoiTheoDoiChiDaoThanhVienCanXuLy = nguoiTheoDoi.Name;
                }
            }
            data.IsHoanThanhThanhVienCanXuLy = thanhVien.IsHoanThanh;
            data.TrangThaiThanhVienCanXuLy = thanhVien.TrangThai;
            data.KetQuaDanhGiaThanhVienCanXuLy = thanhVien.KetQuaDanhGia;
            data.YKienDanhGiaThanhVienCanXuLy = thanhVien.YKienDanhGia;

            return data;
        }

        [HttpPost]
        [JwtAuthorize("GiaoViecCanXuLy")]
        public ActionResult ThemTienTrinhXuLyCuaThanhVien(GiaoViecThanhVienTienTrinhViewModel viewModel)
        {
            var data = _service.ThemTienTrinhXuLyCuaThanhVien(viewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("GiaoViecCanXuLy")]
        public ActionResult HuyXacNhanHoanThanhCuaThanhVien(HuyXacNhanHoanThanhViewModel viewModel)
        {
            viewModel.CurrentUserId = UserClaimHelper.GetCurrentUserId((System.Security.Claims.ClaimsIdentity)User.Identity);
            var data = _service.HuyXacNhanHoanThanhCuaThanhVien(viewModel);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [JwtAuthorize("GiaoViecCanXuLy")]
        public ActionResult Xoa(int Id)
        {
            var data = _service.Xoa(Id);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("GiaoViecCanXuLy")]
        public JsonResult GetByTienTrinhCapNhatCuaThanhVien(int giaoViecThanhVienId = 0)
        {
            var currentUserId = UserClaimHelper.GetCurrentUserId((System.Security.Claims.ClaimsIdentity)User.Identity);
            var data = _service.GetByTienTrinhCapNhatCuaThanhVien(giaoViecThanhVienId);

            return Json(new
            {
                Item = data
            }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("GiaoViecCanXuLy")]
        public JsonResult XoaTienTrinhCuaThanhVien(int Id = 0)
        {
            var currentUserId = UserClaimHelper.GetCurrentUserId((System.Security.Claims.ClaimsIdentity)User.Identity);
            var data = _service.XoaTienTrinhCuaThanhVien(new GiaoViecThanhVienTienTrinhGridVo
            {
                Id = Id,
                CurrentUserId = currentUserId
            });

            return Json(new
            {
                Item = data
            }, JsonRequestBehavior.AllowGet);
        }
    }
}

