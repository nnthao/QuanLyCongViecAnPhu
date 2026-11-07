using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Dashboard.AdoNetHelper;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.CommonModel;
using Dashboard.Models.Entities;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace Dashboard.Repository
{
    public interface IGiaoViecRepository
    {
        DataTableResult<GiaoViecGridVo> GetAll(GiaoViecQueryViewModel queryParams);

        GiaoViecGridVo GetById(int id);
        List<GiaoViecThanhVienTienTrinhGridVo> GetByTienTrinhCapNhatCuaThanhVien(int giaoViecThanhVienId);
        List<GiaoViecThanhVienTienTrinhTheoGiaoViecThanhVienInfo> GetByListTienTrinhCapNhatCuaThanhVien(List<int> giaoViecThanhVienIds);
        DataTableResult<GiaoViecThanhVienGridVo> GetListCongViecCanXuLy(GiaoViecQueryViewModel queryParams);
        DataTableResult<GiaoViecThanhVienGridVo> GetListCongViecDaChuyenGiao(GiaoViecQueryViewModel queryParams);
        DataTableResult<GiaoViecThanhVienGridVo> GetListCongViecTuGiaoCanXuLy(GiaoViecQueryViewModel queryParams);
        List<GiaoViecNguoiNhanChuyenGiaoVo> GetListNguoiNhanChuyenGiaoByListIds(List<int> giaoViecThanhVienIds);
        dynamic GuiNhacNho(GuiNhacNhoViewModel viewModel);
        bool KhoiPhuc(int Id);

        dynamic ThemOrCapNhat(GiaoViecViewModel viewModel);
        dynamic ThemTienTrinhXuLyCuaThanhVien(GiaoViecThanhVienTienTrinhViewModel viewModel);
        dynamic HuyXacNhanHoanThanhCuaThanhVien(HuyXacNhanHoanThanhViewModel viewModel);
        bool Xoa(int Id);
        bool XoaCongViecTuGiao(int giaoViecId, int currentUserId);
        GiaoViecThanhVienTienTrinh LayThongTinTienDoCuoiCung(int giaoViecThanhVienId);
        dynamic XoaTienTrinhCuaThanhVien(GiaoViecThanhVienTienTrinhGridVo giaoViecThanhVienTienTrinhGridVo);
        int DemSoLuongCongViecDaGiao(int nguoiGiaoId);
        int DemSoLuongCongViecCanXuLy(int thanhVienId);
        int DemSoLuongCongViecDaChuyenGiaoChuaHoanThanh(int nguoiChuyenGiaoId);
        dynamic ChuyenGiaoViecTiepTuc(ChuyenGiaoViecTiepTucViewModel viewModel);
        GiaoViecThanhVien GetGiaoViecThanhVienById(int id);
        dynamic LayLaiCongViecChuyenGiao(int giaoViecThanhVienId, int currentUserId);
        List<GiaoViecThanhVienTheoGiaoViecInfo> GetListGiaoViecThanhVienByListGiaoViecIds(List<int> giaoViecIds);
        List<GoiYNoiDungItemVo> GetListGoiYNoiDung(GoiYNoiDungQueryViewModel queryParams);
        void LuuGoiYNoiDung(LuuGoiYNoiDungViewModel viewModel);
    }

    public class GiaoViecRepository : IGiaoViecRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<GiaoViecViewModel, GiaoViec>()
                .ForMember(x => x.Id, opt => opt.Ignore());
        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IGiaoViecValidate _validate;

        private static ILog log = log4net.LogManager.GetLogger(typeof(MvcApplication));
        public GiaoViecRepository(IContext context, IGiaoViecValidate validate)
        {
            _context = context;
            _validate = validate;
        }

        private readonly string sortStringDefault = "TenCongViec asc";
        private readonly List<string> searchCol = new List<string> { "TenCongViec", "NoiDung", "GhiChu", "NoiDungNhiemVu" };

        public DataTableResult<GiaoViecGridVo> GetAll(GiaoViecQueryViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.GiaoViecs.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
                query = query.Where(o => o.IsDeleted == true);
            else
                query = query.Where(o => o.IsDeleted != true);

            query = query.Where(o => o.IsTuGiao != true);

            if (queryParams.NguoiGiaoId.GetValueOrDefault() != 0)
            {
                query = query.Where(o => o.NguoiGiaoId == queryParams.NguoiGiaoId.Value);
            }

            // Thêm điều kiện tìm kiếm theo ngày tạo
            if (queryParams.TuNgay.HasValue)
            {
                query = query.Where(o => o.NgayTao >= queryParams.TuNgay.Value);
            }

            if (queryParams.DenNgay.HasValue)
            {
                query = query.Where(o => o.NgayTao <= queryParams.DenNgay.Value);
            }

            if (queryParams.TrangThaiSearch == 1) // Đã hoàn thành
            {
                query = query.Where(o => o.IsHoanThanh == true);
            }
            else if (queryParams.TrangThaiSearch == 2) // Chưa hoàn thành
            {
                query = query.Where(o => o.IsHoanThanh != true);
            }

            if (queryParams.NguoiDuocGiaoId.GetValueOrDefault() != 0)
            {
                query = query.Where(o => o.GiaoViecThanhViens.Any(tv => tv.IsDeleted != true && tv.ThanhVienId == queryParams.NguoiDuocGiaoId.Value));
            }

            var data = query.Select(x => new GiaoViecGridVo
            {
                Id = x.Id,
                TenCongViec = x.TenCongViec,
                NoiDung = x.NoiDung,
                GhiChu = x.GhiChu,
                NguoiGiaoId = x.NguoiGiaoId,
                NgayTao = x.NgayTao,
                IsHoanThanh = x.IsHoanThanh,
                IsTuGiao = x.IsTuGiao,
                IofficeNumber = x.IofficeNumber,
                VanBanId = x.VanBanId,
                GiaoViecThanhViens = x.GiaoViecThanhViens.Where(o => o.IsDeleted != true).Select(o => new GiaoViecThanhVienGridVo
                {
                    Id = o.Id,
                    ThanhVienId = o.ThanhVienId,
                    HanXuLy = o.HanXuLy,
                    NoiDungNhiemVu = o.NoiDungNhiemVu,
                    IsHoanThanh = o.IsHoanThanh,
                    SoGioUocTinh = o.SoGioUocTinh,
                    VaiTroXuLyId = o.VaiTroXuLyId,
                    NguoiTheoDoiChiDaoId = o.NguoiTheoDoiChiDaoId,
                    TrangThai = o.TrangThai,
                    KetQuaDanhGia = o.KetQuaDanhGia,
                    YKienDanhGia = o.YKienDanhGia,
                    NgayXacNhanHoanThanh = o.NgayXacNhanHoanThanh,
                    NguoiDanhGiaId = o.NguoiDanhGiaId,
                    NgayDanhGia = o.NgayDanhGia,
                    GiaoViecThanhVienTienTrinhs = o.GiaoViecThanhVienTienTrinhs.Select(s => new GiaoViecThanhVienTienTrinhGridVo
                    {
                        PhanTramHoanThanh = s.PhanTramHoanThanh,
                        NgayTao = s.NgayTao
                    }).ToList()
                }).ToList(),
                IsDeleted = x.IsDeleted
            }).Where(queryParams.searchString);

            var total = data.Count();
            return new DataTableResult<GiaoViecGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(GiaoViecViewModel viewModel)
        {
            var errors = _validate.Validate(viewModel);
            if (errors.Count > 0) return new { Errors = errors };

            if (viewModel.Id == 0)
            {
                var entity = _iMapper.Map<GiaoViecViewModel, GiaoViec>(viewModel);

                entity.NgayTao = DateTime.Now;
                entity.NguoiGiaoId = viewModel.CurrentUserId ?? 0;
                AddFileToEntity(entity, viewModel);
                AddVanBanLienQuanToEntity(entity, viewModel);
                AddThanhVienToEntity(entity, viewModel);

                var hanXuLy = viewModel.ThanhViens.Max(o => o.HanXuLy);
                entity.HanXuLy = hanXuLy;

                // Set IsTuGiao: true nếu chỉ có 1 thành viên và thành viên đó trùng với người giao
                var thanhViensActive = entity.GiaoViecThanhViens.Where(tv => tv.IsDeleted != true).ToList();
                if (thanhViensActive.Count == 1 && thanhViensActive[0].ThanhVienId == entity.NguoiGiaoId)
                {
                    entity.IsTuGiao = true;
                }
                else
                {
                    entity.IsTuGiao = false;
                }

                _context.GiaoViecs.Add(entity);
                _context.SaveChanges();

                // Gửi thông báo cho tất cả thành viên được giao việc
                Task.Run(() =>
                {
                    foreach (var thanhVien in entity.GiaoViecThanhViens.Where(tv => tv.IsDeleted != true))
                    {
                        var noiDungThongBao = $"Tên công việc: {entity.TenCongViec}";
                        if (thanhVien.HanXuLy.HasValue)
                        {
                            noiDungThongBao += $"\nHạn xử lý: {thanhVien.HanXuLy.Value:dd/MM/yyyy}";
                        }
                        if (!string.IsNullOrEmpty(thanhVien.NoiDungNhiemVu))
                        {
                            noiDungThongBao += $"\nNội dung: {thanhVien.NoiDungNhiemVu}";
                        }

                        ThongBaoAdoNetHelper.Them(new ThongBao
                        {
                            NoiDung = noiDungThongBao,
                            NguoiNhanId = thanhVien.ThanhVienId,
                            EnumLoaiThongBao = EnumLoaiThongBao.CongViecMoiCanXuLy,
                            ThongBaoInfoJson = JsonConvert.SerializeObject(new ThongBaoThongTinJson
                            {
                                GiaoViecId = entity.Id,
                                GiaoViecThanhVienId = thanhVien.Id
                            })
                        }, BienDungChung.ConnectDBStr);
                    }
                });

                // Tải file đính kèm ngầm từ iOffice nếu có
                if (viewModel.IofficeFiles != null && viewModel.IofficeFiles.Any())
                {
                    var newGiaoViecId = entity.Id;
                    var iofficeFiles = viewModel.IofficeFiles.ToList();
                    Task.Run(async () =>
                    {
                        await IofficeFileHelper.DownloadAndSaveFilesForGiaoViecAsync(newGiaoViecId, iofficeFiles);
                    });
                }
            }
            else
            {
                var oldEntity = _context.GiaoViecs.Include(o => o.GiaoViecThanhViens)
                    .Include(o => o.GiaoViecVanBanLienQuans)
                    .Include(o => o.GiaoViecFiles).FirstOrDefault(o => o.Id == viewModel.Id);
                if (oldEntity == null) throw new Exception("Giao việc đã bị xóa");

                var oldThanhViens = oldEntity.GiaoViecThanhViens.Where(tv => tv.IsDeleted != true)
                    .Select(o => new GiaoViecThanhVienGridVo
                    {
                        Id = o.Id,
                        ThanhVienId = o.ThanhVienId,
                        NoiDungNhiemVu = o.NoiDungNhiemVu,
                        HanXuLy = o.HanXuLy,
                        SoGioUocTinh = o.SoGioUocTinh,
                        VaiTroXuLyId = o.VaiTroXuLyId,
                    }).ToList();

                var newThanhViens = viewModel.ThanhViens
                   .Select(o => new GiaoViecThanhVienGridVo
                   {
                       Id = o.Id,
                       ThanhVienId = o.ThanhVienId,
                       NoiDungNhiemVu = o.NoiDungNhiemVu,
                       HanXuLy = o.HanXuLy,
                       SoGioUocTinh = o.SoGioUocTinh,
                       VaiTroXuLyId = o.VaiTroXuLyId,
                   }).ToList();

                _iMapper.Map(viewModel, oldEntity);
                AddFileToEntity(oldEntity, viewModel);
                AddVanBanLienQuanToEntity(oldEntity, viewModel);
                AddThanhVienToEntity(oldEntity, viewModel);

                var hanXuLy = viewModel.ThanhViens.Max(o => o.HanXuLy);
                oldEntity.HanXuLy = hanXuLy;

                // Set IsTuGiao: true nếu chỉ có 1 thành viên và thành viên đó trùng với người giao
                var thanhViensActive = oldEntity.GiaoViecThanhViens.Where(tv => tv.IsDeleted != true).ToList();
                if (thanhViensActive.Count == 1 && thanhViensActive[0].ThanhVienId == oldEntity.NguoiGiaoId)
                {
                    oldEntity.IsTuGiao = true;
                }
                else
                {
                    oldEntity.IsTuGiao = false;
                }

                var giaoViecInfo = new GiaoViecGridVo
                {
                    Id = oldEntity.Id,
                    TenCongViec = oldEntity.TenCongViec,
                };

                _context.SaveChanges();

                // Tải file đính kèm ngầm từ iOffice nếu có
                if (viewModel.IofficeFiles != null && viewModel.IofficeFiles.Any())
                {
                    var updatedGiaoViecId = oldEntity.Id;
                    var iofficeFiles = viewModel.IofficeFiles.ToList();
                    Task.Run(async () =>
                    {
                        await IofficeFileHelper.DownloadAndSaveFilesForGiaoViecAsync(updatedGiaoViecId, iofficeFiles);
                    });
                }

                XuLyThongBaoCapNhatCongViecAsync(giaoViecInfo, oldThanhViens, newThanhViens);
            }

            LuuGoiYNoiDung(new LuuGoiYNoiDungViewModel
            {
                NoiDung = viewModel.NoiDung,
                Loai = (int)LoaiGoiYNoiDungEnum.NoiDungCongViec,
                CurrentUserId = viewModel.CurrentUserId
            });

            if (viewModel.ThanhViens != null && viewModel.ThanhViens.Any())
            {
                foreach (var tv in viewModel.ThanhViens)
                {
                    if (!string.IsNullOrWhiteSpace(tv.NoiDungNhiemVu))
                    {
                        LuuGoiYNoiDung(new LuuGoiYNoiDungViewModel
                        {
                            NoiDung = tv.NoiDungNhiemVu,
                            Loai = (int)LoaiGoiYNoiDungEnum.NoiDungNhiemVu,
                            CurrentUserId = viewModel.CurrentUserId
                        });
                    }
                }
            }

            return true;
        }

        private int? LayNguoiTheoDoiChiDaoIdCuaThanhVien(int? thanhVienId)
        {
            if (!thanhVienId.HasValue || thanhVienId.Value == 0)
                return null;

            var user = CacheManagerHelper.GetUserByIdFromCache(thanhVienId.Value);
            var phongBanId = (user != null && user.PhongBanId.GetValueOrDefault() > 0)
                ? user.PhongBanId
                : _context.Users.Where(u => u.Id == thanhVienId.Value).Select(u => u.PhongBanId).FirstOrDefault();

            if (phongBanId.GetValueOrDefault() > 0)
            {
                var phongBan = CacheManagerHelper.GetPhongBanByIdFromCache(phongBanId.Value);
                if (phongBan != null && phongBan.NguoiTheoDoiChiDaoId.GetValueOrDefault() > 0)
                {
                    return phongBan.NguoiTheoDoiChiDaoId;
                }

                var nguoiTheoDoiChiDaoId = _context.PhongBans.Where(p => p.Id == phongBanId.Value).Select(p => p.NguoiTheoDoiChiDaoId).FirstOrDefault();
                if (nguoiTheoDoiChiDaoId.GetValueOrDefault() > 0)
                {
                    return nguoiTheoDoiChiDaoId;
                }
            }

            return null;
        }

        private void AddThanhVienToEntity(GiaoViec item, GiaoViecViewModel itemViewModel)
        {
            var oldIds = item.GiaoViecThanhViens.Where(x => x.IsDeleted != true)
                .Select(x => (int)x.ThanhVienId).ToList();

            var newIds = itemViewModel.ThanhViens.Select(x => (int)x.ThanhVienId).ToList();

            var removeIds = oldIds.Except(newIds).ToList();

            var addIds = newIds.Except(oldIds).ToList();

            foreach (var removeId in removeIds)
            {
                var removeItem = item.GiaoViecThanhViens.FirstOrDefault(x => x.ThanhVienId == removeId && x.IsDeleted != true);
                if (removeItem != null)
                {
                    removeItem.IsDeleted = true;
                    removeItem.NgayXoa = DateTime.Now;
                    removeItem.NguoiXoaId = itemViewModel.CurrentUserId;
                }
            }

            foreach (var thanhVienViewModel in itemViewModel.ThanhViens)
            {
                if (addIds.Contains((int)thanhVienViewModel.ThanhVienId))
                {
                    var addItem = new GiaoViecThanhVien
                    {
                        ThanhVienId = thanhVienViewModel.ThanhVienId,
                        NoiDungNhiemVu = thanhVienViewModel.NoiDungNhiemVu,
                        HanXuLy = thanhVienViewModel.HanXuLy,
                        SoGioUocTinh = thanhVienViewModel.SoGioUocTinh,
                        VaiTroXuLyId = thanhVienViewModel.VaiTroXuLyId,
                        NguoiTheoDoiChiDaoId = LayNguoiTheoDoiChiDaoIdCuaThanhVien(thanhVienViewModel.ThanhVienId)
                    };
                    item.GiaoViecThanhViens.Add(addItem);
                }
            }

            foreach (var thanhVienViewModel in itemViewModel.ThanhViens)
            {
                if (!addIds.Contains((int)thanhVienViewModel.ThanhVienId))
                {
                    var existingItem = item.GiaoViecThanhViens.FirstOrDefault(x => x.ThanhVienId == thanhVienViewModel.ThanhVienId && x.IsDeleted != true);
                    if (existingItem != null)
                    {
                        existingItem.NoiDungNhiemVu = thanhVienViewModel.NoiDungNhiemVu;
                        existingItem.HanXuLy = thanhVienViewModel.HanXuLy;
                        existingItem.SoGioUocTinh = thanhVienViewModel.SoGioUocTinh;
                        existingItem.VaiTroXuLyId = thanhVienViewModel.VaiTroXuLyId;
                        if (!existingItem.NguoiTheoDoiChiDaoId.HasValue || existingItem.NguoiTheoDoiChiDaoId.Value == 0)
                        {
                            existingItem.NguoiTheoDoiChiDaoId = LayNguoiTheoDoiChiDaoIdCuaThanhVien(thanhVienViewModel.ThanhVienId);
                        }
                    }
                }
            }
        }

        private void AddVanBanLienQuanToEntity(GiaoViec item, GiaoViecViewModel itemViewModel)
        {
            var oldIds = item.GiaoViecVanBanLienQuans.Where(x => x.IsDeleted != true)
                .Select(x => (int)x.VanBanPhapLuatId).ToList();

            var removeIds = oldIds.Except(itemViewModel.VanBanLienQuanIdInts).ToList();
            var addIds = itemViewModel.VanBanLienQuanIdInts.Except(oldIds).ToList();

            foreach (var removeId in removeIds)
            {
                var removeItem = item.GiaoViecVanBanLienQuans.FirstOrDefault(x => x.VanBanPhapLuatId == removeId);
                if (removeItem != null)
                {
                    removeItem.IsDeleted = true;
                    removeItem.NgayXoa = DateTime.Now;
                    removeItem.NguoiXoaId = itemViewModel.CurrentUserId;
                }
            }

            foreach (var addId in addIds)
            {
                var addItem = new GiaoViecVanBanLienQuan
                {
                    VanBanPhapLuatId = addId,
                };
                item.GiaoViecVanBanLienQuans.Add(addItem);
            }
        }

        private void AddFileToEntity(GiaoViec item, GiaoViecViewModel itemViewModel)
        {
            var uploadPath = System.Web.HttpContext.Current.Server.MapPath(BienDungChung.UploadsPath);
            //xoá file cũ
            foreach (var file in item.GiaoViecFiles)
            {
                if (!itemViewModel.TaiLieuDinhKemIdInts.Contains(file.Id))
                {
                    file.IsDeleted = true;
                    file.NguoiXoaId = itemViewModel.CurrentUserId;
                    file.NgayXoa = DateTime.Now;
                }
            }
            foreach (var taiLieu in itemViewModel.FileDinhKems)
            {
                if (taiLieu == null) continue;

                try
                {
                    item.GiaoViecFiles.Add(new GiaoViecFile
                    {
                        TenFile = taiLieu.FileName,
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

        public bool Xoa(int Id)
        {
            var item = _context.GiaoViecs
                .Include(o => o.GiaoViecThanhViens)
                .Include(o => o.GiaoViecFiles)
                .FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                // Lưu thông tin để gửi thông báo
                var tenCongViec = item.TenCongViec;
                var giaoViecId = item.Id;
                var danhSachThanhVien = item.GiaoViecThanhViens
                    .Where(tv => tv.IsDeleted != true && tv.ThanhVienId.HasValue)
                    .Select(tv => new { tv.Id, tv.ThanhVienId })
                    .ToList();

                // Lấy danh sách GiaoViecThanhVienId để load các bảng liên quan
                var giaoViecThanhVienIds = item.GiaoViecThanhViens
                    .Where(tv => tv.IsDeleted != true)
                    .Select(tv => tv.Id)
                    .ToList();

                // Load GiaoViecThanhVienTienTrinh và GiaoViecThanhVienTienTrinhFile
                var danhSachTienTrinh = _context.GiaoViecThanhVienTienTrinhs
                    .Where(tt => giaoViecThanhVienIds.Contains(tt.GiaoViecThanhVienId.Value) && tt.IsDeleted != true)
                    .Include(tt => tt.GiaoViecThanhVienTienTrinhFiles)
                    .ToList();

                // Xóa mềm GiaoViec
                item.IsDeleted = true;
                item.NgayXoa = DateTime.Now;

                // Xóa mềm GiaoViecFile
                foreach (var file in item.GiaoViecFiles.Where(f => f.IsDeleted != true))
                {
                    file.IsDeleted = true;
                    file.NgayXoa = DateTime.Now;
                }

                // Xóa mềm GiaoViecThanhVien
                foreach (var tv in item.GiaoViecThanhViens.Where(tv => tv.IsDeleted != true))
                {
                    tv.IsDeleted = true;
                    tv.NgayXoa = DateTime.Now;
                }

                // Xóa mềm GiaoViecThanhVienTienTrinh và GiaoViecThanhVienTienTrinhFile
                foreach (var tienTrinh in danhSachTienTrinh)
                {
                    tienTrinh.IsDeleted = true;

                    // Xóa mềm GiaoViecThanhVienTienTrinhFile
                    foreach (var file in tienTrinh.GiaoViecThanhVienTienTrinhFiles.Where(f => f.IsDeleted != true))
                    {
                        file.IsDeleted = true;
                    }
                }

                _context.SaveChanges();

                return true;
            }
            return false;
        }

        public bool XoaCongViecTuGiao(int giaoViecId, int currentUserId)
        {
            var item = _context.GiaoViecs
                .AsNoTracking()
                .FirstOrDefault(o => o.Id == giaoViecId
                    && o.NguoiGiaoId == currentUserId
                    && o.IsTuGiao == true
                    && o.IsDeleted != true
                    && o.GiaoViecThanhViens.Any(tv => tv.ThanhVienId == currentUserId && tv.IsDeleted != true));

            if (item == null)
            {
                return false;
            }

            return Xoa(giaoViecId);
        }

        public bool KhoiPhuc(int Id)
        {
            var item = _context.GiaoViecs.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                item.NgayXoa = null;

                foreach (var tv in item.GiaoViecVanBanLienQuans)
                {
                    tv.IsDeleted = false;
                }

                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public GiaoViecGridVo GetById(int id)
        {
            var item = _context.GiaoViecs.Include(o => o.GiaoViecThanhViens)
                .Include(o => o.GiaoViecVanBanLienQuans)
                .Include(o => o.GiaoViecFiles)
                .AsNoTracking().FirstOrDefault(o => o.Id == id && o.IsDeleted != true);
            if (item != null)
            {
                return new GiaoViecGridVo
                {
                    Id = item.Id,
                    TenCongViec = item.TenCongViec,
                    NoiDung = item.NoiDung,
                    GhiChu = item.GhiChu,
                    NguoiGiaoId = item.NguoiGiaoId,
                    NgayTao = item.NgayTao,
                    IsTuGiao = item.IsTuGiao,
                    IofficeNumber = item.IofficeNumber,
                    VanBanId = item.VanBanId,
                    GiaoViecFiles = item.GiaoViecFiles.Where(o => o.IsDeleted != true).Select(o => new GiaoViecFileGridVo
                    {
                        Id = o.Id,
                        TenFile = o.TenFile,
                        DuongDan = o.DuongDan
                    }).ToList(),
                    GiaoViecThanhViens = item.GiaoViecThanhViens.Where(o => o.IsDeleted != true).Select(o => new GiaoViecThanhVienGridVo
                    {
                        Id = o.Id,
                        ThanhVienId = o.ThanhVienId,
                        NoiDungNhiemVu = o.NoiDungNhiemVu,
                        HanXuLy = o.HanXuLy,
                        SoGioUocTinh = o.SoGioUocTinh,
                        VaiTroXuLyId = o.VaiTroXuLyId,
                        NguoiTheoDoiChiDaoId = o.NguoiTheoDoiChiDaoId,
                    }).ToList(),
                    GiaoViecVanBanLienQuans = item.GiaoViecVanBanLienQuans.Where(o => o.IsDeleted != true).Select(o => new GiaoViecVanBanLienQuanGridVo
                    {
                        Id = o.Id,
                        VanBanPhapLuatId = o.VanBanPhapLuatId,
                    }).ToList()
                };
            }
            return new GiaoViecGridVo();
        }

        public DataTableResult<GiaoViecThanhVienGridVo> GetListCongViecCanXuLy(GiaoViecQueryViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            // Lấy các công việc được giao cho người dùng, loại bỏ công việc tự giao
            var query = _context.GiaoViecThanhViens
                .Where(o => o.ThanhVienId == queryParams.NguoiDuocGiaoId
                    && (o.GiaoViec.IsDeleted == null || o.GiaoViec.IsDeleted == false))
                .AsNoTracking()
                .AsQueryable();

            if (queryParams.DaXoaSearch == true)
                query = query.Where(o => o.IsDeleted == true);
            else
                query = query.Where(o => o.IsDeleted != true);

            // Thêm điều kiện tìm kiếm theo trạng thái
            if (queryParams.TrangThaiSearch == 1) // Đã hoàn thành (Đạt)
            {
                query = query.Where(o => o.TrangThai == (int)EnumTrangThaiGiaoViec.HoanThanh || (o.IsHoanThanh == true && o.TrangThai == null));
            }
            else if (queryParams.TrangThaiSearch == 2) // Chưa hoàn thành
            {
                query = query.Where(o => o.TrangThai != (int)EnumTrangThaiGiaoViec.HoanThanh && (o.IsHoanThanh != true || o.TrangThai == (int)EnumTrangThaiGiaoViec.ChoDanhGia || o.TrangThai == (int)EnumTrangThaiGiaoViec.ChuaDat));
            }
            else if (queryParams.TrangThaiSearch == 3) // Chờ đánh giá
            {
                query = query.Where(o => o.TrangThai == (int)EnumTrangThaiGiaoViec.ChoDanhGia);
            }
            else if (queryParams.TrangThaiSearch == 4) // Chưa đạt
            {
                query = query.Where(o => o.TrangThai == (int)EnumTrangThaiGiaoViec.ChuaDat);
            }

            var data = query.Select(x => new GiaoViecThanhVienGridVo
            {
                Id = x.Id,
                GiaoViecId = x.GiaoViec.Id,
                TenCongViec = x.GiaoViec.TenCongViec,
                NoiDung = x.GiaoViec.NoiDung,
                GhiChu = x.GiaoViec.GhiChu,
                NgayTao = x.GiaoViec.NgayTao,
                NguoiGiaoId = x.GiaoViec.NguoiGiaoId,
                HanXuLy = x.HanXuLy,
                NoiDungNhiemVu = x.NoiDungNhiemVu,
                SoGioUocTinh = x.SoGioUocTinh,
                VaiTroXuLyId = x.VaiTroXuLyId,
                IsHoanThanh = x.IsHoanThanh,
                GiaoViecThanhVienChuyenGiaoId = x.GiaoViecThanhVienChuyenGiaoId,
                IsTuGiao = x.GiaoViec.IsTuGiao,
                NguoiTheoDoiChiDaoId = x.NguoiTheoDoiChiDaoId,
                TrangThai = x.TrangThai,
                NgayXacNhanHoanThanh = x.NgayXacNhanHoanThanh,
                NguoiDanhGiaId = x.NguoiDanhGiaId,
                NgayDanhGia = x.NgayDanhGia,
                KetQuaDanhGia = x.KetQuaDanhGia,
                YKienDanhGia = x.YKienDanhGia
            }).Where(queryParams.searchString);

            data = data.Where(o => o.IsTuGiao != true && o.NguoiGiaoId != queryParams.NguoiDuocGiaoId);

            // Thêm điều kiện tìm kiếm theo ngày tạo
            if (queryParams.TuNgay.HasValue)
            {
                data = data.Where(o => o.NgayTao >= queryParams.TuNgay.Value);
            }

            if (queryParams.DenNgay.HasValue)
            {
                data = data.Where(o => o.NgayTao <= queryParams.DenNgay.Value);
            }

            var total = data.Count();
            var rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList();
            foreach (var item in rows)
            {
                if (item.NguoiDanhGiaId.GetValueOrDefault() > 0)
                {
                    var u = CacheManagerHelper.GetUserByIdFromCache(item.NguoiDanhGiaId.Value);
                    if (u != null) item.NguoiDanhGia = u.Name;
                }
            }
            return new DataTableResult<GiaoViecThanhVienGridVo>
            {
                rows = rows,
                total = total
            };
        }

        public DataTableResult<GiaoViecThanhVienGridVo> GetListCongViecTuGiaoCanXuLy(GiaoViecQueryViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            // Lấy danh sách công việc tự giao:
            // - NguoiGiaoId = currentUserId
            // - IsTuGiao = true
            // - GiaoViec chưa bị xóa
            var query = _context.GiaoViecThanhViens
                .Where(o => o.ThanhVienId == queryParams.NguoiDuocGiaoId
                    && (o.GiaoViec.IsDeleted == null || o.GiaoViec.IsDeleted == false)
                    && o.GiaoViec.NguoiGiaoId == queryParams.NguoiDuocGiaoId
                    && o.GiaoViec.IsTuGiao == true)
                .AsNoTracking()
                .AsQueryable();

            if (queryParams.DaXoaSearch == true)
                query = query.Where(o => o.IsDeleted == true);
            else
                query = query.Where(o => o.IsDeleted != true);

            // Thêm điều kiện tìm kiếm theo trạng thái
            if (queryParams.TrangThaiSearch == 1) // Đã hoàn thành (Đạt)
            {
                query = query.Where(o => o.TrangThai == (int)EnumTrangThaiGiaoViec.HoanThanh || (o.IsHoanThanh == true && o.TrangThai == null));
            }
            else if (queryParams.TrangThaiSearch == 2) // Chưa hoàn thành
            {
                query = query.Where(o => o.TrangThai != (int)EnumTrangThaiGiaoViec.HoanThanh && (o.IsHoanThanh != true || o.TrangThai == (int)EnumTrangThaiGiaoViec.ChoDanhGia || o.TrangThai == (int)EnumTrangThaiGiaoViec.ChuaDat));
            }
            else if (queryParams.TrangThaiSearch == 3) // Chờ đánh giá
            {
                query = query.Where(o => o.TrangThai == (int)EnumTrangThaiGiaoViec.ChoDanhGia);
            }
            else if (queryParams.TrangThaiSearch == 4) // Chưa đạt
            {
                query = query.Where(o => o.TrangThai == (int)EnumTrangThaiGiaoViec.ChuaDat);
            }
            else if (queryParams.TrangThaiSearch == 3) // Hoàn thành trễ hạn
            {
                query = query.Where(o => o.IsHoanThanh == true
                    && o.NgayHoanThanh.HasValue
                    && o.HanXuLy.HasValue
                    && o.NgayHoanThanh.Value.Date > o.HanXuLy.Value.Date);
            }

            var data = query.Select(x => new GiaoViecThanhVienGridVo
            {
                Id = x.Id,
                GiaoViecId = x.GiaoViec.Id,
                TenCongViec = x.GiaoViec.TenCongViec,
                NoiDung = x.GiaoViec.NoiDung,
                GhiChu = x.GiaoViec.GhiChu,
                NgayTao = x.GiaoViec.NgayTao,
                NguoiGiaoId = x.GiaoViec.NguoiGiaoId,
                HanXuLy = x.HanXuLy,
                NoiDungNhiemVu = x.NoiDungNhiemVu,
                SoGioUocTinh = x.SoGioUocTinh,
                VaiTroXuLyId = x.VaiTroXuLyId,
                IsHoanThanh = x.IsHoanThanh,
                NgayHoanThanh = x.NgayHoanThanh,
                GiaoViecThanhVienChuyenGiaoId = x.GiaoViecThanhVienChuyenGiaoId,
                NguoiTheoDoiChiDaoId = x.NguoiTheoDoiChiDaoId
            }).Where(queryParams.searchString);

            // Thêm điều kiện tìm kiếm theo ngày tạo
            if (queryParams.TuNgay.HasValue)
            {
                data = data.Where(o => o.NgayTao >= queryParams.TuNgay.Value);
            }

            if (queryParams.DenNgay.HasValue)
            {
                data = data.Where(o => o.NgayTao <= queryParams.DenNgay.Value);
            }

            var total = data.Count();
            var rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList();
            foreach (var item in rows)
            {
                if (item.NguoiDanhGiaId.GetValueOrDefault() > 0)
                {
                    var u = CacheManagerHelper.GetUserByIdFromCache(item.NguoiDanhGiaId.Value);
                    if (u != null) item.NguoiDanhGia = u.Name;
                }
            }
            return new DataTableResult<GiaoViecThanhVienGridVo>
            {
                rows = rows,
                total = total
            };
        }

        public DataTableResult<GiaoViecThanhVienGridVo> GetListCongViecDaChuyenGiao(GiaoViecQueryViewModel queryParams)
        {
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            // Lấy danh sách các GiaoViecThanhVien mà người dùng hiện tại đã chuyển giao
            // Điều kiện: ThanhVienId = người đăng nhập và NgayChuyenGiao != null
            var query = _context.GiaoViecThanhViens
                .Where(o => o.ThanhVienId == queryParams.NguoiDuocGiaoId && o.NgayChuyenGiao != null)
                .AsNoTracking()
                .AsQueryable();

            var nguoiNhanChuyenGiaoId = queryParams.NguoiNhanChuyenGiaoId.GetValueOrDefault();
            if (nguoiNhanChuyenGiaoId > 0)
            {
                query = query.Where(o => o.GiaoViec.GiaoViecThanhViens.Any(tv => tv.GiaoViecThanhVienChuyenGiaoId == o.Id
                    && tv.IsDeleted != true
                    && tv.ThanhVienId == nguoiNhanChuyenGiaoId));
            }

            if (queryParams.TrangThaiSearch == 1) // Đã hoàn thành
            {
                if (nguoiNhanChuyenGiaoId > 0)
                {
                    query = query.Where(o => o.GiaoViec.GiaoViecThanhViens.Any(tv => tv.GiaoViecThanhVienChuyenGiaoId == o.Id
                        && tv.IsDeleted != true
                        && tv.ThanhVienId == nguoiNhanChuyenGiaoId
                        && tv.IsHoanThanh == true));
                }
                else
                {
                    query = query.Where(o => o.GiaoViec.GiaoViecThanhViens.Any(tv => tv.GiaoViecThanhVienChuyenGiaoId == o.Id && tv.IsDeleted != true)
                        && !o.GiaoViec.GiaoViecThanhViens.Any(tv => tv.GiaoViecThanhVienChuyenGiaoId == o.Id && tv.IsDeleted != true && tv.IsHoanThanh != true));
                }
            }
            else if (queryParams.TrangThaiSearch == 2) // Chưa hoàn thành
            {
                if (nguoiNhanChuyenGiaoId > 0)
                {
                    query = query.Where(o => o.GiaoViec.GiaoViecThanhViens.Any(tv => tv.GiaoViecThanhVienChuyenGiaoId == o.Id
                        && tv.IsDeleted != true
                        && tv.ThanhVienId == nguoiNhanChuyenGiaoId
                        && tv.IsHoanThanh != true));
                }
                else
                {
                    query = query.Where(o => o.GiaoViec.GiaoViecThanhViens.Any(tv => tv.GiaoViecThanhVienChuyenGiaoId == o.Id
                        && tv.IsDeleted != true
                        && tv.IsHoanThanh != true));
                }
            }

            var data = query.Select(x => new GiaoViecThanhVienGridVo
            {
                Id = x.Id,
                GiaoViecId = x.GiaoViec.Id,
                TenCongViec = x.GiaoViec.TenCongViec,
                NoiDung = x.GiaoViec.NoiDung,
                GhiChu = x.GiaoViec.GhiChu,
                NgayTao = x.GiaoViec.NgayTao,
                NguoiGiaoId = x.GiaoViec.NguoiGiaoId,
                HanXuLy = x.HanXuLy,
                NoiDungNhiemVu = x.NoiDungNhiemVu,
                SoGioUocTinh = x.SoGioUocTinh,
                VaiTroXuLyId = x.VaiTroXuLyId,
                IsHoanThanh = x.IsHoanThanh,
                ThanhVienId = x.ThanhVienId,
                NgayChuyenGiao = x.NgayChuyenGiao,
                NguoiTheoDoiChiDaoId = x.NguoiTheoDoiChiDaoId
            }).Where(queryParams.searchString);

            // Thêm điều kiện tìm kiếm theo ngày chuyển giao
            if (queryParams.TuNgay.HasValue)
            {
                data = data.Where(o => o.NgayChuyenGiao >= queryParams.TuNgay.Value);
            }

            if (queryParams.DenNgay.HasValue)
            {
                data = data.Where(o => o.NgayChuyenGiao <= queryParams.DenNgay.Value);
            }

            var total = data.Count();
            var rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList();
            foreach (var item in rows)
            {
                if (item.NguoiDanhGiaId.GetValueOrDefault() > 0)
                {
                    var u = CacheManagerHelper.GetUserByIdFromCache(item.NguoiDanhGiaId.Value);
                    if (u != null) item.NguoiDanhGia = u.Name;
                }
            }
            return new DataTableResult<GiaoViecThanhVienGridVo>
            {
                rows = rows,
                total = total
            };
        }

        public List<GiaoViecNguoiNhanChuyenGiaoVo> GetListNguoiNhanChuyenGiaoByListIds(List<int> giaoViecThanhVienIds)
        {
            if (giaoViecThanhVienIds == null || giaoViecThanhVienIds.Count == 0)
            {
                return new List<GiaoViecNguoiNhanChuyenGiaoVo>();
            }

            var nguoiNhans = _context.GiaoViecThanhViens
                .Where(o => o.GiaoViecThanhVienChuyenGiaoId.HasValue &&
                           giaoViecThanhVienIds.Contains(o.GiaoViecThanhVienChuyenGiaoId.Value) &&
                           o.IsDeleted != true)
                .Select(x => new GiaoViecThanhVienGridVo
                {
                    Id = x.Id,
                    GiaoViecId = x.GiaoViec.Id,
                    ThanhVienId = x.ThanhVienId,
                    HanXuLy = x.HanXuLy,
                    SoGioUocTinh = x.SoGioUocTinh,
                    VaiTroXuLyId = x.VaiTroXuLyId,
                    IsHoanThanh = x.IsHoanThanh,
                    GiaoViecThanhVienChuyenGiaoId = x.GiaoViecThanhVienChuyenGiaoId,
                    NguoiTheoDoiChiDaoId = x.NguoiTheoDoiChiDaoId
                })
                .ToList();

            var resultDict = new Dictionary<int, List<GiaoViecThanhVienGridVo>>();
            foreach (var item in nguoiNhans)
            {
                if (item.GiaoViecThanhVienChuyenGiaoId.HasValue)
                {
                    var giaoViecThanhVienId = item.GiaoViecThanhVienChuyenGiaoId.Value;
                    if (!resultDict.ContainsKey(giaoViecThanhVienId))
                    {
                        resultDict[giaoViecThanhVienId] = new List<GiaoViecThanhVienGridVo>();
                    }

                    resultDict[giaoViecThanhVienId].Add(item);
                }
            }

            var result = resultDict.Select(o => new GiaoViecNguoiNhanChuyenGiaoVo
            {
                GiaoViecThanhVienId = o.Key,
                NguoiNhans = o.Value
            }).ToList();

            return result;
        }

        public dynamic GuiNhacNho(GuiNhacNhoViewModel viewModel)
        {
            if (viewModel.ThanhVienList == null || viewModel.ThanhVienList.Count == 0)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "ThanhViens",
                            Description = "Vui lòng chọn ít nhất 1 thành viên để nhắc nhở."
                        }
                    }
                };
            }

            var giaoViec = _context.GiaoViecs.FirstOrDefault(o => o.Id == viewModel.GiaoViecId && o.IsDeleted != true);
            if (giaoViec == null)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "GiaoViecId",
                            Description = "Không tìm thấy công việc."
                        }
                    }
                };
            }

            Task.Run(async () =>
            {
                var giaoViecThanhVienIds = viewModel.ThanhVienList
                    .Where(o => o.GiaoViecThanhVienId > 0)
                    .Select(o => o.GiaoViecThanhVienId)
                    .Distinct()
                    .ToList();

                if (giaoViecThanhVienIds.Count == 0)
                {
                    return;
                }

                var danhSachIdHopLe = LayDanhSachIdGiaoViecThanhVienHopLe(giaoViecThanhVienIds);

                foreach (var thanhVien in viewModel.ThanhVienList)
                {
                    if (thanhVien.ThanhVienId.HasValue &&
                        danhSachIdHopLe.Contains(thanhVien.GiaoViecThanhVienId))
                    {
                        var noiDung = $"{viewModel.TenCongViec} - Sắp hết hạn ({thanhVien.HanXuLy})";

                        await ThongBaoAdoNetHelper.Them(new ThongBao
                        {
                            NoiDung = noiDung,
                            NguoiNhanId = thanhVien.ThanhVienId,
                            EnumLoaiThongBao = EnumLoaiThongBao.CongViecSapDenHan,
                            ThongBaoInfoJson = JsonConvert.SerializeObject(new ThongBaoThongTinJson
                            {
                                GiaoViecId = viewModel.GiaoViecId,
                                GiaoViecThanhVienId = thanhVien.GiaoViecThanhVienId
                            })
                        }, BienDungChung.ConnectDBStr);
                    }
                }
            });

            return true;
        }

        public dynamic ThemTienTrinhXuLyCuaThanhVien(GiaoViecThanhVienTienTrinhViewModel viewModel)
        {
            if (viewModel.GiaoViecThanhVienId.GetValueOrDefault() > 0)
            {
                var thanhVienCheck = _context.GiaoViecThanhViens.AsNoTracking()
                    .FirstOrDefault(o => o.Id == viewModel.GiaoViecThanhVienId && o.IsDeleted != true);

                if (thanhVienCheck != null && (thanhVienCheck.TrangThai == (int)EnumTrangThaiGiaoViec.HoanThanh || (thanhVienCheck.IsHoanThanh == true && thanhVienCheck.KetQuaDanhGia == (int)EnumKetQuaDanhGia.Dat)))
                {
                    return new
                    {
                        Errors = new List<ValidateError>
                        {
                            new ValidateError
                            {
                                Id = "TrangThai",
                                Description = "Nhiệm vụ đã được đánh giá Đạt và hoàn thành, không thể cập nhật thêm tiến trình."
                            }
                        }
                    };
                }
            }
            var hasNoiDung = !string.IsNullOrWhiteSpace(viewModel.NoiDungXuLy);
            var hasFile = viewModel.FileDinhKems != null && viewModel.FileDinhKems.Any(f => f != null && f.ContentLength > 0);

            if (viewModel.GiaoViecThanhVienId.GetValueOrDefault() > 0)
            {
                var thanhVien = _context.GiaoViecThanhViens.AsNoTracking()
                    .FirstOrDefault(o => o.Id == viewModel.GiaoViecThanhVienId && o.IsDeleted != true);

                if (thanhVien != null && thanhVien.VaiTroXuLyId.GetValueOrDefault() > 0)
                {
                    var vaiTroXuLy = CacheManagerHelper.GetVaiTroXuLyByIdFromCache(thanhVien.VaiTroXuLyId.Value);
                    if (vaiTroXuLy != null && vaiTroXuLy.IsCanDinhKemFileKhiXacNhanHoanThanh == true && !hasFile)
                    {
                        return new
                        {
                            Errors = new List<ValidateError>
                            {
                                new ValidateError
                                {
                                    Id = "NoiDungXuLy",
                                    Description = "Vai trò xử lý của bạn bắt buộc phải đính kèm file khi xác nhận hoàn thành công việc."
                                }
                            }
                        };
                    }
                }
            }

            if (!hasNoiDung && !hasFile)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "NoiDungXuLy",
                            Description = "Vui lòng nhập nội dung xử lý hoặc đính kèm file để xác nhận hoàn thành"
                        }
                    }
                };
            }

            var tienTrinh = new GiaoViecThanhVienTienTrinh
            {
                GiaoViecThanhVienId = viewModel.GiaoViecThanhVienId,
                NoiDungXuLy = viewModel.NoiDungXuLy,
                PhanTramHoanThanh = 100,
                NgayTao = DateTime.Now
            };

            var uploadPath = System.Web.HttpContext.Current.Server.MapPath(BienDungChung.UploadsPath);
            if (viewModel.FileDinhKems != null)
            {
                foreach (var taiLieu in viewModel.FileDinhKems)
                {
                    if (taiLieu == null) continue;

                    try
                    {
                        var files = new GiaoViecThanhVienTienTrinhFile
                        {
                            TenFile = taiLieu.FileName,
                            DuongDan = FileHelper.SaveHttpFilesToUploads(uploadPath, taiLieu),
                            NgayTao = DateTime.Now
                        };

                        tienTrinh.GiaoViecThanhVienTienTrinhFiles.Add(files);
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

            _context.GiaoViecThanhVienTienTrinhs.Add(tienTrinh);
            _context.SaveChanges();

            CapNhatTrangThaiHoanThanhCuaGiaoViecThanhVien((int)viewModel.GiaoViecThanhVienId);

            return true;
        }

        public dynamic HuyXacNhanHoanThanhCuaThanhVien(HuyXacNhanHoanThanhViewModel viewModel)
        {
            var giaoviecThanhVien = _context.GiaoViecThanhViens.FirstOrDefault(o => o.Id == viewModel.GiaoViecThanhVienId && o.IsDeleted != true);
            if (giaoviecThanhVien == null)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "GiaoViecThanhVienId",
                            Description = "Không tìm thấy dữ liệu công việc thành viên"
                        }
                    }
                };
            }

            if (giaoviecThanhVien.ThanhVienId != viewModel.CurrentUserId)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "Permission",
                            Description = "Bạn không có quyền hủy xác nhận công việc này"
                        }
                    }
                };
            }

            if (giaoviecThanhVien.TrangThai == (int)EnumTrangThaiGiaoViec.HoanThanh || (giaoviecThanhVien.IsHoanThanh == true && giaoviecThanhVien.KetQuaDanhGia == (int)EnumKetQuaDanhGia.Dat))
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "TrangThai",
                            Description = "Nhiệm vụ đã được đánh giá Đạt và hoàn thành, không thể hủy xác nhận hoàn thành."
                        }
                    }
                };
            }

            var tienTrinhs = _context.GiaoViecThanhVienTienTrinhs
                .Where(o => o.GiaoViecThanhVienId == viewModel.GiaoViecThanhVienId && o.IsDeleted != true)
                .ToList();

            foreach (var tt in tienTrinhs)
            {
                tt.IsDeleted = true;
            }

            giaoviecThanhVien.TrangThai = (int)EnumTrangThaiGiaoViec.ChuaHoanThanh;
            giaoviecThanhVien.NgayXacNhanHoanThanh = null;
            giaoviecThanhVien.IsHoanThanh = false;
            giaoviecThanhVien.NgayHoanThanh = null;

            _context.SaveChanges();

            Task.Run(async () =>
            {
                await CapNhatTrangThaiHoanThanhCongViecToanBo(viewModel.GiaoViecThanhVienId);
            });

            return true;
        }

        private async Task CapNhatTrangThaiHoanThanhCongViecToanBo(int giaoViecThanhVienId)
        {
            var dsGiaoViecThanhViens = LayDanhSachGiaoViecThanhVienAdoNet(giaoViecThanhVienId);
            if (dsGiaoViecThanhViens.Count != 0)
            {
                var giaoViecId = dsGiaoViecThanhViens[0].GiaoViecId;

                var isHoanThanhTotal = dsGiaoViecThanhViens.Where(o => o.IsHoanThanh == true).Count();
                var isHoanThanhGiaoViec = isHoanThanhTotal == dsGiaoViecThanhViens.Count;

                using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
                {
                    await conn.OpenAsync();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE GiaoViec SET IsHoanThanh = @IsHoanThanh WHERE Id = @Id";
                        cmd.Parameters.AddWithValue("@IsHoanThanh", isHoanThanhGiaoViec ? 1 : 0);
                        cmd.Parameters.AddWithValue("@Id", giaoViecId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
        }

        public async Task<bool> CapNhatIsHoanThanhGiaoViecAsync(int giaoViecId, bool isHoanThanh)
        {
            using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE GiaoViec SET IsHoanThanh = @IsHoanThanh WHERE Id = @Id";
                    cmd.Parameters.AddWithValue("@IsHoanThanh", isHoanThanh ? 1 : 0);
                    cmd.Parameters.AddWithValue("@Id", giaoViecId);
                    var affected = await cmd.ExecuteNonQueryAsync();
                    return affected > 0;
                }
            }
        }

        public GiaoViecThanhVienTienTrinh LayThongTinTienDoCuoiCung(int giaoViecThanhVienId)
        {
            GiaoViecThanhVienTienTrinh result = null;

            using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
            {
                conn.Open();

                try
                {
                    string sql = @"SELECT TOP 1 
                                    Id, 
                                    GiaoViecThanhVienId, 
                                    NoiDungXuLy, 
                                    NgayTao, 
                                    IsDeleted, 
                                    PhanTramHoanThanh
                                  FROM GiaoViecThanhVienTienTrinh 
                                  WHERE GiaoViecThanhVienId = @GiaoViecThanhVienId 
                                    AND (IsDeleted IS NULL OR IsDeleted = 0)
                                  ORDER BY NgayTao DESC";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@GiaoViecThanhVienId", giaoViecThanhVienId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result = new GiaoViecThanhVienTienTrinh
                                {
                                    Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id"),
                                    GiaoViecThanhVienId = SqlServeHelper.LayGiaTriCuaCotLaInteger(reader, "GiaoViecThanhVienId"),
                                    NoiDungXuLy = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "NoiDungXuLy"),
                                    NgayTao = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "NgayTao"),
                                    IsDeleted = SqlServeHelper.LayGiaTriCuaCotLaBoolean(reader, "IsDeleted"),
                                    PhanTramHoanThanh = SqlServeHelper.LayGiaTriCuaCotLaInteger(reader, "PhanTramHoanThanh")
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in LayThongTinTienDoCuoiCung: {ex.Message}");
                }
            }

            return result;
        }

        public List<GiaoViecThanhVien> LayDanhSachGiaoViecThanhVienAdoNet(int giaoViecThanhVienId)
        {
            var result = new List<GiaoViecThanhVien>();

            using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
            {
                conn.Open();

                try
                {
                    string sql = @"SELECT  Id, IsHoanThanh, GiaoViecId
                                  FROM GiaoViecThanhVien 
                                  WHERE GiaoViecId = (SELECT TOP 1 GiaoViecId 
                                        FROM GiaoViecThanhVien WHERE Id = @GiaoViecThanhVienId)";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@GiaoViecThanhVienId", giaoViecThanhVienId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new GiaoViecThanhVien
                                {
                                    Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id"),
                                    IsHoanThanh = SqlServeHelper.LayGiaTriCuaCotLaBoolean(reader, "IsHoanThanh"),
                                    GiaoViecId = SqlServeHelper.LayGiaTriCuaCotLaInteger(reader, "GiaoViecId")
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in LayThongTinTienDoCuoiCung: {ex.Message}");
                }
            }

            return result;
        }

        private List<int> LayDanhSachIdGiaoViecThanhVienHopLe(List<int> giaoViecThanhVienIds)
        {
            var result = new List<int>();

            if (giaoViecThanhVienIds == null || giaoViecThanhVienIds.Count == 0)
            {
                return result;
            }

            using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
            {
                conn.Open();
                try
                {
                    var idsString = string.Join(",", giaoViecThanhVienIds);
                    string sql = $@"SELECT Id 
                                   FROM GiaoViecThanhVien 
                                   WHERE Id IN ({idsString}) AND IsDeleted = 0";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id");
                                if (id > 0)
                                {
                                    result.Add(id);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in LayDanhSachIdGiaoViecThanhVienHopLe: {ex.Message}");
                }
            }

            return result;
        }

        public List<GiaoViecThanhVienTienTrinhGridVo> GetByTienTrinhCapNhatCuaThanhVien(int giaoViecThanhVienId)
        {
            return _context.GiaoViecThanhVienTienTrinhs.AsNoTracking()
                 .Where(o => o.GiaoViecThanhVienId == giaoViecThanhVienId && o.IsDeleted != true)
                 .Select(o => new GiaoViecThanhVienTienTrinhGridVo
                 {
                     Id = o.Id,
                     NoiDungXuLy = o.NoiDungXuLy,
                     PhanTramHoanThanh = o.PhanTramHoanThanh,
                     NgayTao = o.NgayTao,
                     Files = o.GiaoViecThanhVienTienTrinhFiles.Where(f => f.IsDeleted != true).Select(f => new GiaoViecThanhVienTienTrinhFileGridVo
                     {
                         Id = f.Id,
                         TenFile = f.TenFile,
                         DuongDan = f.DuongDan
                     }).ToList(),
                     ThanhVienId = o.GiaoViecThanhVien.ThanhVienId
                 }).OrderByDescending(o => o.NgayTao).ToList();
        }

        public List<GiaoViecThanhVienTienTrinhTheoGiaoViecThanhVienInfo> GetByListTienTrinhCapNhatCuaThanhVien(List<int> giaoViecThanhVienIds)
        {
            var result = new List<GiaoViecThanhVienTienTrinhTheoGiaoViecThanhVienInfo>();

            if (giaoViecThanhVienIds == null || giaoViecThanhVienIds.Count == 0)
            {
                return result;
            }

            var tienTrinhs = _context.GiaoViecThanhVienTienTrinhs.AsNoTracking()
                .Where(o => giaoViecThanhVienIds.Contains(o.GiaoViecThanhVienId.Value) && o.IsDeleted != true)
                .Select(o => new GiaoViecThanhVienTienTrinhGridVo
                {
                    Id = o.Id,
                    GiaoViecThanhVienId = o.GiaoViecThanhVienId,
                    NoiDungXuLy = o.NoiDungXuLy,
                    PhanTramHoanThanh = o.PhanTramHoanThanh,
                    NgayTao = o.NgayTao,
                    Files = o.GiaoViecThanhVienTienTrinhFiles.Where(f => f.IsDeleted != true).Select(f => new GiaoViecThanhVienTienTrinhFileGridVo
                    {
                         Id = f.Id,
                         TenFile = f.TenFile,
                         DuongDan = f.DuongDan
                    }).ToList(),
                    ThanhVienId = o.GiaoViecThanhVien.ThanhVienId
                })
                .OrderByDescending(o => o.NgayTao)
                .ToList();

            foreach (var tienTrinh in tienTrinhs)
            {
                if (!tienTrinh.GiaoViecThanhVienId.HasValue)
                {
                    continue;
                }

                var giaoViecThanhVienId = tienTrinh.GiaoViecThanhVienId.Value;
                var tienTrinhInfo = result.FirstOrDefault(o => o.GiaoViecThanhVienId == giaoViecThanhVienId);
                if (tienTrinhInfo == null)
                {
                    tienTrinhInfo = new GiaoViecThanhVienTienTrinhTheoGiaoViecThanhVienInfo
                    {
                        GiaoViecThanhVienId = giaoViecThanhVienId
                    };
                    result.Add(tienTrinhInfo);
                }

                tienTrinhInfo.TienTrinhs.Add(tienTrinh);
            }

            return result;
        }

        public dynamic XoaTienTrinhCuaThanhVien(GiaoViecThanhVienTienTrinhGridVo giaoViecThanhVienTienTrinhGridVo)
        {
            var item = _context.GiaoViecThanhVienTienTrinhs.Include(o => o.GiaoViecThanhVien)
                 .FirstOrDefault(o => o.Id == giaoViecThanhVienTienTrinhGridVo.Id && o.IsDeleted != true);
            if (item != null)
            {
                if (item.GiaoViecThanhVien != null)
                {
                    var giaoViecThanhVienId = item.GiaoViecThanhVien.Id;

                    var thanhVienId = item.GiaoViecThanhVien.ThanhVienId;
                    if (thanhVienId == giaoViecThanhVienTienTrinhGridVo.CurrentUserId)
                    {
                        item.IsDeleted = true;

                        _context.SaveChanges();
                    }

                    CapNhatTrangThaiHoanThanhCuaGiaoViecThanhVien(giaoViecThanhVienId);

                }
            }
            return true;
        }

        private void CapNhatTrangThaiHoanThanhCuaGiaoViecThanhVien(int giaoViecThanhVienId)
        {
            var itemLastHoanThanh = _context.GiaoViecThanhVienTienTrinhs
                .AsNoTracking()
                .Where(o => o.GiaoViecThanhVienId == giaoViecThanhVienId && o.IsDeleted != true)
                .OrderByDescending(o => o.NgayTao)
                .FirstOrDefault();

            var giaoviecThanhVien = _context.GiaoViecThanhViens
                .Include(o => o.GiaoViec)
                .FirstOrDefault(o => o.Id == giaoViecThanhVienId);

            if (giaoviecThanhVien != null)
            {
                int phanTram = itemLastHoanThanh != null ? itemLastHoanThanh.PhanTramHoanThanh.GetValueOrDefault() : 0;
                if (phanTram >= 100)
                {
                    giaoviecThanhVien.TrangThai = (int)EnumTrangThaiGiaoViec.ChoDanhGia;
                    giaoviecThanhVien.NgayXacNhanHoanThanh = itemLastHoanThanh != null ? itemLastHoanThanh.NgayTao : DateTime.Now;
                    giaoviecThanhVien.IsHoanThanh = false;

                    _context.SaveChanges();

                    if (giaoviecThanhVien.GiaoViec != null && giaoviecThanhVien.GiaoViec.NguoiGiaoId.GetValueOrDefault() > 0)
                    {
                        var tenNguoiNhan = giaoviecThanhVien.ThanhVienId.HasValue ? CacheManagerHelper.GetUserByIdFromCache(giaoviecThanhVien.ThanhVienId.Value)?.Name : "Thành viên";
                        ThongBaoAdoNetHelper.Them(new ThongBao
                        {
                            NoiDung = $"{tenNguoiNhan} đã xác nhận hoàn thành nhiệm vụ \"{giaoviecThanhVien.GiaoViec.TenCongViec}\". Vui lòng vào mục \"Đánh giá nhiệm vụ\" để đánh giá.",
                            NguoiNhanId = giaoviecThanhVien.GiaoViec.NguoiGiaoId.Value,
                            EnumLoaiThongBao = EnumLoaiThongBao.CongViecMoiCanXuLy,
                            NgayTao = DateTime.Now,
                            IsDaXem = false
                        }, BienDungChung.ConnectDBStr);
                    }
                }
                else
                {
                    giaoviecThanhVien.TrangThai = (int)EnumTrangThaiGiaoViec.ChuaHoanThanh;
                    giaoviecThanhVien.NgayXacNhanHoanThanh = null;
                    giaoviecThanhVien.IsHoanThanh = false;
                    giaoviecThanhVien.NgayHoanThanh = null;
                    _context.SaveChanges();
                }

                Task.Run(async () =>
                {
                    await CapNhatTrangThaiHoanThanhCongViecToanBo(giaoViecThanhVienId);
                });
            }
        }

        private static void XuLyThongBaoCapNhatCongViecAsync(GiaoViecGridVo giaoViec, List<GiaoViecThanhVienGridVo> oldThanhViens, List<GiaoViecThanhVienGridVo> newThanhViens)
        {
            try
            {
                // Lấy danh sách ID thành viên cũ và mới
                var oldThanhVienIds = oldThanhViens.Select(tv => (int)tv.ThanhVienId).ToList();
                var newThanhVienIds = newThanhViens.Select(tv => (int)tv.ThanhVienId).ToList();

                // Gửi thông báo cho thành viên được thêm mới
                var thanhVienMoi = newThanhVienIds.Except(oldThanhVienIds).ToList();
                foreach (var thanhVienId in thanhVienMoi)
                {
                    var thanhVien = newThanhViens.FirstOrDefault(tv => (int)tv.ThanhVienId == thanhVienId);
                    if (thanhVien != null)
                    {
                        var noiDungThongBao = $"Tên công việc: {giaoViec.TenCongViec}";
                        if (thanhVien.HanXuLy.HasValue)
                        {
                            noiDungThongBao += $"\nHạn xử lý: {thanhVien.HanXuLy.Value:dd/MM/yyyy}";
                        }
                        if (!string.IsNullOrEmpty(thanhVien.NoiDungNhiemVu))
                        {
                            noiDungThongBao += $"\nNội dung: {thanhVien.NoiDungNhiemVu}";
                        }

                        ThongBaoAdoNetHelper.Them(new ThongBao
                        {
                            NoiDung = noiDungThongBao,
                            NguoiNhanId = thanhVienId,
                            EnumLoaiThongBao = EnumLoaiThongBao.CongViecMoiCanXuLy,
                            ThongBaoInfoJson = JsonConvert.SerializeObject(new ThongBaoThongTinJson
                            {
                                GiaoViecId = giaoViec.Id,
                                GiaoViecThanhVienId = thanhVien.Id
                            })
                        }, BienDungChung.ConnectDBStr);
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error(exception.Message + Environment.NewLine + exception.StackTrace);
            }
        }

        private static void XuLyThongBaoChuyenGiaoViecAsync(GiaoViecGridVo giaoViec, List<GiaoViecThanhVienGridVo> newThanhViens)
        {
            try
            {
                // Chỉ gửi thông báo cho các thành viên mới được chuyển giao
                foreach (var thanhVien in newThanhViens)
                {
                    if (thanhVien.ThanhVienId.HasValue)
                    {
                        var noiDungThongBao = $"Tên công việc: {giaoViec.TenCongViec}";
                        if (thanhVien.HanXuLy.HasValue)
                        {
                            noiDungThongBao += $"\nHạn xử lý: {thanhVien.HanXuLy.Value:dd/MM/yyyy}";
                        }
                        if (!string.IsNullOrEmpty(thanhVien.NoiDungNhiemVu))
                        {
                            noiDungThongBao += $"\nNội dung: {thanhVien.NoiDungNhiemVu}";
                        }

                        ThongBaoAdoNetHelper.Them(new ThongBao
                        {
                            NoiDung = noiDungThongBao,
                            NguoiNhanId = (int)thanhVien.ThanhVienId,
                            EnumLoaiThongBao = EnumLoaiThongBao.CongViecMoiCanXuLy,
                            ThongBaoInfoJson = JsonConvert.SerializeObject(new ThongBaoThongTinJson
                            {
                                GiaoViecId = giaoViec.Id,
                                GiaoViecThanhVienId = thanhVien.Id
                            })
                        }, BienDungChung.ConnectDBStr);
                    }
                }
            }
            catch (Exception exception)
            {
                log.Error(exception.Message + Environment.NewLine + exception.StackTrace);
            }
        }

        public int DemSoLuongCongViecDaGiao(int nguoiGiaoId)
        {
            return _context.GiaoViecs
                .Where(x => x.NguoiGiaoId == nguoiGiaoId &&
                           x.IsDeleted != true &&
                           x.IsHoanThanh != true && x.IsTuGiao != true)
                .Count();
        }

        public int DemSoLuongCongViecCanXuLy(int thanhVienId)
        {
            return _context.GiaoViecThanhViens
                .Where(x => x.ThanhVienId == thanhVienId &&
                           x.IsDeleted != true &&
                           x.IsHoanThanh != true && (x.GiaoViec.NguoiGiaoId != thanhVienId && x.GiaoViec.IsTuGiao != true))
                .Count();
        }

        public int DemSoLuongCongViecDaChuyenGiaoChuaHoanThanh(int nguoiChuyenGiaoId)
        {
            var danhSachGiaoViecThanhVienDaChuyenGiao = _context.GiaoViecThanhViens
                .Where(x => x.ThanhVienId == nguoiChuyenGiaoId &&
                           x.IsDeleted == true &&
                           x.GiaoViec.IsHoanThanh != true)
                .Select(x => x.Id)
                .ToList();

            if (danhSachGiaoViecThanhVienDaChuyenGiao.Count == 0)
            {
                return 0;
            }

            return _context.GiaoViecThanhViens
                .Where(x => x.GiaoViecThanhVienChuyenGiaoId.HasValue &&
                           danhSachGiaoViecThanhVienDaChuyenGiao.Contains(x.GiaoViecThanhVienChuyenGiaoId.Value) &&
                           x.IsDeleted != true &&
                           x.IsHoanThanh != true &&
                           x.GiaoViec.IsHoanThanh != true &&
                           x.GiaoViecId.HasValue)
                .Select(x => x.GiaoViecId.Value)
                .Distinct()
                .Count();
        }

        public dynamic ChuyenGiaoViecTiepTuc(ChuyenGiaoViecTiepTucViewModel viewModel)
        {
            if (viewModel.ThanhViens == null || viewModel.ThanhViens.Count == 0)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "ThanhViens",
                            Description = "Vui lòng chọn ít nhất 1 người để chuyển giao."
                        }
                    }
                };
            }

            var giaoViec = _context.GiaoViecs.Include(o => o.GiaoViecThanhViens)
                .FirstOrDefault(o => o.Id == viewModel.GiaoViecId && o.IsDeleted != true);
            if (giaoViec == null)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "GiaoViecId",
                            Description = "Không tìm thấy công việc."
                        }
                    }
                };
            }

            var thanhVienHienTai = giaoViec.GiaoViecThanhViens
                .FirstOrDefault(o => o.Id == viewModel.GiaoViecThanhVienId && o.IsDeleted != true);
            if (thanhVienHienTai == null)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "GiaoViecThanhVienId",
                            Description = "Không tìm thấy thành viên hiện tại."
                        }
                    }
                };
            }

            var currentUserId = viewModel.CurrentUserId ?? 0;
            if (thanhVienHienTai.ThanhVienId != currentUserId)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "CurrentUserId",
                            Description = "Bạn không có quyền chuyển giao công việc này."
                        }
                    }
                };
            }

            var validationErrors = new List<ValidateError>();
            var errorThanhViens = new List<ThanhVienValidationError>();

            foreach (var thanhVien in viewModel.ThanhViens)
            {
                if (thanhVien.ThanhVienId == currentUserId)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(thanhVien.NoiDungNhiemVu))
                {
                    thanhVien.NoiDungNhiemVu = thanhVienHienTai.NoiDungNhiemVu ?? giaoViec.GhiChu;
                }

                if (string.IsNullOrWhiteSpace(thanhVien.HanXuLyStr) && thanhVienHienTai.HanXuLy.HasValue)
                {
                    thanhVien.HanXuLyStr = thanhVienHienTai.HanXuLy.Value.ToString("dd/MM/yyyy");
                }

                if (thanhVien.SoGioUocTinh == null || thanhVien.SoGioUocTinh <= 0)
                {
                    thanhVien.SoGioUocTinh = thanhVienHienTai.SoGioUocTinh.GetValueOrDefault() > 0 ? thanhVienHienTai.SoGioUocTinh : 8;
                }

                var userName = CacheManagerHelper.GetUserByIdFromCache((int)thanhVien.ThanhVienId)?.Name ?? "Người dùng";
                var userErrors = new List<string>();

                if (string.IsNullOrWhiteSpace(thanhVien.NoiDungNhiemVu))
                {
                    userErrors.Add("Nội dung nhiệm vụ không được trống");
                }

                if (thanhVien.HanXuLy == null)
                {
                    userErrors.Add("Hạn xử lý không được trống");
                }
                else
                {
                    var hanXuLyDate = thanhVien.HanXuLy.Value;
                    var ngayHienTai = DateTimeHelper.BeginningOfDay(DateTime.Now);

                    if (hanXuLyDate < ngayHienTai)
                    {
                        userErrors.Add("Hạn xử lý không được nhỏ hơn ngày hiện tại");
                    }
                }

                if (thanhVien.SoGioUocTinh == null || thanhVien.SoGioUocTinh <= 0)
                {
                    userErrors.Add("Số giờ ước tính không được trống");
                }

                if (userErrors.Count > 0)
                {
                    errorThanhViens.Add(new ThanhVienValidationError
                    {
                        TenNguoiDung = userName,
                        DanhSachLoi = userErrors
                    });
                }
            }

            if (errorThanhViens.Count > 0)
            {
                var errorMessages = new List<string>();
                var stt = 1;
                foreach (var errorThanhVien in errorThanhViens)
                {
                    var userErrors = string.Join("; ", errorThanhVien.DanhSachLoi);
                    errorMessages.Add($"{stt}. {errorThanhVien.TenNguoiDung}: {userErrors}");
                    stt++;
                }

                validationErrors.Add(new ValidateError
                {
                    Id = "ThanhViens",
                    Description = string.Join("<br/>", errorMessages)
                });

                return new
                {
                    Errors = validationErrors
                };
            }

            var oldThanhVienId = (int)thanhVienHienTai.ThanhVienId;

            thanhVienHienTai.IsDeleted = true;
            thanhVienHienTai.NgayXoa = DateTime.Now;
            thanhVienHienTai.NguoiXoaId = currentUserId;
            thanhVienHienTai.NgayChuyenGiao = DateTime.Now;

            foreach (var thanhVienViewModel in viewModel.ThanhViens)
            {
                if (thanhVienViewModel.ThanhVienId == currentUserId)
                {
                    continue;
                }

                var existingThanhVien = giaoViec.GiaoViecThanhViens
                    .FirstOrDefault(o => o.ThanhVienId == thanhVienViewModel.ThanhVienId && o.IsDeleted != true);
                if (existingThanhVien != null)
                {
                    existingThanhVien.NoiDungNhiemVu = thanhVienViewModel.NoiDungNhiemVu;
                    existingThanhVien.HanXuLy = thanhVienViewModel.HanXuLy;
                    existingThanhVien.SoGioUocTinh = thanhVienViewModel.SoGioUocTinh;
                    if (existingThanhVien.GiaoViecThanhVienChuyenGiaoId == null)
                    {
                        existingThanhVien.GiaoViecThanhVienChuyenGiaoId = thanhVienHienTai.Id;
                    }
                }
                else
                {
                    var newThanhVien = new GiaoViecThanhVien
                    {
                        ThanhVienId = thanhVienViewModel.ThanhVienId,
                        NoiDungNhiemVu = thanhVienViewModel.NoiDungNhiemVu,
                        HanXuLy = thanhVienViewModel.HanXuLy,
                        SoGioUocTinh = thanhVienViewModel.SoGioUocTinh,
                        GiaoViecThanhVienChuyenGiaoId = thanhVienHienTai.Id,
                        NguoiTheoDoiChiDaoId = LayNguoiTheoDoiChiDaoIdCuaThanhVien(thanhVienViewModel.ThanhVienId)
                    };
                    giaoViec.GiaoViecThanhViens.Add(newThanhVien);
                }
            }

            var hanXuLy = viewModel.ThanhViens.Max(o => o.HanXuLy);
            if (hanXuLy.HasValue)
            {
                giaoViec.HanXuLy = hanXuLy;
            }

            _context.SaveChanges();

            var giaoViecInfo = new GiaoViecGridVo
            {
                Id = giaoViec.Id,
                TenCongViec = giaoViec.TenCongViec,
            };

            var newThanhViens = new List<GiaoViecThanhVienGridVo>();
            foreach (var thanhVienViewModel in viewModel.ThanhViens)
            {
                if (thanhVienViewModel.ThanhVienId == currentUserId)
                {
                    continue;
                }

                var thanhVien = giaoViec.GiaoViecThanhViens
                    .FirstOrDefault(o => o.ThanhVienId == thanhVienViewModel.ThanhVienId && o.IsDeleted != true);
                if (thanhVien != null)
                {
                    newThanhViens.Add(new GiaoViecThanhVienGridVo
                    {
                        Id = thanhVien.Id,
                        ThanhVienId = thanhVien.ThanhVienId,
                        NoiDungNhiemVu = thanhVien.NoiDungNhiemVu,
                        HanXuLy = thanhVien.HanXuLy,
                        SoGioUocTinh = thanhVien.SoGioUocTinh,
                    });
                }
            }

            XuLyThongBaoChuyenGiaoViecAsync(giaoViecInfo, newThanhViens);

            return true;
        }

        public GiaoViecThanhVien GetGiaoViecThanhVienById(int id)
        {
            return _context.GiaoViecThanhViens.AsNoTracking().FirstOrDefault(o => o.Id == id);
        }

        public dynamic LayLaiCongViecChuyenGiao(int giaoViecThanhVienId, int currentUserId)
        {
            var nguoiChuyenGiaoId = _context.GiaoViecThanhViens
                .Where(o => o.Id == giaoViecThanhVienId && o.IsDeleted != true)
                .Select(o => o.GiaoViecThanhVienChuyenGiaoId)
                .FirstOrDefault();

            if (!nguoiChuyenGiaoId.HasValue)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "GiaoViecThanhVienId",
                            Description = "Không tìm thấy công việc chuyển giao hoặc công việc này không phải là công việc được chuyển giao."
                        }
                    }
                };
            }

            var nguoiChuyenGiao = _context.GiaoViecThanhViens
                .FirstOrDefault(o => o.Id == nguoiChuyenGiaoId.Value);

            if (nguoiChuyenGiao == null)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "NguoiChuyenGiao",
                            Description = "Không tìm thấy thông tin người chuyển giao gốc."
                        }
                    }
                };
            }

            if (nguoiChuyenGiao.ThanhVienId != currentUserId)
            {
                return new
                {
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "CurrentUserId",
                            Description = "Bạn không có quyền lấy lại công việc này."
                        }
                    }
                };
            }

            var danhSachNguoiNhan = _context.GiaoViecThanhViens
              .Where(o => o.GiaoViecThanhVienChuyenGiaoId == nguoiChuyenGiaoId.Value)
              .ToList();
            foreach (var item in danhSachNguoiNhan)
            {
                _context.GiaoViecThanhViens.Remove(item);
            }

            nguoiChuyenGiao.IsDeleted = false;
            nguoiChuyenGiao.NgayXoa = null;
            nguoiChuyenGiao.NguoiXoaId = null;
            nguoiChuyenGiao.NgayChuyenGiao = null;

            _context.SaveChanges();

            return true;
        }

        public List<GiaoViecThanhVienTheoGiaoViecInfo> GetListGiaoViecThanhVienByListGiaoViecIds(List<int> giaoViecIds)
        {
            var result = new List<GiaoViecThanhVienTheoGiaoViecInfo>();

            if (giaoViecIds == null || giaoViecIds.Count == 0)
            {
                return result;
            }

            var giaoViecThanhViens = _context.GiaoViecThanhViens
                .Where(o => giaoViecIds.Contains(o.GiaoViecId.Value) && o.IsDeleted != true)
                .Select(x => new GiaoViecThanhVienGridVo
                {
                    Id = x.Id,
                    GiaoViecId = x.GiaoViecId.Value,
                    ThanhVienId = x.ThanhVienId,
                    HanXuLy = x.HanXuLy,
                    NoiDungNhiemVu = x.NoiDungNhiemVu,
                    SoGioUocTinh = x.SoGioUocTinh,
                    IsHoanThanh = x.IsHoanThanh,
                    NguoiTheoDoiChiDaoId = x.NguoiTheoDoiChiDaoId,
                    TrangThai = x.TrangThai,
                    KetQuaDanhGia = x.KetQuaDanhGia,
                    YKienDanhGia = x.YKienDanhGia,
                    NgayXacNhanHoanThanh = x.NgayXacNhanHoanThanh,
                    NguoiDanhGiaId = x.NguoiDanhGiaId,
                    NgayDanhGia = x.NgayDanhGia
                })
                .ToList();

            var giaoViecThanhVienIds = giaoViecThanhViens.Select(o => o.Id).Distinct().ToList();
            var danhSachTienTrinh = new List<GiaoViecThanhVienTienTrinhInfo>();

            if (giaoViecThanhVienIds.Count > 0)
            {
                using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
                {
                    conn.Open();

                    try
                    {
                        var paramNames = new List<string>();
                        var parameters = new List<SqlParameter>();

                        for (int i = 0; i < giaoViecThanhVienIds.Count; i++)
                        {
                            var paramName = $"@Id{i}";
                            paramNames.Add(paramName);
                            parameters.Add(new SqlParameter(paramName, giaoViecThanhVienIds[i]));
                        }

                        string sql = $@"SELECT 
                                        GiaoViecThanhVienId, 
                                        PhanTramHoanThanh
                                      FROM (
                                          SELECT 
                                              GiaoViecThanhVienId, 
                                              PhanTramHoanThanh,
                                              ROW_NUMBER() OVER (PARTITION BY GiaoViecThanhVienId ORDER BY NgayTao DESC) as RowNum
                                          FROM GiaoViecThanhVienTienTrinh 
                                          WHERE GiaoViecThanhVienId IN ({string.Join(",", paramNames)})
                                            AND (IsDeleted IS NULL OR IsDeleted = 0)
                                      ) AS Ranked
                                      WHERE RowNum = 1";

                        using (var cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddRange(parameters.ToArray());

                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var giaoViecThanhVienId = SqlServeHelper.LayGiaTriCuaCotLaInteger(reader, "GiaoViecThanhVienId");
                                    var phanTramHoanThanh = SqlServeHelper.LayGiaTriCuaCotLaInteger(reader, "PhanTramHoanThanh");
                                    danhSachTienTrinh.Add(new GiaoViecThanhVienTienTrinhInfo
                                    {
                                        GiaoViecThanhVienId = giaoViecThanhVienId,
                                        PhanTramHoanThanh = phanTramHoanThanh
                                    });
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error in GetListGiaoViecThanhVienByListGiaoViecIds: {ex.Message}");
                    }
                }
            }

            foreach (var giaoViecThanhVien in giaoViecThanhViens)
            {
                var tienTrinh = danhSachTienTrinh.FirstOrDefault(o => o.GiaoViecThanhVienId == giaoViecThanhVien.Id);
                if (tienTrinh != null)
                {
                    giaoViecThanhVien.PhanTramHoanThanh = (int)tienTrinh.PhanTramHoanThanh;
                }

                var giaoViecInfo = result.FirstOrDefault(o => o.GiaoViecId == giaoViecThanhVien.GiaoViecId);
                if (giaoViecInfo == null)
                {
                    giaoViecInfo = new GiaoViecThanhVienTheoGiaoViecInfo
                    {
                        GiaoViecId = giaoViecThanhVien.GiaoViecId
                    };
                    result.Add(giaoViecInfo);
                }

                giaoViecInfo.GiaoViecThanhViens.Add(giaoViecThanhVien);
            }

            return result;
        }

        public List<GoiYNoiDungItemVo> GetListGoiYNoiDung(GoiYNoiDungQueryViewModel queryParams)
        {
            if (queryParams == null) queryParams = new GoiYNoiDungQueryViewModel();
            var limit = queryParams.Limit > 0 ? queryParams.Limit : 10;

            var query = _context.GiaoViecGoiYNoiDungs.AsNoTracking()
                .Where(o => o.IsDeleted != true && o.NoiDung != null && o.NoiDung != "");

            var loaiValue = queryParams.Loai.GetValueOrDefault();
            if (loaiValue != 0)
            {
                if (loaiValue == (int)LoaiGoiYNoiDungEnum.NoiDungCongViec)
                {
                    query = query.Where(o => o.Loai == null || o.Loai == loaiValue);
                }
                else
                {
                    query = query.Where(o => o.Loai == loaiValue);
                }
            }

            var currentUserIdValue = queryParams.CurrentUserId.GetValueOrDefault();
            if (currentUserIdValue != 0)
            {
                query = query.Where(o => o.NguoiTaoId == currentUserIdValue);
            }

            if (!string.IsNullOrWhiteSpace(queryParams.Keyword))
            {
                var kw = queryParams.Keyword.Trim();
                query = query.Where(o => o.NoiDung.Contains(kw))
                             .OrderByDescending(o => o.NoiDung.StartsWith(kw))
                             .ThenByDescending(o => o.SoLanSuDung)
                             .ThenByDescending(o => o.NgayCapNhat);
            }
            else
            {
                query = query.OrderByDescending(o => o.SoLanSuDung)
                             .ThenByDescending(o => o.NgayCapNhat);
            }

            return query.Take(limit).Select(o => new GoiYNoiDungItemVo
            {
                Id = o.Id,
                NoiDung = o.NoiDung,
                SoLanSuDung = o.SoLanSuDung
            }).ToList();
        }

        public void LuuGoiYNoiDung(LuuGoiYNoiDungViewModel viewModel)
        {
            if (viewModel == null || string.IsNullOrWhiteSpace(viewModel.NoiDung)) return;

            var noiDungTrim = viewModel.NoiDung.Trim();
            if (string.IsNullOrEmpty(noiDungTrim)) return;

            var loai = viewModel.Loai.GetValueOrDefault((int)LoaiGoiYNoiDungEnum.NoiDungCongViec);
            var currentUserId = viewModel.CurrentUserId;

            try
            {
                var existing = _context.GiaoViecGoiYNoiDungs
                    .FirstOrDefault(o => o.IsDeleted != true 
                                      && o.NoiDung == noiDungTrim 
                                      && (o.Loai == loai || (loai == (int)LoaiGoiYNoiDungEnum.NoiDungCongViec && o.Loai == null))
                                      && (currentUserId == null || o.NguoiTaoId == currentUserId));

                if (existing != null)
                {
                    existing.SoLanSuDung += 1;
                    existing.NgayCapNhat = DateTime.Now;
                    if (!existing.Loai.HasValue)
                    {
                        existing.Loai = loai;
                    }
                }
                else
                {
                    _context.GiaoViecGoiYNoiDungs.Add(new GiaoViecGoiYNoiDung
                    {
                        NoiDung = noiDungTrim,
                        SoLanSuDung = 1,
                        Loai = loai,
                        NguoiTaoId = currentUserId,
                        NgayTao = DateTime.Now,
                        NgayCapNhat = DateTime.Now,
                        IsDeleted = false
                    });
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error($"Error in LuuGoiYNoiDung: {ex.Message}", ex);
            }
        }
    }
}
