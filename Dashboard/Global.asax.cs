using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Dashboard.Common;
using Dashboard.Models;
using Dashboard.Models.Validates;
using Dashboard.Repository;
using Dashboard.Services;
using log4net;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dashboard
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static ILog log = log4net.LogManager.GetLogger(typeof(MvcApplication));
        protected void Application_Start()
        {
            AutofacRegister();

            GlobalFilters.Filters.Add(new IsAuthorize());
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ModelBinders.Binders.DefaultBinder = new TrimModelBinder();

            log4net.Config.XmlConfigurator.Configure();

            DatabaseHelper.RunSqlFileUpdateDatabaseWebsite();
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            // Khởi tạo cache
            CacheManagerHelper.InitializeCache();

            // Lưu domain vào cache ngay khi app start
            if (!string.IsNullOrEmpty(BienDungChung.Domain))
            {
                CacheManagerHelper.UpdateDomainCache(BienDungChung.Domain);
            }

            // Khởi tạo Telegram bot để nhận lệnh từ người dùng
            TelegramHelper.InitBotAsync();

            // Khởi tạo Quartz Scheduler
            QuartzSchedulerHelper.Start();
        }

        protected void Application_End()
        {
            QuartzSchedulerHelper.Stop();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            // Lấy domain từ request đầu tiên và đưa vào cache
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                try
                {
                    // Chỉ cập nhật nếu cache chưa có domain
                    var currentDomain = CacheManagerHelper.GetDomainCache();
                    if (string.IsNullOrEmpty(currentDomain))
                    {
                        var request = HttpContext.Current.Request;
                        var scheme = request.IsSecureConnection ? "https" : "http";
                        var host = request.Url.Host;
                        var port = request.Url.Port;
                        var newDomain = $"{scheme}://{host}";
                        if ((scheme == "http" && port != 80) || (scheme == "https" && port != 443))
                        {
                            newDomain += $":{port}";
                        }
                        CacheManagerHelper.UpdateDomainCache(newDomain);
                    }
                }
                catch (Exception ex)
                {
                    log.Warn($"Không thể cập nhật domain từ request: {ex.Message}");
                }
            }
        }

        //public class FilterConfig
        //{
        //    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        //    {
        //        filters.Add(new HandleErrorAttribute());
        //    }
        //}

        private void AutofacRegister()
        {
            var builder = new ContainerBuilder();

            // Register all controllers in the Mvc Application assembly
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterModule(new LogInjectionModule());

            builder.RegisterType(typeof(ConnectDB)).As(typeof(IContext)).InstancePerLifetimeScope();

            // Registered Warehouse Reservoir Service
            builder.RegisterType<PhongBanRepository>().As<IPhongBanRepository>();
            builder.RegisterType<UserRepository>().As<IUserRepository>();
            builder.RegisterType<VaiTroRepository>().As<IVaiTroRepository>();
            builder.RegisterType<ChucDanhRepository>().As<IChucDanhRepository>();
            builder.RegisterType<ThongBaoRepository>().As<IThongBaoRepository>();
            builder.RegisterType<DanhMucTraCuuThongTinRepository>().As<IDanhMucTraCuuThongTinRepository>();
            builder.RegisterType<DanhMucBaoCaoRepository>().As<IDanhMucBaoCaoRepository>();
            builder.RegisterType<QuanLyBaoCaoChuyenNganhRepository>().As<IQuanLyBaoCaoChuyenNganhRepository>();
            builder.RegisterType<VanBanPhapLuatRepository>().As<IVanBanPhapLuatRepository>();
            builder.RegisterType<HinhThucVanBanRepository>().As<IHinhThucVanBanRepository>();
            builder.RegisterType<CoQuanBanHanhRepository>().As<ICoQuanBanHanhRepository>();
            builder.RegisterType<DoKhanRepository>().As<IDoKhanRepository>();
            builder.RegisterType<VaiTroXuLyRepository>().As<IVaiTroXuLyRepository>();
            builder.RegisterType<DonViDoanRepository>().As<IDonViDoanRepository>();
            builder.RegisterType<DonViRepository>().As<IDonViRepository>();
            builder.RegisterType<DoanVienRepository>().As<IDoanVienRepository>();
            builder.RegisterType<DoanVien_DanTocRepository>().As<IDoanVien_DanTocRepository>();
            builder.RegisterType<DoanVien_TaiLieuRepository>().As<IDoanVien_TaiLieuRepository>();
            builder.RegisterType<QuanLyThongTinTraCuuRepository>().As<IQuanLyThongTinTraCuuRepository>();
            builder.RegisterType<NhomNhanViecRepository>().As<INhomNhanViecRepository>();
            builder.RegisterType<DuAnRepository>().As<IDuAnRepository>();
            builder.RegisterType<SangKienRepository>().As<ISangKienRepository>();
            builder.RegisterType<LinhVucSangKienRepository>().As<ILinhVucSangKienRepository>();
            builder.RegisterType<SoKiHieuVanBanRepository>().As<ISoKiHieuVanBanRepository>();
            builder.RegisterType<SangKienThanhVienRepository>().As<ISangKienThanhVienRepository>();
            builder.RegisterType<VaiTroXuLyRepository>().As<IVaiTroXuLyRepository>();
            builder.RegisterType<DonViDoanRepository>().As<IDonViDoanRepository>();
            builder.RegisterType<DonViRepository>().As<IDonViRepository>();
            builder.RegisterType<DoanVienRepository>().As<IDoanVienRepository>();
            builder.RegisterType<DoanVien_DanTocRepository>().As<IDoanVien_DanTocRepository>();
            builder.RegisterType<DoanVien_TaiLieuRepository>().As<IDoanVien_TaiLieuRepository>();
            builder.RegisterType<QuanLyThongTinTraCuuRepository>().As<IQuanLyThongTinTraCuuRepository>();
            builder.RegisterType<NhomNhanViecRepository>().As<INhomNhanViecRepository>();
            builder.RegisterType<DuAnRepository>().As<IDuAnRepository>();
            builder.RegisterType<DuAnHangMucRepository>().As<IDuAnHangMucRepository>();
            builder.RegisterType<DuAnTrienKhaiRepository>().As<IDuAnTrienKhaiRepository>();
            builder.RegisterType<QuanLyCongTrinhHuHongRepository>().As<IQuanLyCongTrinhHuHongRepository>();
            builder.RegisterType<QuanLyCongTrinhHuHongChiTietRepository>().As<IQuanLyCongTrinhHuHongChiTietRepository>();
            builder.RegisterType<QuanLyCongTrinhHuHongFileRepository>().As<IQuanLyCongTrinhHuHongFileRepository>();
            builder.RegisterType<DanhSachCongTrinhRepository>().As<IDanhSachCongTrinhRepository>();
            builder.RegisterType<GiaoViecRepository>().As<IGiaoViecRepository>();
            builder.RegisterType<GiaoViecDanhGiaRepository>().As<IGiaoViecDanhGiaRepository>();
            builder.RegisterType<GiaoViecThongKeRepository>().As<IGiaoViecThongKeRepository>();
            builder.RegisterType<DashboardRepository>().As<IDashboardRepository>();
            builder.RegisterType<LichHopThongKeRepository>().As<ILichHopThongKeRepository>();
            builder.RegisterType<CuocHopRepository>().As<ICuocHopRepository>();
            builder.RegisterType<LyDoRepository>().As<ILyDoRepository>();
            builder.RegisterType<DotXetDuyetRepository>().As<IDotXetDuyetRepository>();
            builder.RegisterType<ChamSangKienRepository>().As<IChamSangKienRepository>();
            builder.RegisterType<PhanCongChamRepository>().As<IPhanCongChamRepository>();
            builder.RegisterType<SangKienRepository>().As<ISangKienRepository>();
            builder.RegisterType<LinhVucSangKienRepository>().As<ILinhVucSangKienRepository>();
            builder.RegisterType<SoKiHieuVanBanRepository>().As<ISoKiHieuVanBanRepository>();
            builder.RegisterType<SangKienThanhVienRepository>().As<ISangKienThanhVienRepository>();
            builder.RegisterType<SangKienThanhVienCNTTRepository>().As<ISangKienThanhVienCNTTRepository>();
            builder.RegisterType<SangKienPhanThuongRepository>().As<ISangKienPhanThuongRepository>();
            builder.RegisterType<SangKienCNTTRepository>().As<ISangKienCNTTRepository>();
            builder.RegisterType<SangKienCNTTFileRepository>().As<ISangKienCNTTFileRepository>();
            builder.RegisterType<CongViecChungRepository>().As<ICongViecChungRepository>();
            builder.RegisterType<DiemDanhRepository>().As<IDiemDanhRepository>();
            builder.RegisterType<LuuTruFileRepository>().As<ILuuTruFileRepository>();
            builder.RegisterType<ZaloRepository>().As<IZaloRepository>();

            // Registered Warehouse Reservoir Service
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<VaiTroService>().As<IVaiTroService>();
            builder.RegisterType<PhongBanService>().As<IPhongBanService>();
            builder.RegisterType<ChucDanhService>().As<IChucDanhService>();
            builder.RegisterType<ThongBaoService>().As<IThongBaoService>();
            builder.RegisterType<DanhMucTraCuuThongTinService>().As<IDanhMucTraCuuThongTinService>();
            builder.RegisterType<DanhMucBaoCaoService>().As<IDanhMucBaoCaoService>();
            builder.RegisterType<QuanLyBaoCaoChuyenNganhService>().As<IQuanLyBaoCaoChuyenNganhService>();
            builder.RegisterType<VanBanPhapLuatService>().As<IVanBanPhapLuatService>();
            builder.RegisterType<HinhThucVanBanService>().As<IHinhThucVanBanService>();
            builder.RegisterType<CoQuanBanHanhService>().As<ICoQuanBanHanhService>();
            builder.RegisterType<DoKhanService>().As<IDoKhanService>();
            builder.RegisterType<SangKienService>().As<ISangKienService>();
            builder.RegisterType<LinhVucSangKienService>().As<ILinhVucSangKienService>();
            builder.RegisterType<SoKiHieuVanBanService>().As<ISoKiHieuVanBanService>();
            builder.RegisterType<SangKienThanhVienService>().As<ISangKienThanhVienService>();
            builder.RegisterType<SangKienThanhVienCNTTService>().As<ISangKienThanhVienCNTTService>();
            builder.RegisterType<SangKienPhanThuongService>().As<ISangKienPhanThuongService>();
            builder.RegisterType<SangKienCNTTService>().As<ISangKienCNTTService>();
            builder.RegisterType<SangKienCNTTFileService>().As<ISangKienCNTTFileService>();
            builder.RegisterType<CongViecChungService>().As<ICongViecChungService>();
            builder.RegisterType<DiemDanhService>().As<IDiemDanhService>();
            builder.RegisterType<LuuTruFileService>().As<ILuuTruFileService>();
            builder.RegisterType<ZaloService>().As<IZaloService>();
            builder.RegisterType<DashboardService>().As<IDashboardService>();
            builder.RegisterType<GiaoViecService>().As<IGiaoViecService>();
            builder.RegisterType<GiaoViecDanhGiaService>().As<IGiaoViecDanhGiaService>();
            builder.RegisterType<GiaoViecThongKeService>().As<IGiaoViecThongKeService>();
            builder.RegisterType<CuocHopService>().As<ICuocHopService>();
            builder.RegisterType<DanhSachCongTrinhService>().As<IDanhSachCongTrinhService>();
            builder.RegisterType<DoanVienService>().As<IDoanVienService>();
            builder.RegisterType<DoanVien_DanTocService>().As<IDoanVien_DanTocService>();
            builder.RegisterType<DoanVien_TaiLieuService>().As<IDoanVien_TaiLieuService>();
            builder.RegisterType<DonViDoanService>().As<IDonViDoanService>();
            builder.RegisterType<DonViService>().As<IDonViService>();
            builder.RegisterType<DotXetDuyetService>().As<IDotXetDuyetService>();
            builder.RegisterType<DuAnHangMucService>().As<IDuAnHangMucService>();
            builder.RegisterType<DuAnService>().As<IDuAnService>();
            builder.RegisterType<DuAnTrienKhaiService>().As<IDuAnTrienKhaiService>();
            builder.RegisterType<LichHopThongKeService>().As<ILichHopThongKeService>();
            builder.RegisterType<LyDoService>().As<ILyDoService>();
            builder.RegisterType<NhomNhanViecService>().As<INhomNhanViecService>();
            builder.RegisterType<PhanCongChamService>().As<IPhanCongChamService>();
            builder.RegisterType<QuanLyCongTrinhHuHongService>().As<IQuanLyCongTrinhHuHongService>();
            builder.RegisterType<QuanLyCongTrinhHuHongChiTietService>().As<IQuanLyCongTrinhHuHongChiTietService>();
            builder.RegisterType<QuanLyCongTrinhHuHongFileService>().As<IQuanLyCongTrinhHuHongFileService>();
            builder.RegisterType<QuanLyThongTinTraCuuService>().As<IQuanLyThongTinTraCuuService>();
            builder.RegisterType<VaiTroXuLyService>().As<IVaiTroXuLyService>();
            builder.RegisterType<ChamSangKienService>().As<IChamSangKienService>();

            //Registered validate
            builder.RegisterType<PhongBanValidate>().As<IPhongBanValidate>();
            builder.RegisterType<MatKhauValidate>().As<IMatKhauValidate>();
            builder.RegisterType<UserValidate>().As<IUserValidate>();
            builder.RegisterType<VaiTroValidate>().As<IVaiTroValidate>();
            builder.RegisterType<ChucDanhValidate>().As<IChucDanhValidate>();
            builder.RegisterType<DanhMucTraCuuThongTinValidate>().As<IDanhMucTraCuuThongTinValidate>();
            builder.RegisterType<DanhMucBaoCaoValidate>().As<IDanhMucBaoCaoValidate>();
            builder.RegisterType<VanBanPhapLuatValidate>().As<IVanBanPhapLuatValidate>();
            builder.RegisterType<HinhThucVanBanValidate>().As<IHinhThucVanBanValidate>();
            builder.RegisterType<CoQuanBanHanhValidate>().As<ICoQuanBanHanhValidate>();
            builder.RegisterType<DoKhanValidate>().As<IDoKhanValidate>();
            builder.RegisterType<VaiTroXuLyValidate>().As<IVaiTroXuLyValidate>();
            builder.RegisterType<DonViDoanValidate>().As<IDonViDoanValidate>();
            builder.RegisterType<DonViValidate>().As<IDonViValidate>();
            builder.RegisterType<DoanVienValidate>().As<IDoanVienValidate>();
            builder.RegisterType<DoanVien_DanTocValidate>().As<IDoanVien_DanTocValidate>();
            builder.RegisterType<DoanVien_TaiLieuValidate>().As<IDoanVien_TaiLieuValidate>();
            builder.RegisterType<QuanLyThongTinTraCuuValidate>().As<IQuanLyThongTinTraCuuValidate>();
            builder.RegisterType<NhomNhanViecValidate>().As<INhomNhanViecValidate>();
            builder.RegisterType<DuAnValidate>().As<IDuAnValidate>();
            builder.RegisterType<DuAnHangMucValidate>().As<IDuAnHangMucValidate>();
            builder.RegisterType<DuAnTrienKhaiValidate>().As<IDuAnTrienKhaiValidate>();
            builder.RegisterType<QuanLyCongTrinhHuHongValidate>().As<IQuanLyCongTrinhHuHongValidate>();
            builder.RegisterType<QuanLyCongTrinhHuHongChiTietValidate>().As<IQuanLyCongTrinhHuHongChiTietValidate>();
            builder.RegisterType<QuanLyCongTrinhHuHongFileValidate>().As<IQuanLyCongTrinhHuHongFileValidate>();
            builder.RegisterType<DanhSachCongTrinhValidate>().As<IDanhSachCongTrinhValidate>();
            builder.RegisterType<GiaoViecValidate>().As<IGiaoViecValidate>();
            builder.RegisterType<CuocHopValidate>().As<ICuocHopValidate>();
            builder.RegisterType<LyDoValidate>().As<ILyDoValidate>();
            builder.RegisterType<DotXetDuyetValidate>().As<IDotXetDuyetValidate>();
            builder.RegisterType<SangKienValidate>().As<ISangKienValidate>();
            builder.RegisterType<LinhVucSangKienValidate>().As<ILinhVucSangKienValidate>();
            builder.RegisterType<DiemDanhValidate>().As<IDiemDanhValidate>();
            builder.RegisterType<ZaloValidate>().As<IZaloValidate>();

            // Registration filter
            builder.RegisterFilterProvider();

            var container = builder.Build();

            // Setting Dependency Injection Parser
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            if (exception != null)
            {
                log.Error(exception.Message + Environment.NewLine + exception.StackTrace);
            }
            Response.Clear();

            HttpException httpException = exception as HttpException;

            if (httpException != null)
            {
                string action;

                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // page not found
                        action = "PageNotFound";
                        break;
                    case 500:
                        // server error
                        action = "Index";
                        break;
                    default:
                        action = "Index";
                        break;
                }

                // clear error on server
                Server.ClearError();

                Response.Redirect(String.Format("~/Error/{0}", action));
            }
        }
    }

    public class TrimModelBinder : System.Web.Mvc.DefaultModelBinder
    {
        protected override void SetProperty(ControllerContext controllerContext,
            System.Web.Mvc.ModelBindingContext bindingContext,
            System.ComponentModel.PropertyDescriptor propertyDescriptor, object value)
        {
            if (propertyDescriptor.PropertyType == typeof(string))
            {
                var stringValue = (string)value;
                if (!string.IsNullOrWhiteSpace(stringValue))
                {
                    value = stringValue.Trim();
                }
                else
                {
                    value = null;
                }
            }
            base.SetProperty(controllerContext, bindingContext,
                                propertyDescriptor, value);
        }
    }

    public class LogInjectionModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistry registry, IComponentRegistration registration)
        {
            registration.Preparing += OnComponentPreparing;
        }

        static void OnComponentPreparing(object sender, PreparingEventArgs e)
        {
            var t = e.Component.Activator.LimitType;
            e.Parameters = e.Parameters.Union(new[]
            {
            new ResolvedParameter((p, i) => p.ParameterType == typeof(ILog), (p, i) => LogManager.GetLogger(t))
        });
        }
    }
}
