using Dashboard.Models;
using Dashboard.Models.CommonModel;
using Dashboard.Models.Entities;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;

namespace Dashboard.Repository
{
    public interface IDiemDanhRepository
    {
        DataTableResult<DiemDanhGridVo> GetAll(DiemDanhQueryViewModel search);
        dynamic ThemOrCapNhat(DiemDanhViewModel model);
        bool Xoa(int id);
        List<DiemDanhReportViewModel> GetBaoCao(DiemDanhSearchViewModel search);
        DiemDanhSummaryViewModel GetBaoCaoSummary(DiemDanhSearchViewModel search);
        List<DiemDanhDetailByDateViewModel> GetChiTietNgayNghi(DiemDanhSearchViewModel search);
        DiemDanhViewModel GetById(int id);
    }

    public class DiemDanhRepository : IDiemDanhRepository
    {
        private readonly IContext _context;
        private readonly IDiemDanhValidate _validator;

        public DiemDanhRepository(IContext context, IDiemDanhValidate validator)
        {
            _context = context;
            _validator = validator;
        }

        public DataTableResult<DiemDanhGridVo> GetAll(DiemDanhQueryViewModel search)
        {
            search.sortStringDefault = "NgayNghi desc";
            search.searchColumn = new List<string> { "UserName", "LyDo" };

            var query = from dd in _context.DiemDanhs
                        join u in _context.Users on dd.UserId equals u.Id
                        where dd.IsDeleted != true
                        select new DiemDanhGridVo
                        {
                            Id = dd.Id,
                            UserId = dd.UserId,
                            NgayNghi = dd.NgayNghi,
                            LoaiNghi = dd.LoaiNghi,
                            LyDo = dd.LyDo,
                            IsDeleted = dd.IsDeleted,
                            UserName = u.Name
                        };

            if (search.TuNgay.HasValue)
            {
                query = query.Where(x => x.NgayNghi >= search.TuNgay.Value);
            }
            if (search.DenNgay.HasValue)
            {
                query = query.Where(x => x.NgayNghi <= search.DenNgay.Value);
            }
            if (search.UserId.GetValueOrDefault() != 0)
            {
                query = query.Where(x => x.UserId == search.UserId.Value);
            }

            var data = query.Where(search.searchString);

            var total = data.Count();
            var result = data.OrderBy(search.sortString).Skip(search.offset).Take(search.limit).ToList();

            return new DataTableResult<DiemDanhGridVo>
            {
                rows = result,
                total = total
            };
        }

        public DiemDanhViewModel GetById(int id)
        {
            var entity = _context.DiemDanhs.FirstOrDefault(x => x.Id == id);
            if (entity != null)
            {
                var ngayNghiStr = Dashboard.Common.DateTimeHelper.ConvertDateTimeToDateOnly(entity.NgayNghi);
                return new DiemDanhViewModel
                {
                    Id = entity.Id,
                    UserId = entity.UserId,
                    NgayNghiStr = ngayNghiStr,
                    TuNgayStr = ngayNghiStr,
                    DenNgayStr = ngayNghiStr,
                    LoaiNghi = entity.LoaiNghi,
                    LyDo = entity.LyDo,
                    IsDeleted = entity.IsDeleted
                };
            }
            return null;
        }

