using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Linq;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class GiaoViecThongTinTraCuuController : Controller
    {
        private readonly IGiaoViecService _service;
        public GiaoViecThongTinTraCuuController(IGiaoViecService service)
        {
            _service = service;
        }

        [JwtAuthorize("Public")]
        public ActionResult XemThongTin(ParamControllerToView paramControllerToView)
        {
            return View(paramControllerToView);
        }

        [JwtAuthorize("Public")]
        public JsonResult XemThongTinGetById(ParamControllerToView paramControllerToView)
        {
            var data = _service.GetById(paramControllerToView.GiaoViecId);
            if (data == null)
            {
                return Json(new { Item = (object)null }, JsonRequestBehavior.AllowGet);
            }

            GiaoViecThanhVienGridVo thanhVien = null;
            if (paramControllerToView.GiaoViecThanhVienId > 0)
            {
                thanhVien = data.GiaoViecThanhViens.FirstOrDefault(o => o.Id == paramControllerToView.GiaoViecThanhVienId);
            }

            if (thanhVien == null && paramControllerToView.ThanhVienId > 0)
            {
                thanhVien = data.GiaoViecThanhViens.FirstOrDefault(o => o.ThanhVienId == paramControllerToView.ThanhVienId);
            }

            if (thanhVien == null && data.GiaoViecThanhViens != null)
            {
                thanhVien = data.GiaoViecThanhViens.FirstOrDefault();
            }

            if (thanhVien != null)
            {
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
            }

            return Json(new { Item = data }, JsonRequestBehavior.AllowGet);
        }

        [JwtAuthorize("Public")]
        public JsonResult GetByTienTrinhCapNhatCuaThanhVien(int giaoViecThanhVienId = 0)
        {
            var data = _service.GetByTienTrinhCapNhatCuaThanhVien(giaoViecThanhVienId);
            return Json(new
            {
                Item = data
            }, JsonRequestBehavior.AllowGet);
        }
    }
}

