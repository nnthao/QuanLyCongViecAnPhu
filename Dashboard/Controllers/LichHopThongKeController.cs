using Dashboard.Common;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class LichHopThongKeController : Controller
    {
        private readonly ILichHopThongKeService _service;

        public LichHopThongKeController(ILichHopThongKeService service)
        {
            _service = service;
        }

        [JwtAuthorize("LichHopThongKe")]
        public ActionResult Index()
        {
            return View();
        }

        [JwtAuthorize("LichHopThongKe")]
        [HttpPost]
        public JsonResult GetThongKeLyDoCuocHop(QueryParamViewModel queryViewModel)
        {
            var errors = new List<ValidateError>();
            if (queryViewModel.TuNgay == null)
            {
                errors.Add(new ValidateError
                {
                    Id = "TuNgayStr",
                    Description = "Từ ngày không được trống"
                });
            }
            if (queryViewModel.DenNgay == null)
            {
                errors.Add(new ValidateError
                {
                    Id = "DenNgayStr",
                    Description = "Đến ngày không được trống"
                });
            }

            if (errors.Count != 0)
            {
                return Json(new { Errors = errors }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var phongBanIdsStr = Request["PhongBanIdsStr"];
                if (!string.IsNullOrEmpty(phongBanIdsStr))
                {
                    queryViewModel.PhongBanIds = phongBanIdsStr.Split(',')
                        .Where(o => !string.IsNullOrEmpty(o))
                        .Select(o => (int?)Convert.ToInt32(o))
                        .ToList();
                }

                var lyDoIdsStr = Request["LyDoIdsStr"];
                if (!string.IsNullOrEmpty(lyDoIdsStr))
                {
                    queryViewModel.LyDoIdsStr = lyDoIdsStr;
                }

                var result = _service.GetThongKeLyDoCuocHop(queryViewModel);
                var top10Nguoi = _service.GetTop10NguoiCoNhieuLyDo(queryViewModel);
                return Json(new { success = true, data = result, top10Nguoi = top10Nguoi }, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [JwtAuthorize("LichHopThongKe")]
        public ActionResult GetChiTietLyDoCuocHop(QueryParamViewModel queryParams)
        {
            var errors = new List<ValidateError>();
            if (queryParams.TuNgay == null)
            {
                errors.Add(new ValidateError
                {
                    Id = "TuNgayStr",
                    Description = "Từ ngày không được trống"
                });
            }
            if (queryParams.DenNgay == null)
            {
                errors.Add(new ValidateError
                {
                    Id = "DenNgayStr",
                    Description = "Đến ngày không được trống"
                });
            }

            if (errors.Count != 0)
            {
                return Json(new { Errors = errors }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var phongBanIdsStr = Request["PhongBanIdsStr"];
                if (!string.IsNullOrEmpty(phongBanIdsStr))
                {
                    queryParams.PhongBanIds = phongBanIdsStr.Split(',')
                        .Where(o => !string.IsNullOrEmpty(o))
                        .Select(o => (int?)Convert.ToInt32(o))
                        .ToList();
                }

                var lyDoIdsStr = Request["LyDoIdsStr"];
                if (!string.IsNullOrEmpty(lyDoIdsStr))
                {
                    queryParams.LyDoIdsStr = lyDoIdsStr;
                }

                var data = _service.GetChiTietLyDoCuocHop(queryParams);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
