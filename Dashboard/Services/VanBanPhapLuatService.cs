using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Dashboard.Services
{
    public interface IVanBanPhapLuatService
    {
        DataTableResult<VanBanPhapLuatGridVo> GetAll(QueryParamViewModel queryParams);
        VanBanPhapLuatGridVo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(VanBanPhapLuatViewModel model);
        bool Xoa(int id, int currentUserId);
        DataLookupViewModel HinhThucVanBanLookup(LookupQueryViewModel lookupQueryViewModel);
        DataLookupViewModel CoQuanBanHanhLookup(LookupQueryViewModel lookupQueryViewModel);
        DataLookupViewModel DoKhanLookup(LookupQueryViewModel lookupQueryViewModel);
        List<FileDinhKemGridVo> GetFileDinhKemById(int id);
    }

    public class VanBanPhapLuatService : IVanBanPhapLuatService
    {
        private readonly IVanBanPhapLuatRepository _repository;
        private readonly IHinhThucVanBanRepository _hinhThucVanBanRepository;
        private readonly ICoQuanBanHanhRepository _coQuanBanHanhRepository;
        private readonly IUserRepository _userRepository;
        public VanBanPhapLuatService(IVanBanPhapLuatRepository repository, IUserRepository userRepository,
            IHinhThucVanBanRepository hinhThucVanBanRepository, ICoQuanBanHanhRepository coQuanBanHanhRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
            _hinhThucVanBanRepository = hinhThucVanBanRepository;
            _coQuanBanHanhRepository = coQuanBanHanhRepository;
        }

        public DataTableResult<VanBanPhapLuatGridVo> GetAll(QueryParamViewModel queryParams)
        {
            var data = _repository.GetAll(queryParams);

            var coQuans = _coQuanBanHanhRepository.CoQuanBanHanhLookup(new LookupQueryViewModel());
            var hinhThucs = _hinhThucVanBanRepository.HinhThucVanBanLookup(new LookupQueryViewModel());
            foreach (var item in data.rows)
            {
                if (item.HinhThucVanBanId.GetValueOrDefault() != 0)
                {
                    var ht = hinhThucs.Data.FirstOrDefault(o => o.id == item.HinhThucVanBanId);
                    if (ht != null)
                    {
                        item.HinhThucVanBan = ht.text;
                    }
                }

                if (item.CoQuanBanHanhId.GetValueOrDefault() != 0)
                {
                    var cq = coQuans.Data.FirstOrDefault(o => o.id == item.CoQuanBanHanhId);
                    if (cq != null)
                    {
                        item.CoQuanBanHanh = cq.text;
                    }
                }
            }
            return data;
        }

        public VanBanPhapLuatGridVo GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _repository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(VanBanPhapLuatViewModel model)
        {
            return _repository.ThemOrCapNhat(model);
        }

        public bool Xoa(int id, int currentUserId)
        {
            return _repository.Xoa(id, currentUserId);
        }

        public DataLookupViewModel HinhThucVanBanLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _repository.HinhThucVanBanLookup(lookupQueryViewModel);
        }

        public DataLookupViewModel CoQuanBanHanhLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _repository.CoQuanBanHanhLookup(lookupQueryViewModel);
        }

        public DataLookupViewModel DoKhanLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _repository.DoKhanLookup(lookupQueryViewModel);
        }

        public List<FileDinhKemGridVo> GetFileDinhKemById(int id)
        {
            return _repository.GetFileDinhKemById(id);
        }
    }
}


