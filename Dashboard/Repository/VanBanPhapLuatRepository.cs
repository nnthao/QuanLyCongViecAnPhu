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
    public interface IVanBanPhapLuatRepository
    {
        DataLookupViewModel HinhThucVanBanLookup(LookupQueryViewModel lookupQueryViewModel);
        DataLookupViewModel CoQuanBanHanhLookup(LookupQueryViewModel lookupQueryViewModel);
        DataLookupViewModel DoKhanLookup(LookupQueryViewModel lookupQueryViewModel);
        DataTableResult<VanBanPhapLuatGridVo> GetAll(QueryParamViewModel queryParams);
        VanBanPhapLuatGridVo GetById(int id);
        bool KhoiPhuc(int Id);
        dynamic ThemOrCapNhat(VanBanPhapLuatViewModel model);
        bool Xoa(int Id, int currentUserId);
        List<FileDinhKemGridVo> GetFileDinhKemById(int id);
        List<VanBanPhapLuatGridVo> GetByListId(List<int> ids);
    }

    public class VanBanPhapLuatRepository : IVanBanPhapLuatRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<VanBanPhapLuatViewModel, VanBanPhapLuat>()
                .ForMember(x => x.Id, opt => opt.Ignore())
                .AfterMap((s, d) =>
                {

                });

        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IVanBanPhapLuatValidate _validate;
        public VanBanPhapLuatRepository(IContext context, IVanBanPhapLuatValidate validate)
        {
            _context = context;
            _validate = validate;
        }
        //required
        private readonly string sortStringDefault = "NgayBanHanh desc";
        private readonly List<string> searchCol = new List<string> { "SoKiHieu", "TrichYeu" };

        public DataTableResult<VanBanPhapLuatGridVo> GetAll(QueryParamViewModel queryParams)
        {
            //required
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.VanBanPhapLuats.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            var data = query.Select(cv => new VanBanPhapLuatGridVo
            {
                Id = cv.Id,
                SoKiHieu = cv.SoKiHieu,
                TrichYeu = cv.TrichYeu,
                HinhThucVanBanId = cv.HinhThucVanBanId,
                CoQuanBanHanhId = cv.CoQuanBanHanhId,
                DoKhanId = cv.DoKhanId,
                NgayBanHanh = cv.NgayBanHanh,
                IsDeleted = cv.IsDeleted,
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<VanBanPhapLuatGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }

        public dynamic ThemOrCapNhat(VanBanPhapLuatViewModel model)
        {
            var errors = _validate.Validate(model);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (model.Id == 0)
            {
                var item = _iMapper.Map<VanBanPhapLuatViewModel, VanBanPhapLuat>(model);
                AddFileToEntity(item, model);

                _context.VanBanPhapLuats.Add(item);
            }
            else
            {
                var item = _context.VanBanPhapLuats.Include(o => o.VanBanPhapLuatFiles).FirstOrDefault(o => o.Id == model.Id);
                if (item == null) throw new Exception("Văn bản đã bị xóa");

                _iMapper.Map(model, item);

                AddFileToEntity(item, model);
            }

            _context.SaveChanges();

            return true;
        }

        private void AddFileToEntity(VanBanPhapLuat item, VanBanPhapLuatViewModel itemViewModel)
        {
            var uploadPath = System.Web.HttpContext.Current.Server.MapPath(BienDungChung.UploadsPath);
            //xoá file cũ 
            foreach (var file in item.VanBanPhapLuatFiles)
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
                    item.VanBanPhapLuatFiles.Add(new VanBanPhapLuatFile
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

        public bool Xoa(int Id, int currentUserId)
        {
            var item = _context.VanBanPhapLuats.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = true;
                item.NguoiXoaId = currentUserId;
                item.NgayXoa = DateTime.Now;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public bool KhoiPhuc(int Id)
        {
            var item = _context.VanBanPhapLuats.FirstOrDefault(o => o.Id == Id);
            if (item != null)
            {
                item.IsDeleted = false;
                item.NguoiXoaId = null;
                item.NgayXoa = null;
                _context.SaveChanges();
                return true;
            }

            return false;
        }

        public VanBanPhapLuatGridVo GetById(int id)
        {
            var item = _context.VanBanPhapLuats.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                return new VanBanPhapLuatGridVo
                {
                    Id = item.Id,
                    SoKiHieu = item.SoKiHieu,
                    TrichYeu = item.TrichYeu,
                    HinhThucVanBanId = item.HinhThucVanBanId,
                    CoQuanBanHanhId = item.CoQuanBanHanhId,
                    DoKhanId = item.DoKhanId,
                    NgayBanHanh = item.NgayBanHanh
                };
            }

            return new VanBanPhapLuatGridVo();
        }

        public DataLookupViewModel HinhThucVanBanLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var caches = CacheManagerHelper.GetHinhThucVanBanCache();
            var query = caches.AsQueryable();
            if (!lookupQueryViewModel.isGetAll)
            {
                query = query.Where(o => o.IsDeleted != true);
            }
            var result = query.Select(o => new LookupViewModel { id = o.Id, text = o.Ten }).ToList();
            return new DataLookupViewModel { Data = result };
        }

        public DataLookupViewModel CoQuanBanHanhLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var caches = CacheManagerHelper.GetCoQuanBanHanhCache();
            var query = caches.AsQueryable();
            if (!lookupQueryViewModel.isGetAll)
            {
                query = query.Where(o => o.IsDeleted != true);
            }
            var result = query.Select(o => new LookupViewModel { id = o.Id, text = o.Ten }).ToList();
            return new DataLookupViewModel { Data = result };
        }

        public DataLookupViewModel DoKhanLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var caches = CacheManagerHelper.GetDoKhanCache();
            var query = caches.AsQueryable();
            if (!lookupQueryViewModel.isGetAll)
            {
                query = query.Where(o => o.IsDeleted != true);
            }
            var result = query.Select(o => new LookupViewModel { id = o.Id, text = o.Ten }).ToList();
            return new DataLookupViewModel { Data = result };
        }

        public List<FileDinhKemGridVo> GetFileDinhKemById(int id)
        {
            return _context.VanBanPhapLuatFiles.Where(o => o.VanBanPhapLuatId == id && o.IsDeleted != true)
                .Select(o => new FileDinhKemGridVo
                {
                    Id = o.Id,
                    TenFile = o.TenFile,
                    DuongDan = o.DuongDan
                }).ToList();
        }

        public List<VanBanPhapLuatGridVo> GetByListId(List<int> ids)
        {
            return _context.VanBanPhapLuats.AsNoTracking().Where(o => ids.Contains(o.Id)).Select(cv => new VanBanPhapLuatGridVo
            {
                Id = cv.Id,
                SoKiHieu = cv.SoKiHieu,
                TrichYeu = cv.TrichYeu,
                HinhThucVanBanId = cv.HinhThucVanBanId,
                CoQuanBanHanhId = cv.CoQuanBanHanhId,
                DoKhanId = cv.DoKhanId,
                NgayBanHanh = cv.NgayBanHanh,
            }).ToList();
        }
    }
}


