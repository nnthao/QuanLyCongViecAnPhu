using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace Dashboard.Repository
{
    public interface IDoanVienRepository
    {
        DataLookupViewModel DoanVienLookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<DoanVienGridVo> GetAll(QueryParamViewModel queryParams);
        DoanVienGridVo GetById(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(DoanVienViewModel model);
        bool Xoa(int Id);
        dynamic ImportDanhSach(List<DoanVienImportGridVo> danhSachDoanVien);
        dynamic DiemDanh(int donViDoanId, string hoTen);
        dynamic DiemDanhById(int id);
        bool ResetDiemDanh();
        dynamic GetThongKeDiemDanh();
        List<DoanVien> GetDanhSachDoanVienForThongKe();
        List<DoanVien_DanToc> GetDanhSachDanTocForThongKe();
        List<DoanVienGridVo> GetDoanVienByDonViDoanId(int donViDoanId);
    }

    public class DoanVienRepository : IDoanVienRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<DoanVienViewModel, DoanVien>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.ThoiGianDiemDanh, opt => opt.Ignore())
                .ForMember(x => x.NgaySinh, opt => opt.Ignore())
                .AfterMap((s, d) =>
                {
                    d.DonViDoanId = s.DonViDoanId == 0 ? null : s.DonViDoanId;
                    d.DanTocId = s.DanTocId == 0 ? null : s.DanTocId;
                    d.ThoiGianDiemDanh = s.ThoiGianDiemDanh;
                    d.NgaySinh = s.NgaySinh;
                });
        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IDoanVienValidate _validate;
        public DoanVienRepository(IContext context, IDoanVienValidate validate)
        {
            _context = context;
            _validate = validate;
        }

        private readonly string sortStringDefault = "HoTen asc";
        private readonly List<string> searchCol = new List<string> { "HoTen", "TenTrim", "ChucVu" };

        public DataTableResult<DoanVienGridVo> GetAll(QueryParamViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.DoanViens.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var data = query.Select(cv => new DoanVienGridVo
            {
                Id = cv.Id,
                HoTen = cv.HoTen,
                TenTrim = cv.TenTrim,
                ChucVu = cv.ChucVu,
                GhiChu = cv.GhiChu,
                DonViDoanId = cv.DonViDoanId,
                ThoiGianDiemDanh = cv.ThoiGianDiemDanh,
                GioiTinh = cv.GioiTinh,
                QueQuan = cv.QueQuan,
                DanTocId = cv.DanTocId,
                ChuyenMonNghiepVu = cv.ChuyenMonNghiepVu,
                LyLuanChinhTri = cv.LyLuanChinhTri,
                NgaySinh = cv.NgaySinh,
                NgayVaoDoan = cv.NgayVaoDoan,
                NgayVaoDang = cv.NgayVaoDang,
                IsDeleted = cv.IsDeleted,
            }).Where(queryParams.searchString);

            var total = data.Count();
            var result = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList();

            if (result.Count != 0)
            {
                var donViDoanIds = result.Where(o => o.DonViDoanId.HasValue).Select(o => o.DonViDoanId.Value).Distinct().ToList();
                if (donViDoanIds.Count > 0)
                {
                    var donViDoans = _context.DonViDoans.AsNoTracking()
                        .Where(o => donViDoanIds.Contains(o.Id))
                        .Select(o => new { o.Id, o.Ten })
                        .ToList();

                    foreach (var item in result)
                    {
                        if (item.DonViDoanId.HasValue)
                        {
                            var donViDoan = donViDoans.FirstOrDefault(o => o.Id == item.DonViDoanId.Value);
                            if (donViDoan != null)
                            {
                                item.TenDonViDoan = donViDoan.Ten;
                            }
                        }
                    }
                }

                var danTocIds = result.Where(o => o.DanTocId.HasValue).Select(o => o.DanTocId.Value).Distinct().ToList();
                if (danTocIds.Count > 0)
                {
                    var danTocs = _context.DoanVien_DanTocs.AsNoTracking()
                        .Where(o => danTocIds.Contains(o.Id))
                        .Select(o => new { o.Id, o.Ten })
                        .ToList();

                    foreach (var item in result)
                    {
                        if (item.DanTocId.HasValue)
                        {
                            var danToc = danTocs.FirstOrDefault(o => o.Id == item.DanTocId.Value);
                            if (danToc != null)
                            {
                                item.TenDanToc = danToc.Ten;
                            }
                        }
                    }
                }
            }

            return new DataTableResult<DoanVienGridVo>
            {
                rows = result,
                total = total
            };
        }

        public dynamic ThemOrCapNhat(DoanVienViewModel model)
        {
            var errors = _validate.Validate(model);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (model.Id == 0)
            {
                var item = _iMapper.Map<DoanVienViewModel, DoanVien>(model);
                if (!string.IsNullOrEmpty(model.HoTen))
                {
                    item.TenTrim = UltiHelper.CreateTenTrim(model.HoTen);
                }
                _context.DoanViens.Add(item);
            }
            else
            {
                var item = _context.DoanViens.FirstOrDefault(o => o.Id == model.Id);
                if (item == null) throw new System.Exception("Dữ liệu đã bị xóa");

                _iMapper.Map(model, item);
                if (!string.IsNullOrEmpty(model.HoTen))
                {
                    item.TenTrim = UltiHelper.CreateTenTrim(model.HoTen);
                }
            }

            _context.SaveChanges();

            return true;
        }

        public bool Xoa(int Id)
        {
            var item = _context.DoanViens.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = true;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public bool KhoiPhuc(int Id)
        {
            var item = _context.DoanViens.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public DoanVienGridVo GetById(int id)
        {
            var item = _context.DoanViens.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new DoanVienGridVo
                {
                    Id = item.Id,
                    HoTen = item.HoTen,
                    TenTrim = item.TenTrim,
                    ChucVu = item.ChucVu,
                    GhiChu = item.GhiChu,
                    DonViDoanId = item.DonViDoanId,
                    ThoiGianDiemDanh = item.ThoiGianDiemDanh,
                    GioiTinh = item.GioiTinh,
                    QueQuan = item.QueQuan,
                    DanTocId = item.DanTocId,
                    ChuyenMonNghiepVu = item.ChuyenMonNghiepVu,
                    LyLuanChinhTri = item.LyLuanChinhTri,
                    NgaySinh = item.NgaySinh,
                    NgayVaoDoan = item.NgayVaoDoan,
                    NgayVaoDang = item.NgayVaoDang
                };
            }

            return new DoanVienGridVo();
        }

        public DataLookupViewModel DoanVienLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var query = _context.DoanViens.AsNoTracking().AsQueryable();
            if (!lookupQueryViewModel.isGetAll)
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var data = new List<LookupViewModel>();
            if (!lookupQueryViewModel.isHideOptionChonValueZero)
            {
                data.Add(new LookupViewModel
                {
                    id = 0,
                    text = "--- Chọn ---"
                });
            }

            var result = query.Select(o => new LookupViewModel
            {
                id = o.Id,
                text = o.HoTen
            }).ToList();

            return new DataLookupViewModel
            {
                Data = data.Concat(result).ToList()
            };
        }

        public dynamic ImportDanhSach(List<DoanVienImportGridVo> danhSachDoanVien)
        {
            var ketQua = new List<DoanVienImportGridVo>();
            var coLoi = false;
            var danhSachDongLoi = new List<string>();

            for (int i = 0; i < danhSachDoanVien.Count; i++)
            {
                var doanVienImport = danhSachDoanVien[i];
                var soDong = i + 9;
                var loi = new List<string>();

                if (string.IsNullOrEmpty(doanVienImport.HoTen?.Trim()))
                {
                    loi.Add("Họ tên không được trống");
                }

                if (string.IsNullOrEmpty(doanVienImport.DonViDoan?.Trim()))
                {
                    loi.Add("Đơn vị đoàn không được trống");
                }
                else
                {
                    var donViDoan = _context.DonViDoans.FirstOrDefault(dvd =>
                        dvd.Ten != null && dvd.Ten.Trim().ToLower() == doanVienImport.DonViDoan.Trim().ToLower() &&
                        dvd.IsDeleted != true);
                    if (donViDoan != null)
                    {
                        doanVienImport.DonViDoanId = donViDoan.Id;
                    }
                    else
                    {
                        var newDonViDoan = new DonViDoan
                        {
                            Ten = doanVienImport.DonViDoan.Trim(),
                            TenTrim = UltiHelper.CreateTenTrim(doanVienImport.DonViDoan.Trim()),
                            IsDeleted = false
                        };
                        _context.DonViDoans.Add(newDonViDoan);
                        _context.SaveChanges();
                        doanVienImport.DonViDoanId = newDonViDoan.Id;
                    }
                }

                if (doanVienImport.GioiTinh == null || doanVienImport.GioiTinh == 0)
                {
                    loi.Add("Giới tính không được trống");
                }

                if (string.IsNullOrEmpty(doanVienImport.NgaySinhStr))
                {
                    loi.Add("Ngày sinh không được trống");
                }

                if (string.IsNullOrEmpty(doanVienImport.DanToc?.Trim()))
                {
                    loi.Add("Dân tộc không được trống");
                }
                else
                {
                    var danToc = _context.DoanVien_DanTocs.FirstOrDefault(dt =>
                        dt.Ten != null && dt.Ten.Trim().ToLower() == doanVienImport.DanToc.Trim().ToLower() &&
                        dt.IsDeleted != true);
                    if (danToc != null)
                    {
                        doanVienImport.DanTocId = danToc.Id;
                    }
                    else
                    {
                        var newDanToc = new DoanVien_DanToc
                        {
                            Ten = doanVienImport.DanToc.Trim(),
                            IsDeleted = false
                        };
                        _context.DoanVien_DanTocs.Add(newDanToc);
                        _context.SaveChanges();
                        doanVienImport.DanTocId = newDanToc.Id;
                    }
                }

                if (string.IsNullOrEmpty(doanVienImport.ChuyenMonNghiepVu?.Trim()))
                {
                    loi.Add("Trình độ chuyên môn không được trống");
                }

                if (loi.Count > 0)
                {
                    coLoi = true;
                    var thongBaoLoi = $"Dòng {soDong}: {string.Join("; ", loi)}";
                    doanVienImport.Loi = thongBaoLoi;
                    danhSachDongLoi.Add(thongBaoLoi);
                }

                ketQua.Add(doanVienImport);
            }

            if (coLoi)
            {
                var thongBaoChiTiet = string.Join("\n", danhSachDongLoi);
                return new { Data = ketQua, Errors = new List<ValidateError> { new ValidateError { Id = "ImportDoanVien", Description = $"File import có lỗi ở các dòng sau:\n{thongBaoChiTiet}\n\nVui lòng kiểm tra và chỉnh sửa lại file trước khi import." } } };
            }

            foreach (var doanVienImport in danhSachDoanVien)
            {
                var ngaySinh = DateTimeHelper.ConvertStrToDate(doanVienImport.NgaySinhStr);

                var doanVienExisting = _context.DoanViens.FirstOrDefault(dv =>
                    dv.HoTen != null && dv.HoTen.Trim().ToLower() == doanVienImport.HoTen.Trim().ToLower() &&
                    dv.DonViDoanId == doanVienImport.DonViDoanId &&
                    dv.IsDeleted != true);

                if (doanVienExisting != null)
                {
                    doanVienExisting.ChucVu = doanVienImport.ChucVu?.Trim();
                    doanVienExisting.GhiChu = doanVienImport.GhiChu?.Trim();
                    doanVienExisting.GioiTinh = doanVienImport.GioiTinh;
                    doanVienExisting.QueQuan = doanVienImport.QueQuan?.Trim();
                    doanVienExisting.DanTocId = doanVienImport.DanTocId;
                    doanVienExisting.ChuyenMonNghiepVu = doanVienImport.ChuyenMonNghiepVu?.Trim();
                    doanVienExisting.LyLuanChinhTri = doanVienImport.LyLuanChinhTri?.Trim();
                    doanVienExisting.NgaySinh = ngaySinh;
                    doanVienExisting.NgayVaoDoan = doanVienImport.NgayVaoDoan?.Trim();
                    doanVienExisting.NgayVaoDang = doanVienImport.NgayVaoDang?.Trim();
                    if (!string.IsNullOrEmpty(doanVienImport.HoTen))
                    {
                        doanVienExisting.TenTrim = UltiHelper.CreateTenTrim(doanVienImport.HoTen.Trim());
                    }
                    doanVienImport.Loi = "Đã cập nhật thành công";
                }
                else
                {
                    var newDoanVien = new DoanVien
                    {
                        HoTen = doanVienImport.HoTen.Trim(),
                        TenTrim = UltiHelper.CreateTenTrim(doanVienImport.HoTen.Trim()),
                        ChucVu = doanVienImport.ChucVu?.Trim(),
                        GhiChu = doanVienImport.GhiChu?.Trim(),
                        DonViDoanId = doanVienImport.DonViDoanId,
                        GioiTinh = doanVienImport.GioiTinh,
                        QueQuan = doanVienImport.QueQuan?.Trim(),
                        DanTocId = doanVienImport.DanTocId,
                        ChuyenMonNghiepVu = doanVienImport.ChuyenMonNghiepVu?.Trim(),
                        LyLuanChinhTri = doanVienImport.LyLuanChinhTri?.Trim(),
                        NgaySinh = ngaySinh,
                        NgayVaoDoan = doanVienImport.NgayVaoDoan?.Trim(),
                        NgayVaoDang = doanVienImport.NgayVaoDang?.Trim(),
                        IsDeleted = false
                    };

                    _context.DoanViens.Add(newDoanVien);
                    doanVienImport.Loi = "Đã thêm mới thành công";
                }
            }

            _context.SaveChanges();

            return new { Data = ketQua };
        }

        public dynamic DiemDanh(int donViDoanId, string hoTen)
        {
            if (donViDoanId == 0)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError { Id = "DonViDoanId", Description = "Vui lòng chọn đơn vị đoàn." }
                    }
                };
            }

            if (string.IsNullOrEmpty(hoTen?.Trim()))
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError { Id = "HoTen", Description = "Họ tên không được trống." }
                    }
                };
            }

            var tenTrim = UltiHelper.CreateTenTrim(hoTen.Trim());
            var doanVien = _context.DoanViens.FirstOrDefault(dv =>
                dv.TenTrim != null && dv.TenTrim.ToLower() == tenTrim.ToLower() &&
                dv.DonViDoanId == donViDoanId &&
                dv.IsDeleted != true);

            if (doanVien == null)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError { Id = "DiemDanh", Description = "Không tìm thấy đoàn viên với thông tin đã nhập. Vui lòng kiểm tra lại họ tên và đơn vị đoàn." }
                    }
                };
            }

            if (doanVien.ThoiGianDiemDanh.HasValue)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError { Id = "DiemDanh", Description = "Bạn đã điểm danh rồi." }
                    }
                };
            }

            doanVien.ThoiGianDiemDanh = DateTime.Now;
            _context.SaveChanges();

            return new { Success = true };
        }

        public dynamic DiemDanhById(int id)
        {
            var doanVien = _context.DoanViens.FirstOrDefault(dv => dv.Id == id && dv.IsDeleted != true);

            if (doanVien == null)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError { Id = "DiemDanh", Description = "Không tìm thấy đoàn viên." }
                    }
                };
            }

            if (doanVien.ThoiGianDiemDanh.HasValue)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError { Id = "DiemDanh", Description = "Đoàn viên này đã điểm danh rồi." }
                    }
                };
            }

            doanVien.ThoiGianDiemDanh = DateTime.Now;
            _context.SaveChanges();

            return new { Success = true };
        }

        public bool ResetDiemDanh()
        {
            var danhSachDoanVien = _context.DoanViens.Where(dv => dv.ThoiGianDiemDanh.HasValue && dv.IsDeleted != true).ToList();
            foreach (var doanVien in danhSachDoanVien)
            {
                doanVien.ThoiGianDiemDanh = null;
            }
            _context.SaveChanges();
            return true;
        }

        public dynamic GetThongKeDiemDanh()
        {
            var tongSo = _context.DoanViens.Count(dv => dv.IsDeleted != true);
            var daDiemDanh = _context.DoanViens.Count(dv => dv.ThoiGianDiemDanh.HasValue && dv.IsDeleted != true);
            var chuaDiemDanh = tongSo - daDiemDanh;

            return new
            {
                TongSo = tongSo,
                DaDiemDanh = daDiemDanh,
                ChuaDiemDanh = chuaDiemDanh
            };
        }

        public List<DoanVien> GetDanhSachDoanVienForThongKe()
        {
            return _context.DoanViens
                .Where(dv => dv.IsDeleted != true)
                .ToList();
        }

        public List<DoanVien_DanToc> GetDanhSachDanTocForThongKe()
        {
            return _context.DoanVien_DanTocs
                .Where(dt => dt.IsDeleted != true)
                .ToList();
        }

        public List<DoanVienGridVo> GetDoanVienByDonViDoanId(int donViDoanId)
        {
            if (donViDoanId == 0)
            {
                return new List<DoanVienGridVo>();
            }

            var query = _context.DoanViens.AsNoTracking()
                .Where(o => o.DonViDoanId == donViDoanId && o.IsDeleted != true)
                .OrderBy(o => o.HoTen)
                .Select(cv => new DoanVienGridVo
                {
                    Id = cv.Id,
                    HoTen = cv.HoTen,
                    TenTrim = cv.TenTrim,
                    ChucVu = cv.ChucVu,
                    GhiChu = cv.GhiChu,
                    DonViDoanId = cv.DonViDoanId,
                    ThoiGianDiemDanh = cv.ThoiGianDiemDanh,
                    GioiTinh = cv.GioiTinh,
                    QueQuan = cv.QueQuan,
                    DanTocId = cv.DanTocId,
                    ChuyenMonNghiepVu = cv.ChuyenMonNghiepVu,
                    LyLuanChinhTri = cv.LyLuanChinhTri,
                    NgaySinh = cv.NgaySinh,
                    NgayVaoDoan = cv.NgayVaoDoan,
                    NgayVaoDang = cv.NgayVaoDang,
                    IsDeleted = cv.IsDeleted,
                }).ToList();

            return query;
        }
    }
}

