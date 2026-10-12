using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Dashboard.Services
{
    public interface IVaiTroService
    {
        dynamic CapNhat(VaiTroViewModel vaiTroViewModel);
        dynamic GanVaiTroKhacChoPhongBan(GanVaiTroKhacViewModel viewModel);
        DataTableResult<VaiTroGridVo> GetAll(QueryParamViewModel queryParams);
        dynamic KhoiPhuc(int id);
        dynamic Them(VaiTroViewModel vaiTroViewModel);
        DataLookupViewModel VaiTroLookup(bool isChonLabelShow = true);
        List<string> VaiTroPhanQuyenStr(int id, string nhomVaiTroId);
        dynamic Xoa(int id);
    }

    public class VaiTroService : IVaiTroService
    {
        private readonly IVaiTroRepository _vaiTroRepository;
        public VaiTroService(IVaiTroRepository vaiTroRepository)
        {
            _vaiTroRepository = vaiTroRepository;
        }

        public dynamic CapNhat(VaiTroViewModel vaiTroViewModel)
        {
            return _vaiTroRepository.CapNhat(vaiTroViewModel);
        }

        public dynamic GanVaiTroKhacChoPhongBan(GanVaiTroKhacViewModel viewModel)
        {
            return _vaiTroRepository.GanVaiTroKhacChoPhongBan(viewModel);
        }

        public DataTableResult<VaiTroGridVo> GetAll(QueryParamViewModel queryParams)
        {
            var data = _vaiTroRepository.GetAll(queryParams);
            return data;
        }

        public dynamic KhoiPhuc(int id)
        {
            return _vaiTroRepository.KhoiPhuc(id);
        }

        public dynamic Them(VaiTroViewModel vaiTroViewModel)
        {
            return _vaiTroRepository.Them(vaiTroViewModel);
        }

        public DataLookupViewModel VaiTroLookup(bool isChonLabelShow = true)
        {
            var vaiTros = CacheManagerHelper.GetVaiTroCache();

            var data = new List<LookupViewModel>
                {
                    new LookupViewModel
                    {
                        id = 0,
                        text = "--- Chọn ---"
                    }
                };

            var query = vaiTros.Where(o => o.IsDeleted != true).Select(o => new LookupViewModel
            {
                id = o.Id,
                text = o.Ten
            }).ToList();

            if (!isChonLabelShow)
            {
                return new DataLookupViewModel
                {
                    Data = query.ToList()
                };
            }

            return new DataLookupViewModel
            {
                Data = data.Concat(query).ToList()
            };
        }

        public List<string> VaiTroPhanQuyenStr(int id, string nhomVaiTroId)
        {
            return _vaiTroRepository.VaiTroPhanQuyenStr(id, nhomVaiTroId);
        }

        public dynamic Xoa(int id)
        {
            return _vaiTroRepository.Xoa(id);
        }
    }
}
