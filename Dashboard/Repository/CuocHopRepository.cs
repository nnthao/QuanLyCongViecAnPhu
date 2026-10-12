using AutoMapper;
using Dashboard.AdoNetHelper;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using System.Web;

namespace Dashboard.Repository
{
    public interface ICuocHopRepository
    {
        dynamic CapNhat(CuocHopViewModel lichHopViewModel);
        dynamic CapNhatThongTinPhongThoiGian(CuocHopViewModel cuocHopViewModel);
        FileDinhKemGridVo DownloadTaiLieuCuocHop(int id);
        dynamic DuyetPhieu(CuocHopViewModel cuocHopViewModel);
        DataTableResult<CuocHopGridVo> GetAll(CuocHopQueryParamViewModel queryParams);
        List<NgaySscheduler> GetAllCuocHops(QueryParamViewModel queryParams);
        CuocHopGridVo GetCuocHopInfoFromId(int id);
        dynamic GetTaiLieuCuocHop(int id);
        List<TaiLieuCuocHopGridVo> GetTaiLieuCuocHopByCuocHopIds(List<int> cuocHopIds);
        dynamic GuiLaiPhieu(CuocHopViewModel cuocHopViewModel);
        dynamic ImportDanhSachLich(ImportLichCuocHopRequestViewModel request);
        bool KhoiPhuc(int Id);
        List<ValidateError> KiemTraLoiKhiDieuPhoiPhong(CuocHopViewModel cuocHopViewModel);
        bool KiemTraThoiGianThayDoiKhiDieuPhoiPhong(CuocHopViewModel cuocHopViewModel);
        List<CuocHopGridVo> LayCuocHopTuDanhDachIds(List<int> cuocHopIds);
        dynamic Them(CuocHopViewModel lichHopViewModel);
        dynamic TraPhieu(CuocHopViewModel cuocHopViewModel);
        bool ThongBaoLaiLichHop(int cuocHopId);
        bool Xoa(int Id);
        dynamic XoaNguoiThamGiaCuocHopDaChon(int cuocHopId, int nguoiLienQuanId);
        dynamic XoaTaiLieuCuocHop(int id);
    }

    public class CuocHopRepository : ICuocHopRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CuocHopViewModel, CuocHop>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                // TrangThai đã bị comment trong entity
                .ForMember(x => x.NguoiTaoId, opt => opt.Ignore())
                .ForMember(x => x.NguoiDuyetId, opt => opt.Ignore())
                .ForMember(x => x.NguoiSuaId, opt => opt.Ignore())
                .ForMember(x => x.NgayTao, opt => opt.Ignore())
                .ForMember(x => x.NgayDuyet, opt => opt.Ignore())
                .ForMember(x => x.NgaySua, opt => opt.Ignore())
                .ForMember(x => x.NguoiThamGia, opt => opt.Ignore())
                //.ForMember(x => x.SearchStr, opt => opt.Ignore())
                // LoaiLichHop đã bị comment trong entity
                .AfterMap((s, d) =>
                {
                    //d.SearchStr = (!string.IsNullOrEmpty(s.TenCuocHop) ? s.TenCuocHop : "") + " "
                    //                  + (!string.IsNullOrEmpty(s.NoiDung) ? s.NoiDung : "") + " "
                    //                  + (!string.IsNullOrEmpty(s.GhiChu) ? s.GhiChu : "");
                });

        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly ICuocHopValidate _cuocHopValidate;
        public CuocHopRepository(IContext context, ICuocHopValidate cuocHopValidate)
        {
            _context = context;
            _cuocHopValidate = cuocHopValidate;
        }

        //required
        private readonly string sortStringDefault = "ThoiGianBatDau asc";
        private readonly List<string> searchCol = new List<string> { "TenCuocHop" };

        public DataTableResult<CuocHopGridVo> GetAll(CuocHopQueryParamViewModel queryParams)
        {
            //required
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.CuocHops.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            // TrangThai đã bị comment trong entity
            //if (queryParams.TrangThaiXuLy.Count() > 0)
            //{
            //    query = query.Where(o => queryParams.TrangThaiXuLy.Contains(o.TrangThai));
            //}

            if (queryParams.UserId.GetValueOrDefault() != 0)
            {
                query = query.Where(o => o.NguoiTaoId == queryParams.UserId);
            }

            if (queryParams.NguoiDuyetId.GetValueOrDefault() != 0)
            {
                query = query.Where(o => o.NguoiDuyetId == queryParams.NguoiDuyetId);
            }

            // NguoiDuocGanDuyetId, LoaiLichHop đã bị comment trong entity

            var data = query.Select(cv => new CuocHopGridVo
            {
                Id = cv.Id,
                TenCuocHop = cv.TenCuocHop,
                PhongHopId = cv.PhongHopId,
                ThoiGianBatDau = cv.ThoiGianBatDau,
                ThoiGianKetThuc = cv.ThoiGianKetThuc,
                //NoiDung = cv.NoiDung,
                IsDeleted = cv.IsDeleted,
                // TrangThai đã bị comment trong entity
                SoNguoiThamGia = cv.SoNguoiThamGia,
                GhiChu = cv.GhiChu,
                LyDoTraPhieu = cv.LyDoTraPhieu,
                NguoiTaoId = cv.NguoiTaoId,
                NgayTao = cv.NgayTao,
                // NguoiChuTri đã bị comment trong entity
                PhongHopTxt = cv.PhongHopTxt,
                NguoiDuyetId = cv.NguoiDuyetId,
                NgayDuyet = cv.NgayDuyet,
                // CoQuanChuTriId, CoQuanThamGiaId, LoaiLichHop đã bị comment trong entity
                CuocHopTrungId = cv.CuocHopTrungId,
                NguoiThamGiaKhac = cv.NguoiThamGiaKhac,
            }).Where(queryParams.searchString);

            var total = data.Count();

            var datas = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList();

            // Load danh sách người tham gia từ bảng NguoiThamGia
            var cuocHopIds = datas.Select(o => o.Id).ToList();
            if (cuocHopIds.Count > 0)
            {
                var nguoiThamGias = _context.NguoiThamGias.AsNoTracking()
                    .Where(o => o.CuocHopId.HasValue && cuocHopIds.Contains(o.CuocHopId.Value) && o.UserId.HasValue)
                    .GroupBy(o => o.CuocHopId.Value)
                    .ToDictionary(g => g.Key, g => g.Select(x => new NguoiThamGiaGridVo
                    {
                        Id = x.Id,
                        CuocHopId = x.CuocHopId,
                        UserId = x.UserId
                    }).ToList());
                var phongBanThamGias = _context.PhongBanThamGiaCuocHops.AsNoTracking()
                    .Where(o => o.CuocHopId.HasValue && cuocHopIds.Contains(o.CuocHopId.Value) && o.PhongBanId.HasValue)
                    .GroupBy(o => o.CuocHopId.Value)
                    .ToDictionary(g => g.Key, g => g.Select(x => new PhongBanThamGiaCuocHopGridVo
                    {
                        Id = x.Id,
                        CuocHopId = x.CuocHopId,
                        PhongBanId = x.PhongBanId
                    }).ToList());

                foreach (var item in datas)
                {
                    if (nguoiThamGias.ContainsKey(item.Id))
                    {
                        item.NguoiThamGias = nguoiThamGias[item.Id];
                        item.NguoiThamGia = item.NguoiThamGias.Count > 0
                            ? string.Join(",", item.NguoiThamGias.Where(o => o.UserId.HasValue).Select(o => o.UserId.Value))
                            : "";
                    }
                    else
                    {
                        item.NguoiThamGias = new List<NguoiThamGiaGridVo>();
                        item.NguoiThamGia = "";
                    }

                    if (phongBanThamGias.ContainsKey(item.Id))
                    {
                        item.PhongBanThamGiaCuocHops = phongBanThamGias[item.Id];
                        item.PhongBanThamGiaIds = string.Join(",", item.PhongBanThamGiaCuocHops
                            .Where(o => o.PhongBanId.HasValue)
                            .Select(o => o.PhongBanId.Value));
                    }
                }
            }

            return new DataTableResult<CuocHopGridVo>
            {
                rows = datas,
                total = total
            };
        }

