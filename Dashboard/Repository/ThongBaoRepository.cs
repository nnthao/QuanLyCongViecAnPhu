using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;

namespace Dashboard.Repository
{
    public interface IThongBaoRepository
    {
        DataTableResult<ThongBaoGridVo> GetAll(QueryParamViewModel queryParams);
    }

    public class ThongBaoRepository : IThongBaoRepository
    {
        private readonly IContext _context;
        public ThongBaoRepository(IContext context)
        {
            _context = context;
        }
        //required
        private readonly string sortStringDefault = "NgayTao desc";
        private readonly List<string> searchCol = new List<string> { "NoiDung" };

        public DataTableResult<ThongBaoGridVo> GetAll(QueryParamViewModel queryParams)
        {
            //required
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.ThongBaos.AsNoTracking().AsQueryable();
            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            if (queryParams.CurrentUserId != 0 && !queryParams.IsSystemUser)
            {
                query = query.Where(o => o.NguoiNhanId == queryParams.CurrentUserId);
            }

            var dateTimeCurrent = DateTime.Now;
            var data = query.Select(cv => new ThongBaoGridVo
            {
                Id = cv.Id,
                NoiDung = cv.NoiDung,
                NgayTao = cv.NgayTao,
                IsDaXem = cv.IsDaXem,
                EnumLoaiThongBao = cv.EnumLoaiThongBao,
                NguoiNhanId = cv.NguoiNhanId,
                ThongBaoInfoJson = cv.ThongBaoInfoJson,
            }).Where(queryParams.searchString);

            var total = data.Count();

            return new DataTableResult<ThongBaoGridVo>
            {
                rows = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList(),
                total = total
            };
        }


    }
}