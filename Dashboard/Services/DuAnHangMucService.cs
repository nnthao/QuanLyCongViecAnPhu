using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;

namespace Dashboard.Services
{
    public interface IDuAnHangMucService
    {
        dynamic CapNhat(DuAnHangMucInfo model);
        DataTableResult<DuAnHangMucGridVo> GetAll(QueryParamViewModel queryParams);
        List<DuAnHangMucGridVo> GetActiveList();
        DuAnHangMuc GetById(int id);
        dynamic Them(DuAnHangMucInfo model);
        bool Xoa(int id);
    }

    public class DuAnHangMucService : IDuAnHangMucService
    {
        private readonly IDuAnHangMucRepository _repository;

        public DuAnHangMucService(IDuAnHangMucRepository repository)
        {
            _repository = repository;
        }

        public DataTableResult<DuAnHangMucGridVo> GetAll(QueryParamViewModel queryParams)
        {
            return _repository.GetAll(queryParams);
        }

        public List<DuAnHangMucGridVo> GetActiveList()
        {
            return _repository.GetActiveList();
        }

        public DuAnHangMuc GetById(int id)
        {
            return _repository.GetById(id);
        }

        public dynamic Them(DuAnHangMucInfo model)
        {
            return _repository.Them(model);
        }

        public dynamic CapNhat(DuAnHangMucInfo model)
        {
            return _repository.CapNhat(model);
        }

        public bool Xoa(int id)
        {
            return _repository.Xoa(id);
        }
    }
}