        public List<CuocHopGridVo> LayCuocHopTuDanhDachIds(List<int> cuocHopIds)
        {
            return _context.CuocHops.AsNoTracking().Where(o => cuocHopIds.Contains(o.Id)).Select(s => new CuocHopGridVo
            {
                Id = s.Id,
                TenCuocHop = s.TenCuocHop,
            }).ToList();
        }

        public List<NgaySscheduler> GetAllCuocHops(QueryParamViewModel queryParams)
        {
            var results = new List<CuocHopGridVo>();

            var query = _context.CuocHops.AsNoTracking().Where(o => o.ThoiGianBatDau >= queryParams.LichHopTuNgay
                && o.IsDeleted != true && o.ThoiGianBatDau <= queryParams.LichHopDenNgay);

            if (queryParams.PhongBanIds != null && queryParams.PhongBanIds.Count > 0)
            {
                var phongBanIdsBanDau = queryParams.PhongBanIds.Select(id => id.GetValueOrDefault()).Where(id => id > 0).ToList();

                if (phongBanIdsBanDau.Count > 0)
                {
                    // Lấy danh sách UserId có PhongBanId thuộc allPhongBanIds từ cache
                    var users = CacheManagerHelper.GetUserCache();
                    var userIdsInPhongBan = users
                        .Where(u => u.PhongBanId.HasValue && phongBanIdsBanDau.Contains(u.PhongBanId.Value) && (u.IsDeleted != true))
                        .Select(u => u.Id)
                        .Where(id => id > 0)
                        .ToList();

                    query = query.Where(o =>
                        (o.CoQuanId.HasValue && phongBanIdsBanDau.Contains(o.CoQuanId.Value))
                        || (o.NguoiTaoId.HasValue && userIdsInPhongBan.Contains(o.NguoiTaoId.Value))
                        || o.PhongBanThamGiaCuocHops.Any(pbtg => pbtg.PhongBanId.HasValue && phongBanIdsBanDau.Contains(pbtg.PhongBanId.Value))
                        || o.NguoiThamGias.Any(ntg => ntg.UserId.HasValue && userIdsInPhongBan.Contains(ntg.UserId.Value)));
                }
            }

            results = query.Select(x => new CuocHopGridVo
            {
                Id = x.Id,
                TenCuocHop = x.TenCuocHop,
                ThoiGianBatDau = x.ThoiGianBatDau,
                ThoiGianKetThuc = x.ThoiGianKetThuc,
                GhiChu = x.GhiChu,
                PhongHopTxt = x.PhongHopTxt,
                PhongHopId = x.PhongHopId,
                LyDoId = x.LyDoId,
                NguoiThamGiaKhac = x.NguoiThamGiaKhac,
                NguoiThamGias = x.NguoiThamGias.Select(o => new NguoiThamGiaGridVo
                {
                    UserId = o.UserId
                }).ToList(),
                PhongBanThamGiaCuocHops = x.PhongBanThamGiaCuocHops.Select(o => new PhongBanThamGiaCuocHopGridVo
                {
                    PhongBanId = o.PhongBanId
                }).ToList()
            }).OrderBy(o => o.ThoiGianBatDau).ToList();

            var datas = new List<NgaySscheduler>();

            var ngay = queryParams.LichHopTuNgay;
            while (ngay <= queryParams.LichHopDenNgay)
            {
                // xử lý
                var item = new NgaySscheduler
                {
                    Ngay = ngay
                };

                item.Sangs = results.Where(o => o.NgayDienRaStr == item.NgayStr && o.ThoiGianBatDau.GetValueOrDefault().Hour < 12).ToList();
                item.Chieus = results.Where(o => o.NgayDienRaStr == item.NgayStr && o.ThoiGianBatDau.GetValueOrDefault().Hour >= 12).ToList();

                datas.Add(item);

                ngay = ngay.AddDays(1);
            }

            return datas;
        }

