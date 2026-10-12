using Dashboard.Common;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;

namespace Dashboard.Services
{
    public interface IDashboardService
    {
        DashboardDataResponseVo GetDashboardData(DashboardQueryViewModel query);
        DataTableResult<DashboardCongViecChiTietVo> GetListChiTietCongViecDashboard(DashboardDetailQueryViewModel query);
    }

    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repository;

        public DashboardService(IDashboardRepository repository)
        {
            _repository = repository;
        }

        public DashboardDataResponseVo GetDashboardData(DashboardQueryViewModel query)
        {
            var response = new DashboardDataResponseVo
            {
                Summary = _repository.GetSummaryMetrics(query),
                DepartmentStats = _repository.GetDepartmentStats(query),
                RiskDepartments = _repository.GetRiskDepartments(query),
                RiskMembers = _repository.GetRiskMembers(query),
                TodaySchedule = _repository.GetTodaySchedule(),
                Notifications = _repository.GetRecentNotifications()
            };

            return response;
        }

        public DataTableResult<DashboardCongViecChiTietVo> GetListChiTietCongViecDashboard(DashboardDetailQueryViewModel query)
        {
            return _repository.GetListChiTietCongViecDashboard(query);
        }
    }
}
