using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Dashboard.Services
{
    public interface IDiemDanhService
    {
        DataTableResult<DiemDanhGridVo> GetAll(DiemDanhQueryViewModel search);
        dynamic ThemOrCapNhat(DiemDanhViewModel model);
        bool Xoa(int id);
        List<DiemDanhReportViewModel> GetBaoCao(DiemDanhSearchViewModel search);
        DiemDanhSummaryViewModel GetBaoCaoSummary(DiemDanhSearchViewModel search);
        List<DiemDanhDetailByDateViewModel> GetChiTietNgayNghi(DiemDanhSearchViewModel search);
        DiemDanhViewModel GetById(int id);
        dynamic TaoLichHopTuDiemDanh(TaoLichHopTuDiemDanhRequest request);
    }

    public class DiemDanhService : IDiemDanhService
    {
        private readonly IDiemDanhRepository _repository;
        private readonly ICuocHopService _cuocHopService;

        public DiemDanhService(IDiemDanhRepository repository, ICuocHopService cuocHopService)
        {
            _repository = repository;
            _cuocHopService = cuocHopService;
        }

        public DataTableResult<DiemDanhGridVo> GetAll(DiemDanhQueryViewModel search)
        {
            var data = _repository.GetAll(search);
            if (data != null && data.rows != null)
            {
                foreach (var item in data.rows)
                {
                    var user = Dashboard.Common.CacheManagerHelper.GetUserByIdFromCache(item.UserId);
                    if (user != null)
                    {
                        item.UserName = user.Name;
                    }
                }
            }
            return data;
        }

        public DiemDanhViewModel GetById(int id)
        {
            return _repository.GetById(id);
        }

        public dynamic ThemOrCapNhat(DiemDanhViewModel model)
        {
            return _repository.ThemOrCapNhat(model);
        }

        public bool Xoa(int id)
        {
            return _repository.Xoa(id);
        }

        public List<DiemDanhReportViewModel> GetBaoCao(DiemDanhSearchViewModel search)
        {
            return _repository.GetBaoCao(search);
        }

        public DiemDanhSummaryViewModel GetBaoCaoSummary(DiemDanhSearchViewModel search)
        {
            return _repository.GetBaoCaoSummary(search);
        }

        public List<DiemDanhDetailByDateViewModel> GetChiTietNgayNghi(DiemDanhSearchViewModel search)
        {
            return _repository.GetChiTietNgayNghi(search);
        }

        private static string GetEnumDescription<TEnum>(TEnum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public dynamic TaoLichHopTuDiemDanh(TaoLichHopTuDiemDanhRequest request)
        {
            if (request.CurrentUserId == 0)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "CurrentUserId",
                            Description = "Không xác định được người dùng hiện tại."
                        }
                    }
                };
            }

            var diemDanh = _repository.GetById(request.DiemDanhId);
            if (diemDanh == null)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "DiemDanhId",
                            Description = "Không tìm thấy thông tin điểm danh."
                        }
                    }
                };
            }

            if (!diemDanh.NgayNghi.HasValue)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "NgayNghi",
                            Description = "Ngày nghỉ không hợp lệ."
                        }
                    }
                };
            }

            var loaiNghi = (Dashboard.Models.CommonModel.LoaiNghiEnum)diemDanh.LoaiNghi;
            string batDauStr = "";
            string ketThucStr = "";

            switch (loaiNghi)
            {
                case Dashboard.Models.CommonModel.LoaiNghiEnum.Sang:
                    batDauStr = "07:00";
                    ketThucStr = "11:30";
                    break;
                case Dashboard.Models.CommonModel.LoaiNghiEnum.Chieu:
                    batDauStr = "13:30";
                    ketThucStr = "17:00";
                    break;
                case Dashboard.Models.CommonModel.LoaiNghiEnum.CaNgay:
                    batDauStr = "07:00";
                    ketThucStr = "17:00";
                    break;
                case Dashboard.Models.CommonModel.LoaiNghiEnum.DiTre:
                    return new
                    {
                        Errors = new List<ValidateError>
                        {
                            new ValidateError
                            {
                                Id = "LoaiNghi",
                                Description = "Không thể tạo lịch họp từ loại 'Đi trễ'."
                            }
                        }
                    };
            }

            string tenCuocHop = GetEnumDescription(loaiNghi);

            var currentUser = CacheManagerHelper.GetUserByIdFromCache(request.CurrentUserId);
            var coQuanId = currentUser?.PhongBanId;

            var cuocHopViewModel = new CuocHopViewModel
            {
                TenCuocHop = tenCuocHop,
                NoiDung = tenCuocHop,
                NguoiThamGia = "," + diemDanh.UserId.ToString() + ",",
                NgayDienRaStr = diemDanh.NgayNghi.Value.ToString("dd/MM/yyyy"),
                BatDauStr = batDauStr,
                KetThucStr = ketThucStr,
                PhongHopId = null,
                PhongHopTxt = "",
                LyDoId = 1000,
                GhiChu = BuildGhiChu(diemDanh.LyDo, request.LyDoChuyenNghi, request.GhiChu),
                NguoiTaoId = request.CurrentUserId,
                CoQuanId = coQuanId,
                IsAddOrUpdate = true
            };

            return _cuocHopService.Them(cuocHopViewModel);
        }

        private string BuildGhiChu(string lyDoGoc, string lyDoChuyenNghi, string ghiChuMoi)
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(lyDoGoc))
            {
                parts.Add($"Lý do gốc: {lyDoGoc}");
            }

            if (!string.IsNullOrEmpty(lyDoChuyenNghi))
            {
                var tenLyDo = GetTenLyDoChuyenNghi(lyDoChuyenNghi);
                parts.Add($"Lý do chuyển nghỉ: {tenLyDo}");
            }

            if (!string.IsNullOrEmpty(ghiChuMoi))
            {
                parts.Add($"Ghi chú: {ghiChuMoi}");
            }

            return string.Join(" | ", parts);
        }

        private string GetTenLyDoChuyenNghi(string lyDoId)
        {
            switch (lyDoId)
            {
                case "1": return "Đi công tác";
                case "2": return "Họp đột xuất";
                case "3": return "Bệnh đột xuất";
                case "4": return "Gia đình có việc";
                case "5": return "Lý do khác";
                default: return lyDoId;
            }
        }
    }
}
