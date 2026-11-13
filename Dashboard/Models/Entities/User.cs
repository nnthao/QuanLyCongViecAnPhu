using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Script.Serialization;

namespace Dashboard.Models.Entities
{
    [Table("Users")]
    public class User
    {
        public User()
        {
            NguoiThamGias = new List<NguoiThamGia>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        [NotMapped]
        public string Code { get; set; }
        [NotMapped]
        public string CurentCode { get; set; }
        public string Guid { get; set; }
        public int? VaiTroId { get; set; }
        public int? CoQuanId { get; set; }
        public int? PhongBanId { get; set; }
        public int? ChucDanhId { get; set; }
        public bool? IsSystemUser { get; set; }
        public bool? IsDeleted { get; set; }
        public string SearchStr { get; set; }
        public int? LoginFail { get; set; }
        public string NhomVaiTroId { get; set; }
        public string TelegramId { get; set; }
        public bool? IsLanhDaoPhong { get; set; }
        public bool? IsTruongPhong { get; set; }
        public string UserZaloId { get; set; }

        [NotMapped]
        public string NewPassword { get; set; }
        [NotMapped]
        public string RePassword { get; set; }
        [NotMapped]
        public bool? IsNewPassword { get; set; }
        public DateTime? TelegramDateUpdate { get; internal set; }
        public virtual ICollection<NguoiThamGia> NguoiThamGias { get; set; }
    }

    public class UserGridVo
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public bool? IsDeleted { get; set; }
        public int? VaiTroId { get; set; }
        public int? ChucDanhId { get; set; }
        public string TenVaiTro { get; set; }
        public int? CoQuanId { get; set; }
        public int? PhongBanId { get; set; }
        public string PhongBan { get; set; }
        public bool? IsSystemUser { get; set; }
        [ScriptIgnore]
        public string Password { get; set; }
        [ScriptIgnore]
        public string Guid { get; set; }
        public string Quyen { get; set; }
        public string ChucDanh { get; set; }
        public string NhomVaiTroId { get; set; }
        public List<int> NhomVaiTroIdInt
        {
            get
            {
                var datas = new List<int>();
                if (VaiTroId.GetValueOrDefault() != 0)
                {
                    datas.Add(VaiTroId.GetValueOrDefault());
                }
                if (!string.IsNullOrEmpty(NhomVaiTroId))
                {
                    datas.AddRange(NhomVaiTroId.Split(',').Where(o => !string.IsNullOrEmpty(o))
                        .Select(o => int.Parse(o)).ToList());

                }

                return datas.Distinct().ToList();
            }
        }
        public int? LoginFail { get; set; }
        public string TelegramId { get; set; }
        public bool? IsNguoiCoQuyenDuyetLichHop { get; set; }
        public bool? IsLaiXeKhuVuc { get; set; }
        public bool? IsLanhDaoPhong { get; set; }
        public bool? IsTruongPhong { get; set; }
        public string UserZaloId { get; set; }
    }

    public class UserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string NewPassword { get; set; }
        public string RePassword { get; set; }
        public int? VaiTroId { get; set; }
        public string CurrentPassword { get; set; }
        public string DiaChi { get; set; }
        public int? PhongBanId { get; set; }
        public int? ChucDanhId { get; set; }
        public bool? IsTaiXe { get; set; }
        public string NhomVaiTroId { get; set; }
        public bool? IsNguoiCoQuyenDuyetLichHop { get; set; }
        public string TelegramId { get; set; }
        public int? CoQuanId { get; set; }
        public bool? IsLanhDaoPhong { get; set; }
        public bool? IsTruongPhong { get; set; }
        public string UserZaloId { get; set; }
    }
}
