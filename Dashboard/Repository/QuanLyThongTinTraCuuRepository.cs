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
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;

namespace Dashboard.Repository
{
    public interface IQuanLyThongTinTraCuuRepository
    {
        DataTableResult<QuanLyThongTinTraCuuGridVo> GetAll(QuanLyThongTinTraCuuQueryViewModel queryParams);
        QuanLyThongTinTraCuuGridVo GetById(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(QuanLyThongTinTraCuuViewModel QuanLyThongTinTraCuuViewModel);
        bool Xoa(int Id);
        List<VanBanPhapLuatGridVo> GetVanBanLienQuan(int id);
        List<FileDinhKemGridVo> GetFileDinhKemById(int id);
    }

    public class QuanLyThongTinTraCuuRepository : IQuanLyThongTinTraCuuRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<QuanLyThongTinTraCuuViewModel, QuanLyThongTinTraCuu>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .AfterMap((s, d) =>
                {

                });

        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IQuanLyThongTinTraCuuValidate _quanLyThongTinTraCuuValidate;
        public QuanLyThongTinTraCuuRepository(IContext context, IQuanLyThongTinTraCuuValidate quanLyThongTinTraCuuValidate)
        {
            _context = context;
            _quanLyThongTinTraCuuValidate = quanLyThongTinTraCuuValidate;
        }
        //required
        private readonly string sortStringDefault = "TieuDe asc";
        private readonly List<string> searchCol = new List<string> { "MaTraCuu", "TieuDe", "MoTa" };

