using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.Validates;
using Dashboard.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using User = Dashboard.Models.Entities.User;

namespace Dashboard.Repository
{
    public interface IUserRepository
    {
        dynamic CapNhat(UserViewModel userViewModel);
        dynamic CapNhatMatKhau(UserViewModel userViewModel);
        DataTableResult<UserGridVo> GetAll(UserQueryViewModel queryParams);
        UserGridVo GetById(int id);
        List<string> GetListDataScriptVersions();
        UserGridVo GetUserByIdFromCahe(int id);
        UserGridVo GetUserDangNhap(string email);
        bool KhoiPhuc(int id);
        dynamic Them(UserViewModel userViewModel);
        DataLookupViewModel UserLookup(LookupQueryViewModel lookupQueryViewModel);
        bool Xoa(int id);
        bool XoaKhoiDatabase(int id);
        bool MoKhoaTaiKhoan(int id);
        bool ResetMatKhau(int id);
        dynamic CapNhatTelegramId(UserViewModel userViewModel);
        dynamic ImportDanhSach(List<Models.GridVo.UserImportGridVo> danhSachUser);
        dynamic CapNhatSoDienThoaiTuExcel(List<UserCapNhatSoDienThoaiImportVo> danhSach);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IMapper _iMapper = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserViewModel, User>()
                .ForMember(d => d.Password, opt => opt.Ignore())
                .ForMember(d => d.Guid, opt => opt.Ignore())
                .ForMember(x => x.SearchStr, opt => opt.Ignore())
                .AfterMap((s, d) =>
                {
                    d.VaiTroId = s.VaiTroId == 0 ? null : s.VaiTroId;
                    d.UserZaloId = string.IsNullOrWhiteSpace(s.UserZaloId) || s.UserZaloId.Trim().Equals("null", StringComparison.OrdinalIgnoreCase) ? null : s.UserZaloId.Trim();
                    d.TelegramId = string.IsNullOrWhiteSpace(s.TelegramId) || s.TelegramId.Trim().Equals("null", StringComparison.OrdinalIgnoreCase) ? null : s.TelegramId.Trim();
                    d.SearchStr = (!string.IsNullOrEmpty(s.Name) ? s.Name : "") + " "
                                      + (!string.IsNullOrEmpty(s.Email) ? s.Email : "") + " "
                                      + (!string.IsNullOrEmpty(s.Phone) ? s.Phone : "") + " "
                                      + (!string.IsNullOrEmpty(d.UserZaloId) ? d.UserZaloId : "");
                });
        }, NullLoggerFactory.Instance).CreateMapper();

        private readonly IContext _context;
        private readonly IMatKhauValidate _matKhauValidate;
        private readonly IUserValidate _userValidate;
        public UserRepository(IContext context, IMatKhauValidate matKhauValidate,
            IUserValidate userValidate)
        {
            _context = context;
            _matKhauValidate = matKhauValidate;
            _userValidate = userValidate;
        }

        //required
        private readonly string sortStringDefault = "Name asc";
        private readonly List<string> searchCol = new List<string> { "Name", "Email" };
        public DataTableResult<UserGridVo> GetAll(UserQueryViewModel queryParams)
        {
            //required
            queryParams.sortStringDefault = sortStringDefault;
            queryParams.searchColumn = searchCol;

            var query = _context.Users.AsNoTracking().Where(o => o.IsSystemUser != true);

            if (queryParams.VaiTroIdSearch > 0)
            {
                query = query.Where(o => o.VaiTroId == queryParams.VaiTroIdSearch);
            }

            if (queryParams.DaXoaSearch == true)
            {
                query = query.Where(o => o.IsDeleted == true);
            }
            else
            {
                query = query.Where(o => o.IsDeleted != true);
            }

            if (queryParams.PhongBanId > 0)
            {
                query = query.Where(o => o.PhongBanId == queryParams.PhongBanId);
            }

            if (queryParams.CoTelegramSearch)
            {
                query = query.Where(o => o.TelegramId != null && o.TelegramId != "");
            }

            if (queryParams.ChuaKetNoiTelegramSearch)
            {
                query = query.Where(o => o.TelegramId == null || o.TelegramId == "");
            }

            if (queryParams.TaiKhoanBiKhoaSearch)
            {
                query = query.Where(o => o.LoginFail != null && o.LoginFail >= 5);
            }

            var data = query.Select(cv => new UserGridVo
            {
                Id = cv.Id,
                Name = cv.Name,
                Email = cv.Email,
                Phone = cv.Phone,
                TelegramId = cv.TelegramId,
                UserZaloId = cv.UserZaloId,
                VaiTroId = cv.VaiTroId,
                IsDeleted = cv.IsDeleted,
                PhongBanId = cv.PhongBanId,
                ChucDanhId = cv.ChucDanhId,
                NhomVaiTroId = cv.NhomVaiTroId,
                LoginFail = cv.LoginFail,
            }).Where(queryParams.searchString);

            var total = data.Count();
            var result = data.OrderBy(queryParams.sortString).Skip(queryParams.offset).Take(queryParams.limit).ToList();

            if (result.Count != 0)
            {
                foreach (var item in result)
                {
                    MapThongTinVaiTroChucDanhPhongBanChoUser(item, new User
                    {
                        VaiTroId = item.VaiTroId,
                        NhomVaiTroId = item.NhomVaiTroId,
                        PhongBanId = item.PhongBanId,
                        ChucDanhId = item.ChucDanhId
                    });
                }
            }

            return new DataTableResult<UserGridVo>
            {
                rows = result,
                total = total
            };
        }

        public dynamic Them(UserViewModel userViewModel)
        {
            var item = _iMapper.Map<UserViewModel, User>(userViewModel);
            item.IsNewPassword = true;

            var errors = _userValidate.Validate(item);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            var guid = Guid.NewGuid().ToString();
            item.Guid = guid;
            item.Password = PasswordHelper.HashString(guid, item.NewPassword);

            _context.Users.Add(item);
            _context.SaveChanges();
            CacheManagerHelper.InitUserCache();

            return new { Id = item.Id };
        }

        public dynamic CapNhat(UserViewModel userViewModel)
        {
            var item = _context.Users.FirstOrDefault(o => o.Id == userViewModel.Id);
            if (item == null)
            {
                return new { Id = 0 };
            }

            _iMapper.Map(userViewModel, item);

            if (!string.IsNullOrEmpty(userViewModel.NewPassword) && !string.IsNullOrEmpty(userViewModel.NewPassword.Trim()))
            {
                item.IsNewPassword = true;
            }
            var errors = _userValidate.Validate(item);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            if (item.IsNewPassword != null && item.IsNewPassword == true)
            {
                item.Password = PasswordHelper.HashString(item.Guid, item.NewPassword);
            }
            _context.SaveChanges();
            CacheManagerHelper.InitUserCache();
            return new { Id = item.Id };
        }

        public UserGridVo GetById(int id)
        {
            var user = new UserGridVo();
            var item = _context.Users.AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                user.Id = item.Id;
                user.Name = item.Name;
                user.Email = item.Email;
                user.Phone = item.Phone;
                user.VaiTroId = item.VaiTroId;
                user.NhomVaiTroId = item.NhomVaiTroId;
                user.ChucDanhId = item.ChucDanhId;
                user.PhongBanId = item.PhongBanId;
                user.TelegramId = item.TelegramId;
                user.UserZaloId = item.UserZaloId;
                user.IsLanhDaoPhong = item.IsLanhDaoPhong;
                MapThongTinVaiTroChucDanhPhongBanChoUser(user, item);
            }

            return user;
        }

        private static void MapThongTinVaiTroChucDanhPhongBanChoUser(UserGridVo user, User item)
        {
            if (item.PhongBanId.GetValueOrDefault() != 0)
            {
                var phongBan = CacheManagerHelper.GetPhongBanByIdFromCache(item.PhongBanId.Value);
                if (phongBan != null)
                {
                    user.PhongBan = phongBan.Ten;
                }
            }

            if (item.ChucDanhId.GetValueOrDefault() != 0)
            {
                var chucDanh = CacheManagerHelper.GetChucDanhByIdFromCache(item.ChucDanhId.Value);
                if (chucDanh != null)
                {
                    user.ChucDanh = chucDanh.Ten;
                }
            }

            var vaiTros = new List<string>();
            var nhomVaiTroIds = !string.IsNullOrEmpty(item.NhomVaiTroId)
              ? item.NhomVaiTroId.Split(',').Where(o => !string.IsNullOrEmpty(o))
              .Select(o => int.Parse(o)).ToList() : new List<int>();

            if (item.VaiTroId.GetValueOrDefault() != 0)
            {
                nhomVaiTroIds.Insert(0, item.VaiTroId.Value);
            }

            foreach (var vt in nhomVaiTroIds.Distinct())
            {
                var vaiTro = CacheManagerHelper.GetVaiTroByIdFromCache(vt);
                if (vaiTro != null)
                {
                    vaiTros.Add(vaiTro.Ten);
                }
            }

            user.TenVaiTro = string.Join(", ", vaiTros);
        }

        public bool Xoa(int id)
        {
            var item = _context.Users.FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                item.IsDeleted = true;
                _context.SaveChanges();
                CacheManagerHelper.InitUserCache();

                return true;
            }

            return false;
        }

        public bool XoaKhoiDatabase(int id)
        {
            var item = _context.Users.FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                _context.Users.Remove(item);
                _context.SaveChanges();
                CacheManagerHelper.InitUserCache();

                return true;
            }

            return false;
        }

        public bool KhoiPhuc(int id)
        {
            var item = _context.Users.FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                item.IsDeleted = false;
                _context.SaveChanges();
                CacheManagerHelper.InitUserCache();

                return true;
            }

            return false;
        }

        public List<string> GetListDataScriptVersions()
        {
            return _context.DatabaseVersions.AsNoTracking().Select(o => o.Name).ToList();
        }

        public UserGridVo GetUserDangNhap(string email)
        {
            var user = new UserGridVo();
            var searchEmail = email?.Trim();
            var searchEmailWithAt = searchEmail + "@";
            var item = _context.Users.AsNoTracking().FirstOrDefault(n =>
                (n.Email == searchEmail || (!searchEmail.Contains("@") && n.Email.StartsWith(searchEmailWithAt)))
                && n.IsDeleted != true);

            if (item != null)
            {
                user.Id = item.Id;
                user.Name = item.Name;
                user.Email = item.Email;
                user.Phone = item.Phone;
                user.VaiTroId = item.VaiTroId;
                user.IsSystemUser = item.IsSystemUser != null ? item.IsSystemUser : false;
                user.Password = item.Password;
                user.Guid = item.Guid;
                user.PhongBanId = item.PhongBanId;
                user.LoginFail = item.LoginFail;
                user.NhomVaiTroId = item.NhomVaiTroId;
                user.CoQuanId = item.CoQuanId;
            }

            return user;
        }

        public dynamic CapNhatMatKhau(UserViewModel userViewModel)
        {
            var errors = _matKhauValidate.Validate(userViewModel);
            if (errors.Count > 0)
            {
                return new { Errors = errors };
            }

            var item = _context.Users.FirstOrDefault(o => o.Id == userViewModel.Id);
            if (item != null)
            {
                item.Password = PasswordHelper.HashString(item.Guid, userViewModel.NewPassword);
                _context.SaveChanges();

                return new { Id = item.Id };
            }

            return new { Id = 0 };
        }

        public bool MoKhoaTaiKhoan(int id)
        {
            var item = _context.Users.FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                item.LoginFail = 0;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool ResetMatKhau(int id)
        {
            var item = _context.Users.FirstOrDefault(o => o.Id == id);
            if (item != null)
            {
                item.Password = PasswordHelper.HashString(item.Guid, "123456");
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public DataLookupViewModel UserLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            var data = new List<LookupViewModel>
            {
                new LookupViewModel
                {
                    id = 0,
                    text = "--- Chọn ---"
                }
            };

            var users = CacheManagerHelper.GetUserCache().Where(o => o.IsDeleted != true).ToList();
            if (lookupQueryViewModel.isGetAll)
            {
                users = CacheManagerHelper.GetUserCache().ToList();
            }

            if (lookupQueryViewModel.phongBanId != 0)
            {
                users = users.Where(o => o.PhongBanId == lookupQueryViewModel.phongBanId).ToList();
            }

            if (lookupQueryViewModel.phongBanIds != null && lookupQueryViewModel.phongBanIds.Count > 0)
            {
                users = users.Where(o => lookupQueryViewModel.phongBanIds.Contains(o.PhongBanId.GetValueOrDefault())).ToList();
            }

            var phongBans = UltiHelper.GetPhongBanTreeSorted();
            var chucDanhs = CacheManagerHelper.GetChucDanhCache();

            var result = new List<LookupViewModel>();
            var processedUserIds = new HashSet<int>();

            foreach (var pb in phongBans)
            {
                var usersInPb = users.Where(u => u.PhongBanId == pb.Id)
                    .OrderBy(us =>
                    {
                        var cd = chucDanhs.FirstOrDefault(c => c.Id == us.ChucDanhId);
                        return cd != null && cd.ThuTu.HasValue ? cd.ThuTu.Value : int.MaxValue;
                    })
                    .ThenBy(us => us.Name)
                    .ToList();

                foreach (var us in usersInPb)
                {
                    processedUserIds.Add(us.Id);
                    var cd = chucDanhs.FirstOrDefault(c => c.Id == us.ChucDanhId);
                    var chucDanhTen = cd != null && !string.IsNullOrEmpty(cd.Ten) ? $"({cd.Ten})" : "";
                    var phongBanTen = !string.IsNullOrEmpty(pb.Ten) ? $"({pb.Ten})" : "";

                    string textDisplay;
                    if (lookupQueryViewModel.tenKemTen)
                    {
                        var parts = new List<string> { us.Name };
                        if (!string.IsNullOrEmpty(chucDanhTen)) parts.Add(chucDanhTen);
                        if (!string.IsNullOrEmpty(phongBanTen)) parts.Add(phongBanTen);
                        textDisplay = string.Join(" ", parts);
                    }
                    else
                    {
                        textDisplay = us.Name;
                    }

                    result.Add(new LookupViewModel
                    {
                        id = us.Id,
                        text = textDisplay,
                        phongBanId = us.PhongBanId
                    });
                }
            }

            var usersNoPb = users.Where(u => !processedUserIds.Contains(u.Id))
                .OrderBy(us => us.Name)
                .ToList();

            foreach (var us in usersNoPb)
            {
                var cd = chucDanhs.FirstOrDefault(c => c.Id == us.ChucDanhId);
                var chucDanhTen = cd != null && !string.IsNullOrEmpty(cd.Ten) ? $"({cd.Ten})" : "";

                string textDisplay;
                if (lookupQueryViewModel.tenKemTen)
                {
                    var parts = new List<string> { us.Name };
                    if (!string.IsNullOrEmpty(chucDanhTen)) parts.Add(chucDanhTen);
                    textDisplay = string.Join(" ", parts);
                }
                else
                {
                    textDisplay = us.Name;
                }

                result.Add(new LookupViewModel
                {
                    id = us.Id,
                    text = textDisplay,
                    phongBanId = us.PhongBanId
                });
            }

            if (lookupQueryViewModel.isHideOptionChonValueZero)
            {
                return new DataLookupViewModel
                {
                    Data = result
                };
            }

            return new DataLookupViewModel
            {
                Data = data.Concat(result).ToList()
            };
        }

        public UserGridVo GetUserByIdFromCahe(int id)
        {
            var users = CacheManagerHelper.GetUserCache();
            var user = users.FirstOrDefault(x => x.Id == id);
            if (user != null)
            {
                var phongBan = CacheManagerHelper.GetPhongBanByIdFromCache(user.PhongBanId.GetValueOrDefault());
                if (phongBan != null)
                {
                    user.PhongBan = phongBan.Ten;
                }

                var chucDanh = CacheManagerHelper.GetChucDanhByIdFromCache(user.ChucDanhId.GetValueOrDefault());
                if (chucDanh != null)
                {
                    user.ChucDanh = chucDanh.Ten;
                }

                return user;
            }

            return new UserGridVo();
        }

        public dynamic CapNhatTelegramId(UserViewModel userViewModel)
        {
            var item = _context.Users.FirstOrDefault(o => o.Id == userViewModel.Id);
            if (item != null)
            {
                item.TelegramId = userViewModel.TelegramId;
                item.TelegramDateUpdate = DateTime.Now;
                _context.SaveChanges();

                return new { Id = item.Id };
            }

            return new { Id = 0 };
        }

        public dynamic ImportDanhSach(List<UserImportGridVo> danhSachUser)
        {
            var ketQua = new List<UserImportGridVo>();
            var phongBanCache = CacheManagerHelper.GetPhongBanCache();
            var chucDanhCache = CacheManagerHelper.GetChucDanhCache();
            var vaiTroCache = CacheManagerHelper.GetVaiTroCache();
            var userCache = CacheManagerHelper.GetListUserFromCacheNotDeleted();
            var matKhauMacDinh = "123456";
            var coLoi = false;
            var danhSachDongLoi = new List<string>();

            for (int i = 0; i < danhSachUser.Count; i++)
            {
                var userImport = danhSachUser[i];
                var soDong = i + 2;
                var loi = new List<string>();

                if (string.IsNullOrEmpty(userImport.HoTen?.Trim()))
                {
                    loi.Add("Họ tên không được trống");
                }

                if (string.IsNullOrEmpty(userImport.Email?.Trim()))
                {
                    loi.Add("Email không được trống");
                }

                if (!string.IsNullOrEmpty(userImport.ChucDanh?.Trim()))
                {
                    var chucDanh = chucDanhCache.FirstOrDefault(cd =>
                        cd.Ten != null && cd.Ten.Trim().ToLower() == userImport.ChucDanh.Trim().ToLower() &&
                        cd.IsDeleted != true);
                    if (chucDanh != null)
                    {
                        userImport.ChucDanhId = chucDanh.Id;
                    }
                    else
                    {
                        loi.Add($"Không tìm thấy chức danh: {userImport.ChucDanh}");
                    }
                }

                if (!string.IsNullOrEmpty(userImport.PhongBan?.Trim()))
                {
                    var phongBan = phongBanCache.FirstOrDefault(pb =>
                        pb.Ten != null && pb.Ten.Trim().ToLower() == userImport.PhongBan.Trim().ToLower() &&
                        pb.IsDeleted != true);
                    if (phongBan != null)
                    {
                        userImport.PhongBanId = phongBan.Id;
                    }
                    else
                    {
                        loi.Add($"Không tìm thấy phòng ban: {userImport.PhongBan}");
                    }
                }

                if (!string.IsNullOrEmpty(userImport.VaiTro?.Trim()))
                {
                    var vaiTro = vaiTroCache.FirstOrDefault(vt =>
                        vt.Ten != null && vt.Ten.Trim().ToLower() == userImport.VaiTro.Trim().ToLower() &&
                        vt.IsDeleted != true);
                    if (vaiTro != null)
                    {
                        userImport.VaiTroId = vaiTro.Id;
                    }
                    else
                    {
                        loi.Add($"Không tìm thấy vai trò: {userImport.VaiTro}");
                    }
                }

                if (!string.IsNullOrEmpty(userImport.Email?.Trim()))
                {
                    var emailTrim = userImport.Email.Trim().ToLower();
                    var emailTrungTrongFile = danhSachUser
                        .Where((u, idx) => idx != i && !string.IsNullOrEmpty(u.Email?.Trim()) && u.Email.Trim().ToLower() == emailTrim)
                        .Any();
                    if (emailTrungTrongFile)
                    {
                        loi.Add("Email đã trùng với email khác trong file import");
                    }
                    else
                    {
                        var emailTrungTrongDatabase = userCache.Any(u =>
                            u.Email != null && u.Email.Trim().ToLower() == emailTrim);
                        if (emailTrungTrongDatabase)
                        {
                            loi.Add("Email đã tồn tại trong hệ thống");
                        }
                    }
                }

                if (!string.IsNullOrEmpty(userImport.SoDienThoai?.Trim()))
                {
                    var soDienThoaiTrim = userImport.SoDienThoai.Trim();
                    var soDienThoaiTrungTrongFile = danhSachUser
                        .Where((u, idx) => idx != i && !string.IsNullOrEmpty(u.SoDienThoai?.Trim()) && u.SoDienThoai.Trim() == soDienThoaiTrim)
                        .Any();
                    if (soDienThoaiTrungTrongFile)
                    {
                        loi.Add("Số điện thoại đã trùng với số điện thoại khác trong file import");
                    }
                    else
                    {
                        var soDienThoaiTrungTrongDatabase = userCache.Any(u =>
                            u.Phone != null && u.Phone.Trim() == soDienThoaiTrim);
                        if (soDienThoaiTrungTrongDatabase)
                        {
                            loi.Add("Số điện thoại đã tồn tại trong hệ thống");
                        }
                    }
                }

                if (loi.Count > 0)
                {
                    coLoi = true;
                    var thongBaoLoi = $"Dòng {soDong}: {string.Join("; ", loi)}";
                    userImport.Loi = thongBaoLoi;
                    danhSachDongLoi.Add(thongBaoLoi);
                }

                ketQua.Add(userImport);
            }

            if (coLoi)
            {
                var thongBaoChiTiet = string.Join("\n", danhSachDongLoi);
                return new { Data = ketQua, Errors = new List<ValidateError> { new ValidateError { Id = "ImportUser", Description = $"File import có lỗi ở các dòng sau:\n{thongBaoChiTiet}\n\nVui lòng kiểm tra và chỉnh sửa lại file trước khi import." } } };
            }

            foreach (var userImport in danhSachUser)
            {
                var userExisting = _context.Users.FirstOrDefault(u =>
                    u.Email != null && u.Email.Trim().ToLower() == userImport.Email.Trim().ToLower());

                if (userExisting != null)
                {
                    userExisting.Name = userImport.HoTen.Trim();
                    userExisting.Phone = userImport.SoDienThoai?.Trim();
                    userExisting.ChucDanhId = userImport.ChucDanhId;
                    userExisting.PhongBanId = userImport.PhongBanId;
                    userExisting.VaiTroId = userImport.VaiTroId;
                    userExisting.SearchStr = (!string.IsNullOrEmpty(userImport.HoTen) ? userImport.HoTen.Trim() : "") + " "
                        + (!string.IsNullOrEmpty(userImport.Email) ? userImport.Email.Trim() : "") + " "
                        + (!string.IsNullOrEmpty(userImport.SoDienThoai) ? userImport.SoDienThoai.Trim() : "");
                    userExisting.IsDeleted = false;

                    userImport.Loi = "Đã cập nhật thành công";
                }
                else
                {
                    var guid = Guid.NewGuid().ToString();
                    var newUser = new User
                    {
                        Name = userImport.HoTen.Trim(),
                        Email = userImport.Email.Trim(),
                        Phone = userImport.SoDienThoai?.Trim(),
                        ChucDanhId = userImport.ChucDanhId,
                        PhongBanId = userImport.PhongBanId,
                        VaiTroId = userImport.VaiTroId,
                        Guid = guid,
                        Password = PasswordHelper.HashString(guid, matKhauMacDinh),
                        SearchStr = (!string.IsNullOrEmpty(userImport.HoTen) ? userImport.HoTen.Trim() : "") + " "
                            + (!string.IsNullOrEmpty(userImport.Email) ? userImport.Email.Trim() : "") + " "
                            + (!string.IsNullOrEmpty(userImport.SoDienThoai) ? userImport.SoDienThoai.Trim() : "")
                    };

                    _context.Users.Add(newUser);
                    userImport.Loi = "Đã thêm mới thành công";
                }
            }

            _context.SaveChanges();

            CacheManagerHelper.InitUserCache();

            return new { Data = ketQua };
        }

        public dynamic CapNhatSoDienThoaiTuExcel(List<UserCapNhatSoDienThoaiImportVo> danhSach)
        {
            if (danhSach == null || !danhSach.Any())
            {
                return new
                {
                    Success = false,
                    Message = "Không tìm thấy dữ liệu trong file import.",
                    Errors = new List<ValidateError>
                    {
                        new ValidateError
                        {
                            Id = "ImportPhone",
                            Description = "Không tìm thấy dữ liệu trong file import."
                        }
                    }
                };
            }

            var errors = new List<ValidateError>();
            var danhSachLoi = new List<UserCapNhatSoDienThoaiImportVo>();
            var usersInDb = _context.Users.ToList();

            for (int i = 0; i < danhSach.Count; i++)
            {
                var item = danhSach[i];
                var itemErrors = new List<string>();

                if (string.IsNullOrWhiteSpace(item.Email))
                {
                    itemErrors.Add("Email không được để trống");
                }
                else
                {
                    var emailTrim = item.Email.Trim().ToLower();
                    if (!EmailValidateHelper.IsValidEmail(emailTrim))
                    {
                        itemErrors.Add($"Email '{item.Email}' không đúng định dạng");
                    }
                    else
                    {
                        var userInDb = usersInDb.FirstOrDefault(u => u.Email != null && u.Email.Trim().ToLower() == emailTrim);
                        if (userInDb == null)
                        {
                            itemErrors.Add($"Email '{item.Email}' chưa có trong hệ thống");
                        }
                    }

                    var isDuplicateEmailInFile = danhSach
                        .Where((x, idx) => idx != i && !string.IsNullOrWhiteSpace(x.Email) && x.Email.Trim().ToLower() == emailTrim)
                        .Any();
                    if (isDuplicateEmailInFile)
                    {
                        itemErrors.Add($"Email '{item.Email}' bị trùng lặp trong file import");
                    }
                }

                var phoneValidation = ImportUserSoDienThoaiHelper.ValidateAndNormalizePhoneNumber(item.SoDienThoaiGoc);
                if (!phoneValidation.IsValid)
                {
                    itemErrors.Add(phoneValidation.ErrorMessage);
                }
                else
                {
                    item.SoDienThoaiChuanHoa = phoneValidation.NormalizedPhone;

                    var isDuplicatePhoneInFile = danhSach
                        .Where((x, idx) => idx != i && !string.IsNullOrWhiteSpace(x.SoDienThoaiGoc))
                        .Any(x =>
                        {
                            var otherPhoneNorm = ImportUserSoDienThoaiHelper.ValidateAndNormalizePhoneNumber(x.SoDienThoaiGoc);
                            return otherPhoneNorm.IsValid && otherPhoneNorm.NormalizedPhone == item.SoDienThoaiChuanHoa;
                        });

                    if (isDuplicatePhoneInFile)
                    {
                        itemErrors.Add($"Số điện thoại '{item.SoDienThoaiChuanHoa}' bị trùng lặp nhiều dòng trong file import");
                    }
                    else if (!string.IsNullOrWhiteSpace(item.Email))
                    {
                        var emailTrim = item.Email.Trim().ToLower();
                        var duplicateUserPhoneInDb = usersInDb.FirstOrDefault(u =>
                            u.Phone != null && u.Phone.Trim() == item.SoDienThoaiChuanHoa &&
                            (u.Email == null || u.Email.Trim().ToLower() != emailTrim) &&
                            u.IsDeleted != true);

                        if (duplicateUserPhoneInDb != null)
                        {
                            itemErrors.Add($"Số điện thoại '{item.SoDienThoaiChuanHoa}' đã được sử dụng bởi tài khoản '{duplicateUserPhoneInDb.Name}' ({duplicateUserPhoneInDb.Email})");
                        }
                    }
                }

                if (itemErrors.Count > 0)
                {
                    item.Loi = string.Join("; ", itemErrors);
                    danhSachLoi.Add(item);
                    errors.Add(new ValidateError
                    {
                        Id = $"Dong_{item.DongSo}",
                        Description = $"Dòng {item.DongSo}" + (!string.IsNullOrWhiteSpace(item.HoTen) ? $" ({item.HoTen})" : "") + $": {item.Loi}"
                    });
                }
            }

            if (errors.Count > 0)
            {
                return new
                {
                    Success = false,
                    Message = $"Phát hiện {errors.Count} lỗi trong file import. Vui lòng kiểm tra và sửa lại file trước khi cập nhật.",
                    Errors = errors,
                    ErrorDetails = danhSachLoi
                };
            }

            int updatedCount = 0;
            foreach (var item in danhSach)
            {
                var emailTrim = item.Email.Trim().ToLower();
                var user = _context.Users.FirstOrDefault(u => u.Email != null && u.Email.Trim().ToLower() == emailTrim);
                if (user != null)
                {
                    user.Phone = item.SoDienThoaiChuanHoa;
                    user.SearchStr = (!string.IsNullOrEmpty(user.Name) ? user.Name.Trim() : "") + " "
                        + (!string.IsNullOrEmpty(user.Email) ? user.Email.Trim() : "") + " "
                        + (!string.IsNullOrEmpty(user.Phone) ? user.Phone.Trim() : "") + " "
                        + (!string.IsNullOrEmpty(user.UserZaloId) ? user.UserZaloId.Trim() : "");
                    updatedCount++;
                }
            }

            _context.SaveChanges();
            CacheManagerHelper.InitUserCache();

            return new
            {
                Success = true,
                Message = $"Cập nhật thành công số điện thoại cho {updatedCount} người dùng.",
                TotalUpdated = updatedCount
            };
        }
    }
}
