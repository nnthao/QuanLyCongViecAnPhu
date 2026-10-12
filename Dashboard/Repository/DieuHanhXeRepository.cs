using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Dashboard.AdoNetHelper;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
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
    public interface IDieuHanhXeRepository
    {
        dynamic CapNhat(DieuHanhXeViewModel dieuHanhXeViewModel);
        dynamic CapXeVaTaiXe(DieuHanhXeViewModel dieuHanhXeViewModel);
        dynamic CapXeVaTaiXeChoLichhop(DieuHanhXeViewModel dieuHanhXeViewModel);
        dynamic CountChoToiDuyet();
        dynamic DuyetPhieuNgoaiTinh(DieuHanhXeViewModel dieuHanhXeViewModel);
        DataTableResult<DieuHanhXeGridVo> GetAll(DieuHanhXeQueryParamViewModel queryParams);
        List<NgaySscheduler> GetAllLichXes(QueryParamViewModel queryParams);
        DieuHanhXeGridVo GetDieuHanhXeInfoFromId(int id);
        dynamic GetLaiXeVaXeDeDuyetPhieu(string search);
        dynamic GetTaiLieuDieuHanhXe(int id);
        dynamic GuiLaiPhieu(DieuHanhXeViewModel dieuHanhXeViewModel);
        dynamic HuyBo(DieuHanhXeViewModel dieuHanhXeViewModel);
        dynamic HuyCapXe(DieuHanhXeViewModel dieuHanhXeViewModel);
        dynamic KetThuc(DieuHanhXeViewModel dieuHanhXeViewModel);
        dynamic LaiXeRanhLookup(DateTime tgbd, DateTime tgkt, int coQuanId = 0);
        dynamic Them(DieuHanhXeViewModel dieuHanhXeViewModel);
        dynamic ThemOrCapNhatDatXeKhuVuc(DieuHanhXeViewModel dieuHanhXeViewModel);
        dynamic TraPhieu(DieuHanhXeViewModel dieuHanhXeViewModel);
        dynamic XeRanhLookup(DateTime tgbd, DateTime tgkt, int coQuanId = 0);
        dynamic LaiXeKhuVucLookup(int coQuanId = 0);
        dynamic XeKhuVucLookup(int coQuanId = 0);
        bool Xoa(int id, int nguoiTaoId);
        dynamic XoaTaiLieuDatXe(int id, int nguoiTaoId);
    }

    public class DieuHanhXeRepository : IDieuHanhXeRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<DieuHanhXeViewModel, DieuHanhXe>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.NguoiTaoId, opt => opt.Ignore())
                .ForMember(x => x.NgayTao, opt => opt.Ignore())
                .ForMember(x => x.SearchStr, opt => opt.Ignore())
                 .ForMember(x => x.TrangThaiXuLy, opt => opt.Ignore())
                .AfterMap((s, d) =>
                {
                    d.SearchStr = (!string.IsNullOrEmpty(s.TieuDe) ? s.TieuDe : "") + " "
                                      + (!string.IsNullOrEmpty(s.DiaDiemDon) ? s.DiaDiemDon : "") + " "
                                      + (!string.IsNullOrEmpty(s.NoiDi) ? s.NoiDi : "") + " "
                                      + (!string.IsNullOrEmpty(s.NoiDen) ? s.NoiDen : "") + " "
                                      + (!string.IsNullOrEmpty(s.GhiChu) ? s.GhiChu : "");
                });

        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IDieuHanhXeValidate _dieuHanhXeValidate;

        public DieuHanhXeRepository(IContext context,
            IDieuHanhXeValidate dieuHanhXeValidate)
        {
            _context = context;
            _dieuHanhXeValidate = dieuHanhXeValidate;
        }

        //required
        private readonly string sortStringDefault = "NgayTao desc";
        private readonly List<string> searchCol = new List<string> { "TieuDe" };

        public DataTableResult<DieuHanhXeGridVo> GetAll(DieuHanhXeQueryParamViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;


            var query = _context.DieuHanhXes.AsNoTracking().Where(o => o.IsDeleted != true);

            if (queryParams.TrangThaiXuLy.Count() > 0)
            {
                query = query.Where(o => queryParams.TrangThaiXuLy.Contains(o.TrangThaiXuLy));
            }

            if (queryParams.NguoiTaoId.GetValueOrDefault() != 0)
            {
                query = query.Where(o => o.NguoiTaoId == queryParams.NguoiTaoId);
            }

            if (queryParams.LaiXeId.GetValueOrDefault() != 0)
            {
                query = query.Where(o => o.TaiXeId == queryParams.LaiXeId);
            }

            if (queryParams.NguoiDuyetId.GetValueOrDefault() != 0)
            {
                query = query.Where(o => o.NguoiDuyetId == queryParams.NguoiDuyetId);
            }

            if (queryParams.CoQuanId.GetValueOrDefault() != 0)
            {
                query = query.Where(o => o.CoQuanId == queryParams.CoQuanId);
            }

            if (queryParams.NguoiDuocGanDuyetId != 0)
            {
                query = query.Where(o => o.NguoiDuocGanDuyetId == queryParams.NguoiDuocGanDuyetId);
            }

            if (queryParams.IsNoiTinhSearch == true)
            {
                query = query.Where(o => o.IsNgoaiTinh != true);
            }

            if (queryParams.IsLichXeKhuVuc == true)
            {
                query = query.Where(o => o.IsKhuVuc == true);
            }
            else
            {
                query = query.Where(o => o.IsKhuVuc != true);
            }

            var data = query.Select(cv => new DieuHanhXeGridVo
            {
                Id = cv.Id,
                TieuDe = cv.TieuDe,
                NoiDi = cv.NoiDi,
                NoiDen = cv.NoiDen,
                TrangThaiXuLy = cv.TrangThaiXuLy,
                NgayTao = cv.NgayTao,
                NguoiTaoId = cv.NguoiTaoId,
                SoNguoiDi = cv.SoNguoiDi,
                LyDoTraPhieu = cv.LyDoTraPhieu,
                GhiChu = cv.GhiChu,
                DiaDiemDon = cv.DiaDiemDon,
                SearchStr = cv.SearchStr,
                LaiXeId = cv.TaiXeId,
                XeId = cv.XeId,
                NgayDuyet = cv.NgayDuyet,
                NguoiDuyetId = cv.NguoiDuyetId,
                ThoiGianBatDau = cv.ThoiGianBatDau,
                ThoiGianKetThuc = cv.ThoiGianKetThuc,
                IsNgoaiTinh = cv.IsNgoaiTinh,
                CuocHopId = cv.CuocHopId,
            });

            if (!string.IsNullOrEmpty(queryParams.searchDecode))
            {
                data = data.Where(queryParams.searchString);
            }

            var total = data.Count();
            var datas = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList();

            return new DataTableResult<DieuHanhXeGridVo>
            {
                rows = datas,
                total = total
            };
        }

        public dynamic CountChoToiDuyet()
        {
            var data = new CountMenuGridVo();

            try
            {
                var query = _context.DieuHanhXes.AsNoTracking().AsQueryable();

                data.DieuHanhXeChoToiDuyet = query.Where(o => o.TrangThaiXuLy == BienDungChung.STATUS_CHODUYET_NOI_TINH).Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return data;
        }

        public dynamic Them(DieuHanhXeViewModel dieuHanhXeViewModel)
        {
            var errors = _dieuHanhXeValidate.Validate(dieuHanhXeViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            var item = _iMapper.Map<DieuHanhXeViewModel, DieuHanhXe>(dieuHanhXeViewModel);
            item.TrangThaiXuLy = dieuHanhXeViewModel.IsNgoaiTinh == true ?
                BienDungChung.STATUS_CHODUYET_NGOAI_TINH : BienDungChung.STATUS_CHODUYET_NOI_TINH;

            item.NgayTao = DateTime.Now;
            item.NguoiTaoId = dieuHanhXeViewModel.NguoiTaoId;
            item.CoQuanId = dieuHanhXeViewModel.CoQuanId;
            var nguoiDuyetNgoaiTinhDuocGanId = item.NguoiDuocGanDuyetId;

            AddFileToEntity(item, dieuHanhXeViewModel.FileDinhKems);

            _context.DieuHanhXes.Add(item);
            _context.SaveChanges();

            if (dieuHanhXeViewModel.IsKhuVuc != true)
            {
                ThongBaoDenNguoiDuyetPhieuXe(item);
            }


            return new { Id = item.Id };
        }

        private static void ThongBaoDenNguoiDuyetPhieuXe(DieuHanhXe item)
        {
            var connStr = BienDungChung.ConnectDBStr;
            if (item.IsNgoaiTinh == true)
            {
                Task.Run(async () =>
                {
                    await ThongBaoAdoNetHelper.Them(new ThongBao
                    {
                        NoiDung = item.TieuDe + $" (Thời gian: {item.ThoiGianBatDau.Value.ToString("dd/MM/yyyy HH:mm")})",
                        NguoiNhanId = item.NguoiDuocGanDuyetId,
                        EnumLoaiThongBao = EnumLoaiThongBao.ThongBaoLichXeCanDuocDuyet,
                        DieuHanhXeId = item.Id,
                        ThongTinJson = JsonConvert.SerializeObject(new ThongBaoThongTinJson
                        {
                            IsNgoaiTinh = item.IsNgoaiTinh,
                        })
                    }, connStr);
                });
            }
            else
            {
                Task.Run(async () =>
                {
                    var matchedUsers = UserAdoNetHelper.GetAllUserDuyetXeNoiTinh();
                    if (matchedUsers.Count != 0)
                    {
                        foreach (var usr in matchedUsers)
                        {
                            await ThongBaoAdoNetHelper.Them(new ThongBao
                            {
                                NoiDung = item.TieuDe + $" (Thời gian: {item.ThoiGianBatDau.Value.ToString("dd/MM/yyyy HH:mm")})",
                                NguoiNhanId = usr.Id,
                                EnumLoaiThongBao = EnumLoaiThongBao.ThongBaoLichXeCanDuocDuyet,
                                DieuHanhXeId = item.Id,
                                ThongTinJson = JsonConvert.SerializeObject(new ThongBaoThongTinJson
                                {
                                    IsNgoaiTinh = item.IsNgoaiTinh,
                                })
                            }, connStr);
                        }
                    }
                });
            }
        }

        private static void AddFileToEntity(DieuHanhXe item, List<HttpPostedFileBase> FileDinhKems)
        {
            var uploadPath = System.Web.HttpContext.Current.Server.MapPath(BienDungChung.UploadsPath);
            foreach (var taiLieu in FileDinhKems)
            {
                if (taiLieu == null) continue;

                try
                {
                    item.TaiLieuDatXes.Add(new TaiLieuDatXe
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

        public dynamic CapNhat(DieuHanhXeViewModel dieuHanhXeViewModel)
        {
            var item = _context.DieuHanhXes.FirstOrDefault(o => o.Id == dieuHanhXeViewModel.Id);
            if (item == null || item.NguoiTaoId != dieuHanhXeViewModel.NguoiTaoId)
            {
                return new { Id = 0 };
            }

            if (item.TrangThaiXuLy == BienDungChung.STATUS_DADUYET && !dieuHanhXeViewModel.QuyenChinhSuaLichXeDaDuyet)
            {
                return new { Id = item.Id };
            }

            var errors = _dieuHanhXeValidate.Validate(dieuHanhXeViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            dieuHanhXeViewModel.IsKhuVuc = item.IsKhuVuc;

            _iMapper.Map(dieuHanhXeViewModel, item);

            item.NgaySua = DateTime.Now;
            item.NguoiSuaId = dieuHanhXeViewModel.NguoiTaoId;

            AddFileToEntity(item, dieuHanhXeViewModel.FileDinhKems);

            _context.SaveChanges();

            return new { Id = item.Id };
        }

        public dynamic DuyetPhieuNgoaiTinh(DieuHanhXeViewModel dieuHanhXeViewModel)
        {
            var item = _context.DieuHanhXes.FirstOrDefault(o => o.Id == dieuHanhXeViewModel.Id);
            if (item == null) throw new Exception("Điều hành xe đã bị xóa");
            var noiDung = item.TieuDe;

            item.TrangThaiXuLy = BienDungChung.STATUS_CUOCHOP_DADUYET;
            item.NguoiDuyetId = dieuHanhXeViewModel.NguoiDuyetId;
            item.NgayDuyet = DateTime.Now;

            _context.SaveChanges();

            var connStr = BienDungChung.ConnectDBStr;

            Task.Run(async () =>
            {
                //var vaiTroDieuPhieuIds = VaiTroAdoNetHelper.GetVaiTriIdCoQuyen(QuyenMoRong.CapXeChoLichXe.ToString());
                var vaiTroDieuPhieuIds = new List<int>();
                var users = CacheManagerHelper.GetUserCache();
                var matchedUsers = users.Where(u => u.NhomVaiTroIdInt.Intersect(vaiTroDieuPhieuIds).Any()).ToList();
                if (matchedUsers.Count != 0)
                {
                    foreach (var usr in matchedUsers)
                    {
                        await ThongBaoAdoNetHelper.Them(new ThongBao
                        {
                            NoiDung = noiDung + $" (Thời gian: {item.ThoiGianBatDau.Value.ToString("dd/MM/yyyy HH:mm")})",
                            NguoiNhanId = usr.Id,
                            EnumLoaiThongBao = EnumLoaiThongBao.ThongBaoLichXeMoiCanCapXe,
                            DieuHanhXeId = item.Id,
                        }, connStr);
                    }
                }
            });

            return new { Id = item.Id };
        }

        public dynamic CapXeVaTaiXe(DieuHanhXeViewModel dieuHanhXeViewModel)
        {
            dieuHanhXeViewModel.IsCapXeVaTaiXe = true;
            var errors = _dieuHanhXeValidate.Validate(dieuHanhXeViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            var item = _context.DieuHanhXes.FirstOrDefault(o => o.Id == dieuHanhXeViewModel.Id);
            if (item == null) throw new Exception("Lịch xe đã bị xóa");
            var noiDung = item.TieuDe;
            var nguoiTaoId = item.NguoiTaoId;

            item.TrangThaiXuLy = BienDungChung.STATUS_DACAPXE;
            item.TaiXeId = dieuHanhXeViewModel.TaiXeId;
            item.XeId = dieuHanhXeViewModel.XeId;

            _context.SaveChanges();

            var connStr = BienDungChung.ConnectDBStr;

            Task.Run(async () =>
            {
                var taiXe = "";
                var sdt = "";
                if (item.TaiXeId.GetValueOrDefault() != 0)
                {
                    var taiXeExist = CacheManagerHelper.GetUserByIdFromCache(item.TaiXeId.Value);
                    if (taiXeExist != null)
                    {
                        taiXe = taiXeExist.Name;
                        sdt = taiXeExist.Phone;
                    }
                }

                var xe = "";
                if (item.XeId.GetValueOrDefault() != 0)
                {
                    var xeExist = CacheManagerHelper.GetXeCacheByIdFromCache(item.XeId.Value);
                    if (xeExist != null)
                    {
                        xe = xeExist.BienSoXe;
                    }
                }

                await ThongBaoAdoNetHelper.Them(new ThongBao
                {
                    NoiDung = noiDung + $" (Thời gian: {item.ThoiGianBatDau.Value.ToString("dd/MM/yyyy HH:mm")}) - Lái xe: {taiXe} - SĐT: {sdt} - Xe: {xe}",
                    NguoiNhanId = nguoiTaoId,
                    EnumLoaiThongBao = EnumLoaiThongBao.ThongBaoLichXeDaDuocCapXe,
                    DieuHanhXeId = item.Id,
                }, connStr);

                await ThongBaoAdoNetHelper.Them(new ThongBao
                {
                    NoiDung = noiDung + $" (Thời gian: {item.ThoiGianBatDau.Value.ToString("dd/MM/yyyy HH:mm")})",
                    NguoiNhanId = dieuHanhXeViewModel.TaiXeId,
                    EnumLoaiThongBao = EnumLoaiThongBao.ThongBaoLichXeDaDuocCapXeChoLaiXe,
                    DieuHanhXeId = item.Id,
                }, connStr);
            });

            return new { Id = item.Id };
        }

        public dynamic HuyCapXe(DieuHanhXeViewModel dieuHanhXeViewModel)
        {
            var item = _context.DieuHanhXes.FirstOrDefault(o => o.Id == dieuHanhXeViewModel.Id);
            if (item == null) throw new Exception("Lịch xe đã bị xóa");

            var noiDung = item.TieuDe;
            var nguoiTaoId = item.NguoiTaoId;
            var taiXeCuId = item.TaiXeId;

            item.TrangThaiXuLy = BienDungChung.STATUS_DADUYET;
            _context.SaveChanges();

            Task.Run(async () =>
            {
                await ThongBaoAdoNetHelper.Them(new ThongBao
                {
                    NoiDung = noiDung + $" (Thời gian: {item.ThoiGianBatDau.Value.ToString("dd/MM/yyyy HH:mm")})",
                    NguoiNhanId = nguoiTaoId,
                    EnumLoaiThongBao = EnumLoaiThongBao.ThongBaoLichXeDaBiHuyCapXe,
                    DieuHanhXeId = item.Id,
                }, BienDungChung.ConnectDBStr);

                await ThongBaoAdoNetHelper.Them(new ThongBao
                {
                    NoiDung = noiDung + $" (Thời gian: {item.ThoiGianBatDau.Value.ToString("dd/MM/yyyy HH:mm")})",
                    NguoiNhanId = taiXeCuId,
                    EnumLoaiThongBao = EnumLoaiThongBao.ThongBaoLichXeDaBiHuyCapXeChoLaiXe,
                    DieuHanhXeId = item.Id,
                }, BienDungChung.ConnectDBStr);
            });

            return new { Id = 1 };
        }

        public dynamic TraPhieu(DieuHanhXeViewModel dieuHanhXeViewModel)
        {
            var errors = _dieuHanhXeValidate.Validate(dieuHanhXeViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            var item = _context.DieuHanhXes.FirstOrDefault(o => o.Id == dieuHanhXeViewModel.Id);
            if (item == null) throw new Exception("Điều hành xe đã bị xóa");
            var noiDung = item.TieuDe;
            var nguoiTaoId = item.NguoiTaoId;

            item.TrangThaiXuLy = dieuHanhXeViewModel.TrangThaiXuLy;
            item.LyDoTraPhieu = dieuHanhXeViewModel.LyDoTraPhieu;

            item.NgaySua = DateTime.Now;
            item.NguoiSuaId = dieuHanhXeViewModel.NguoiTaoId;
            item.NguoiDuyetId = dieuHanhXeViewModel.NguoiDuyetId;

            _context.SaveChanges();

            var connStr = BienDungChung.ConnectDBStr;
            Task.Run(async () =>
            {
                await ThongBaoAdoNetHelper.Them(new ThongBao
                {
                    NoiDung = noiDung + $" (Thời gian: {item.ThoiGianBatDau.Value.ToString("dd/MM/yyyy HH:mm")})",
                    NguoiNhanId = nguoiTaoId,
                    EnumLoaiThongBao = EnumLoaiThongBao.ThongBaoLichXeBiTuChoi,
                    DieuHanhXeId = item.Id,
                }, connStr);
            });

            return new { Id = item.Id };
        }
        public dynamic GuiLaiPhieu(DieuHanhXeViewModel dieuHanhXeViewModel)
        {
            var item = _context.DieuHanhXes.FirstOrDefault(o => o.Id == dieuHanhXeViewModel.Id);
            if (item == null) throw new Exception("Điều hành xe đã bị xóa");

            item.TrangThaiXuLy = item.IsNgoaiTinh == true ?
                BienDungChung.STATUS_CHODUYET_NGOAI_TINH : BienDungChung.STATUS_CHODUYET_NOI_TINH;

            item.NgaySua = DateTime.Now;
            item.NguoiSuaId = dieuHanhXeViewModel.NguoiTaoId;

            item.LogDieuHanhXes.Add(new LogDieuHanhXe
            {
                TrangThaiXuLy = dieuHanhXeViewModel.TrangThaiXuLy,
                NguoiTaoId = dieuHanhXeViewModel.NguoiTaoId,
                NgayTao = DateTime.Now
            });

            _context.SaveChanges();

            ThongBaoDenNguoiDuyetPhieuXe(item);

            return new { Id = item.Id };
        }

        public dynamic HuyBo(DieuHanhXeViewModel dieuHanhXeViewModel)
        {
            var errors = _dieuHanhXeValidate.Validate(dieuHanhXeViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            var item = _context.DieuHanhXes.FirstOrDefault(o => o.Id == dieuHanhXeViewModel.Id);
            if (item == null) throw new Exception("Điều hành xe đã bị xóa");
            item.TrangThaiXuLy = dieuHanhXeViewModel.TrangThaiXuLy;
            item.LyDoHuyBoXe = dieuHanhXeViewModel.LyDoHuyBoXe;



            var now = DateTime.Now;
            item.NgaySua = now;
            item.NguoiSuaId = dieuHanhXeViewModel.NguoiTaoId;

            item.LogDieuHanhXes.Add(new LogDieuHanhXe
            {
                TrangThaiXuLy = dieuHanhXeViewModel.TrangThaiXuLy,
                NguoiTaoId = dieuHanhXeViewModel.NguoiTaoId,
                NgayTao = now,
                LyDoTraPhieu = dieuHanhXeViewModel.LyDoHuyBoXe
            });

            _context.SaveChanges();
            return new { Id = item.Id };
        }

        public dynamic KetThuc(DieuHanhXeViewModel dieuHanhXeViewModel)
        {
            var errors = _dieuHanhXeValidate.Validate(dieuHanhXeViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            var item = _context.DieuHanhXes.FirstOrDefault(o => o.Id == dieuHanhXeViewModel.Id);
            if (item == null) throw new Exception("Điều hành xe đã bị xóa");
            item.TrangThaiXuLy = dieuHanhXeViewModel.TrangThaiXuLy;
            item.SoKmKhiVe = dieuHanhXeViewModel.SoKmKhiVe;



            var now = DateTime.Now;
            item.NgaySua = now;
            item.NguoiSuaId = dieuHanhXeViewModel.NguoiTaoId;

            item.LogDieuHanhXes.Add(new LogDieuHanhXe
            {
                TrangThaiXuLy = item.TrangThaiXuLy,
                NguoiTaoId = item.NguoiTaoId,
                NgayTao = now
            });

            _context.SaveChanges();
            return new { Id = item.Id };
        }

        public bool Xoa(int id, int nguoiTaoId)
        {
            var item = _context.DieuHanhXes.FirstOrDefault(o => o.Id == id);
            if (item != null && item.NguoiTaoId == nguoiTaoId)
            {
                item.IsDeleted = true;
                _context.SaveChanges();
            }

            return true;
        }


        public DieuHanhXeGridVo GetDieuHanhXeInfoFromId(int id)
        {
            var data = new DieuHanhXeGridVo();
            var item = _context.DieuHanhXes.AsNoTracking().FirstOrDefault(o => o.Id == id && o.IsDeleted != true);
            if (item != null)
            {
                data.Id = item.Id;
                data.TieuDe = item.TieuDe;
                data.SoNguoiDi = item.SoNguoiDi;
                data.NoiDi = item.NoiDi;
                data.NoiDen = item.NoiDen;
                data.GhiChu = item.GhiChu;
                data.DiaDiemDon = item.DiaDiemDon;
                data.ThoiGianBatDau = item.ThoiGianBatDau;
                data.ThoiGianKetThuc = item.ThoiGianKetThuc;
                data.NguoiDuocGanDuyetId = item.NguoiDuocGanDuyetId;
                data.IsNgoaiTinh = item.IsNgoaiTinh;
                data.CuocHopId = item.CuocHopId;
                data.LaiXeId = item.TaiXeId;
                data.XeId = item.XeId;
            }

            return data;
        }

        public dynamic GetLaiXeVaXeDeDuyetPhieu(string search)
        {
            var datas = new List<LaiXeVaXeGridVo>();
            try
            {

                var query = _context.Users.AsNoTracking().Where(o => o.IsTaiXe == true).Select(o => new LaiXeVaXeGridVo
                {
                    LaiXeId = o.Id,
                    LaiXe = o.Name,
                    //XeCuaLaiXeGridVos = o.Xes.Select(x => new XeCuaLaiXeGridVo
                    //{
                    //    XeId = x.Id,
                    //    BienSoXe = x.BienSoXe,
                    //    SoCho = x.SoCho,
                    //    XeDaDuocDuyets = x.DieuHanhXes.Where(p => p.TrangThaiXuLy == BienDungChung.STATUS_DADUYET
                    //        || p.TrangThaiXuLy == BienDungChung.STATUS_DANGCHAY).Select(cv => new XeDaDuocDuyet
                    //        {
                    //            ThoiGianBatDau = cv.ThoiGianBatDau,
                    //            ThoiGianKetThuc = cv.ThoiGianKetThuc
                    //        }).OrderBy(od => od.ThoiGianBatDau).ToList()
                    //}).ToList()
                });

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(o => o.LaiXe.ToLower().Contains(search.ToLower()));
                }

                return query.OrderBy(o => o.LaiXe).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return datas;
        }

        public dynamic XoaTaiLieuDatXe(int id, int nguoiTaoId)
        {
            if (id != 0)
            {
                var item = _context.TaiLieuDatXes.Include(o => o.DieuHanhXe).FirstOrDefault(o => o.Id == id);
                if (item != null && item.DieuHanhXe != null)
                {
                    if (item.DieuHanhXe.NguoiTaoId == nguoiTaoId)
                    {
                        item.IsDeleted = true;
                        _context.SaveChanges();
                    }
                }
            }

            return true;
        }

        public dynamic GetTaiLieuDieuHanhXe(int id)
        {
            return _context.TaiLieuDatXes.AsNoTracking().Where(o => o.DieuHanhXeId == id && o.IsDeleted != true)
                    .Select(o => new TaiLieuDatXeGridVo
                    {
                        Id = o.Id,
                        Name = o.Ten,
                        DuongDan = o.DuongDan
                    }).OrderByDescending(o => o.Id).ToList();
        }

        public dynamic LaiXeRanhLookup(DateTime tgbd, DateTime tgkt, int coQuanId = 0)
        {
            var data = new List<LookupViewModel>
                {
                    new LookupViewModel
                    {
                        id = 0,
                        text = "--- Chọn  ---"
                    }
                };

            var taiXeBan = _context.DieuHanhXes.AsNoTracking().Where(o => o.TrangThaiXuLy == BienDungChung.STATUS_DACAPXE
            && o.ThoiGianBatDau < tgkt && o.ThoiGianKetThuc > tgbd).Select(o => o.TaiXeId).ToList();

            var query = _context.Users.AsNoTracking().Where(o => o.IsTaiXe == true && o.IsDeleted != true && !taiXeBan.Contains(o.Id) && o.IsLaiXeKhuVuc != true);
            if (coQuanId != 0)
            {
                query = query.Where(o => o.CoQuanId == coQuanId);
            }

            var result = query.Select(cv => new LookupViewModel
            {
                id = cv.Id,
                text = cv.Name
            }).OrderBy(o => o.text);

            return new
            {
                Data = data.Concat(result).ToList()
            };
        }

        public dynamic XeRanhLookup(DateTime tgbd, DateTime tgkt, int coQuanId = 0)
        {
            var data = new List<LookupViewModel>
                {
                    new LookupViewModel
                    {
                        id = 0,
                        text = "--- Chọn  ---"
                    }
                };

            var xeBan = _context.DieuHanhXes.AsNoTracking().Where(o => o.TrangThaiXuLy == BienDungChung.STATUS_DACAPXE
            && o.ThoiGianBatDau < tgkt && o.ThoiGianKetThuc > tgbd).Select(o => o.XeId).ToList();

            var query = _context.Xes.AsNoTracking().Where(o => o.IsDeleted != true && !xeBan.Contains(o.Id)
            && o.IsKhuVuc != true);
            if (coQuanId != 0)
            {
                query = query.Where(o => o.CoQuanId == coQuanId);
            }

            var result = query.Select(cv => new LookupViewModel
            {
                id = cv.Id,
                text = cv.BienSoXe + " - Số chỗ: " + cv.SoCho
            }).OrderBy(o => o.text);

            return new
            {
                Data = data.Concat(result).ToList()
            };
        }

        public dynamic LaiXeKhuVucLookup(int coQuanId = 0)
        {
            var data = new List<LookupViewModel>
                {
                    new LookupViewModel
                    {
                        id = 0,
                        text = "--- Chọn  ---"
                    }
                };

            var query = _context.Users.AsNoTracking().Where(o => o.IsTaiXe == true && o.IsDeleted != true && o.IsLaiXeKhuVuc == true);
            if (coQuanId != 0)
            {
                query = query.Where(o => o.CoQuanId == coQuanId);
            }

            var result = query.Select(cv => new LookupViewModel
            {
                id = cv.Id,
                text = cv.Name
            }).OrderBy(o => o.text);

            return new
            {
                Data = data.Concat(result).ToList()
            };
        }

        public dynamic XeKhuVucLookup(int coQuanId = 0)
        {
            var data = new List<LookupViewModel>
                {
                    new LookupViewModel
                    {
                        id = 0,
                        text = "--- Chọn  ---"
                    }
                };

            var query = _context.Xes.AsNoTracking().Where(o => o.IsDeleted != true && o.IsKhuVuc == true);
            if (coQuanId != 0)
            {
                query = query.Where(o => o.CoQuanId == coQuanId);
            }

            var result = query.Select(cv => new LookupViewModel
            {
                id = cv.Id,
                text = cv.BienSoXe + " - Số chỗ: " + cv.SoCho
            }).OrderBy(o => o.text);

            return new
            {
                Data = data.Concat(result).ToList()
            };
        }

        public List<NgaySscheduler> GetAllLichXes(QueryParamViewModel queryParams)
        {
            var results = new List<DieuHanhXeGridVo>();
            var trangThai = new List<string> { BienDungChung.STATUS_DACAPXE };

            var query = _context.DieuHanhXes.AsNoTracking().Where(o => o.ThoiGianBatDau >= queryParams.LichHopTuNgay && o.IsDeleted != true
                    && o.ThoiGianBatDau <= queryParams.LichHopDenNgay).Where(x => trangThai.Contains(x.TrangThaiXuLy));

            if (queryParams.CoQuanId.GetValueOrDefault() != 0)
            {
                query = query.Where(o => o.CoQuanId == queryParams.CoQuanId);
            }

            if (queryParams.TaiXeId.GetValueOrDefault() != 0)
            {
                query = query.Where(o => o.TaiXeId == queryParams.TaiXeId);
            }

            if (queryParams.XeId.GetValueOrDefault() != 0)
            {
                query = query.Where(o => o.XeId == queryParams.XeId);
            }

            if (queryParams.CurrentUserId.GetValueOrDefault() != 0)
            {
                query = query.Where(o => o.NguoiTaoId == queryParams.CurrentUserId);
            }

            results = query.Select(cv => new DieuHanhXeGridVo
            {
                Id = cv.Id,
                TieuDe = cv.TieuDe,
                NoiDi = cv.NoiDi,
                NoiDen = cv.NoiDen,
                DiaDiemDon = cv.DiaDiemDon,
                LaiXeId = cv.TaiXeId,
                XeId = cv.XeId,
                ThoiGianBatDau = cv.ThoiGianBatDau,
                ThoiGianKetThuc = cv.ThoiGianKetThuc
            }).OrderBy(o => o.ThoiGianBatDau).ToList();

            if (queryParams.IsLaiXe || queryParams.TaiXeId != 0)
            {
                foreach (var item in results)
                {
                    item.GhiChu = "";
                }
            }

            var datas = new List<NgaySscheduler>();

            var ngay = queryParams.LichHopTuNgay;
            while (ngay <= queryParams.LichHopDenNgay)
            {
                // xử lý
                var item = new NgaySscheduler
                {
                    Ngay = ngay
                };

                item.XeSangs = results.Where(o => o.NgayDiStr == item.NgayStr && o.ThoiGianBatDau.GetValueOrDefault().Hour < 12).ToList();
                item.XeChieus = results.Where(o => o.NgayDiStr == item.NgayStr && o.ThoiGianBatDau.GetValueOrDefault().Hour >= 12).ToList();

                datas.Add(item);

                ngay = ngay.AddDays(1);
            }

            return datas;
        }

        public dynamic CapXeVaTaiXeChoLichhop(DieuHanhXeViewModel dieuHanhXeViewModel)
        {
            var errors = _dieuHanhXeValidate.ValidateCapXeChoLichHop(dieuHanhXeViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            var item = new DieuHanhXe();
            item.TrangThaiXuLy = BienDungChung.STATUS_DADUYET;
            item.NgayTao = DateTime.Now;
            item.NguoiTaoId = dieuHanhXeViewModel.NguoiTaoId;

            item.NoiDi = dieuHanhXeViewModel.NoiDi;
            item.NoiDen = dieuHanhXeViewModel.NoiDen;
            item.SoNguoiDi = dieuHanhXeViewModel.SoNguoiDi;
            item.CuocHopId = dieuHanhXeViewModel.CuocHopId;
            item.TaiXeId = dieuHanhXeViewModel.TaiXeId;
            item.XeId = dieuHanhXeViewModel.XeId;
            item.GhiChu = dieuHanhXeViewModel.GhiChu;

            var cuocHop = _context.CuocHops.AsNoTracking().FirstOrDefault(o => o.Id == dieuHanhXeViewModel.CuocHopId);
            if (cuocHop != null)
            {
                item.TieuDe = cuocHop.TenCuocHop;
                item.NgayDi = cuocHop.ThoiGianBatDau;
                item.ThoiGianBatDau = cuocHop.ThoiGianBatDau;
                item.ThoiGianKetThuc = cuocHop.ThoiGianKetThuc;
            }

            _context.DieuHanhXes.Add(item);
            _context.SaveChanges();

            Task.Run(async () =>
            {
                await ThongBaoAdoNetHelper.Them(new ThongBao
                {
                    NoiDung = cuocHop.TenCuocHop + $" (Thời gian: {cuocHop.ThoiGianBatDau.Value.ToString("dd/MM/yyyy HH:mm")})",
                    NguoiNhanId = dieuHanhXeViewModel.TaiXeId,
                    EnumLoaiThongBao = EnumLoaiThongBao.ThongBaoLichXeDaDuocCapXeChoLaiXe,
                    DieuHanhXeId = item.Id,
                }, BienDungChung.ConnectDBStr);
            });

            return new { Id = item.Id };
        }

        public dynamic ThemOrCapNhatDatXeKhuVuc(DieuHanhXeViewModel dieuHanhXeViewModel)
        {
            // Validate dữ liệu đầu vào
            var errors = _dieuHanhXeValidate.ValidateXeKhuVuc(dieuHanhXeViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            DieuHanhXe item;
            bool isUpdate = dieuHanhXeViewModel.Id > 0;
            DateTime? oldThoiGianBatDau = null;

            if (isUpdate)
            {
                // Xử lý cập nhật
                item = _context.DieuHanhXes.FirstOrDefault(o => o.Id == dieuHanhXeViewModel.Id);
                if (item == null)
                {
                    return new { Id = 0 };
                }
                else if (item.NguoiTaoId != dieuHanhXeViewModel.NguoiTaoId)
                {
                    return new { Id = 0 };
                }

                // Lưu thời gian bắt đầu cũ để so sánh
                oldThoiGianBatDau = item.ThoiGianBatDau;

                // Cập nhật thông tin
                _iMapper.Map(dieuHanhXeViewModel, item);
                item.NgaySua = DateTime.Now;
                item.NguoiSuaId = dieuHanhXeViewModel.NguoiTaoId;
            }
            else
            {
                // Xử lý thêm mới
                item = _iMapper.Map<DieuHanhXeViewModel, DieuHanhXe>(dieuHanhXeViewModel);
                item.TrangThaiXuLy = BienDungChung.STATUS_DACAPXE; // Đặt xe khu vực luôn là DaCapXe
                item.NgayTao = DateTime.Now;
                item.NguoiTaoId = dieuHanhXeViewModel.NguoiTaoId;
                item.CoQuanId = dieuHanhXeViewModel.CoQuanId;
                item.IsKhuVuc = true; // Đánh dấu là đặt xe khu vực

                _context.DieuHanhXes.Add(item);
            }

            // Thêm file đính kèm
            AddFileToEntity(item, dieuHanhXeViewModel.FileDinhKems);

            _context.SaveChanges();

            // Xử lý thông báo
            XuLyThongBaoDatXeKhuVuc(item, isUpdate, oldThoiGianBatDau);

            return new { Id = item.Id };
        }

        private void XuLyThongBaoDatXeKhuVuc(DieuHanhXe item, bool isUpdate, DateTime? oldThoiGianBatDau)
        {
            // Chỉ thông báo khi: thêm mới hoặc sửa có thay đổi thời gian bắt đầu
            bool shouldNotify = !isUpdate ||
                (oldThoiGianBatDau.HasValue &&
                 item.ThoiGianBatDau.HasValue &&
                 oldThoiGianBatDau.Value != item.ThoiGianBatDau.Value);

            if (shouldNotify && item.TaiXeId.HasValue)
            {
                ThongBaoDenLaiXeKhuVuc(item);
            }
        }

        private static void ThongBaoDenLaiXeKhuVuc(DieuHanhXe item)
        {
            if (!item.TaiXeId.HasValue) return;

            var connStr = BienDungChung.ConnectDBStr;
            Task.Run(async () =>
            {
                // Lấy thông tin tài xế
                var taiXe = "";
                if (item.TaiXeId.HasValue)
                {
                    var taiXeExist = CacheManagerHelper.GetUserByIdFromCache(item.TaiXeId.Value);
                    if (taiXeExist != null)
                    {
                        taiXe = taiXeExist.Name;
                    }
                }

                // Lấy thông tin xe
                var xe = "";
                if (item.XeId.HasValue)
                {
                    var xeExist = CacheManagerHelper.GetXeCacheByIdFromCache(item.XeId.Value);
                    if (xeExist != null)
                    {
                        xe = xeExist.BienSoXe;
                    }
                }

                // Tạo thông báo
                var noiDung = $"{item.TieuDe} (Thời gian: {item.ThoiGianBatDau.Value:dd/MM/yyyy HH:mm}) - Lái xe: {taiXe} - Xe: {xe}";

                await ThongBaoAdoNetHelper.Them(new ThongBao
                {
                    NoiDung = noiDung,
                    NguoiNhanId = item.TaiXeId.Value,
                    EnumLoaiThongBao = EnumLoaiThongBao.ThongBaoLichXeDaDuocCapXeChoLaiXe,
                    DieuHanhXeId = item.Id,
                }, connStr);
            });
        }



    }
}