        public dynamic ThemOrCapNhat(DiemDanhViewModel model)
        {
            var errors = _validator.Validate(model);
            if (errors.Count > 0)
            {
                return new { Errors = errors, status = false };
            }

            var tuNgay = model.TuNgay.Value.Date;
            var denNgay = model.DenNgay.Value.Date;

            if (model.Id == 0)
            {
                var ngayBiTrung = new List<string>();

                for (var ngay = tuNgay; ngay <= denNgay; ngay = ngay.AddDays(1))
                {
                    var ngayLocal = ngay;
                    var exists = _context.DiemDanhs.Any(x => x.UserId == model.UserId
                        && x.NgayNghi.HasValue
                        && x.NgayNghi.Value == ngayLocal
                        && x.LyDo == model.LyDo
                        && x.IsDeleted != true);

                    if (exists)
                    {
                        ngayBiTrung.Add(Dashboard.Common.DateTimeHelper.ConvertDateTimeToDateOnly(ngayLocal));
                    }
                }

                if (ngayBiTrung.Any())
                {
                    return new
                    {
                        status = false,
                        Errors = new List<ValidateError>
                        {
                            new ValidateError
                            {
                                Id = "DenNgayStr",
                                Description = "Đã tồn tại bản ghi cùng nhân viên, ngày nghỉ và lý do tại: " + string.Join(", ", ngayBiTrung)
                            }
                        }
                    };
                }

                for (var ngay = tuNgay; ngay <= denNgay; ngay = ngay.AddDays(1))
                {
                    _context.DiemDanhs.Add(new DiemDanh
                    {
                        UserId = model.UserId,
                        NgayNghi = ngay,
                        LoaiNghi = model.LoaiNghi,
                        LyDo = model.LyDo,
                        IsDeleted = false
                    });
                }
            }
            else
            {
                var entity = _context.DiemDanhs.FirstOrDefault(x => x.Id == model.Id);
                if (entity == null)
                {
                    return new { status = false, message = "Không tìm thấy bản ghi." };
                }

                if (tuNgay != denNgay)
                {
                    return new
                    {
                        status = false,
                        Errors = new List<ValidateError>
                        {
                            new ValidateError
                            {
                                Id = "DenNgayStr",
                                Description = "Khi cập nhật, từ ngày và đến ngày phải giống nhau."
                            }
                        }
                    };
                }

                if (entity.NgayNghi != tuNgay || entity.UserId != model.UserId || entity.LyDo != model.LyDo)
                {
                    var exists = _context.DiemDanhs.Any(x => x.UserId == model.UserId
                        && x.NgayNghi.HasValue
                        && x.NgayNghi.Value == tuNgay
                        && x.LyDo == model.LyDo
                        && x.IsDeleted != true
                        && x.Id != model.Id);

                    if (exists)
                    {
                        return new
                        {
                            status = false,
                            Errors = new List<ValidateError>
                            {
                                new ValidateError
                                {
                                    Id = "DenNgayStr",
                                    Description = "Đã tồn tại bản ghi cùng nhân viên, ngày nghỉ và lý do."
                                }
                            }
                        };
                    }
                }

                entity.UserId = model.UserId;
                entity.LoaiNghi = model.LoaiNghi;
                entity.LyDo = model.LyDo;
                entity.NgayNghi = tuNgay;
            }

            _context.SaveChanges();

            return new
            {
                status = true,
                message = model.Id == 0
                    ? $"Thêm mới thành công {((denNgay - tuNgay).Days + 1)} bản ghi."
                    : "Cập nhật thành công!"
            };
        }

