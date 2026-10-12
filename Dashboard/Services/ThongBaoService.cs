using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Linq;

namespace Dashboard.Services
{
    public interface IThongBaoService
    {
        DataTableResult<ThongBaoGridVo> GetAll(QueryParamViewModel queryParams);
    }

    public class ThongBaoService : IThongBaoService
    {
        private readonly IThongBaoRepository _thongBaoRepository;
        public ThongBaoService(IThongBaoRepository thongBaoRepository)
        {
            _thongBaoRepository = thongBaoRepository;



        }

        public DataTableResult<ThongBaoGridVo> GetAll(QueryParamViewModel queryParams)
        {
            var data = _thongBaoRepository.GetAll(queryParams);
            if (queryParams.IsSystemUser)
            {
                var users = CacheManagerHelper.GetUserCache();
                foreach (var item in data.rows)
                {
                    var usr = users.FirstOrDefault(o => o.Id == item.NguoiNhanId);
                    if (usr != null)
                    {
                        item.NguoiNhan = usr.Name;
                    }
                }
            }

            return data;
        }
    }
}