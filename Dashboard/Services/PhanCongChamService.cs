using Dashboard.Models.Entities;
using Dashboard.Repository;
using System;
using System.Collections.Generic;

namespace Dashboard.Services
{
    public interface IPhanCongChamService
    {
        List<PhanCongChamViewModel> GetBySangKien(int sangKienId);
        bool Them(PhanCongChamViewModel viewModel);
        bool Xoa(int id);
    }

    public class PhanCongChamService : IPhanCongChamService
    {
        private readonly IPhanCongChamRepository _repository;

        public PhanCongChamService(IPhanCongChamRepository repository)
        {
            _repository = repository;
        }

        public List<PhanCongChamViewModel> GetBySangKien(int sangKienId)
        {
            return _repository.GetBySangKien(sangKienId);
        }

        public bool Them(PhanCongChamViewModel viewModel)
        {
            viewModel.NgayPhanCong = DateTime.Now;
            return _repository.Them(viewModel);
        }

        public bool Xoa(int id)
        {
            return _repository.Xoa(id);
        }
    }
}

