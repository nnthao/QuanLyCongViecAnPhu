using Dashboard.AdoNetHelper;
using Dashboard.Models.Entities;
using Dashboard.Models.GridVo;
using Dashboard.Models.ViewModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace Dashboard.Common
{
    public class CacheManagerHelper
    {
        private static readonly Cache _cache = HttpContext.Current.Cache;
        public const string DomainCacheKey = "DomainCacheKey";
        private const string UserCacheKey = "UserCacheKey";
        private const string VaiTroCacheKey = "VaiTroCacheKey";
        private const string PhongBanCacheKey = "PhongBanCacheKey";
        private const string ChucDanhCacheKey = "ChucDanhCacheKey";
        private const string DanhSachMenu = "DanhSachMenu";
        private const string DanhMucTraCuuThongTinCacheKey = "DanhMucTraCuuThongTinCacheKey";
        private const string HinhThucVanBanCacheKey = "HinhThucVanBanCacheKey";
        private const string CoQuanBanHanhCacheKey = "CoQuanBanHanhCacheKey";
        private const string DoKhanCacheKey = "DoKhanCacheKey";
        private const string VaiTroXuLyCacheKey = "VaiTroXuLyCacheKey";
        private const string PhongHopCacheKey = "PhongHopCacheKey";
        private const string LyDoCacheKey = "LyDoCacheKey";

        // Khởi tạo dữ liệu vào cache khi ứng dụng khởi động
        public static void InitializeCache()
        {
            InitUserCache();
            InitVaiTroCache();
            InitPhongBanCache();
            InitChucDanhCache();
            InitDanhSachMenu();
            InitDanhMucTraCuuThongTinCache();
            InitHinhThucVanBanCache();
            InitCoQuanBanHanhCache();
            InitDoKhanCache();
            InitVaiTroXuLyCache();
            InitLyDoCache();
        }

        private static void InitDanhSachMenu()
        {
            var menuInit = LayDanhSachMenuTuFileJson();

            _cache.Insert(
                 DanhSachMenu,
                 menuInit,
                 null,
                 Cache.NoAbsoluteExpiration,
                 Cache.NoSlidingExpiration
             );
        }

        private static List<MenuGenerateViewModel> LayDanhSachMenuTuFileJson()
        {
            var menuInit = new List<MenuGenerateViewModel>();
            var path = System.Web.HttpContext.Current.Server.MapPath(BienDungChung.MenuJsonPath);
            if (System.IO.File.Exists(path))
            {
                var dataStr = System.IO.File.ReadAllText(path);
                if (!string.IsNullOrEmpty(dataStr))
                {
                    menuInit = JsonConvert.DeserializeObject<List<MenuGenerateViewModel>>(dataStr);
                }
            }

            return menuInit;
        }

        public static List<MenuGenerateViewModel> GetDanhSachMenuCache()
        {
            var menus = _cache[DanhSachMenu] as List<MenuGenerateViewModel>;
            if (menus == null)
            {
                menus = LayDanhSachMenuTuFileJson();
                _cache.Insert(
                     DanhSachMenu,
                     menus,
                     null,
                     Cache.NoAbsoluteExpiration,
                     Cache.NoSlidingExpiration
                 );
            }
            return menus;
        }

        public static void InitUserCache()
        {
            var users = UserAdoNetHelper.GetAllUserUseAdoDotNet(BienDungChung.ConnectDBStr);
            _cache.Insert(
                 UserCacheKey,
                 users,
                 null,
                 Cache.NoAbsoluteExpiration,
                 Cache.NoSlidingExpiration
             );
        }

        public static List<UserGridVo> GetUserCache()
        {
            var users = _cache[UserCacheKey] as List<UserGridVo>;
            if (users == null)
            {
                users = UserAdoNetHelper.GetAllUserUseAdoDotNet(BienDungChung.ConnectDBStr);
                _cache.Insert(
                     UserCacheKey,
                     users,
                     null,
                     Cache.NoAbsoluteExpiration,
                     Cache.NoSlidingExpiration
                 );
            }
            return users;
        }

        public static List<UserGridVo> GetListUserFromCacheNotDeleted()
        {
            var users = GetUserCache();
            return users.Where(x => x.IsDeleted != true).ToList();
        }

        public static UserGridVo GetUserByIdFromCache(int id)
        {
            var users = GetUserCache();
            return users.FirstOrDefault(x => x.Id == id);
        }

        public static void InitVaiTroCache()
        {
            var vaiTros = VaiTroAdoNetHelper.GetAllVaiTroUseAdoDotNet(BienDungChung.ConnectDBStr);
            _cache.Insert(
                VaiTroCacheKey,
                vaiTros,
                null,
                Cache.NoAbsoluteExpiration,
                Cache.NoSlidingExpiration
            );
        }

        public static List<VaiTroGridVo> GetVaiTroCache()
        {
            var vaiTros = _cache[VaiTroCacheKey] as List<VaiTroGridVo>;
            if (vaiTros == null)
            {
                vaiTros = VaiTroAdoNetHelper.GetAllVaiTroUseAdoDotNet(BienDungChung.ConnectDBStr);
                _cache.Insert(
                    VaiTroCacheKey,
                    vaiTros,
                    null,
                    Cache.NoAbsoluteExpiration,
                    Cache.NoSlidingExpiration
                );
            }
            return vaiTros;
        }

        public static VaiTroGridVo GetVaiTroByIdFromCache(int id)
        {
            var phongHopCaches = GetVaiTroCache();
            return phongHopCaches.FirstOrDefault(x => x.Id == id);
        }

        public static void InitPhongBanCache()
        {
            var phongBans = PhongBanAdoNetHelper.GetAllPhongBanUseAdoDotNet(BienDungChung.ConnectDBStr);
            _cache.Insert(
                PhongBanCacheKey,
                phongBans,
                null,
                Cache.NoAbsoluteExpiration,
                Cache.NoSlidingExpiration
            );
        }

        public static List<PhongBanGridVo> GetPhongBanCache()
        {
            var phongBans = _cache[PhongBanCacheKey] as List<PhongBanGridVo>;
            if (phongBans == null)
            {
                phongBans = PhongBanAdoNetHelper.GetAllPhongBanUseAdoDotNet(BienDungChung.ConnectDBStr);
                _cache.Insert(
                    PhongBanCacheKey,
                    phongBans,
                    null,
                    Cache.NoAbsoluteExpiration,
                    Cache.NoSlidingExpiration
                );
            }
            return phongBans;
        }

        public static PhongBanGridVo GetPhongBanByIdFromCache(int id)
        {
            var phongBanCaches = GetPhongBanCache();
            return phongBanCaches.FirstOrDefault(x => x.Id == id);
        }

        public static List<PhongBanGridVo> GetPhongBanCacheNotDeleted()
        {
            var pbs = GetPhongBanCache();
            return pbs.Where(x => x.IsDeleted != true).ToList();
        }

        public static List<PhongBanGridVo> GetPhongBanNhanViecCacheNotDeleted()
        {
            var pbs = GetPhongBanCacheNotDeleted();
            return pbs.Where(x => x.IsPhongNhanViec != false).ToList();
        }

        public static void InitChucDanhCache()
        {
            var chucDanhs = ChucDanhAdoNetHelper.GetAllChucDanhUseAdoDotNet(BienDungChung.ConnectDBStr);
            _cache.Insert(
                ChucDanhCacheKey,
                chucDanhs,
                null,
                Cache.NoAbsoluteExpiration,
                Cache.NoSlidingExpiration
            );
        }
        public static List<ChucDanhGridVo> GetChucDanhCache()
        {
            var chucDanhs = _cache[ChucDanhCacheKey] as List<ChucDanhGridVo>;
            if (chucDanhs == null)
            {
                chucDanhs = ChucDanhAdoNetHelper.GetAllChucDanhUseAdoDotNet(BienDungChung.ConnectDBStr);
                _cache.Insert(
                    ChucDanhCacheKey,
                    chucDanhs,
                    null,
                    Cache.NoAbsoluteExpiration,
                    Cache.NoSlidingExpiration
                );
            }
            return chucDanhs;
        }

        public static ChucDanhGridVo GetChucDanhByIdFromCache(int id)
        {
            var chucDanhCaches = GetChucDanhCache();
            return chucDanhCaches.FirstOrDefault(x => x.Id == id);
        }



        public static void InitDanhMucTraCuuThongTinCache()
        {
            var datas = DanhMucTraCuuThongTinAdoNetHelper.GetAllDanhMucTraCuuThongTinUseAdoDotNet(BienDungChung.ConnectDBStr);
            _cache.Insert(
                 DanhMucTraCuuThongTinCacheKey,
                 datas,
                 null,
                 Cache.NoAbsoluteExpiration,
                 Cache.NoSlidingExpiration
             );
        }

        public static List<DanhMucTraCuuThongTinGridVo> GetDanhMucTraCuuThongTinCache()
        {
            var datas = _cache[DanhMucTraCuuThongTinCacheKey] as List<DanhMucTraCuuThongTinGridVo>;
            if (datas == null)
            {
                datas = DanhMucTraCuuThongTinAdoNetHelper.GetAllDanhMucTraCuuThongTinUseAdoDotNet(BienDungChung.ConnectDBStr);
                _cache.Insert(
                     DanhMucTraCuuThongTinCacheKey,
                     datas,
                     null,
                     Cache.NoAbsoluteExpiration,
                     Cache.NoSlidingExpiration
                 );
            }
            return datas;
        }

        public static DanhMucTraCuuThongTinGridVo GetDanhMucTraCuuThongTinByIdFromCache(int id)
        {
            var datas = GetDanhMucTraCuuThongTinCache();
            return datas.FirstOrDefault(x => x.Id == id);
        }

        public static void InitHinhThucVanBanCache()
        {
            var datas = HinhThucVanBanAdoNetHelper.GetAllHinhThucVanBanUseAdoDotNet(BienDungChung.ConnectDBStr);
            _cache.Insert(
                 HinhThucVanBanCacheKey,
                 datas,
                 null,
                 Cache.NoAbsoluteExpiration,
                 Cache.NoSlidingExpiration
             );
        }

        public static List<HinhThucVanBanGridVo> GetHinhThucVanBanCache()
        {
            var datas = _cache[HinhThucVanBanCacheKey] as List<HinhThucVanBanGridVo>;
            if (datas == null)
            {
                datas = HinhThucVanBanAdoNetHelper.GetAllHinhThucVanBanUseAdoDotNet(BienDungChung.ConnectDBStr);
                _cache.Insert(
                     HinhThucVanBanCacheKey,
                     datas,
                     null,
                     Cache.NoAbsoluteExpiration,
                     Cache.NoSlidingExpiration
                 );
            }
            return datas;
        }

        public static HinhThucVanBanGridVo GetHinhThucVanBanByIdFromCache(int id)
        {
            var datas = GetHinhThucVanBanCache();
            return datas.FirstOrDefault(x => x.Id == id);
        }

        public static void InitCoQuanBanHanhCache()
        {
            var datas = CoQuanBanHanhAdoNetHelper.GetAllCoQuanBanHanhUseAdoDotNet(BienDungChung.ConnectDBStr);
            _cache.Insert(
                 CoQuanBanHanhCacheKey,
                 datas,
                 null,
                 Cache.NoAbsoluteExpiration,
                 Cache.NoSlidingExpiration
             );
        }

        public static List<CoQuanBanHanhGridVo> GetCoQuanBanHanhCache()
        {
            var datas = _cache[CoQuanBanHanhCacheKey] as List<CoQuanBanHanhGridVo>;
            if (datas == null)
            {
                datas = CoQuanBanHanhAdoNetHelper.GetAllCoQuanBanHanhUseAdoDotNet(BienDungChung.ConnectDBStr);
                _cache.Insert(
                     CoQuanBanHanhCacheKey,
                     datas,
                     null,
                     Cache.NoAbsoluteExpiration,
                     Cache.NoSlidingExpiration
                 );
            }
            return datas;
        }

        public static CoQuanBanHanhGridVo GetCoQuanBanHanhByIdFromCache(int id)
        {
            var datas = GetCoQuanBanHanhCache();
            return datas.FirstOrDefault(x => x.Id == id);
        }

        public static void InitDoKhanCache()
        {
            var datas = DoKhanAdoNetHelper.GetAllDoKhanUseAdoDotNet(BienDungChung.ConnectDBStr);
            _cache.Insert(
                 DoKhanCacheKey,
                 datas,
                 null,
                 Cache.NoAbsoluteExpiration,
                 Cache.NoSlidingExpiration
             );
        }

        public static List<DoKhanGridVo> GetDoKhanCache()
        {
            var datas = _cache[DoKhanCacheKey] as List<DoKhanGridVo>;
            if (datas == null)
            {
                datas = DoKhanAdoNetHelper.GetAllDoKhanUseAdoDotNet(BienDungChung.ConnectDBStr);
                _cache.Insert(
                     DoKhanCacheKey,
                     datas,
                     null,
                     Cache.NoAbsoluteExpiration,
                     Cache.NoSlidingExpiration
                 );
            }
            return datas;
        }

        public static DoKhanGridVo GetDoKhanByIdFromCache(int id)
        {
            var datas = GetDoKhanCache();
            return datas.FirstOrDefault(x => x.Id == id);
        }

        public static void InitVaiTroXuLyCache()
        {
            var datas = VaiTroXuLyAdoNetHelper.GetAllVaiTroXuLyUseAdoDotNet(BienDungChung.ConnectDBStr);
            _cache.Insert(
                 VaiTroXuLyCacheKey,
                 datas,
                 null,
                 Cache.NoAbsoluteExpiration,
                 Cache.NoSlidingExpiration
             );
        }

        public static List<VaiTroXuLyGridVo> GetVaiTroXuLyCache()
        {
            var datas = _cache[VaiTroXuLyCacheKey] as List<VaiTroXuLyGridVo>;
            if (datas == null)
            {
                datas = VaiTroXuLyAdoNetHelper.GetAllVaiTroXuLyUseAdoDotNet(BienDungChung.ConnectDBStr);
                _cache.Insert(
                     VaiTroXuLyCacheKey,
                     datas,
                     null,
                     Cache.NoAbsoluteExpiration,
                     Cache.NoSlidingExpiration
                 );
            }
            return datas;
        }

        public static VaiTroXuLyGridVo GetVaiTroXuLyByIdFromCache(int id)
        {
            var datas = GetVaiTroXuLyCache();
            return datas.FirstOrDefault(x => x.Id == id);
        }
        public static List<PhongHopGridVo> GetPhongHopCache()
        {
            var phongHops = _cache[PhongHopCacheKey] as List<PhongHopGridVo>;
            if (phongHops == null)
            {
                phongHops = PhongHopAdoNetHelper.GetAllPhongHopUseAdoDotNet(BienDungChung.ConnectDBStr);
                _cache.Insert(
                    PhongHopCacheKey,
                    phongHops,
                    null,
                    Cache.NoAbsoluteExpiration,
                    Cache.NoSlidingExpiration
                );
            }
            return phongHops;
        }
        public static PhongHopGridVo GetPhongHopByIdFromCache(int id)
        {
            var phongHopCaches = GetPhongHopCache();
            return phongHopCaches.FirstOrDefault(x => x.Id == id);
        }
        public static void InitLyDoCache()
        {
            var lyDos = LyDoAdoNetHelper.GetAllLyDoUseAdoDotNet(BienDungChung.ConnectDBStr);
            _cache.Insert(
                LyDoCacheKey,
                lyDos,
                null,
                Cache.NoAbsoluteExpiration,
                Cache.NoSlidingExpiration
            );
        }
        public static List<LyDoGridVo> GetLyDoCache()
        {
            var lyDos = _cache[LyDoCacheKey] as List<LyDoGridVo>;
            if (lyDos == null)
            {
                lyDos = LyDoAdoNetHelper.GetAllLyDoUseAdoDotNet(BienDungChung.ConnectDBStr);
                _cache.Insert(
                    LyDoCacheKey,
                    lyDos,
                    null,
                    Cache.NoAbsoluteExpiration,
                    Cache.NoSlidingExpiration
                );
            }
            return lyDos;
        }

        public static LyDoGridVo GetLyDoByIdFromCache(int id)
        {
            var lyDoCaches = GetLyDoCache();
            var item = lyDoCaches != null ? lyDoCaches.FirstOrDefault(x => x.Id == id) : null;
            if (item == null)
            {
                InitLyDoCache();
                lyDoCaches = GetLyDoCache();
                item = lyDoCaches != null ? lyDoCaches.FirstOrDefault(x => x.Id == id) : null;
            }
            return item;
        }

        public static string GetDomainCache()
        {
            return _cache[DomainCacheKey] as string;
        }

        public static void UpdateDomainCache(string domain)
        {
            if (!string.IsNullOrEmpty(domain))
            {
                _cache.Insert(
                    DomainCacheKey,
                    domain,
                    null,
                    Cache.NoAbsoluteExpiration,
                    Cache.NoSlidingExpiration
                );
            }
        }
    }
}