        public DataTableResult<QuanLyThongTinTraCuuGridVo> GetAll(QuanLyThongTinTraCuuQueryViewModel queryParams)
        {
            //required
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.QuanLyThongTinTraCuus.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            if (queryParams.DanhMucTraCuuThongTinId.GetValueOrDefault() != 0)
            {
                query = query.Where(o => o.DanhMucTraCuuThongTinId == queryParams.DanhMucTraCuuThongTinId);
            }

            if (queryParams.TrangThaiId.GetValueOrDefault() != 0)
            {
                query = query.Where(o => o.TrangThai == queryParams.TrangThaiId);
            }

            var dateTimeCurrent = DateTime.Now;
            var data = query.Select(cv => new QuanLyThongTinTraCuuGridVo
            {
                Id = cv.Id,
                MaTraCuu = cv.MaTraCuu,
                TieuDe = cv.TieuDe,
                MoTa = cv.MoTa,
                NgayHieuLuc = cv.NgayHieuLuc,
                TrangThai = cv.TrangThai,
                NgayTao = cv.NgayTao,
                DanhMucTraCuuThongTinId = cv.DanhMucTraCuuThongTinId
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<QuanLyThongTinTraCuuGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(QuanLyThongTinTraCuuViewModel QuanLyThongTinTraCuuViewModel)
        {
            var errors = _quanLyThongTinTraCuuValidate.Validate(QuanLyThongTinTraCuuViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (QuanLyThongTinTraCuuViewModel.Id == 0)
            {
                var item = _iMapper.Map<QuanLyThongTinTraCuuViewModel, QuanLyThongTinTraCuu>(QuanLyThongTinTraCuuViewModel);
                item.NgayTao = DateTime.Now;
                item.NguoiTaoId = QuanLyThongTinTraCuuViewModel.CurrentUserId;

                AddVanBanLienQuanToEntity(item, QuanLyThongTinTraCuuViewModel);
                AddFileToEntity(item, QuanLyThongTinTraCuuViewModel);

                _context.QuanLyThongTinTraCuus.Add(item);
                _context.SaveChanges();
            }
            else
            {
                var item = _context.QuanLyThongTinTraCuus.Include(x => x.QuanLyThongTinTraCuuChiTiets).Include(x => x.QuanLyThongTinTraCuuFiles).FirstOrDefault(o => o.Id == QuanLyThongTinTraCuuViewModel.Id);
                if (item == null) throw new Exception("Quản lý thông tin tra cứu đã bị xóa");

                _iMapper.Map(QuanLyThongTinTraCuuViewModel, item);

                AddVanBanLienQuanToEntity(item, QuanLyThongTinTraCuuViewModel);
                AddFileToEntity(item, QuanLyThongTinTraCuuViewModel);
            }

            _context.SaveChanges();

            return true;
        }

        private void AddVanBanLienQuanToEntity(QuanLyThongTinTraCuu item, QuanLyThongTinTraCuuViewModel itemViewModel)
        {
            var oldIds = item.QuanLyThongTinTraCuuChiTiets.Where(x => x.IsDeleted != true)
                .Select(x => (int)x.VanBanPhapLuatId).ToList();

            var removeIds = oldIds.Except(itemViewModel.VanBanLienQuanIdInts).ToList();
            var addIds = itemViewModel.VanBanLienQuanIdInts.Except(oldIds).ToList();

            foreach (var removeId in removeIds)
            {
                var removeItem = item.QuanLyThongTinTraCuuChiTiets.FirstOrDefault(x => x.VanBanPhapLuatId == removeId && x.IsDeleted != true);
                if (removeItem != null)
                {
                    removeItem.IsDeleted = true;
                    removeItem.NgayXoa = DateTime.Now;
                    removeItem.NguoiXoaId = itemViewModel.CurrentUserId;
                }
            }

            foreach (var addId in addIds)
            {
                var addItem = new QuanLyThongTinTraCuuChiTiet
                {
                    QuanLyThongTinTraCuuId = item.Id,
                    VanBanPhapLuatId = addId,
                    NgayTao = DateTime.Now,
                    NguoiTaoId = itemViewModel.CurrentUserId
                };
                item.QuanLyThongTinTraCuuChiTiets.Add(addItem);
            }
        }

        public bool Xoa(int Id)
        {
            var item = _context.QuanLyThongTinTraCuus.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = true;
                item.NgayXoa = DateTime.Now;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public bool KhoiPhuc(int Id)
        {
            var item = _context.QuanLyThongTinTraCuus.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                item.NgayXoa = null;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public QuanLyThongTinTraCuuGridVo GetById(int id)
        {
            var item = _context.QuanLyThongTinTraCuus.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new QuanLyThongTinTraCuuGridVo
                {
                    Id = item.Id,
                    MaTraCuu = item.MaTraCuu,
                    TieuDe = item.TieuDe,
                    MoTa = item.MoTa,
                    NgayHieuLuc = item.NgayHieuLuc,
                    TrangThai = item.TrangThai,
                    NgayTao = item.NgayTao,
                    DanhMucTraCuuThongTinId = item.DanhMucTraCuuThongTinId
                };
            }

            return new QuanLyThongTinTraCuuGridVo();
        }

        public List<VanBanPhapLuatGridVo> GetVanBanLienQuan(int id)
        {
            var sql = $@"  select vbpl.Id, vbpl.SoKiHieu, vbpl.TrichYeu, vbpl.HinhThucVanBanId, 
                vbpl.CoQuanBanHanhId, vbpl.NgayBanHanh
              from QuanLyThongTinTraCuuChiTiet ct, VanBanPhapLuat vbpl
              where (ct.[IsDeleted] is null Or ct.[IsDeleted] != 1) 
                AND  ct.VanBanPhapLuatId = vbpl.Id AND QuanLyThongTinTraCuuId = {id}";

            var result = new List<VanBanPhapLuatGridVo>();
            using (var conn = new SqlConnection(BienDungChung.ConnectDBStr))
            {
                try
                {
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new VanBanPhapLuatGridVo();
                                item.Id = SqlServeHelper.LayGiaTriCuaCotLaIntegerDefaultIsZero(reader, "Id");
                                item.SoKiHieu = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "SoKiHieu");
                                item.TrichYeu = SqlServeHelper.LayGiaTriCuaCotLaString(reader, "TrichYeu");
                                item.HinhThucVanBanId = SqlServeHelper.LayGiaTriCuaCotLaInteger(reader, "HinhThucVanBanId");
                                item.CoQuanBanHanhId = SqlServeHelper.LayGiaTriCuaCotLaInteger(reader, "CoQuanBanHanhId");
                                item.NgayBanHanh = SqlServeHelper.LayGiaTriCuaCotLaDateTime(reader, "NgayBanHanh");

                                result.Add(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return result;
        }

        private void AddFileToEntity(QuanLyThongTinTraCuu item, QuanLyThongTinTraCuuViewModel itemViewModel)
        {
            var uploadPath = System.Web.HttpContext.Current.Server.MapPath(BienDungChung.UploadsPath);
            //xoá file cũ 
            foreach (var file in item.QuanLyThongTinTraCuuFiles)
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
                    item.QuanLyThongTinTraCuuFiles.Add(new QuanLyThongTinTraCuuFile
                    {
                        TenFile = taiLieu.FileName,
                        DuongDan = FileHelper.SaveHttpFilesToUploads(uploadPath, taiLieu),
                        NgayTao = DateTime.Now,
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

        public List<FileDinhKemGridVo> GetFileDinhKemById(int id)
        {
            return _context.QuanLyThongTinTraCuuFiles.Where(o => o.QuanLyThongTinTraCuuId == id && o.IsDeleted != true)
                .Select(o => new FileDinhKemGridVo
                {
                    Id = o.Id,
                    TenFile = o.TenFile,
                    DuongDan = o.DuongDan
                }).ToList();
        }
    }
}
