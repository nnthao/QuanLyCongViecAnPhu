using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dashboard.Services
{
    public interface IDoanVienService
    {
        DataTableResult<DoanVienGridVo> GetAll(QueryParamViewModel queryParams);
        DoanVienGridVo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(DoanVienViewModel model);
        bool Xoa(int id);
        DataLookupViewModel DoanVienLookup(LookupQueryViewModel lookupQueryViewModel);
        dynamic ImportDanhSach(List<DoanVienImportGridVo> danhSachDoanVien);
        dynamic DiemDanh(int donViDoanId, string hoTen);
        dynamic DiemDanhById(int id);
        bool ResetDiemDanh();
        dynamic GetThongKeDiemDanh();
        dynamic GetThongKeDaiHoi();
        List<DoanVienGridVo> GetDoanVienByDonViDoanId(int donViDoanId);
    }

    public class DoanVienService : IDoanVienService
    {
        private readonly IDoanVienRepository _repository;
        public DoanVienService(IDoanVienRepository repository)
        {
            _repository = repository;
        }

        public DataTableResult<DoanVienGridVo> GetAll(QueryParamViewModel queryParams)
        {
            var data = _repository.GetAll(queryParams);
            return data;
        }

        public DoanVienGridVo GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _repository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(DoanVienViewModel model)
        {
            return _repository.ThemOrCapNhat(model);
        }

        public bool Xoa(int id)
        {
            return _repository.Xoa(id);
        }

        public DataLookupViewModel DoanVienLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _repository.DoanVienLookup(lookupQueryViewModel);
        }

        public dynamic ImportDanhSach(List<DoanVienImportGridVo> danhSachDoanVien)
        {
            return _repository.ImportDanhSach(danhSachDoanVien);
        }

        public dynamic DiemDanh(int donViDoanId, string hoTen)
        {
            return _repository.DiemDanh(donViDoanId, hoTen);
        }

        public dynamic DiemDanhById(int id)
        {
            return _repository.DiemDanhById(id);
        }

        public bool ResetDiemDanh()
        {
            return _repository.ResetDiemDanh();
        }

        public dynamic GetThongKeDiemDanh()
        {
            return _repository.GetThongKeDiemDanh();
        }

        public List<DoanVienGridVo> GetDoanVienByDonViDoanId(int donViDoanId)
        {
            return _repository.GetDoanVienByDonViDoanId(donViDoanId);
        }

        public dynamic GetThongKeDaiHoi()
        {
            var danhSachDoanVien = _repository.GetDanhSachDoanVienForThongKe();
            var danhSachDanToc = _repository.GetDanhSachDanTocForThongKe();
            var now = DateTime.Now;

            var soLuong = danhSachDoanVien.Count;

            var tuoiTrungBinh = 0.0;
            var danhSachCoNgaySinh = danhSachDoanVien.Where(dv => dv.NgaySinh.HasValue).ToList();
            if (danhSachCoNgaySinh.Count > 0)
            {
                var tongTuoi = danhSachCoNgaySinh.Sum(dv =>
                {
                    var ngaySinh = dv.NgaySinh.Value;
                    var tuoi = now.Year - ngaySinh.Year;
                    if (now.Month < ngaySinh.Month || (now.Month == ngaySinh.Month && now.Day < ngaySinh.Day))
                    {
                        tuoi--;
                    }
                    return tuoi;
                });
                tuoiTrungBinh = Math.Round((double)tongTuoi / danhSachCoNgaySinh.Count, 1);
            }

            var gioiTinh = new
            {
                Nam = danhSachDoanVien.Count(dv => dv.GioiTinh == 1),
                Nu = danhSachDoanVien.Count(dv => dv.GioiTinh == 2)
            };

            var doTuoiDuoi18 = 0;
            var doTuoiTu18Den24 = 0;
            var doTuoiTu25Den30 = 0;
            var doTuoiTren30 = 0;

            foreach (var dv in danhSachCoNgaySinh)
            {
                var ngaySinh = dv.NgaySinh.Value;
                var tuoi = now.Year - ngaySinh.Year;
                if (now.Month < ngaySinh.Month || (now.Month == ngaySinh.Month && now.Day < ngaySinh.Day))
                {
                    tuoi--;
                }

                if (tuoi < 18)
                {
                    doTuoiDuoi18++;
                }
                else if (tuoi >= 18 && tuoi <= 24)
                {
                    doTuoiTu18Den24++;
                }
                else if (tuoi >= 25 && tuoi <= 30)
                {
                    doTuoiTu25Den30++;
                }
                else
                {
                    doTuoiTren30++;
                }
            }

            var doTuoi = new
            {
                Duoi18 = doTuoiDuoi18,
                Tu18Den24 = doTuoiTu18Den24,
                Tu25Den30 = doTuoiTu25Den30,
                Tren30 = doTuoiTren30
            };

            var danToc = danhSachDoanVien
                .Where(dv => dv.DanTocId.HasValue)
                .GroupBy(dv => dv.DanTocId)
                .Select(g => new
                {
                    DanTocId = g.Key,
                    SoLuong = g.Count()
                })
                .ToList();

            var danTocChiTiet = danToc.Select(dt =>
            {
                var danTocInfo = danhSachDanToc.FirstOrDefault(d => d.Id == dt.DanTocId);
                return new
                {
                    Ten = danTocInfo != null ? danTocInfo.Ten : "Không xác định",
                    SoLuong = dt.SoLuong
                };
            }).OrderByDescending(dt => dt.SoLuong).ToList();

            var chuyenMonNghiepVu = danhSachDoanVien
                .Where(dv => !string.IsNullOrEmpty(dv.ChuyenMonNghiepVu))
                .GroupBy(dv => dv.ChuyenMonNghiepVu.Trim())
                .Select(g => new
                {
                    Ten = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(c => c.SoLuong)
                .ToList();

            var dangVien = danhSachDoanVien.Count(dv => !string.IsNullOrEmpty(dv.NgayVaoDang));

            return new
            {
                SoLuong = soLuong,
                TuoiTrungBinh = tuoiTrungBinh,
                GioiTinh = gioiTinh,
                DoTuoi = doTuoi,
                DanToc = danTocChiTiet,
                ChuyenMonNghiepVu = chuyenMonNghiepVu,
                DangVien = dangVien
            };
        }
    }
}

