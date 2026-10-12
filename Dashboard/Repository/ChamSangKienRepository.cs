using Dashboard.Models;
using Dashboard.Models.Entities;
using System.Linq;

namespace Dashboard.Repository
{
    public interface IChamSangKienRepository
    {
        ChamSangKienViewModel GetBySangKien(int sangKienId, int userId);
        bool LuuChamDiem(ChamSangKienViewModel viewModel);
        IQueryable<ChamSangKien> GetAllBySangKien(int sangKienId);
    }

    public class ChamSangKienRepository : IChamSangKienRepository
    {
        private readonly IContext _context;

        public ChamSangKienRepository(IContext context)
        {
            _context = context;
        }

        public ChamSangKienViewModel GetBySangKien(int sangKienId, int userId)
        {
            var item = _context.ChamSangKiens.AsQueryable()
                .FirstOrDefault(o => o.SangKienId == sangKienId && o.UserId == userId);

            if (item == null)
            {
                return new ChamSangKienViewModel
                {
                    SangKienId = sangKienId,
                    UserId = userId
                };
            }

            return new ChamSangKienViewModel
            {
                Id = item.Id,
                SangKienId = item.SangKienId,
                UserId = item.UserId,
                KetQuaChiTiet = item.KetQuaChiTiet
            };
        }

        public bool LuuChamDiem(ChamSangKienViewModel viewModel)
        {
            var item = _context.ChamSangKiens
                .FirstOrDefault(o => o.SangKienId == viewModel.SangKienId && o.UserId == viewModel.UserId);

            if (item == null)
            {
                item = new ChamSangKien
                {
                    SangKienId = viewModel.SangKienId,
                    UserId = viewModel.UserId,
                    KetQuaChiTiet = viewModel.KetQuaChiTiet
                };
                _context.ChamSangKiens.Add(item);
            }
            else
            {
                item.KetQuaChiTiet = viewModel.KetQuaChiTiet;
            }

            _context.SaveChanges();
            return true;
        }

        public IQueryable<ChamSangKien> GetAllBySangKien(int sangKienId)
        {
            return _context.ChamSangKiens.AsQueryable().Where(o => o.SangKienId == sangKienId);
        }
    }
}

