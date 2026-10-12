using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;

namespace Dashboard.Services
{
    public interface ICongViecChungService
    {
        DataTableResult<CongViecChungGridVo> GetAll(string keyword, int page, int pageSize);
        CongViecChungGridVo GetById(int id);
        bool Insert(CongViecChungViewModel model);
        bool Update(CongViecChungViewModel model);
        bool Delete(int id);
        List<CongViecChungThongKeGridVo> ThongKeCongViec(CongViecChungThongKeQueryViewModel query);
    }

    public class CongViecChungService : ICongViecChungService
    {
        private readonly ICongViecChungRepository _repository;

        public CongViecChungService(ICongViecChungRepository repository)
        {
            _repository = repository;
        }

        public bool Delete(int id)
        {
            return _repository.Delete(id);
        }

        public DataTableResult<CongViecChungGridVo> GetAll(string keyword, int page, int pageSize)
        {
            return _repository.GetAll(keyword, page, pageSize);
        }

        public CongViecChungGridVo GetById(int id)
        {
            return _repository.GetById(id);
        }

        public bool Insert(CongViecChungViewModel model)
        {
            return _repository.Insert(model);
        }

        public List<CongViecChungThongKeGridVo> ThongKeCongViec(CongViecChungThongKeQueryViewModel query)
        {
            return _repository.ThongKeCongViec(query);
        }

        public bool Update(CongViecChungViewModel model)
        {
            return _repository.Update(model);
        }
    }
}
