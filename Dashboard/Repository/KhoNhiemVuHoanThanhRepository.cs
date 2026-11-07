using Dashboard.AdoNetHelper;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.CommonModel;
using Dashboard.Models.Entities;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace Dashboard.Repository
{
    public interface IKhoNhiemVuHoanThanhRepository
    {
        DataTableResult<GiaoViecThanhVienGridVo> GetListHoanThanh(KhoNhiemVuHoanThanhQueryViewModel queryParams);
        dynamic HuyDanhGia(HuyDanhGiaViewModel viewModel);
        int DemSoLuongHoanThanh(int currentUserId);
    }

    public class KhoNhiemVuHoanThanhRepository : IKhoNhiemVuHoanThanhRepository
    {
        private readonly IContext _context;
        private readonly string sortStringDefault = "NgayDanhGia desc";
        private readonly List<string> searchCol = new List<string> { "TenCongViec", "NoiDung", "NoiDungNhiemVu", "YKienDanhGia" };

        public KhoNhiemVuHoanThanhRepository(IContext context)
        {
            _context = context;
        }

        public DataTableResult<GiaoViecThanhVienGridVo> GetListHoanThanh(KhoNhiemVuHoanThanhQueryViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            int currentUserId = queryParams.CurrentUserId.GetValueOrDefault();
            var currentUser = CacheManagerHelper.GetUserByIdFromCache(currentUserId);
            bool isSystemUser = currentUser != null && currentUser.IsSystemUser == true;

            var query = _context.GiaoViecThanhViens
                .Include(o => o.GiaoViec)
                .Where(o => o.IsDeleted != true
                    && (o.GiaoViec.IsDeleted == null || o.GiaoViec.IsDeleted == false)
                    && (o.TrangThai == (int)EnumTrangThaiGiaoViec.HoanThanh || (o.TrangThai == 2) || (o.IsHoanThanh == true && o.TrangThai == null)))
                .AsNoTracking()
                .AsQueryable();

            if (!isSystemUser)
            {
                query = query.Where(o => o.GiaoViec.NguoiGiaoId == currentUserId
                    || o.NguoiTheoDoiChiDaoId == currentUserId
                    || o.ThanhVienId == currentUserId);
            }

            if (queryParams.TuNgay.HasValue)
            {
                query = query.Where(o => (o.NgayDanhGia ?? o.NgayHoanThanh ?? o.GiaoViec.NgayTao) >= queryParams.TuNgay.Value);
            }
            if (queryParams.DenNgay.HasValue)
            {
                query = query.Where(o => (o.NgayDanhGia ?? o.NgayHoanThanh ?? o.GiaoViec.NgayTao) <= queryParams.DenNgay.Value);
            }

            if (queryParams.ThanhVienId.GetValueOrDefault() > 0)
            {
                query = query.Where(o => o.ThanhVienId == queryParams.ThanhVienId.Value);
            }

            if (queryParams.NguoiGiaoId.GetValueOrDefault() > 0)
            {
                query = query.Where(o => o.GiaoViec.NguoiGiaoId == queryParams.NguoiGiaoId.Value);
            }

            var projected = query.Select(x => new GiaoViecThanhVienGridVo
            {
                Id = x.Id,
                GiaoViecId = x.GiaoViec.Id,
                TenCongViec = x.GiaoViec.TenCongViec,
                NoiDung = x.GiaoViec.NoiDung,
                GhiChu = x.GiaoViec.GhiChu,
                NgayTao = x.GiaoViec.NgayTao,
                NguoiGiaoId = x.GiaoViec.NguoiGiaoId,
                ThanhVienId = x.ThanhVienId,
                HanXuLy = x.HanXuLy,
                NoiDungNhiemVu = x.NoiDungNhiemVu,
                VaiTroXuLyId = x.VaiTroXuLyId,
                NguoiTheoDoiChiDaoId = x.NguoiTheoDoiChiDaoId,
                TrangThai = x.TrangThai,
                NgayXacNhanHoanThanh = x.NgayXacNhanHoanThanh,
                IsHoanThanh = x.IsHoanThanh,
                NgayHoanThanh = x.NgayHoanThanh,
                KetQuaDanhGia = x.KetQuaDanhGia,
                YKienDanhGia = x.YKienDanhGia,
                NguoiDanhGiaId = x.NguoiDanhGiaId,
                NgayDanhGia = x.NgayDanhGia
            }).Where(queryParams.searchString);

            var total = projected.Count();
            var rows = projected.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList();

            foreach (var row in rows)
            {
                if (row.ThanhVienId.GetValueOrDefault() > 0)
                {
                    var u = CacheManagerHelper.GetUserByIdFromCache(row.ThanhVienId.Value);
                    if (u != null) row.ThanhVien = u.Name;
                }
                if (row.NguoiGiaoId.GetValueOrDefault() > 0)
                {
                    var u = CacheManagerHelper.GetUserByIdFromCache(row.NguoiGiaoId.Value);
                    if (u != null) row.NguoiGiao = u.Name;
                }
                if (row.VaiTroXuLyId.GetValueOrDefault() > 0)
                {
                    var vt = CacheManagerHelper.GetVaiTroXuLyByIdFromCache(row.VaiTroXuLyId.Value);
                    if (vt != null) row.TenVaiTroXuLy = vt.Ten;
                }
                if (row.NguoiDanhGiaId.GetValueOrDefault() > 0)
                {
                    var u = CacheManagerHelper.GetUserByIdFromCache(row.NguoiDanhGiaId.Value);
                    if (u != null) row.NguoiDanhGia = u.Name;
                }
                if (row.NguoiTheoDoiChiDaoId.GetValueOrDefault() > 0)
                {
                    var u = CacheManagerHelper.GetUserByIdFromCache(row.NguoiTheoDoiChiDaoId.Value);
                    if (u != null) row.NguoiTheoDoiChiDao = u.Name;
                }

                bool isNguoiGiao = row.NguoiGiaoId == currentUserId;
                bool isNguoiTheoDoi = row.NguoiTheoDoiChiDaoId == currentUserId;
                row.CanHuyDanhGia = isSystemUser || isNguoiGiao || isNguoiTheoDoi;
            }

            return new DataTableResult<GiaoViecThanhVienGridVo>
            {
                rows = rows,
                total = total
            };
        }

        public dynamic HuyDanhGia(HuyDanhGiaViewModel viewModel)
        {
            var thanhVien = _context.GiaoViecThanhViens
                .Include(o => o.GiaoViec)
                .FirstOrDefault(o => o.Id == viewModel.GiaoViecThanhVienId && o.IsDeleted != true);

            if (thanhVien == null)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "GiaoViecThanhVienId",
                            Description = "Không tìm thấy nhiệm vụ cần huỷ đánh giá."
                        }
                    }
                };
            }

            bool isNguoiGiao = thanhVien.GiaoViec != null && thanhVien.GiaoViec.NguoiGiaoId == viewModel.CurrentUserId;
            bool isNguoiTheoDoi = thanhVien.NguoiTheoDoiChiDaoId.GetValueOrDefault() == viewModel.CurrentUserId;
            var currentUser = CacheManagerHelper.GetUserByIdFromCache(viewModel.CurrentUserId);
            bool isSystem = currentUser != null && currentUser.IsSystemUser == true;

            if (!isNguoiGiao && !isNguoiTheoDoi && !isSystem)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "Permission",
                            Description = "Bạn không có quyền huỷ đánh giá nhiệm vụ này."
                        }
                    }
                };
            }

            thanhVien.TrangThai = (int)EnumTrangThaiGiaoViec.ChoDanhGia;
            thanhVien.IsHoanThanh = false;
            thanhVien.NgayHoanThanh = null;
            thanhVien.KetQuaDanhGia = null;
            thanhVien.YKienDanhGia = null;
            thanhVien.NguoiDanhGiaId = null;
            thanhVien.NgayDanhGia = null;

            var danhGias = _context.GiaoViecDanhGias
                .Where(o => o.GiaoViecThanhVienId == thanhVien.Id && o.IsDeleted != true)
                .ToList();
            foreach (var dg in danhGias)
            {
                dg.IsDeleted = true;
            }

            _context.SaveChanges();

            int giaoViecId = thanhVien.GiaoViecId.GetValueOrDefault();
            int thanhVienId = thanhVien.ThanhVienId.GetValueOrDefault();
            string tenCongViec = thanhVien.GiaoViec != null ? thanhVien.GiaoViec.TenCongViec : "";

            Task.Run(async () =>
            {
                await GiaoViecHelper.CapNhatTrangThaiHoanThanhCongViecToanBoAdoNet(giaoViecId);
            });

            if (thanhVienId > 0)
            {
                string lyDo = !string.IsNullOrWhiteSpace(viewModel.LyDoHuy) ? $"\nLý do: {viewModel.LyDoHuy}" : "";
                string noiDungThongBao = $"Đánh giá nhiệm vụ \"{tenCongViec}\" của bạn đã được huỷ để thực hiện đánh giá lại.{lyDo}";

                ThongBaoAdoNetHelper.Them(new ThongBao
                {
                    NoiDung = noiDungThongBao,
                    NguoiNhanId = thanhVienId,
                    EnumLoaiThongBao = EnumLoaiThongBao.CongViecMoiCanXuLy,
                    NgayTao = DateTime.Now,
                    IsDaXem = false
                }, BienDungChung.ConnectDBStr);
            }

            return true;
        }

        public int DemSoLuongHoanThanh(int currentUserId)
        {
            var currentUser = CacheManagerHelper.GetUserByIdFromCache(currentUserId);
            bool isSystemUser = currentUser != null && currentUser.IsSystemUser == true;

            var query = _context.GiaoViecThanhViens
                .Where(o => o.IsDeleted != true
                    && (o.GiaoViec.IsDeleted == null || o.GiaoViec.IsDeleted == false)
                    && (o.TrangThai == (int)EnumTrangThaiGiaoViec.HoanThanh || (o.TrangThai == 2) || (o.IsHoanThanh == true && o.TrangThai == null)));

            if (!isSystemUser)
            {
                query = query.Where(o => o.GiaoViec.NguoiGiaoId == currentUserId
                    || o.NguoiTheoDoiChiDaoId == currentUserId
                    || o.ThanhVienId == currentUserId);
            }

            return query.Count();
        }
    }
}