        public dynamic Them(CuocHopViewModel lichHopViewModel)
        {
            var errors = _cuocHopValidate.Validate(lichHopViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            var item = _iMapper.Map<CuocHopViewModel, CuocHop>(lichHopViewModel);

            item.NguoiTaoId = lichHopViewModel.NguoiTaoId;
            item.NgayTao = DateTime.Now;
            item.CoQuanId = lichHopViewModel.CoQuanId;

            AddFileToEntity(item, lichHopViewModel.FileDinhKems);

            //cap nhat thoi gian ket thuc
            item.ThoiGianKetThucTemp = item.ThoiGianKetThuc != null
              ? item.ThoiGianKetThuc : DateTimeHelper.EndOfDay(item.ThoiGianBatDau.Value);

            _context.CuocHops.Add(item);

            // Lưu người tham gia vào bảng NguoiThamGia
            var danhSachNguoiThamGia = new List<int>();
            if (!string.IsNullOrEmpty(lichHopViewModel.NguoiThamGia))
            {
                danhSachNguoiThamGia = UltiHelper.ConvertIdStringToListInt(lichHopViewModel.NguoiThamGia);
                foreach (var userId in danhSachNguoiThamGia)
                {
                    item.NguoiThamGias.Add(new NguoiThamGia
                    {
                        UserId = userId
                    });
                }
            }

            var danhSachPhongBanThamGia = LayDanhSachId(lichHopViewModel.PhongBanThamGiaIds);
            foreach (var phongBanId in danhSachPhongBanThamGia)
            {
                item.PhongBanThamGiaCuocHops.Add(new PhongBanThamGiaCuocHop
                {
                    PhongBanId = phongBanId
                });
            }

            _context.SaveChanges();



            return new { Id = item.Id };
        }

        private static string LayThongBaoLoiChiTiet(Exception ex)
        {
            if (ex == null) return "";
            var current = ex;
            while (current.InnerException != null)
            {
                current = current.InnerException;
            }
            return current.Message;
        }

        public dynamic ImportDanhSachLich(ImportLichCuocHopRequestViewModel request)
        {
            var ketQua = new List<CuocHopImportGridVo>();
            if (request == null || request.DanhSachCuocHop == null || request.DanhSachCuocHop.Count == 0)
            {
                return new
                {
                    Success = false,
                    Message = "Không tìm thấy dữ liệu lịch họp trong file.",
                    Data = ketQua,
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "ImportLich",
                            Description = "Không tìm thấy dữ liệu lịch họp trong file."
                        }
                    }
                };
            }

            var defaultLyDoId = request.LyDoId.GetValueOrDefault();
            if (defaultLyDoId == 0)
            {
                var lyDoDefault = CacheManagerHelper.GetLyDoCache().FirstOrDefault(o => o.IsDeleted != true);
                defaultLyDoId = lyDoDefault != null ? lyDoDefault.Id : 1;
            }

            var coLoi = false;
            var danhSachCuocHopVm = new List<Tuple<CuocHopImportGridVo, CuocHopViewModel>>();

            foreach (var item in request.DanhSachCuocHop)
            {
                var loiDong = new List<string>();

                if (string.IsNullOrWhiteSpace(item.NoiDung))
                {
                    loiDong.Add("Nội dung cuộc họp không được để trống");
                }

                if (string.IsNullOrWhiteSpace(item.NgayStr))
                {
                    loiDong.Add("Ngày diễn ra không được để trống");
                }
                else
                {
                    var ngayParsed = DateTimeHelper.ConvertStrToDate(item.NgayStr.Trim());
                    if (!ngayParsed.HasValue)
                    {
                        loiDong.Add("Ngày diễn ra không hợp lệ (định dạng dd/MM/yyyy)");
                    }
                }

                var thamGiaList = new List<string>();
                if (!string.IsNullOrWhiteSpace(item.ChuTri))
                {
                    thamGiaList.Add($"Chủ trì: {item.ChuTri.Trim()}");
                }
                if (!string.IsNullOrWhiteSpace(item.ThanhPhan))
                {
                    thamGiaList.Add($"Thành phần: {item.ThanhPhan.Trim()}");
                }

                var nguoiThamGiaKhac = string.Join("; ", thamGiaList);
                if (string.IsNullOrWhiteSpace(nguoiThamGiaKhac))
                {
                    nguoiThamGiaKhac = "Theo thông báo";
                }

                var batDauStr = !string.IsNullOrWhiteSpace(item.BatDauStr) ? item.BatDauStr.Trim() : "08:00";
                var ketThucStr = !string.IsNullOrWhiteSpace(item.KetThucStr) ? item.KetThucStr.Trim() : "";

                var cuocHopVm = new CuocHopViewModel
                {
                    Id = 0,
                    TenCuocHop = item.NoiDung?.Trim(),
                    NgayDienRaStr = item.NgayStr?.Trim(),
                    BatDauStr = batDauStr,
                    KetThucStr = ketThucStr,
                    NguoiThamGiaKhac = nguoiThamGiaKhac,
                    PhongHopTxt = item.DiaDiem?.Trim() ?? "",
                    GhiChu = item.GhiChu?.Trim() ?? "",
                    LyDoId = item.LyDoId.GetValueOrDefault() > 0 ? item.LyDoId.Value : defaultLyDoId,
                    NguoiTaoId = request.UserId,
                    CoQuanId = request.PhongBanId,
                    IsAddOrUpdate = true
                };

                if (cuocHopVm.ThoiGianBatDau.HasValue && cuocHopVm.ThoiGianKetThuc.HasValue && cuocHopVm.ThoiGianBatDau >= cuocHopVm.ThoiGianKetThuc)
                {
                    cuocHopVm.KetThucStr = "";
                }

                var validateErrors = _cuocHopValidate.Validate(cuocHopVm);
                if (validateErrors != null && validateErrors.Count > 0)
                {
                    loiDong.AddRange(validateErrors.Select(e => e.Description));
                }

                if (loiDong.Count > 0)
                {
                    coLoi = true;
                    item.IsThanhCong = false;
                    item.Loi = string.Join("; ", loiDong);
                }
                else
                {
                    item.IsThanhCong = true;
                    item.Loi = "";
                    danhSachCuocHopVm.Add(new Tuple<CuocHopImportGridVo, CuocHopViewModel>(item, cuocHopVm));
                }

                ketQua.Add(item);
            }

            if (coLoi)
            {
                return new
                {
                    Success = false,
                    Message = "File import có lỗi. Toàn bộ dữ liệu chưa được lưu vào hệ thống.",
                    TongSo = request.DanhSachCuocHop.Count,
                    SoLuongThanhCong = 0,
                    SoLuongThatBai = ketQua.Count(o => !o.IsThanhCong),
                    Data = ketQua
                };
            }

            var dbContext = _context as DbContext;
            DbContextTransaction transaction = null;

