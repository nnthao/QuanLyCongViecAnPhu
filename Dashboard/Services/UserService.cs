using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;

namespace Dashboard.Services
{
    public interface IUserService
    {
        dynamic CapNhat(UserViewModel userViewModel);
        DataTableResult<UserGridVo> GetAll(UserQueryViewModel queryParams);
        dynamic GetById(int id);
        List<string> GetListDataScriptVersions();
        UserGridVo GetUserByIdFromCahe(int id);
        UserGridVo GetUserDangNhap(string email);
        dynamic KhoiPhuc(int id);
        dynamic Them(UserViewModel userViewModel);
        DataLookupViewModel UserLookup(LookupQueryViewModel lookupQueryViewModel);
        dynamic Xoa(int id);
        dynamic XoaKhoiDatabase(int id);
        dynamic CapNhatMatKhau(UserViewModel userViewModel);
        dynamic CapNhatTelegramId(UserViewModel userViewModel);
        dynamic ImportDanhSach(List<UserImportGridVo> danhSachUser);
        dynamic MoKhoaTaiKhoan(int id);
        dynamic ResetMatKhau(int id);
        dynamic CapNhatSoDienThoaiTuExcel(List<UserCapNhatSoDienThoaiImportVo> danhSach);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public DataTableResult<UserGridVo> GetAll(UserQueryViewModel queryParams)
        {
            var data = _userRepository.GetAll(queryParams);
            return data;
        }

        public UserGridVo GetUserByIdFromCahe(int id)
        {
            return _userRepository.GetUserByIdFromCahe(id);
        }

        public dynamic CapNhat(UserViewModel userViewModel)
        {
            return _userRepository.CapNhat(userViewModel);
        }

        public dynamic Them(UserViewModel userViewModel)
        {
            return _userRepository.Them(userViewModel);
        }

        public dynamic GetById(int id)
        {
            return _userRepository.GetById(id);
        }

        public dynamic KhoiPhuc(int id)
        {
            return _userRepository.KhoiPhuc(id);
        }

        public dynamic XoaKhoiDatabase(int id)
        {
            return _userRepository.XoaKhoiDatabase(id);
        }

        public dynamic Xoa(int id)
        {
            return _userRepository.Xoa(id);
        }

        public DataLookupViewModel UserLookup(LookupQueryViewModel lookupQueryViewModel)
        {
            return _userRepository.UserLookup(lookupQueryViewModel);
        }

        public List<string> GetListDataScriptVersions()
        {
            return _userRepository.GetListDataScriptVersions();
        }

        public UserGridVo GetUserDangNhap(string email)
        {
            return _userRepository.GetUserDangNhap(email);
        }

        public dynamic CapNhatMatKhau(UserViewModel userViewModel)
        {
            return _userRepository.CapNhatMatKhau(userViewModel);
        }

        public dynamic CapNhatTelegramId(UserViewModel userViewModel)
        {
            return _userRepository.CapNhatTelegramId(userViewModel);
        }

        public dynamic ImportDanhSach(List<UserImportGridVo> danhSachUser)
        {
            return _userRepository.ImportDanhSach(danhSachUser);
        }

        public dynamic MoKhoaTaiKhoan(int id)
        {
            return _userRepository.MoKhoaTaiKhoan(id);
        }

        public dynamic ResetMatKhau(int id)
        {
            return _userRepository.ResetMatKhau(id);
        }

        public dynamic CapNhatSoDienThoaiTuExcel(List<UserCapNhatSoDienThoaiImportVo> danhSach)
        {
            return _userRepository.CapNhatSoDienThoaiTuExcel(danhSach);
        }
    }
}