using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Dashboard.Services
{
    public interface IDuAnService
    {
        DataTableResult<DuAnGridVo> GetAll(DuAnQueryViewModel queryParams);
        DuAnGridVo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(DuAnViewModel model);
        bool Xoa(int id);
        List<ThongKeThanhVienGridVo> ThongKeThanhVienThamGiaNhieuNhat();
        List<ThongKeThanhVienGridVo> ThongKeThanhVienThamGiaThuNghiemNhieuNhat();
        List<ThongKeThanhVienGridVo> ThongKeThanhVienMucDoHoTroCaoNhat();
        List<DuAnGridVo> GetListDuAnByUserId(int userId);
        List<DuAnGridVo> GetListDuAnThuNghiemByUserId(int userId);
        List<DuAnGridVo> GetListAllDuAnByUserId(int userId);
        List<DuAn> GetListAllDuAnActive();
        List<ThongKeDuAnGridVo> ThongKeDuAnTheoSoThanhVien();
        List<DuAnThanhVienInfo> GetListThanhVienByDuAnId(int duAnId);
        List<DuAn> GetListDuAnNotThuNghiem();
        List<MatranThanhVienDuAnInfo> GetMatranThanhVienDuAn();
        List<DuAnThanhVienThongKeGridVo> GetDuAnThanhVienThongKe();
        List<TruongNhomThongKeGridVo> GetTruongNhomThongKe();
    }

    public class DuAnService : IDuAnService
    {
        private readonly IDuAnRepository _repository;

        public DuAnService(IDuAnRepository repository)
        {
            _repository = repository;
        }

        public DataTableResult<DuAnGridVo> GetAll(DuAnQueryViewModel queryParams)
        {
            var result = _repository.GetAll(queryParams);
            MapThongTinThanhVienChoDuAn(result.rows);
            return result;
        }

        private void MapThongTinThanhVienChoDuAn(List<DuAnGridVo> duAns)
        {
            if (duAns == null || duAns.Count == 0)
            {
                return;
            }

            var users = CacheManagerHelper.GetUserCache();
            foreach (var item in duAns)
            {
                var danhSachThanhVien = new List<DuAnThanhVienInfo>();

                foreach (var userId in item.ThanhViens)
                {
                    if (userId.HasValue)
                    {
                        var user = users.FirstOrDefault(u => u.Id == userId);
                        if (user != null)
                        {
                            bool isTruongNhom = item.TruongNhomIds.Contains(userId);
                            danhSachThanhVien.Add(new DuAnThanhVienInfo
                            {
                                UserId = userId.Value,
                                TenUser = user.Name,
                                IsTruongNhom = isTruongNhom
                            });
                        }
                    }
                }

                danhSachThanhVien = danhSachThanhVien.OrderByDescending(x => x.IsTruongNhom).ThenBy(x => x.TenUser).ToList();
                item.DanhSachThanhVien = danhSachThanhVien;
            }
        }

        public DuAnGridVo GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _repository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(DuAnViewModel model)
        {
            return _repository.ThemOrCapNhat(model);
        }

        public bool Xoa(int id)
        {
            return _repository.Xoa(id);
        }

        public List<ThongKeThanhVienGridVo> ThongKeThanhVienThamGiaNhieuNhat()
        {
            var result = _repository.ThongKeThanhVienThamGiaNhieuNhat();
            MapTenUserChoThongKeThanhVien(result);
            return result;
        }

        public List<ThongKeThanhVienGridVo> ThongKeThanhVienThamGiaThuNghiemNhieuNhat()
        {
            var result = _repository.ThongKeThanhVienThamGiaThuNghiemNhieuNhat();
            MapTenUserChoThongKeThanhVien(result);
            return result;
        }

        public List<ThongKeThanhVienGridVo> ThongKeThanhVienMucDoHoTroCaoNhat()
        {
            var result = _repository.ThongKeThanhVienMucDoHoTroCaoNhat();
            MapTenUserChoThongKeThanhVien(result);
            return result;
        }

        private void MapTenUserChoThongKeThanhVien(List<ThongKeThanhVienGridVo> thongKes)
        {
            if (thongKes == null || thongKes.Count == 0)
            {
                return;
            }

            var users = CacheManagerHelper.GetUserCache();
            foreach (var item in thongKes)
            {
                var user = users.FirstOrDefault(u => u.Id == item.UserId);
                if (user != null)
                {
                    item.TenUser = user.Name;
                }
            }
        }

        public List<DuAnGridVo> GetListDuAnByUserId(int userId)
        {
            return _repository.GetListDuAnByUserId(userId);
        }

        public List<DuAnGridVo> GetListDuAnThuNghiemByUserId(int userId)
        {
            return _repository.GetListDuAnThuNghiemByUserId(userId);
        }

        public List<DuAnGridVo> GetListAllDuAnByUserId(int userId)
        {
            return _repository.GetListAllDuAnByUserId(userId);
        }

        public List<DuAn> GetListAllDuAnActive()
        {
            return _repository.GetListAllDuAnActive();
        }

        public List<ThongKeDuAnGridVo> ThongKeDuAnTheoSoThanhVien()
        {
            var result = _repository.ThongKeDuAnTheoSoThanhVien();
            MapTenUserChoThongKeDuAn(result);
            return result;
        }

        public List<DuAnThanhVienInfo> GetListThanhVienByDuAnId(int duAnId)
        {
            var result = _repository.GetListThanhVienByDuAnId(duAnId);
            MapTenUserChoDuAnThanhVienInfo(result);
            return result;
        }

        private void MapTenUserChoThongKeDuAn(List<ThongKeDuAnGridVo> duAns)
        {
            // Không cần map tên user ở đây vì ThongKeDuAnGridVo không có UserId
        }

        private void MapTenUserChoDuAnThanhVienInfo(List<DuAnThanhVienInfo> thanhViens)
        {
            if (thanhViens == null || thanhViens.Count == 0)
            {
                return;
            }

            var users = CacheManagerHelper.GetUserCache();
            foreach (var item in thanhViens)
            {
                var user = users.FirstOrDefault(u => u.Id == item.UserId);
                if (user != null)
                {
                    item.TenUser = user.Name;
                }
            }
        }

        public List<DuAn> GetListDuAnNotThuNghiem()
        {
            return _repository.GetListDuAnNotThuNghiem();
        }

        public List<MatranThanhVienDuAnInfo> GetMatranThanhVienDuAn()
        {
            return _repository.GetMatranThanhVienDuAn();
        }

        public List<DuAnThanhVienThongKeGridVo> GetDuAnThanhVienThongKe()
        {
            var result = _repository.GetDuAnThanhVienThongKe();
            MapTenUserChoDuAnThanhVienThongKe(result);
            return result;
        }

        private void MapTenUserChoDuAnThanhVienThongKe(List<DuAnThanhVienThongKeGridVo> thongKes)
        {
            if (thongKes == null || thongKes.Count == 0)
            {
                return;
            }

            var users = CacheManagerHelper.GetUserCache();
            foreach (var item in thongKes)
            {
                var user = users.FirstOrDefault(u => u.Id == item.UserId);
                if (user != null)
                {
                    item.TenUser = user.Name;
                }
            }
        }

        public List<TruongNhomThongKeGridVo> GetTruongNhomThongKe()
        {
            var result = _repository.GetTruongNhomThongKe();
            MapTenUserChoTruongNhomThongKe(result);
            return result;
        }

        private void MapTenUserChoTruongNhomThongKe(List<TruongNhomThongKeGridVo> thongKes)
        {
            if (thongKes == null || thongKes.Count == 0)
            {
                return;
            }

            var users = CacheManagerHelper.GetUserCache();
            foreach (var item in thongKes)
            {
                var user = users.FirstOrDefault(u => u.Id == item.UserId);
                if (user != null)
                {
                    item.TenUser = user.Name;
                }
            }
        }
    }
}