        public bool Xoa(int id)
        {
            var entity = _context.DiemDanhs.FirstOrDefault(x => x.Id == id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public List<DiemDanhReportViewModel> GetBaoCao(DiemDanhSearchViewModel search)
        {
            var query = from dd in _context.DiemDanhs
                        join u in _context.Users on dd.UserId equals u.Id
                        where dd.IsDeleted != true
                        select new
                        {
                            dd.UserId,
                            u.Name,
                            dd.NgayNghi,
                            dd.LoaiNghi
                        };

            if (search.FromDate.HasValue)
            {
                query = query.Where(x => x.NgayNghi >= search.FromDate.Value);
            }
            if (search.ToDate.HasValue)
            {
                query = query.Where(x => x.NgayNghi <= search.ToDate.Value);
            }

            if (search.LoaiThongKe.HasValue)
            {
                if (search.LoaiThongKe.Value == 1)
                {
                    query = query.Where(x => x.LoaiNghi == (int)LoaiNghiEnum.Sang
                                          || x.LoaiNghi == (int)LoaiNghiEnum.Chieu
                                          || x.LoaiNghi == (int)LoaiNghiEnum.CaNgay);
                }
                else if (search.LoaiThongKe.Value == 2)
                {
                    query = query.Where(x => x.LoaiNghi == (int)LoaiNghiEnum.DiTre);
                }
            }

            var data = query.ToList();

            var result = data.GroupBy(x => new { x.UserId, x.Name })
                .Select((g, index) => new DiemDanhReportViewModel
                {
                    Stt = index + 1,
                    UserId = g.Key.UserId,
                    UserName = g.Key.Name,
                    SoLanNghiCaNgay = g.Count(x => x.LoaiNghi == (int)LoaiNghiEnum.CaNgay),
                    SoLanNghiLe = g.Count(x => x.LoaiNghi == (int)LoaiNghiEnum.Sang || x.LoaiNghi == (int)LoaiNghiEnum.Chieu),
                    SoLanDiTre = g.Count(x => x.LoaiNghi == (int)LoaiNghiEnum.DiTre),
                    TongNgayNghi = (double)g.Count(x => x.LoaiNghi == (int)LoaiNghiEnum.CaNgay)
                        + (double)(g.Count(x => x.LoaiNghi == (int)LoaiNghiEnum.Sang || x.LoaiNghi == (int)LoaiNghiEnum.Chieu) * 0.5)
                }).ToList();

            return result;
        }

        public DiemDanhSummaryViewModel GetBaoCaoSummary(DiemDanhSearchViewModel search)
        {
            var query = _context.DiemDanhs.Where(dd => dd.IsDeleted != true);

            if (search.FromDate.HasValue)
            {
                query = query.Where(x => x.NgayNghi >= search.FromDate.Value);
            }
            if (search.ToDate.HasValue)
            {
                query = query.Where(x => x.NgayNghi <= search.ToDate.Value);
            }

            if (search.LoaiThongKe.HasValue)
            {
                if (search.LoaiThongKe.Value == 1)
                {
                    query = query.Where(x => x.LoaiNghi == (int)LoaiNghiEnum.Sang
                                          || x.LoaiNghi == (int)LoaiNghiEnum.Chieu
                                          || x.LoaiNghi == (int)LoaiNghiEnum.CaNgay);
                }
                else if (search.LoaiThongKe.Value == 2)
                {
                    query = query.Where(x => x.LoaiNghi == (int)LoaiNghiEnum.DiTre);
                }
            }

            var data = query.ToList();

            return new DiemDanhSummaryViewModel
            {
                TotalHalfDays = data.Count(x => x.LoaiNghi == (int)LoaiNghiEnum.Sang || x.LoaiNghi == (int)LoaiNghiEnum.Chieu),
                TotalFullDays = data.Count(x => x.LoaiNghi == (int)LoaiNghiEnum.CaNgay),
                TotalLateDays = data.Count(x => x.LoaiNghi == (int)LoaiNghiEnum.DiTre),
                TotalDaysOff = (double)data.Count(x => x.LoaiNghi == (int)LoaiNghiEnum.CaNgay)
                    + (double)(data.Count(x => x.LoaiNghi == (int)LoaiNghiEnum.Sang || x.LoaiNghi == (int)LoaiNghiEnum.Chieu) * 0.5)
            };
        }

        public List<DiemDanhDetailByDateViewModel> GetChiTietNgayNghi(DiemDanhSearchViewModel search)
        {
            var query = _context.DiemDanhs.Where(x => x.IsDeleted != true);

            if (search.UserId.HasValue && search.UserId.Value > 0)
            {
                query = query.Where(x => x.UserId == search.UserId.Value);
            }

            if (search.FromDate.HasValue)
            {
                query = query.Where(x => x.NgayNghi >= search.FromDate.Value);
            }

            if (search.ToDate.HasValue)
            {
                query = query.Where(x => x.NgayNghi <= search.ToDate.Value);
            }

            query = query.Where(x => x.LoaiNghi == (int)LoaiNghiEnum.Sang
                                  || x.LoaiNghi == (int)LoaiNghiEnum.Chieu
                                  || x.LoaiNghi == (int)LoaiNghiEnum.CaNgay);

            var data = query
                .Select(x => new
                {
                    x.NgayNghi,
                    x.LoaiNghi,
                    x.LyDo
                })
                .ToList();

            var result = data
                .Where(x => x.NgayNghi.HasValue)
                .GroupBy(x => x.NgayNghi.Value.Date)
                .OrderByDescending(g => g.Key)
                .Select(g =>
                {
                    var hasCaNgay = g.Any(x => x.LoaiNghi == (int)LoaiNghiEnum.CaNgay);
                    var hasSang = g.Any(x => x.LoaiNghi == (int)LoaiNghiEnum.Sang);
                    var hasChieu = g.Any(x => x.LoaiNghi == (int)LoaiNghiEnum.Chieu);

                    var chiTietNghi = hasCaNgay
                        ? "Cả ngày"
                        : hasSang && hasChieu
                            ? "Sáng, Chiều"
                            : hasSang
                                ? "Sáng"
                                : "Chiều";

                    var soNgayNghi = hasCaNgay || (hasSang && hasChieu) ? 1 : 0.5;

                    return new DiemDanhDetailByDateViewModel
                    {
                        NgayNghiStr = Dashboard.Common.DateTimeHelper.ConvertDateTimeToDateOnly(g.Key),
                        ChiTietNghi = chiTietNghi,
                        LyDo = string.Join("; ", g.Select(x => x.LyDo).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()),
                        SoNgayNghi = soNgayNghi
                    };
                })
                .ToList();

            return result;
        }
    }
}
