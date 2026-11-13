using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Dashboard.Services
{
    public interface IGiaoViecService
    {
        DataTableResult<GiaoViecGridVo> GetAll(GiaoViecQueryViewModel queryParams);
        GiaoViecGridVo GetById(int id);
        List<GiaoViecLichSuGridVo> GetLichSu(GiaoViecLichSuQueryViewModel query);
        List<GiaoViecThanhVienTienTrinhGridVo> GetByTienTrinhCapNhatCuaThanhVien(int giaoViecThanhVienId);
        DataTableResult<GiaoViecThanhVienGridVo> GetListCongViecCanXuLy(GiaoViecQueryViewModel queryParams);
        DataTableResult<GiaoViecThanhVienGridVo> GetListCongViecDaChuyenGiao(GiaoViecQueryViewModel queryParams);
        DataTableResult<GiaoViecThanhVienGridVo> GetListCongViecTuGiaoCanXuLy(GiaoViecQueryViewModel queryParams);
        dynamic GuiNhacNho(GuiNhacNhoViewModel viewModel);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(GiaoViecViewModel viewModel);
        dynamic ThemTienTrinhXuLyCuaThanhVien(GiaoViecThanhVienTienTrinhViewModel viewModel);
        dynamic HuyXacNhanHoanThanhCuaThanhVien(HuyXacNhanHoanThanhViewModel viewModel);
        bool Xoa(int id);
        bool XoaCongViecTuGiao(int giaoViecId, int currentUserId);
        dynamic XoaTienTrinhCuaThanhVien(GiaoViecThanhVienTienTrinhGridVo giaoViecThanhVienTienTrinhGridVo);
        int DemSoLuongCongViecDaGiao(int nguoiGiaoId);
        int DemSoLuongCongViecCanXuLy(int thanhVienId);
        int DemSoLuongCongViecDaChuyenGiaoChuaHoanThanh(int nguoiChuyenGiaoId);
        dynamic ChuyenGiaoViecTiepTuc(ChuyenGiaoViecTiepTucViewModel viewModel);
        dynamic LayLaiCongViecChuyenGiao(int giaoViecThanhVienId, int currentUserId);
        List<GoiYNoiDungItemVo> GetListGoiYNoiDung(GoiYNoiDungQueryViewModel queryParams);
    }

    public class GiaoViecService : IGiaoViecService
    {
        private readonly IGiaoViecRepository _repository;
        private readonly IVanBanPhapLuatRepository _vanBanPhapLuatRepository;
        public GiaoViecService(IGiaoViecRepository repository, IVanBanPhapLuatRepository vanBanPhapLuatRepository)
        {
            _repository = repository;
            _vanBanPhapLuatRepository = vanBanPhapLuatRepository;
        }

        public DataTableResult<GiaoViecGridVo> GetAll(GiaoViecQueryViewModel queryParams)
        {
            var datas = _repository.GetAll(queryParams);
            foreach (var item in datas.rows)
            {
                if (item.NguoiGiaoId.HasValue && item.NguoiGiaoId.Value > 0)
                {
                    var nguoiGiao = CacheManagerHelper.GetUserByIdFromCache(item.NguoiGiaoId.Value);
                    if (nguoiGiao != null)
                    {
                        item.NguoiGiao = nguoiGiao.Name;
                    }
                }

                foreach (var tv in item.GiaoViecThanhViens)
                {
                    var user = CacheManagerHelper.GetUserByIdFromCache((int)tv.ThanhVienId);
                    if (user != null)
                    {
                        tv.ThanhVien = user.Name;

                        if (tv.VaiTroXuLyId.HasValue)
                        {
                            var vaiTroXuLy = CacheManagerHelper.GetVaiTroXuLyByIdFromCache(tv.VaiTroXuLyId.Value);
                            if (vaiTroXuLy != null)
                            {
                                tv.TenVaiTroXuLy = vaiTroXuLy.Ten;
                            }
                        }

                        var tienTrinhLast = tv.GiaoViecThanhVienTienTrinhs.OrderByDescending(o => o.NgayTao).FirstOrDefault();
                        if (tienTrinhLast != null)
                        {
                            tv.PhanTramHoanThanh = (int)tienTrinhLast.PhanTramHoanThanh;
                        }

                        MapNguoiTheoDoiChiDao(tv);
                    }
                }
            }
            return datas;
        }

        public GiaoViecGridVo GetById(int id)
        {
            var data = _repository.GetById(id);
            var vbs = _vanBanPhapLuatRepository.GetByListId(data.GiaoViecVanBanLienQuans.Select(o => (int)o.VanBanPhapLuatId).ToList());

            foreach (var vb in data.GiaoViecVanBanLienQuans)
            {
                var vbct = vbs.FirstOrDefault(o => o.Id == vb.VanBanPhapLuatId);
                if (vbct != null)
                {
                    vb.SoKiHieu = vbct.SoKiHieu;
                    vb.TrichYeu = vbct.TrichYeu;
                    vb.NgayBanHanh = vbct.NgayBanHanh;
                    vb.CoQuanBanHanhStr = CacheManagerHelper.GetCoQuanBanHanhCache().FirstOrDefault(o => o.Id == vbct.CoQuanBanHanhId)?.Ten;
                }
            }

            if (data.GiaoViecThanhViens != null)
            {
                foreach (var tv in data.GiaoViecThanhViens)
                {
                    if (tv.ThanhVienId.HasValue && tv.ThanhVienId.Value > 0)
                    {
                        var user = CacheManagerHelper.GetUserByIdFromCache(tv.ThanhVienId.Value);
                        if (user != null)
                        {
                            tv.ThanhVien = user.Name;
                            tv.PhongBanId = user.PhongBanId.GetValueOrDefault();
                        }
                    }

                    if (tv.VaiTroXuLyId.HasValue)
                    {
                        var vaiTroXuLy = CacheManagerHelper.GetVaiTroXuLyByIdFromCache(tv.VaiTroXuLyId.Value);
                        if (vaiTroXuLy != null)
                        {
                            tv.TenVaiTroXuLy = vaiTroXuLy.Ten;
                        }
                    }

                    MapNguoiTheoDoiChiDao(tv);
                }
            }

            return data;
        }

        public List<GiaoViecLichSuGridVo> GetLichSu(GiaoViecLichSuQueryViewModel query)
        {
            return _repository.GetLichSu(query);
        }

        public List<GiaoViecThanhVienTienTrinhGridVo> GetByTienTrinhCapNhatCuaThanhVien(int giaoViecThanhVienId)
        {
            return _repository.GetByTienTrinhCapNhatCuaThanhVien(giaoViecThanhVienId);
        }

        public DataTableResult<GiaoViecThanhVienGridVo> GetListCongViecCanXuLy(GiaoViecQueryViewModel queryParams)
        {
            var datas = _repository.GetListCongViecCanXuLy(queryParams);

            var giaoViecIds = datas.rows.Select(o => o.GiaoViecId).Distinct().ToList();
            var danhSachGiaoViecThanhVien = _repository.GetListGiaoViecThanhVienByListGiaoViecIds(giaoViecIds);

            var giaoViecThanhVienChuyenGiaoIds = datas.rows
                .Where(o => o.GiaoViecThanhVienChuyenGiaoId.HasValue)
                .Select(o => o.GiaoViecThanhVienChuyenGiaoId.Value)
                .Distinct()
                .ToList();

            var danhSachNguoiChuyenGiao = new List<NguoiChuyenGiaoInfo>();
            foreach (var chuyenGiaoId in giaoViecThanhVienChuyenGiaoIds)
            {
                var nguoiChuyenGiaoThanhVien = _repository.GetGiaoViecThanhVienById(chuyenGiaoId);
                if (nguoiChuyenGiaoThanhVien != null && nguoiChuyenGiaoThanhVien.ThanhVienId.HasValue)
                {
                    var nguoiChuyenGiao = CacheManagerHelper.GetUserByIdFromCache((int)nguoiChuyenGiaoThanhVien.ThanhVienId);
                    if (nguoiChuyenGiao != null)
                    {
                        danhSachNguoiChuyenGiao.Add(new NguoiChuyenGiaoInfo
                        {
                            GiaoViecThanhVienChuyenGiaoId = chuyenGiaoId,
                            TenNguoiChuyenGiao = nguoiChuyenGiao.Name
                        });
                    }
                }
            }

            foreach (var item in datas.rows)
            {
                MapNguoiTheoDoiChiDao(item);

                if (item.GiaoViecThanhViens == null)
                {
                    item.GiaoViecThanhViens = new List<GiaoViecThanhVienGridVo>();
                }

                var nguoiGiao = CacheManagerHelper.GetUserByIdFromCache((int)item.NguoiGiaoId);
                if (nguoiGiao != null)
                {
                    item.NguoiGiao = nguoiGiao.Name;
                }

                if (item.GiaoViecThanhVienChuyenGiaoId.HasValue)
                {
                    var nguoiChuyenGiaoInfo = danhSachNguoiChuyenGiao.FirstOrDefault(o => o.GiaoViecThanhVienChuyenGiaoId == item.GiaoViecThanhVienChuyenGiaoId.Value);
                    if (nguoiChuyenGiaoInfo != null)
                    {
                        item.NguoiChuyenGiao = nguoiChuyenGiaoInfo.TenNguoiChuyenGiao;
                    }
                }

                var giaoViecInfo = danhSachGiaoViecThanhVien.FirstOrDefault(o => o.GiaoViecId == item.GiaoViecId);
                if (giaoViecInfo != null)
                {
                    var thanhViens = giaoViecInfo.GiaoViecThanhViens
                        .Where(o => o.ThanhVienId != queryParams.NguoiDuocGiaoId)
                        .ToList();

                    foreach (var thanhVien in thanhViens)
                    {
                        if (thanhVien.ThanhVienId.HasValue)
                        {
                            var user = CacheManagerHelper.GetUserByIdFromCache((int)thanhVien.ThanhVienId);
                            if (user != null)
                            {
                                thanhVien.ThanhVien = user.Name;
                            }
                        }
                        MapNguoiTheoDoiChiDao(thanhVien);
                    }
                    item.GiaoViecThanhViens = thanhViens;
                }
            }

            return datas;
        }

        public DataTableResult<GiaoViecThanhVienGridVo> GetListCongViecDaChuyenGiao(GiaoViecQueryViewModel queryParams)
        {
            var datas = _repository.GetListCongViecDaChuyenGiao(queryParams);

            // Lấy tất cả các Id để query một lần
            var giaoViecThanhVienIds = datas.rows.Select(o => o.Id).ToList();

            // Lấy tất cả người nhận trong một lần query
            var listNguoiNhans = _repository.GetListNguoiNhanChuyenGiaoByListIds(giaoViecThanhVienIds);

            foreach (var item in datas.rows)
            {
                MapNguoiTheoDoiChiDao(item);

                var nguoiGiao = CacheManagerHelper.GetUserByIdFromCache((int)item.NguoiGiaoId);
                if (nguoiGiao != null)
                {
                    item.NguoiGiao = nguoiGiao.Name;
                }

                // Tìm danh sách người nhận cho item này
                var nguoiNhanItem = listNguoiNhans.FirstOrDefault(o => o.GiaoViecThanhVienId == item.Id);
                if (nguoiNhanItem != null && nguoiNhanItem.NguoiNhans != null && nguoiNhanItem.NguoiNhans.Count > 0)
                {
                    foreach (var nguoiNhan in nguoiNhanItem.NguoiNhans)
                    {
                        if (nguoiNhan.ThanhVienId.HasValue)
                        {
                            var user = CacheManagerHelper.GetUserByIdFromCache((int)nguoiNhan.ThanhVienId);
                            if (user != null)
                            {
                                nguoiNhan.ThanhVien = user.Name;
                            }

                            var tienTrinhLast = _repository.LayThongTinTienDoCuoiCung(nguoiNhan.Id);
                            if (tienTrinhLast != null)
                            {
                                nguoiNhan.PhanTramHoanThanh = tienTrinhLast.PhanTramHoanThanh ?? 0;
                            }
                        }
                        MapNguoiTheoDoiChiDao(nguoiNhan);
                    }
                    item.GiaoViecThanhViens = nguoiNhanItem.NguoiNhans;
                }
            }

            return datas;
        }

        public DataTableResult<GiaoViecThanhVienGridVo> GetListCongViecTuGiaoCanXuLy(GiaoViecQueryViewModel queryParams)
        {
            var datas = _repository.GetListCongViecTuGiaoCanXuLy(queryParams);

            var giaoViecThanhVienChuyenGiaoIds = datas.rows
                .Where(o => o.GiaoViecThanhVienChuyenGiaoId.HasValue)
                .Select(o => o.GiaoViecThanhVienChuyenGiaoId.Value)
                .Distinct()
                .ToList();

            var danhSachNguoiChuyenGiao = new List<NguoiChuyenGiaoInfo>();
            foreach (var chuyenGiaoId in giaoViecThanhVienChuyenGiaoIds)
            {
                var nguoiChuyenGiaoThanhVien = _repository.GetGiaoViecThanhVienById(chuyenGiaoId);
                if (nguoiChuyenGiaoThanhVien != null && nguoiChuyenGiaoThanhVien.ThanhVienId.HasValue)
                {
                    var nguoiChuyenGiao = CacheManagerHelper.GetUserByIdFromCache((int)nguoiChuyenGiaoThanhVien.ThanhVienId);
                    if (nguoiChuyenGiao != null)
                    {
                        danhSachNguoiChuyenGiao.Add(new NguoiChuyenGiaoInfo
                        {
                            GiaoViecThanhVienChuyenGiaoId = chuyenGiaoId,
                            TenNguoiChuyenGiao = nguoiChuyenGiao.Name
                        });
                    }
                }
            }

            foreach (var item in datas.rows)
            {
                MapNguoiTheoDoiChiDao(item);

                var nguoiGiao = CacheManagerHelper.GetUserByIdFromCache((int)item.NguoiGiaoId);
                if (nguoiGiao != null)
                {
                    item.NguoiGiao = nguoiGiao.Name;
                }

                if (item.GiaoViecThanhVienChuyenGiaoId.HasValue)
                {
                    var nguoiChuyenGiaoInfo = danhSachNguoiChuyenGiao.FirstOrDefault(o => o.GiaoViecThanhVienChuyenGiaoId == item.GiaoViecThanhVienChuyenGiaoId.Value);
                    if (nguoiChuyenGiaoInfo != null)
                    {
                        item.NguoiChuyenGiao = nguoiChuyenGiaoInfo.TenNguoiChuyenGiao;
                    }
                }
            }

            return datas;
        }

        private void MapNguoiTheoDoiChiDao(GiaoViecThanhVienGridVo tv)
        {
            if (tv != null && tv.NguoiTheoDoiChiDaoId.GetValueOrDefault() > 0)
            {
                var nguoiTheoDoi = CacheManagerHelper.GetUserByIdFromCache(tv.NguoiTheoDoiChiDaoId.Value);
                if (nguoiTheoDoi != null)
                {
                    tv.TenNguoiTheoDoiChiDao = nguoiTheoDoi.Name;
                    tv.NguoiTheoDoiChiDao = nguoiTheoDoi.Name;
                }
            }
        }

        public dynamic GuiNhacNho(GuiNhacNhoViewModel viewModel)
        {
            return _repository.GuiNhacNho(viewModel);
        }

        public bool KhoiPhuc(int id)
        {
            return _repository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(GiaoViecViewModel viewModel)
        {
            var phongBans = CacheManagerHelper.GetPhongBanNhanViecCacheNotDeleted();
            var users = CacheManagerHelper.GetListUserFromCacheNotDeleted();
            var errors = new List<ValidateError>();

            foreach (var thanhVien in viewModel.ThanhViens.Where(o => o.PhongBanId.GetValueOrDefault() != 0))
            {
                var phongBan = phongBans.FirstOrDefault(o => o.Id == thanhVien.PhongBanId.GetValueOrDefault());
                if (phongBan == null)
                {
                    errors.Add(new ValidateError { Id = "ThanhVien", Description = "Phòng được chọn không tồn tại hoặc không nhận việc." });
                    continue;
                }

                var truongPhongs = users.Where(o => o.PhongBanId == phongBan.Id && o.IsTruongPhong == true).ToList();
                var nguoiNhan = truongPhongs.Count == 1 ? truongPhongs[0] : null;
                if (nguoiNhan == null)
                {
                    errors.Add(new ValidateError
                    {
                        Id = "ThanhVien",
                        Description = $"Phòng {phongBan.Ten} chưa xác định được một trưởng phòng nhận việc."
                    });
                    continue;
                }

                thanhVien.ThanhVienId = nguoiNhan.Id;
                thanhVien.ThanhVien = nguoiNhan.Name;
            }

            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (viewModel.ThanhViens.GroupBy(o => o.ThanhVienId).Any(o => o.Key.GetValueOrDefault() != 0 && o.Count() > 1))
            {
                return new { Errors = new List<ValidateError> { new ValidateError { Id = "ThanhVien", Description = "Một trưởng phòng đang nhận việc từ nhiều phòng được chọn." } } };
            }

            return _repository.ThemOrCapNhat(viewModel);
        }

        public dynamic ThemTienTrinhXuLyCuaThanhVien(GiaoViecThanhVienTienTrinhViewModel viewModel)
        {
            return _repository.ThemTienTrinhXuLyCuaThanhVien(viewModel);
        }

        public dynamic HuyXacNhanHoanThanhCuaThanhVien(HuyXacNhanHoanThanhViewModel viewModel)
        {
            return _repository.HuyXacNhanHoanThanhCuaThanhVien(viewModel);
        }

        public bool Xoa(int id)
        {
            return _repository.Xoa(id);
        }

        public bool XoaCongViecTuGiao(int giaoViecId, int currentUserId)
        {
            return _repository.XoaCongViecTuGiao(giaoViecId, currentUserId);
        }

        public dynamic XoaTienTrinhCuaThanhVien(GiaoViecThanhVienTienTrinhGridVo giaoViecThanhVienTienTrinhGridVo)
        {
            return _repository.XoaTienTrinhCuaThanhVien(giaoViecThanhVienTienTrinhGridVo);
        }

        public int DemSoLuongCongViecDaGiao(int nguoiGiaoId)
        {
            return _repository.DemSoLuongCongViecDaGiao(nguoiGiaoId);
        }

        public int DemSoLuongCongViecCanXuLy(int thanhVienId)
        {
            return _repository.DemSoLuongCongViecCanXuLy(thanhVienId);
        }

        public int DemSoLuongCongViecDaChuyenGiaoChuaHoanThanh(int nguoiChuyenGiaoId)
        {
            return _repository.DemSoLuongCongViecDaChuyenGiaoChuaHoanThanh(nguoiChuyenGiaoId);
        }

        public dynamic ChuyenGiaoViecTiepTuc(ChuyenGiaoViecTiepTucViewModel viewModel)
        {
            return _repository.ChuyenGiaoViecTiepTuc(viewModel);
        }

        public dynamic LayLaiCongViecChuyenGiao(int giaoViecThanhVienId, int currentUserId)
        {
            return _repository.LayLaiCongViecChuyenGiao(giaoViecThanhVienId, currentUserId);
        }

        public List<GoiYNoiDungItemVo> GetListGoiYNoiDung(GoiYNoiDungQueryViewModel queryParams)
        {
            return _repository.GetListGoiYNoiDung(queryParams);
        }
    }
}

