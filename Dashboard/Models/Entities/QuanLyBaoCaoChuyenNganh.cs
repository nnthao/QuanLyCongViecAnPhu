using Dashboard.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Dashboard.Models.Entities
{
    [Table("QuanLyBaoCaoChuyenNganh")]
    public partial class QuanLyBaoCaoChuyenNganh
    {
        public QuanLyBaoCaoChuyenNganh()
        {
        }

        public int Id { get; set; }
        public DanhMucBaoCaoEnum? DanhMucBaoCaoId { get; set; }
        public string TenFile { get; set; }
        public string DuongDanFile { get; set; }
        public QuanLyBaoCaoChuyenNganhEnum? QuanLyBaoCaoChuyenNganhEnumId { get; set; }
        public string QueryInfo { get; set; }
        public int? NguoiTaoId { get; set; }
        public DateTime? NgayTao { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class QuanLyBaoCaoChuyenNganhViewModel
    {
        public int Id { get; set; }
        public DanhMucBaoCaoEnum? DanhMucBaoCaoId { get; set; }
        public string TenFile { get; set; }
        public string DuongDanFile { get; set; }
        public QuanLyBaoCaoChuyenNganhEnum? QuanLyBaoCaoChuyenNganhEnumId { get; set; }
        public string QueryInfo { get; set; }
        public int? NguoiTaoId { get; set; }
        public DateTime? NgayTao { get; set; }
        public int? CurrentUserId { get; set; }
    }

    public class QuanLyBaoCaoChuyenNganhGridVo
    {
        public int Id { get; set; }
        public DanhMucBaoCaoEnum? DanhMucBaoCaoId { get; set; }
        public string DanhMucBaoCao { get; set; }
        public string TenFile { get; set; }
        public string DuongDanFile { get; set; }
        public QuanLyBaoCaoChuyenNganhEnum? QuanLyBaoCaoChuyenNganhEnumId { get; set; }
        public string QuanLyBaoCaoChuyenNganh { get; set; }
        public string QueryInfo { get; set; }
        public int? NguoiTaoId { get; set; }
        public string NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string NgayTaoStr
        {
            get
            {
                return DateTimeHelper.ConvertDateTimeToDateTime(NgayTao);
            }
        }

        public BaoCaoChuyenNganhQueryInfo BaoCaoChuyenNganhQueryInfo
        {
            get
            {
                if (!string.IsNullOrEmpty(QueryInfo))
                {
                    return JsonConvert.DeserializeObject<BaoCaoChuyenNganhQueryInfo>(QueryInfo);
                }

                return null;
            }
        }
        public string BaoCaoChuyenNganhQueryInfoStr
        {
            get
            {
                var list = new List<string>();
                if (BaoCaoChuyenNganhQueryInfo != null)
                {
                    if (QuanLyBaoCaoChuyenNganhEnumId == QuanLyBaoCaoChuyenNganhEnum.BaoCaoCongViec)
                    {
                        if (BaoCaoChuyenNganhQueryInfo.TuNgay != null)
                        {
                            list.Add("Từ ngày: " + DateTimeHelper.ConvertDateTimeToDateOnly(BaoCaoChuyenNganhQueryInfo.TuNgay));
                        }
                        if (BaoCaoChuyenNganhQueryInfo.DenNgay != null)
                        {
                            list.Add("Đến ngày: " + DateTimeHelper.ConvertDateTimeToDateOnly(BaoCaoChuyenNganhQueryInfo.DenNgay));
                        }

                        if (!string.IsNullOrEmpty(BaoCaoChuyenNganhQueryInfo.PhongBanIdsStr))
                        {
                            var ids = BaoCaoChuyenNganhQueryInfo.PhongBanIdsStr.Split(',')
                                .Where(o => !string.IsNullOrWhiteSpace(o))
                                .Select(o => Convert.ToInt32(o))
                                .ToList();
                            if (ids.Count != 0)
                            {
                                var pbs = CacheManagerHelper.GetPhongBanCache().Where(pb => ids.Contains(pb.Id)).Select(pb => pb.Ten).ToList();
                                if (pbs.Count != 0)
                                {
                                    list.Add("Phòng ban: " + string.Join(", ", pbs));
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(BaoCaoChuyenNganhQueryInfo.ThanhVienIdListStr))
                        {
                            var ids = BaoCaoChuyenNganhQueryInfo.ThanhVienIdListStr.Split(',')
                                .Where(o => !string.IsNullOrWhiteSpace(o))
                                .Select(o => Convert.ToInt32(o))
                                .ToList();
                            if (ids.Count != 0)
                            {
                                var users = CacheManagerHelper.GetUserCache().Where(u => ids.Contains(u.Id)).Select(u => u.Name).ToList();
                                if (users.Count != 0)
                                {
                                    list.Add("Thành viên: " + string.Join(", ", users));
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(BaoCaoChuyenNganhQueryInfo.IsHoanThanhTypeListStr))
                        {
                            var map = new Dictionary<int, string>
                            {
                                {1, "Hoàn thành sớm hạn"},
                                {2, "Hoàn thành đúng hạn"},
                                {3, "Chưa hoàn thành còn hạn"},
                                {4, "Trễ hạn"}
                            };
                            var ids = BaoCaoChuyenNganhQueryInfo.IsHoanThanhTypeListStr.Split(',')
                                .Where(o => !string.IsNullOrWhiteSpace(o))
                                .Select(o => Convert.ToInt32(o))
                                .ToList();
                            var names = ids.Where(map.ContainsKey).Select(i => map[i]).ToList();
                            if (names.Count != 0)
                            {
                                list.Add("Trạng thái: " + string.Join(", ", names));
                            }
                        }
                    }
                }


                return string.Join("; ", list);
            }
        }

        public bool IsDeleted { get; set; }
    }

    public enum QuanLyBaoCaoChuyenNganhEnum : int
    {
        [Description("Báo cáo công việc")]
        BaoCaoCongViec = 1,
        [Description("Báo cáo công trình")]
        BaoCaoCongTrinh = 2,
    }

    public enum DanhMucBaoCaoEnum : int
    {
        [Description("Công việc")]
        CongViec = 1,
        [Description("Công trình")]
        CongTrinh = 2,
    }

    public class BaoCaoChuyenNganhQueryInfo
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string PhongBanIdsStr { get; set; }
        public string IsHoanThanhTypeListStr { get; set; }
        public string ThanhVienIdListStr { get; set; }
    }
}
