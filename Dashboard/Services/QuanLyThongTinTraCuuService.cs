using Dashboard.Common;
using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.ViewModel;
using Dashboard.Repository;
using System.Collections.Generic;
using System.Linq;

namespace Dashboard.Services
{
    public interface IQuanLyThongTinTraCuuService
    {
        DataTableResult<QuanLyThongTinTraCuuGridVo> GetAll(QuanLyThongTinTraCuuQueryViewModel queryParams);
        QuanLyThongTinTraCuuGridVo GetById(int id);
        bool KhoiPhuc(int id);
        dynamic ThemOrCapNhat(QuanLyThongTinTraCuuViewModel quanLyThongTinTraCuuViewModel);
        bool Xoa(int id);
        List<VanBanPhapLuatGridVo> GetVanBanLienQuan(int id);
        List<FileDinhKemGridVo> GetFileDinhKemById(int id);
    }

    public class QuanLyThongTinTraCuuService : IQuanLyThongTinTraCuuService
    {
        private readonly IQuanLyThongTinTraCuuRepository _quanLyThongTinTraCuuRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICoQuanBanHanhRepository _coQuanBanHanhRepository;
        private readonly IHinhThucVanBanRepository _hinhThucVanBanRepository;
        public QuanLyThongTinTraCuuService(IQuanLyThongTinTraCuuRepository quanLyThongTinTraCuuRepository, IUserRepository userRepository, ICoQuanBanHanhRepository coQuanBanHanhRepository,
            IHinhThucVanBanRepository hinhThucVanBanRepository)
        {
            _quanLyThongTinTraCuuRepository = quanLyThongTinTraCuuRepository;
            _userRepository = userRepository;
            _coQuanBanHanhRepository = coQuanBanHanhRepository;
            _hinhThucVanBanRepository = hinhThucVanBanRepository;
        }

        public DataTableResult<QuanLyThongTinTraCuuGridVo> GetAll(QuanLyThongTinTraCuuQueryViewModel queryParams)
        {
            var data = _quanLyThongTinTraCuuRepository.GetAll(queryParams);

            foreach (var item in data.rows)
            {
                var dmExist = CacheManagerHelper.GetDanhMucTraCuuThongTinByIdFromCache(item.DanhMucTraCuuThongTinId.GetValueOrDefault());
                if (dmExist != null)
                {
                    item.DanhMucTraCuuThongTin = dmExist.Ten;
                }
            }
            return data;
        }

        public QuanLyThongTinTraCuuGridVo GetById(int id)
        {
            return _quanLyThongTinTraCuuRepository.GetById(id);
        }

        public bool KhoiPhuc(int id)
        {
            return _quanLyThongTinTraCuuRepository.KhoiPhuc(id);
        }

        public dynamic ThemOrCapNhat(QuanLyThongTinTraCuuViewModel quanLyThongTinTraCuuViewModel)
        {
            return _quanLyThongTinTraCuuRepository.ThemOrCapNhat(quanLyThongTinTraCuuViewModel);
        }

        public bool Xoa(int id)
        {
            return _quanLyThongTinTraCuuRepository.Xoa(id);
        }

        public List<VanBanPhapLuatGridVo> GetVanBanLienQuan(int id)
        {
            var data = _quanLyThongTinTraCuuRepository.GetVanBanLienQuan(id);
            var coQuans = _coQuanBanHanhRepository.CoQuanBanHanhLookup(new LookupQueryViewModel());
            var hinhThucs = _hinhThucVanBanRepository.HinhThucVanBanLookup(new LookupQueryViewModel());
            foreach (var item in data)
            {
                if (item.HinhThucVanBanId.GetValueOrDefault() != 0)
                {
                    var ht = hinhThucs.Data.FirstOrDefault(o => o.id == item.HinhThucVanBanId);
                    if (ht != null)
                    {
                        item.HinhThucVanBan = ht.text;
                    }
                }

                if (item.CoQuanBanHanhId.GetValueOrDefault() != 0)
                {
                    var cq = coQuans.Data.FirstOrDefault(o => o.id == item.CoQuanBanHanhId);
                    if (cq != null)
                    {
                        item.CoQuanBanHanh = cq.text;
                    }
                }
            }
            return data;
        }

        public List<FileDinhKemGridVo> GetFileDinhKemById(int id)
        {
            return _quanLyThongTinTraCuuRepository.GetFileDinhKemById(id);
        }
    }
}
