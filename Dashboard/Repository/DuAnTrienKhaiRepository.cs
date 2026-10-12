using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace Dashboard.Repository
{
    public interface IDuAnTrienKhaiRepository
    {
        DataTableResult<DuAnTrienKhaiGridVo> GetAll(DuAnTrienKhaiQueryViewModel queryParams);
        DuAnTrienKhaiGridVo GetById(int id);
        dynamic ThemOrCapNhat(DuAnTrienKhaiViewModel model);
        bool Xoa(int id);
        bool KhoiPhuc(int id);
        List<DuAnTrienKhaiHangMucGridVo> GetHangMucMau(int duAnId);
    }

    public class DuAnTrienKhaiRepository : IDuAnTrienKhaiRepository
    {
        private readonly IContext _context;
        private readonly IDuAnTrienKhaiValidate _validate;
        private readonly string sortStringDefault = "NgayBatDau desc";
        private readonly List<string> searchCol = new List<string> { "Ten", "TenDuAn", "TenDonVi", "MoTa" };

        public DuAnTrienKhaiRepository(IContext context, IDuAnTrienKhaiValidate validate)
        {
            _context = context;
            _validate = validate;
        }

        public DataTableResult<DuAnTrienKhaiGridVo> GetAll(DuAnTrienKhaiQueryViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.DuAnTrienKhais
                .Include(x => x.DuAn)
                .Include(x => x.DonVi)
                .Include(x => x.DuAnTrienKhaiHangMucs)
                .AsNoTracking()
                .AsQueryable();

            query = queryParams.DaXoaSearch ? query.Where(x => x.IsDeleted == true) : query.Where(x => x.IsDeleted != true);

            if (queryParams.DuAnIdSearch.HasValue && queryParams.DuAnIdSearch.Value > 0)
            {
                query = query.Where(x => x.DuAnId == queryParams.DuAnIdSearch.Value);
            }

            if (queryParams.DonViIdSearch.HasValue && queryParams.DonViIdSearch.Value > 0)
            {
                query = query.Where(x => x.DonViId == queryParams.DonViIdSearch.Value);
            }

            var data = query.Select(x => new DuAnTrienKhaiGridVo
            {
                Id = x.Id,
                DuAnId = x.DuAnId,
                DonViId = x.DonViId,
                Ten = x.Ten,
                TenDuAn = x.DuAn != null ? x.DuAn.Ten : "",
                TenDonVi = x.DonVi != null ? x.DonVi.Ten : "",
                MoTa = x.MoTa,
                NgayBatDau = x.NgayBatDau,
                NgayKetThuc = x.NgayKetThuc,
                IsDeleted = x.IsDeleted,
                SoHangMuc = x.DuAnTrienKhaiHangMucs.Count(h => h.IsDeleted != true),
                SoHangMucHoanThanh = x.DuAnTrienKhaiHangMucs.Count(h => h.IsDeleted != true && (h.TienDo ?? 0) >= 100),
                TienDoTong = (int)Math.Round(x.DuAnTrienKhaiHangMucs.Where(h => h.IsDeleted != true).Select(h => (double?)(h.TienDo ?? 0)).DefaultIfEmpty(0).Average() ?? 0),
                TrangThai = ""
            }).Where(queryParams.searchString);

            var total = data.Count();
            var result = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList();
            MapTrangThai(result);

            return new DataTableResult<DuAnTrienKhaiGridVo>
            {
                rows = result,
                total = total
            };
        }

        public DuAnTrienKhaiGridVo GetById(int id)
        {
            var item = _context.DuAnTrienKhais
                .Include(x => x.DuAn)
                .Include(x => x.DonVi)
                .Include(x => x.DuAnTrienKhaiHangMucs)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                return new DuAnTrienKhaiGridVo();
            }

            var result = new DuAnTrienKhaiGridVo
            {
                Id = item.Id,
                DuAnId = item.DuAnId,
                DonViId = item.DonViId,
                Ten = item.Ten,
                TenDuAn = item.DuAn != null ? item.DuAn.Ten : "",
                TenDonVi = item.DonVi != null ? item.DonVi.Ten : "",
                MoTa = item.MoTa,
                NgayBatDau = item.NgayBatDau,
                NgayKetThuc = item.NgayKetThuc,
                IsDeleted = item.IsDeleted,
                HangMucs = item.DuAnTrienKhaiHangMucs
                    .Where(x => x.IsDeleted != true)
                    .OrderBy(x => x.ThuTu ?? int.MaxValue)
                    .ThenBy(x => x.Id)
                    .Select(x => new DuAnTrienKhaiHangMucGridVo
                    {
                        Id = x.Id,
                        DuAnHangMucId = x.DuAnHangMucId,
                        TenHangMuc = x.TenHangMuc,
                        MoTa = x.MoTa,
                        ThuTu = x.ThuTu,
                        NgayBatDau = x.NgayBatDau,
                        NgayKetThuc = x.NgayKetThuc,
                        TienDo = x.TienDo,
                        GhiChu = x.GhiChu
                    }).ToList()
            };

            result.SoHangMuc = result.HangMucs.Count;
            result.SoHangMucHoanThanh = result.HangMucs.Count(x => (x.TienDo ?? 0) >= 100);
            result.TienDoTong = result.HangMucs.Count == 0 ? 0 : (int)Math.Round(result.HangMucs.Average(x => x.TienDo ?? 0));
            MapTrangThai(new List<DuAnTrienKhaiGridVo> { result });
            return result;
        }

        public dynamic ThemOrCapNhat(DuAnTrienKhaiViewModel model)
        {
            var errors = _validate.Validate(model);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            var hangMucs = BuildHangMucs(model);
            if (hangMucs.Count == 0)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "HangMucs",
                            Description = "Không có hạng mục nào để triển khai."
                        }
                    }
                };
            }

            if (model.Id == 0)
            {
                var entity = new DuAnTrienKhai();
                MapThongTinChung(entity, model, hangMucs);

                foreach (var item in hangMucs)
                {
                    entity.DuAnTrienKhaiHangMucs.Add(item);
                }

                _context.DuAnTrienKhais.Add(entity);
                _context.SaveChanges();
            }
            else
            {
                var entity = _context.DuAnTrienKhais
                    .Include(x => x.DuAnTrienKhaiHangMucs)
                    .FirstOrDefault(x => x.Id == model.Id);

                if (entity == null)
                {
                    throw new Exception("Dữ liệu đã bị xóa");
                }

                MapThongTinChung(entity, model, hangMucs);
                SyncHangMucs(entity, hangMucs);
                _context.SaveChanges();
            }

            return true;
        }

        public bool Xoa(int id)
        {
            var item = _context.DuAnTrienKhais.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                return false;
            }

            item.IsDeleted = true;
            _context.SaveChanges();
            return true;
        }

        public bool KhoiPhuc(int id)
        {
            var item = _context.DuAnTrienKhais.FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                return false;
            }

            item.IsDeleted = false;
            _context.SaveChanges();
            return true;
        }

        public List<DuAnTrienKhaiHangMucGridVo> GetHangMucMau(int duAnId)
        {
            return _context.DuAnHangMucs
                .Where(x => x.IsDeleted != true)
                .OrderBy(x => x.ThuTu ?? int.MaxValue)
                .ThenBy(x => x.Id)
                .Select(x => new DuAnTrienKhaiHangMucGridVo
                {
                    Id = 0,
                    DuAnHangMucId = x.Id,
                    TenHangMuc = x.TenHangMuc,
                    MoTa = x.MoTa,
                    ThuTu = x.ThuTu,
                    TienDo = 0,
                    GhiChu = ""
                }).ToList();
        }

        private void MapThongTinChung(DuAnTrienKhai entity, DuAnTrienKhaiViewModel model, List<DuAnTrienKhaiHangMuc> hangMucs)
        {
            entity.DuAnId = model.DuAnId;
            entity.DonViId = model.DonViId;
            entity.Ten = model.Ten;
            entity.MoTa = model.MoTa;
            entity.IsDeleted = false;

            var ngayBatDau = model.NgayBatDau ?? hangMucs.Where(x => x.NgayBatDau.HasValue).Select(x => x.NgayBatDau.Value).DefaultIfEmpty().Min();
            var ngayKetThuc = model.NgayKetThuc ?? hangMucs.Where(x => x.NgayKetThuc.HasValue).Select(x => x.NgayKetThuc.Value).DefaultIfEmpty().Max();

            entity.NgayBatDau = ngayBatDau.Year > 1900 ? ngayBatDau : (DateTime?)null;
            entity.NgayKetThuc = ngayKetThuc.Year > 1900 ? ngayKetThuc : (DateTime?)null;
        }

        private List<DuAnTrienKhaiHangMuc> BuildHangMucs(DuAnTrienKhaiViewModel model)
        {
            return (model.HangMucs ?? new List<DuAnTrienKhaiHangMucInfo>())
                .Where(x => x.IsDeleted != true)
                .OrderBy(x => x.ThuTu ?? int.MaxValue)
                .ThenBy(x => x.Id)
                .Select(x => new DuAnTrienKhaiHangMuc
                {
                    Id = x.Id,
                    DuAnHangMucId = x.DuAnHangMucId,
                    TenHangMuc = x.TenHangMuc,
                    MoTa = x.MoTa,
                    ThuTu = x.ThuTu,
                    NgayBatDau = x.NgayBatDau,
                    NgayKetThuc = x.NgayKetThuc,
                    TienDo = x.TienDo ?? 0,
                    GhiChu = x.GhiChu,
                    IsDeleted = false
                }).ToList();
        }

        private void SyncHangMucs(DuAnTrienKhai entity, List<DuAnTrienKhaiHangMuc> hangMucs)
        {
            var inputIds = hangMucs.Where(x => x.Id > 0).Select(x => x.Id).ToList();

            foreach (var oldItem in entity.DuAnTrienKhaiHangMucs.Where(x => x.IsDeleted != true).ToList())
            {
                if (oldItem.Id > 0 && !inputIds.Contains(oldItem.Id))
                {
                    oldItem.IsDeleted = true;
                }
            }

            foreach (var item in hangMucs)
            {
                if (item.Id > 0)
                {
                    var oldItem = entity.DuAnTrienKhaiHangMucs.FirstOrDefault(x => x.Id == item.Id);
                    if (oldItem == null)
                    {
                        continue;
                    }

                    oldItem.DuAnHangMucId = item.DuAnHangMucId;
                    oldItem.TenHangMuc = item.TenHangMuc;
                    oldItem.MoTa = item.MoTa;
                    oldItem.ThuTu = item.ThuTu;
                    oldItem.NgayBatDau = item.NgayBatDau;
                    oldItem.NgayKetThuc = item.NgayKetThuc;
                    oldItem.TienDo = item.TienDo;
                    oldItem.GhiChu = item.GhiChu;
                    oldItem.IsDeleted = false;
                }
                else
                {
                    entity.DuAnTrienKhaiHangMucs.Add(new DuAnTrienKhaiHangMuc
                    {
                        DuAnHangMucId = item.DuAnHangMucId,
                        TenHangMuc = item.TenHangMuc,
                        MoTa = item.MoTa,
                        ThuTu = item.ThuTu,
                        NgayBatDau = item.NgayBatDau,
                        NgayKetThuc = item.NgayKetThuc,
                        TienDo = item.TienDo,
                        GhiChu = item.GhiChu,
                        IsDeleted = false
                    });
                }
            }
        }

        private void MapTrangThai(List<DuAnTrienKhaiGridVo> items)
        {
            var homNay = DateTime.Now.Date;
            foreach (var item in items)
            {
                if (item.TienDoTong >= 100)
                {
                    item.TrangThai = "Hoàn thành";
                    continue;
                }

                if (item.NgayKetThuc.HasValue && item.NgayKetThuc.Value.Date < homNay)
                {
                    item.TrangThai = "Chậm tiến độ";
                    continue;
                }

                if (item.TienDoTong <= 0)
                {
                    item.TrangThai = "Chưa bắt đầu";
                    continue;
                }

                item.TrangThai = "Đang triển khai";
            }
        }
    }
}
