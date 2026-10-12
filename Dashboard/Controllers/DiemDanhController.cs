using Dashboard.Models.ViewModel;
using Dashboard.Services;
using System.Web.Mvc;

namespace Dashboard.Controllers
{
    public class DiemDanhController : Controller
    {
        private readonly IDoanVienService _doanVienService;
        private readonly IDonViDoanService _donViDoanService;
        private readonly IDoanVien_TaiLieuService _taiLieuService;

        public DiemDanhController(IDoanVienService doanVienService, IDonViDoanService donViDoanService, IDoanVien_TaiLieuService taiLieuService)
        {
            _doanVienService = doanVienService;
            _donViDoanService = donViDoanService;
            _taiLieuService = taiLieuService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DiemDanh(int DonViDoanId, string HoTen)
        {
            var result = _doanVienService.DiemDanh(DonViDoanId, HoTen);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DiemDanhById(int DoanVienId)
        {
            var result = _doanVienService.DiemDanhById(DoanVienId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DonViDoanLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            if (lookupQueryViewModel == null)
            {
                lookupQueryViewModel = new LookupQueryViewModel();
            }
            var result = _donViDoanService.DonViDoanLookup(lookupQueryViewModel);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ThongKe()
        {
            return View();
        }

        public ActionResult GetThongKeDiemDanh()
        {
            var data = _doanVienService.GetThongKeDiemDanh();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ThongKeDaiHoi()
        {
            return View();
        }

        public ActionResult GetThongKeDaiHoi()
        {
            var data = _doanVienService.GetThongKeDaiHoi();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TaiLieuDaiHoi()
        {
            return View();
        }

        public ActionResult GetTaiLieuDaiHoi()
        {
            var queryParams = new QueryParamViewModel
            {
                limit = 1000,
                offset = 0,
                DaXoaSearch = false
            };
            var data = _taiLieuService.GetAll(queryParams);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult XoaTaiLieu(int id)
        {
            var result = _taiLieuService.Xoa(id);
            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDoanVienByDonViDoanId(int donViDoanId)
        {
            var data = _doanVienService.GetDoanVienByDonViDoanId(donViDoanId);
            return Json(new { Data = data }, JsonRequestBehavior.AllowGet);
        }
    }
}
