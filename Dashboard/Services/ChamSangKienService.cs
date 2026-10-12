using Dashboard.Models.Entities;
using Dashboard.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Dashboard.Services
{
    public interface IChamSangKienService
    {
        ChamSangKienViewModel GetBySangKien(int sangKienId, int userId);
        decimal? LuuChamDiem(ChamSangKienViewModel viewModel);
    }

    public class ChamSangKienService : IChamSangKienService
    {
        private readonly IChamSangKienRepository _repository;

        public ChamSangKienService(IChamSangKienRepository repository)
        {
            _repository = repository;
        }

        public ChamSangKienViewModel GetBySangKien(int sangKienId, int userId)
        {
            return _repository.GetBySangKien(sangKienId, userId);
        }

        public decimal? LuuChamDiem(ChamSangKienViewModel viewModel)
        {
            _repository.LuuChamDiem(viewModel);

            var entries = _repository.GetAllBySangKien(viewModel.SangKienId).ToList();
            if (entries.Count == 0) return null;

            var totals = new List<decimal>();
            foreach (var entry in entries)
            {
                if (string.IsNullOrWhiteSpace(entry.KetQuaChiTiet)) continue;
                try
                {
                    var details = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ChamDiemChiTiet>>(entry.KetQuaChiTiet);
                    if (details == null || details.Count == 0) continue;
                    var total = details.Where(x => x.diem.HasValue).Sum(x => x.diem.Value);
                    totals.Add(total);
                }
                catch
                {
                }
            }

            if (totals.Count == 0) return null;
            return (decimal?)totals.Average();
        }

        private class ChamDiemChiTiet
        {
            public int id { get; set; }
            public decimal? diem { get; set; }
        }
    }
}