            try
            {
                if (dbContext != null)
                {
                    transaction = dbContext.Database.BeginTransaction();
                }

                foreach (var tuple in danhSachCuocHopVm)
                {
                    var gridVo = tuple.Item1;
                    var vm = tuple.Item2;

                    var itemEntity = _iMapper.Map<CuocHopViewModel, CuocHop>(vm);
                    itemEntity.NguoiTaoId = vm.NguoiTaoId;
                    itemEntity.NgayTao = DateTime.Now;
                    itemEntity.CoQuanId = vm.CoQuanId;
                    itemEntity.ThoiGianKetThucTemp = itemEntity.ThoiGianKetThuc != null
                        ? itemEntity.ThoiGianKetThuc
                        : (itemEntity.ThoiGianBatDau.HasValue ? DateTimeHelper.EndOfDay(itemEntity.ThoiGianBatDau.Value) : (DateTime?)null);

                    _context.CuocHops.Add(itemEntity);
                    gridVo.IsThanhCong = true;
                    gridVo.Loi = "Thành công";
                }

                _context.SaveChanges();

                if (transaction != null)
                {
                    transaction.Commit();
                }

                return new
                {
                    Success = true,
                    Message = $"Import thành công {danhSachCuocHopVm.Count} lịch họp.",
                    TongSo = request.DanhSachCuocHop.Count,
                    SoLuongThanhCong = danhSachCuocHopVm.Count,
                    SoLuongThatBai = 0,
                    Data = ketQua
                };
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    try { transaction.Rollback(); } catch { }
                }

                var chiTietLoi = LayThongBaoLoiChiTiet(ex);
                foreach (var item in ketQua)
                {
                    item.IsThanhCong = false;
                    item.Loi = "Lỗi khi lưu vào CSDL: " + chiTietLoi;
                }

