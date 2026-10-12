using Dashboard.Models.GridVo;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Dashboard.Services
{
    public interface ILichHopThongKeService
    {
        ThongKeLyDoCuocHopResponse GetThongKeLyDoCuocHop(QueryParamViewModel queryViewModel);
        List<ThongKeNguoiCoNhieuLyDoGridVo> GetTop10NguoiCoNhieuLyDo(QueryParamViewModel queryViewModel);
        DataTableResult<ChiTietLyDoCuocHopGridVo> GetChiTietLyDoCuocHop(QueryParamViewModel queryViewModel);
    }

    public class LichHopThongKeService : ILichHopThongKeService
    {
        private readonly ILichHopThongKeRepository _repository;

        public LichHopThongKeService(ILichHopThongKeRepository repository)
        {
            _repository = repository;
        }

        public ThongKeLyDoCuocHopResponse GetThongKeLyDoCuocHop(QueryParamViewModel queryViewModel)
        {
            var data = _repository.GetThongKeLyDoCuocHop(queryViewModel);

            var response = new ThongKeLyDoCuocHopResponse
            {
                Data = data,
                Labels = data.Select(x => x.TenLyDo).ToList(),
                SoLuongData = data.Select(x => x.SoLuong).ToList(),
                MaMauData = data.Select(x => x.MaMau).ToList()
            };

            return response;
        }

        public List<ThongKeNguoiCoNhieuLyDoGridVo> GetTop10NguoiCoNhieuLyDo(QueryParamViewModel queryViewModel)
        {
            return _repository.GetTop10NguoiCoNhieuLyDo(queryViewModel);
        }

        public DataTableResult<ChiTietLyDoCuocHopGridVo> GetChiTietLyDoCuocHop(QueryParamViewModel queryViewModel)
        {
            return _repository.GetChiTietLyDoCuocHop(queryViewModel);
        }
    }
}

