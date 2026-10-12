using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;

namespace Dashboard.Services
{
    public interface INhomNhanViecService
    {
        DataTableResult<NhomNhanViecGridVo> GetAll(NhomNhanViecQueryViewModel queryParams);
        NhomNhanViecGridVo GetById(int id);
        object GetListNhomNhanViecDeChon(int currentUserId);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(NhomNhanViecViewModel model);
        bool Xoa(int id);
    }

    public class NhomNhanViecService : INhomNhanViecService
    {
        private readonly INhomNhanViecRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IPhongBanRepository _phongBanRepository;

        public NhomNhanViecService(INhomNhanViecRepository repository, IUserRepository userRepository, IPhongBanRepository phongBanRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
            _phongBanRepository = phongBanRepository;
        }

        public DataTableResult<NhomNhanViecGridVo> GetAll(NhomNhanViecQueryViewModel queryParams)
        {
            return _repository.GetAll(queryParams);
        }

        public NhomNhanViecGridVo GetById(int id)
        {
            return _repository.GetById(id);
        }

        public object GetListNhomNhanViecDeChon(int currentUserId)
        {
            return _repository.GetListNhomNhanViecDeChon(currentUserId);
        }

        public bool KhoiPhuc(int id)
        {
            return _repository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(NhomNhanViecViewModel model)
        {
            return _repository.ThemOrCapNhat(model);
        }

        public bool Xoa(int id)
        {
            return _repository.Xoa(id);
        }
    }
}