                return new
                {
                    Success = false,
                    Message = "Lỗi khi lưu vào cơ sở dữ liệu: " + chiTietLoi,
                    TongSo = request.DanhSachCuocHop.Count,
                    SoLuongThanhCong = 0,
                    SoLuongThatBai = ketQua.Count,
                    Data = ketQua
                };
            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }
            }
        }

        private static List<int> LayDanhSachId(string idString)
        {
            return string.IsNullOrWhiteSpace(idString)
                ? new List<int>()
                : UltiHelper.ConvertIdStringToListInt(idString).Where(id => id > 0).Distinct().ToList();
        }

        private static List<int> LayDanhSachNguoiNhanThongBao(List<int> nguoiThamGiaIds, List<int> phongBanThamGiaIds)
        {
            var nguoiNhanIds = nguoiThamGiaIds.Where(id => id > 0).ToList();
            if (phongBanThamGiaIds.Count > 0)
            {
                nguoiNhanIds.AddRange(CacheManagerHelper.GetUserCache()
                    .Where(o => o.IsDeleted != true
                        && o.PhongBanId.HasValue
                        && phongBanThamGiaIds.Contains(o.PhongBanId.Value))
                    .Select(o => o.Id));
            }

            return nguoiNhanIds.Distinct().ToList();
        }

        private async Task GuiThongBaoNgayChoNguoiThamGia(CuocHop cuocHop, List<int> danhSachNguoiThamGia)
        {
            try
            {
                var thoiGianStr = cuocHop.ThoiGianBatDau.Value.ToString("HH:mm");
                var ngayStr = cuocHop.ThoiGianBatDau.Value.ToString("dd/MM/yyyy");

                var thongBaoNoiDung = $"📅 {cuocHop.TenCuocHop}\n";
                thongBaoNoiDung += $"⏰ Thời gian: {thoiGianStr} ngày {ngayStr}";

                if (!string.IsNullOrEmpty(cuocHop.GhiChu))
                {
                    thongBaoNoiDung += $"\n📝 Ghi chú: {cuocHop.GhiChu}";
                }

                foreach (var userId in danhSachNguoiThamGia)
                {
                    if (userId > 0)
                    {
                        await ThongBaoAdoNetHelper.Them(new ThongBao
                        {
                            NoiDung = thongBaoNoiDung,
                            NguoiNhanId = userId,
                            EnumLoaiThongBao = EnumLoaiThongBao.NhacNhoCuocHop,
                            CuocHopId = cuocHop.Id,
                            ThongBaoInfoJson = JsonConvert.SerializeObject(new ThongBaoThongTinJson
                            {
                                CuocHopId = cuocHop.Id
                            })
                        }, BienDungChung.ConnectDBStr);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error nếu cần
                System.Diagnostics.Debug.WriteLine($"Lỗi khi gửi thông báo cuộc họp: {ex.Message}");
            }
        }

        private async Task GuiThongBaoThayDoiChoNguoiThamGia(CuocHop cuocHop, List<int> danhSachNguoiThamGia,
            bool isTenCuocHopChanged, bool isThoiGianBatDauChanged, bool isThoiGianKetThucChanged,
            bool isPhongHopIdChanged, bool isPhongHopTxtChanged, bool isGhiChuChanged, bool isNguoiThamGiaChanged,
            string tenCuocHopCu, DateTime? thoiGianBatDauCu, DateTime? thoiGianKetThucCu,
            int? phongHopIdCu, string phongHopTxtCu, string ghiChuCu)
        {
            try
            {
                var thoiGianStr = cuocHop.ThoiGianBatDau.Value.ToString("HH:mm");
                var ngayStr = cuocHop.ThoiGianBatDau.Value.ToString("dd/MM/yyyy");

                var thongBaoNoiDung = $"🔄 Cuộc họp đã được cập nhật: {cuocHop.TenCuocHop}\n";
                thongBaoNoiDung += $"⏰ Thời gian: {thoiGianStr} ngày {ngayStr}\n\n";
                thongBaoNoiDung += "📋 Các thay đổi:\n";

                if (isTenCuocHopChanged)
                {
                    thongBaoNoiDung += $"• Tên cuộc họp: {tenCuocHopCu} → {cuocHop.TenCuocHop}\n";
                }

                if (isThoiGianBatDauChanged)
                {
                    var thoiGianBatDauCuStr = thoiGianBatDauCu.HasValue
                        ? thoiGianBatDauCu.Value.ToString("HH:mm dd/MM/yyyy")
                        : "Chưa có";
                    thongBaoNoiDung += $"• Thời gian bắt đầu: {thoiGianBatDauCuStr} → {thoiGianStr} {ngayStr}\n";
                }

                if (isThoiGianKetThucChanged)
                {
                    var thoiGianKetThucCuStr = thoiGianKetThucCu.HasValue
                        ? thoiGianKetThucCu.Value.ToString("HH:mm dd/MM/yyyy")
                        : "Chưa có";
                    var thoiGianKetThucMoiStr = cuocHop.ThoiGianKetThuc.HasValue
                        ? cuocHop.ThoiGianKetThuc.Value.ToString("HH:mm dd/MM/yyyy")
                        : "Chưa có";
                    thongBaoNoiDung += $"• Thời gian kết thúc: {thoiGianKetThucCuStr} → {thoiGianKetThucMoiStr}\n";
                }

                if (isPhongHopIdChanged || isPhongHopTxtChanged)
                {
                    var phongHopCuStr = phongHopIdCu.HasValue && phongHopIdCu.Value > 0
                        ? $"Phòng {phongHopIdCu.Value}"
                        : (!string.IsNullOrEmpty(phongHopTxtCu) ? phongHopTxtCu : "Chưa có");
                    var phongHopMoiStr = cuocHop.PhongHopId.HasValue && cuocHop.PhongHopId.Value > 0
                        ? $"Phòng {cuocHop.PhongHopId.Value}"
                        : (!string.IsNullOrEmpty(cuocHop.PhongHopTxt) ? cuocHop.PhongHopTxt : "Chưa có");
                    thongBaoNoiDung += $"• Phòng họp: {phongHopCuStr} → {phongHopMoiStr}\n";
                }

                if (isGhiChuChanged)
                {
                    var ghiChuCuStr = !string.IsNullOrEmpty(ghiChuCu) ? ghiChuCu : "Không có";
                    var ghiChuMoiStr = !string.IsNullOrEmpty(cuocHop.GhiChu) ? cuocHop.GhiChu : "Không có";
                    thongBaoNoiDung += $"• Ghi chú: {ghiChuCuStr} → {ghiChuMoiStr}\n";
                }

                if (isNguoiThamGiaChanged)
                {
                    thongBaoNoiDung += "• Danh sách người tham gia đã thay đổi\n";
                }

                if (!string.IsNullOrEmpty(cuocHop.GhiChu))
                {
                    thongBaoNoiDung += $"\n📝 Ghi chú hiện tại: {cuocHop.GhiChu}";
                }

                foreach (var userId in danhSachNguoiThamGia)
                {
                    if (userId > 0)
                    {
                        await ThongBaoAdoNetHelper.Them(new ThongBao
                        {
                            NoiDung = thongBaoNoiDung,
                            NguoiNhanId = userId,
                            EnumLoaiThongBao = EnumLoaiThongBao.NhacNhoCuocHop,
                            CuocHopId = cuocHop.Id,
                            ThongBaoInfoJson = JsonConvert.SerializeObject(new ThongBaoThongTinJson
                            {
                                CuocHopId = cuocHop.Id
                            })
                        }, BienDungChung.ConnectDBStr);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error nếu cần
                System.Diagnostics.Debug.WriteLine($"Lỗi khi gửi thông báo thay đổi cuộc họp: {ex.Message}");
            }
        }

        private static void AddFileToEntity(CuocHop item, List<HttpPostedFileBase> FileDinhKems)
        {
            var uploadPath = System.Web.HttpContext.Current.Server.MapPath(BienDungChung.UploadsPath);
            foreach (var taiLieu in FileDinhKems)
            {
                if (taiLieu == null) continue;

                try
                {
                    item.TaiLieuCuocHops.Add(new TaiLieuCuocHop
                    {
                        Ten = taiLieu.FileName,
                        Loai = Path.GetExtension(taiLieu.FileName),
                        DuongDan = FileHelper.SaveHttpFilesToUploads(uploadPath, taiLieu),
                    });

                }
                catch (ArgumentException ex)
                {
                    throw new Exception($"Lỗi khi upload file '{taiLieu?.FileName}': {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Lỗi khi lưu file '{taiLieu?.FileName}': {ex.Message}", ex);
                }
            }
        }

        // Method TaoCoQuanNguoiChuTriThamGiaId đã bị xóa vì NguoiChuTri, CoQuanNguoiChuTriThamGiaId đã bị comment trong entity

        public dynamic CapNhat(CuocHopViewModel lichHopViewModel)
        {
            var item = _context.CuocHops
                .Include("NguoiThamGias")
                .Include("PhongBanThamGiaCuocHops")
                .FirstOrDefault(o => o.Id == lichHopViewModel.Id);
            if (item == null)
            {
                return new { Id = 0 };
            }

            // TrangThai, LoaiLichHop đã bị comment trong entity

            var errors = _cuocHopValidate.Validate(lichHopViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            // Lưu dữ liệu cũ để so sánh
            var tenCuocHopCu = item.TenCuocHop;
            var thoiGianBatDauCu = item.ThoiGianBatDau;
            var thoiGianKetThucCu = item.ThoiGianKetThuc;
            var phongHopIdCu = item.PhongHopId;
            var phongHopTxtCu = item.PhongHopTxt;
            var ghiChuCu = item.GhiChu;
            var nguoiThamGiaCuIds = item.NguoiThamGias.Where(o => o.UserId.HasValue).Select(o => o.UserId.Value).ToList();
            var phongBanThamGiaCuIds = item.PhongBanThamGiaCuocHops
                .Where(o => o.PhongBanId.HasValue)
                .Select(o => o.PhongBanId.Value)
                .ToList();

            _iMapper.Map(lichHopViewModel, item);

            //item.LoaiLichHop = lichHopViewModel.LoaiLichHop;
            item.NgaySua = DateTime.Now;
            item.NguoiSuaId = lichHopViewModel.NguoiTaoId;

            AddFileToEntity(item, lichHopViewModel.FileDinhKems);
            // CoQuanNguoiChuTriThamGiaId, NguoiChuTri, LoaiLichHop đã bị comment trong entity

            // Xóa người tham gia cũ và thêm lại
            var nguoiThamGiaCuEntity = item.NguoiThamGias.ToList();
            foreach (var ntg in nguoiThamGiaCuEntity)
            {
                _context.NguoiThamGias.Remove(ntg);
            }

            // Thêm người tham gia mới
            var danhSachNguoiThamGiaMoi = new List<int>();
            if (!string.IsNullOrEmpty(lichHopViewModel.NguoiThamGia))
            {
                danhSachNguoiThamGiaMoi = UltiHelper.ConvertIdStringToListInt(lichHopViewModel.NguoiThamGia);
                foreach (var userId in danhSachNguoiThamGiaMoi)
                {
                    _context.NguoiThamGias.Add(new NguoiThamGia
                    {
                        CuocHopId = item.Id,
                        UserId = userId
                    });
                }
            }

            var phongBanThamGiaCuEntity = item.PhongBanThamGiaCuocHops.ToList();
            foreach (var phongBanThamGia in phongBanThamGiaCuEntity)
            {
                _context.PhongBanThamGiaCuocHops.Remove(phongBanThamGia);
            }

            var danhSachPhongBanThamGiaMoi = LayDanhSachId(lichHopViewModel.PhongBanThamGiaIds);
            foreach (var phongBanId in danhSachPhongBanThamGiaMoi)
            {
                _context.PhongBanThamGiaCuocHops.Add(new PhongBanThamGiaCuocHop
                {
                    CuocHopId = item.Id,
                    PhongBanId = phongBanId
                });
            }

            //cap nhat thoi gian ket thuc
            item.ThoiGianKetThucTemp = item.ThoiGianKetThuc != null
              ? item.ThoiGianKetThuc : DateTimeHelper.EndOfDay(item.ThoiGianBatDau.Value);

            _context.SaveChanges();

            // So sánh thay đổi và gửi thông báo nếu có
            var isTenCuocHopChanged = tenCuocHopCu != item.TenCuocHop;
            var isThoiGianBatDauChanged = (thoiGianBatDauCu.HasValue != item.ThoiGianBatDau.HasValue)
                || (thoiGianBatDauCu.HasValue && item.ThoiGianBatDau.HasValue
                    && thoiGianBatDauCu.Value.ToString("yyyyMMddHHmm") != item.ThoiGianBatDau.Value.ToString("yyyyMMddHHmm"));
            var isThoiGianKetThucChanged = (thoiGianKetThucCu.HasValue != item.ThoiGianKetThuc.HasValue)
                || (thoiGianKetThucCu.HasValue && item.ThoiGianKetThuc.HasValue
                    && thoiGianKetThucCu.Value.ToString("yyyyMMddHHmm") != item.ThoiGianKetThuc.Value.ToString("yyyyMMddHHmm"));
            var isPhongHopIdChanged = phongHopIdCu != item.PhongHopId;
            var isPhongHopTxtChanged = phongHopTxtCu != item.PhongHopTxt;
            var isGhiChuChanged = ghiChuCu != item.GhiChu;
            var isNguoiThamGiaChanged = !nguoiThamGiaCuIds.OrderBy(x => x).SequenceEqual(danhSachNguoiThamGiaMoi.OrderBy(x => x));
            var isPhongBanThamGiaChanged = !phongBanThamGiaCuIds.OrderBy(x => x)
                .SequenceEqual(danhSachPhongBanThamGiaMoi.OrderBy(x => x));

            var hasThayDoi = isTenCuocHopChanged || isThoiGianBatDauChanged || isThoiGianKetThucChanged
                || isPhongHopIdChanged || isPhongHopTxtChanged || isGhiChuChanged || isNguoiThamGiaChanged
                || isPhongBanThamGiaChanged;

            if (hasThayDoi && item.ThoiGianBatDau.HasValue)
            {
                // Lấy danh sách người tham gia (cả cũ và mới) để gửi thông báo
                var danhSachNguoiThamGiaCanThongBao = LayDanhSachNguoiNhanThongBao(
                    danhSachNguoiThamGiaMoi.Union(nguoiThamGiaCuIds).Distinct().ToList(),
                    danhSachPhongBanThamGiaMoi.Union(phongBanThamGiaCuIds).Distinct().ToList());

                if (danhSachNguoiThamGiaCanThongBao.Count > 0)
                {
                    Task.Run(async () =>
                    {
                        await GuiThongBaoThayDoiChoNguoiThamGia(item, danhSachNguoiThamGiaCanThongBao,
                            isTenCuocHopChanged, isThoiGianBatDauChanged, isThoiGianKetThucChanged,
                            isPhongHopIdChanged, isPhongHopTxtChanged, isGhiChuChanged,
                            isNguoiThamGiaChanged || isPhongBanThamGiaChanged,
                            tenCuocHopCu, thoiGianBatDauCu, thoiGianKetThucCu, phongHopIdCu, phongHopTxtCu, ghiChuCu);
                    });
                }
            }

            return new { Id = item.Id };
        }

        public bool Xoa(int Id)
        {
            var item = _context.CuocHops.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                foreach (var qd in item.TaiLieuCuocHops.ToList())
                {
                    qd.IsDeleted = true;
                }

                item.IsDeleted = true;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public bool KhoiPhuc(int Id)
        {
            var item = _context.CuocHops.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public dynamic GuiLaiPhieu(CuocHopViewModel cuocHopViewModel)
        {
            var item = _context.CuocHops.FirstOrDefault(o => o.Id == cuocHopViewModel.Id);
            if (item == null) throw new Exception("Cuộc họp đã bị xóa");

            // TrangThai, NguoiDuocGanDuyetId đã bị comment trong entity
            var now = DateTime.Now;
            item.NgaySua = now;
            item.NguoiSuaId = item.NguoiTaoId;

            _context.SaveChanges();

            // NguoiDuocGanDuyetId đã bị comment trong entity
            Task.Run(async () =>
            {

            });

            return new { Id = item.Id };
        }

        public dynamic DuyetPhieu(CuocHopViewModel cuocHopViewModel)
        {
            var phongHopCuId = 0;
            var phongHopMoiId = cuocHopViewModel.PhongHopId;

            var errors = _cuocHopValidate.Validate(cuocHopViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            var item = _context.CuocHops.FirstOrDefault(o => o.Id == cuocHopViewModel.Id);
            if (item == null) throw new Exception("Cuộc họp đã bị xóa");
            phongHopCuId = item.PhongHopId.GetValueOrDefault();

            // TrangThai đã bị comment trong entity
            item.NguoiDuyetId = cuocHopViewModel.NguoiDuyetId;
            item.NgayDuyet = DateTime.Now;

            cuocHopViewModel.TenCuocHop = item.TenCuocHop;

            _context.SaveChanges();

            cuocHopViewModel.Id = item.Id;

            Task.Run(async () =>
            {
            });

            return new { Id = item.Id };
        }

        public dynamic TraPhieu(CuocHopViewModel cuocHopViewModel)
        {
            var errors = _cuocHopValidate.Validate(cuocHopViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            var item = _context.CuocHops.FirstOrDefault(o => o.Id == cuocHopViewModel.Id);
            if (item == null) throw new Exception("Cuộc họp đã bị xóa");
            // TrangThai đã bị comment trong entity
            var phongHopCuId = item.PhongHopId;

            item.LyDoTraPhieu = cuocHopViewModel.LyDoTraPhieu;

            item.NgaySua = DateTime.Now;
            item.NguoiSuaId = cuocHopViewModel.CurrentUserId;

            _context.SaveChanges();

            //check trung 
            Task.Run(async () =>
            {

            });

            return new { Id = item.Id };
        }

        public CuocHopGridVo GetCuocHopInfoFromId(int id)
        {
            var data = new CuocHopGridVo();
            var item = _context.CuocHops
                .Include(o => o.NguoiThamGias)
                .Include(o => o.PhongBanThamGiaCuocHops)
                .AsNoTracking()
                .FirstOrDefault(o => o.Id == id && o.IsDeleted != true);
            if (item != null)
            {
                data.Id = item.Id;
                data.TenCuocHop = item.TenCuocHop;
                data.PhongHopId = item.PhongHopId;
                data.ThoiGianBatDau = item.ThoiGianBatDau;
                data.ThoiGianKetThuc = item.ThoiGianKetThuc;
                //data.NoiDung = item.NoiDung;
                data.IsDeleted = item.IsDeleted;
                // TrangThai, LoaiLichHop, NguoiDuocGanDuyetId đã bị comment trong entity
                data.GhiChu = item.GhiChu;
                data.LyDoTraPhieu = item.LyDoTraPhieu;
                data.NgayTao = item.NgayTao;
                // NguoiChuTri đã bị comment trong entity
                data.PhongHopTxt = item.PhongHopTxt;
                data.NguoiTaoId = item.NguoiTaoId;
                data.NguoiThamGiaKhac = item.NguoiThamGiaKhac;

                // Load người tham gia từ bảng NguoiThamGia
                var nguoiThamGiaIds = item.NguoiThamGias
                    .Where(o => o.CuocHopId == item.Id)
                    .Select(o => o.UserId)
                    .ToList();
                var nguoiThamGiaStr = nguoiThamGiaIds.Count > 0 ? string.Join(",", nguoiThamGiaIds) : "";
                data.NguoiThamGia = UltiHelper.DinhDangSelect2ListJs(nguoiThamGiaStr);

                var phongBanThamGiaIds = item.PhongBanThamGiaCuocHops
                    .Where(o => o.PhongBanId.HasValue)
                    .Select(o => o.PhongBanId.Value)
                    .ToList();
                var phongBanThamGiaIdStr = phongBanThamGiaIds.Count > 0 ? string.Join(",", phongBanThamGiaIds) : "";
                data.PhongBanThamGiaIds = UltiHelper.DinhDangSelect2ListJs(phongBanThamGiaIdStr);
                data.PhongBanThamGiaCuocHops = item.PhongBanThamGiaCuocHops.Select(o => new PhongBanThamGiaCuocHopGridVo
                {
                    Id = o.Id,
                    CuocHopId = o.CuocHopId,
                    PhongBanId = o.PhongBanId
                }).ToList();

                // CoQuanChuTriId, CoQuanThamGiaId đã bị comment trong entity
                data.LyDoId = item.LyDoId;
                data.IsThongBao = item.IsThongBao;
            }

            return data;
        }

        public dynamic GetTaiLieuCuocHop(int id)
        {
            var datas = new List<TaiLieuCuocHopGridVo>();
            if (id != 0)
            {
                datas = _context.TaiLieuCuocHops.AsNoTracking().Where(o => o.CuocHopId == id && o.IsDeleted != true)
                    .Select(o => new TaiLieuCuocHopGridVo
                    {
                        Id = o.Id,
                        Name = o.Ten,
                        CuocHopId = o.CuocHopId,
                        DuongDan = o.DuongDan
                    }).OrderByDescending(o => o.Id).ToList();
            }

            return datas;
        }

        public List<TaiLieuCuocHopGridVo> GetTaiLieuCuocHopByCuocHopIds(List<int> cuocHopIds)
        {
            if (cuocHopIds == null || cuocHopIds.Count == 0)
            {
                return new List<TaiLieuCuocHopGridVo>();
            }

            return _context.TaiLieuCuocHops.AsNoTracking()
                .Where(o => cuocHopIds.Contains((int)o.CuocHopId) && o.IsDeleted != true)
                .Select(o => new TaiLieuCuocHopGridVo
                {
                    Id = o.Id,
                    Name = o.Ten,
                    CuocHopId = o.CuocHopId,
                    DuongDan = o.DuongDan
                }).OrderByDescending(o => o.Id).ToList();
        }

        public FileDinhKemGridVo DownloadTaiLieuCuocHop(int id)
        {
            var data = new FileDinhKemGridVo();

            var item = _context.TaiLieuCuocHops.AsNoTracking().FirstOrDefault(o => o.Id == id);

            if (item != null)
            {
                data.DuongDan = item.DuongDan;
                data.Loai = item.Loai;
            }

            return data;
        }

        public dynamic XoaTaiLieuCuocHop(int id)
        {
            if (id != 0)
            {
                var item = _context.TaiLieuCuocHops.FirstOrDefault(o => o.Id == id);
                if (item != null)
                {
                    item.IsDeleted = true;
                    _context.SaveChanges();
                }
            }

            return true;
        }

        public dynamic XoaNguoiThamGiaCuocHopDaChon(int cuocHopId, int nguoiLienQuanId)
        {
            if (cuocHopId != 0 && nguoiLienQuanId != 0)
            {
                var item = _context.NguoiThamGias.FirstOrDefault(o => o.CuocHopId == cuocHopId
                && o.UserId == nguoiLienQuanId);
                if (item != null)
                {
                    _context.NguoiThamGias.Remove(item);
                    _context.SaveChanges();
                }
            }

            return true;
        }

        public dynamic CapNhatThongTinPhongThoiGian(CuocHopViewModel cuocHopViewModel)
        {
            var phongHopMoiId = cuocHopViewModel.PhongHopId;

            var item = _context.CuocHops.Include(o => o.NguoiThamGias).FirstOrDefault(o => o.Id == cuocHopViewModel.Id);
            if (item == null) throw new Exception("Cuộc họp đã bị xóa");

            //so sánh thay doi

            bool isThoiGianBatDauChanged = item.ThoiGianBatDau.GetValueOrDefault().ToString("yyyyMMddHHmm") != cuocHopViewModel.ThoiGianBatDau.GetValueOrDefault().ToString("yyyyMMddHHmm");
            bool isThoiGianKetThucChanged = item.ThoiGianKetThuc.GetValueOrDefault().ToString("yyyyMMddHHmm") != cuocHopViewModel.ThoiGianKetThuc.GetValueOrDefault().ToString("yyyyMMddHHmm");
            bool isPhongHopChanged = item.PhongHopId != cuocHopViewModel.PhongHopId;
            bool isPhongHopKhacChanged = item.PhongHopTxt != cuocHopViewModel.PhongHopTxt;

            bool isThongTinPhongThoiGianChanged = isThoiGianBatDauChanged || isThoiGianKetThucChanged || isPhongHopChanged || isPhongHopKhacChanged;

            var thayDoiThoiGian = isThoiGianBatDauChanged || isThoiGianKetThucChanged;

            //gan gia trị
            item.NgaySua = DateTime.Now;
            item.TenCuocHop = cuocHopViewModel.TenCuocHop;
            item.NguoiSuaId = cuocHopViewModel.CurrentUserId;
            item.PhongHopId = cuocHopViewModel.PhongHopId;
            item.PhongHopTxt = cuocHopViewModel.PhongHopTxt;
            item.ThoiGianBatDau = cuocHopViewModel.ThoiGianBatDau;
            item.ThoiGianKetThuc = cuocHopViewModel.ThoiGianKetThuc;
            item.GhiChu = cuocHopViewModel.GhiChu;

            // Xóa người tham gia cũ và thêm lại
            var nguoiThamGiaCuEntity = item.NguoiThamGias.ToList();
            foreach (var ntg in nguoiThamGiaCuEntity)
            {
                _context.NguoiThamGias.Remove(ntg);
            }

            // Thêm người tham gia mới
            if (!string.IsNullOrEmpty(cuocHopViewModel.NguoiThamGia))
            {
                var danhSachNguoiThamGia = UltiHelper.ConvertIdStringToListInt(cuocHopViewModel.NguoiThamGia);
                foreach (var userId in danhSachNguoiThamGia)
                {
                    _context.NguoiThamGias.Add(new NguoiThamGia
                    {
                        CuocHopId = item.Id,
                        UserId = userId
                    });
                }
            }

            //cap nhat thoi gian ket thuc
            item.ThoiGianKetThucTemp = item.ThoiGianKetThuc != null
              ? item.ThoiGianKetThuc : DateTimeHelper.EndOfDay(item.ThoiGianBatDau.Value);

            cuocHopViewModel.TenCuocHop = item.TenCuocHop;

            var laiXeId = 0;
            if (thayDoiThoiGian)
            {
                var dhx = _context.DieuHanhXes.FirstOrDefault(o => o.CuocHopId == item.Id);
                if (dhx != null)
                {
                    laiXeId = dhx.TaiXeId.GetValueOrDefault();

                    _context.DieuHanhXes.Remove(dhx);
                }
            }

            _context.SaveChanges();

            return new { Id = item.Id };
        }

        public bool KiemTraThoiGianThayDoiKhiDieuPhoiPhong(CuocHopViewModel cuocHopViewModel)
        {
            var item = _context.CuocHops.AsNoTracking().FirstOrDefault(o => o.Id == cuocHopViewModel.Id);
            if (item != null)
            {
                var dhx = _context.DieuHanhXes.AsNoTracking().FirstOrDefault(o => o.CuocHopId == item.Id);
                if (dhx != null)
                {
                    bool isThoiGianBatDauChanged = item.ThoiGianBatDau.GetValueOrDefault().ToString("yyyyMMddHHmm") != cuocHopViewModel.ThoiGianBatDau.GetValueOrDefault().ToString("yyyyMMddHHmm");
                    bool isThoiGianKetThucChanged = item.ThoiGianKetThuc.GetValueOrDefault().ToString("yyyyMMddHHmm") != cuocHopViewModel.ThoiGianKetThuc.GetValueOrDefault().ToString("yyyyMMddHHmm");

                    bool isThongTinPhongThoiGianChanged = isThoiGianBatDauChanged || isThoiGianKetThucChanged;

                    return isThongTinPhongThoiGianChanged;
                }
            }

            return false;
        }

        public List<ValidateError> KiemTraLoiKhiDieuPhoiPhong(CuocHopViewModel cuocHopViewModel)
        {
            var errors = new List<ValidateError>();
            if (cuocHopViewModel.PhongHopId.GetValueOrDefault() == 0
                && string.IsNullOrEmpty(cuocHopViewModel.PhongHopTxt))
            {
                errors.Add(new ValidateError
                {
                    Id = "PhongHopId",
                    Description = "Chọn phòng họp hoặc nhập phòng họp khác."
                });
            }

            if (string.IsNullOrEmpty(cuocHopViewModel.NgayDienRaStr))
            {
                errors.Add(new ValidateError
                {
                    Id = "NgayDienRaStr",
                    Description = "Ngày diễn ra không được trống."
                });
            }

            if (string.IsNullOrEmpty(cuocHopViewModel.BatDauStr))
            {
                errors.Add(new ValidateError
                {
                    Id = "BatDauStr",
                    Description = "Bắt đầu không được trống."
                });
            }

            //if (string.IsNullOrEmpty(cuocHopViewModel.KetThucStr))
            //{
            //    errors.Add(new ValidateError
            //    {
            //        Id = "KetThucStr",
            //        Description = "Kết thúc không được trống."
            //    });
            //}

            if (cuocHopViewModel.ThoiGianBatDau != null && cuocHopViewModel.ThoiGianKetThuc != null && cuocHopViewModel.ThoiGianBatDau.GetValueOrDefault() >= cuocHopViewModel.ThoiGianKetThuc.GetValueOrDefault())
            {
                errors.Add(new ValidateError
                {
                    Id = "KetThucStr",
                    Description = "Thời gian kết thúc phải lớn hơn thời gian bắt đầu."
                });
            }

            return errors;
        }

        public bool ThongBaoLaiLichHop(int cuocHopId)
        {
            var item = _context.CuocHops.AsNoTracking().FirstOrDefault(o => o.Id == cuocHopId && o.IsDeleted != true);
            if (item == null)
            {
                return false;
            }

            Task.Run(async () =>
            {

            });

            return true;
        }
    }
}
