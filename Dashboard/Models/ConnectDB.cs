using Dashboard.Common;
using Dashboard.Models.Entities;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace Dashboard.Models
{
    public interface IContext
    {
        IDbSet<User> Users { get; set; }
        IDbSet<DatabaseVersion> DatabaseVersions { get; set; }
        IDbSet<VaiTro> VaiTros { get; set; }
        IDbSet<PhanQuyen> PhanQuyens { get; set; }
        IDbSet<PhongBan> PhongBans { get; set; }
        IDbSet<ChucDanh> ChucDanhs { get; set; }
        IDbSet<ThongBao> ThongBaos { get; set; }
        IDbSet<CuocHop> CuocHops { get; set; }
        IDbSet<DanhMucBaoCao> DanhMucBaoCaos { get; set; }
        IDbSet<QuanLyBaoCaoChuyenNganh> QuanLyBaoCaoChuyenNganhs { get; set; }
        IDbSet<DanhMucTraCuuThongTin> DanhMucTraCuuThongTins { get; set; }
        IDbSet<VanBanPhapLuat> VanBanPhapLuats { get; set; }
        IDbSet<VanBanPhapLuatFile> VanBanPhapLuatFiles { get; set; }
        IDbSet<HinhThucVanBan> HinhThucVanBans { get; set; }
        IDbSet<CoQuanBanHanh> CoQuanBanHanhs { get; set; }
        IDbSet<DoKhan> DoKhans { get; set; }
        IDbSet<VaiTroXuLy> VaiTroXuLys { get; set; }
        IDbSet<QuanLyThongTinTraCuu> QuanLyThongTinTraCuus { get; set; }
        IDbSet<QuanLyThongTinTraCuuChiTiet> QuanLyThongTinTraCuuChiTiets { get; set; }
        IDbSet<QuanLyThongTinTraCuuFile> QuanLyThongTinTraCuuFiles { get; set; }
        IDbSet<NhomNhanViec> NhomNhanViecs { get; set; }
        IDbSet<NhomNhanViecThanhVien> NhomNhanViecThanhViens { get; set; }
        IDbSet<GiaoViec> GiaoViecs { get; set; }
        IDbSet<GiaoViecThanhVien> GiaoViecThanhViens { get; set; }
        IDbSet<GiaoViecFile> GiaoViecFiles { get; set; }
        IDbSet<GiaoViecVanBanLienQuan> GiaoViecVanBanLienQuans { get; set; }
        IDbSet<CongTrinhHuHong> CongTrinhHuHongs { get; set; }
        IDbSet<CongTrinhHuHongChiTiet> CongTrinhHuHongChiTiets { get; set; }
        IDbSet<CongTrinhHuHongFile> CongTrinhHuHongFiles { get; set; }
        IDbSet<DanhSachCongTrinh> DanhSachCongTrinhs { get; set; }
        IDbSet<GiaoViecThanhVienTienTrinh> GiaoViecThanhVienTienTrinhs { get; set; }
        IDbSet<GiaoViecThanhVienTienTrinhFile> GiaoViecThanhVienTienTrinhFiles { get; set; }
        IDbSet<TaiLieuCuocHop> TaiLieuCuocHops { get; set; }
        IDbSet<NguoiLienQuanCuocHop> NguoiLienQuanCuocHops { get; set; }
        IDbSet<DieuHanhXe> DieuHanhXes { get; set; }
        IDbSet<LogDieuHanhXe> LogDieuHanhXes { get; set; }
        IDbSet<LyDo> LyDos { get; set; }
        IDbSet<DotXetDuyet> DotXetDuyets { get; set; }
        IDbSet<SangKien> SangKiens { get; set; }
        IDbSet<SangKienFile> SangKienFiles { get; set; }
        IDbSet<ChamSangKien> ChamSangKiens { get; set; }
        IDbSet<PhanCongCham> PhanCongChams { get; set; }
        IDbSet<NguoiThamGia> NguoiThamGias { get; set; }
        IDbSet<PhongBanThamGiaCuocHop> PhongBanThamGiaCuocHops { get; set; }
        IDbSet<LinhVucSangKien> LinhVucSangKiens { get; set; }
        IDbSet<SangKienCNTT> SangKienCNTTs { get; set; }
        IDbSet<SangKienCNTTFile> SangKienCNTTFiles { get; set; }
        IDbSet<SoKiHieuVanBan> SoKiHieuVanBans { get; set; }
        IDbSet<SangKienThanhVien> SangKienThanhViens { get; set; }
        IDbSet<SangKienThanhVienCNTT> SangKienThanhVienCNTTs { get; set; }
        IDbSet<SangKienPhanThuong> SangKienPhanThuongs { get; set; }
        IDbSet<DonViDoan> DonViDoans { get; set; }
        IDbSet<DonVi> DonVis { get; set; }
        IDbSet<DoanVien> DoanViens { get; set; }
        IDbSet<DoanVien_DanToc> DoanVien_DanTocs { get; set; }
        IDbSet<DoanVien_TaiLieu> DoanVien_TaiLieus { get; set; }
        IDbSet<DuAn> DuAns { get; set; }
        IDbSet<DuAnThanhVien> DuAnThanhViens { get; set; }
        IDbSet<DuAnHangMuc> DuAnHangMucs { get; set; }
        IDbSet<DuAnTrienKhai> DuAnTrienKhais { get; set; }
        IDbSet<DuAnTrienKhaiHangMuc> DuAnTrienKhaiHangMucs { get; set; }
        IDbSet<CongViecChung> CongViecChungs { get; set; }
        IDbSet<CongViecChungThanhVien> CongViecChungThanhViens { get; set; }
        IDbSet<DiemDanh> DiemDanhs { get; set; }
        IDbSet<UserPersonalDriveFolder> UserPersonalDriveFolders { get; set; }
        IDbSet<UserPersonalDriveFile> UserPersonalDriveFiles { get; set; }
        IDbSet<UserPersonalDriveFolderShare> UserPersonalDriveFolderShares { get; set; }
        IDbSet<ZaloToken> ZaloTokens { get; set; }
        IDbSet<GiaoViecGoiYNoiDung> GiaoViecGoiYNoiDungs { get; set; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        int SaveChanges();
    }
    public class ConnectDB : DbContext, IContext
    {
        public ConnectDB()
            : base("name=ConnectDB")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;

        }

        public IDbSet<User> Users { get; set; }
        public IDbSet<DatabaseVersion> DatabaseVersions { get; set; }
        public IDbSet<VaiTro> VaiTros { get; set; }
        public IDbSet<PhanQuyen> PhanQuyens { get; set; }
        public IDbSet<PhongBan> PhongBans { get; set; }
        public IDbSet<ChucDanh> ChucDanhs { get; set; }
        public IDbSet<ThongBao> ThongBaos { get; set; }
        public IDbSet<CuocHop> CuocHops { get; set; }
        public IDbSet<DanhMucBaoCao> DanhMucBaoCaos { get; set; }
        public IDbSet<QuanLyBaoCaoChuyenNganh> QuanLyBaoCaoChuyenNganhs { get; set; }
        public IDbSet<DanhMucTraCuuThongTin> DanhMucTraCuuThongTins { get; set; }
        public IDbSet<VanBanPhapLuat> VanBanPhapLuats { get; set; }
        public IDbSet<VanBanPhapLuatFile> VanBanPhapLuatFiles { get; set; }
        public IDbSet<HinhThucVanBan> HinhThucVanBans { get; set; }
        public IDbSet<CoQuanBanHanh> CoQuanBanHanhs { get; set; }
        public IDbSet<DoKhan> DoKhans { get; set; }
        public IDbSet<VaiTroXuLy> VaiTroXuLys { get; set; }
        public IDbSet<QuanLyThongTinTraCuu> QuanLyThongTinTraCuus { get; set; }
        public IDbSet<QuanLyThongTinTraCuuChiTiet> QuanLyThongTinTraCuuChiTiets { get; set; }
        public IDbSet<QuanLyThongTinTraCuuFile> QuanLyThongTinTraCuuFiles { get; set; }
        public IDbSet<NhomNhanViec> NhomNhanViecs { get; set; }
        public IDbSet<NhomNhanViecThanhVien> NhomNhanViecThanhViens { get; set; }
        public IDbSet<GiaoViec> GiaoViecs { get; set; }
        public IDbSet<GiaoViecThanhVien> GiaoViecThanhViens { get; set; }
        public IDbSet<GiaoViecFile> GiaoViecFiles { get; set; }
        public IDbSet<GiaoViecVanBanLienQuan> GiaoViecVanBanLienQuans { get; set; }
        public IDbSet<CongTrinhHuHong> CongTrinhHuHongs { get; set; }
        public IDbSet<CongTrinhHuHongChiTiet> CongTrinhHuHongChiTiets { get; set; }
        public IDbSet<CongTrinhHuHongFile> CongTrinhHuHongFiles { get; set; }
        public IDbSet<DanhSachCongTrinh> DanhSachCongTrinhs { get; set; }
        public IDbSet<GiaoViecThanhVienTienTrinh> GiaoViecThanhVienTienTrinhs { get; set; }
        public IDbSet<GiaoViecThanhVienTienTrinhFile> GiaoViecThanhVienTienTrinhFiles { get; set; }
        public IDbSet<TaiLieuCuocHop> TaiLieuCuocHops { get; set; }
        public IDbSet<NguoiLienQuanCuocHop> NguoiLienQuanCuocHops { get; set; }
        public IDbSet<DieuHanhXe> DieuHanhXes { get; set; }
        public IDbSet<LogDieuHanhXe> LogDieuHanhXes { get; set; }
        public IDbSet<LyDo> LyDos { get; set; }
        public IDbSet<DotXetDuyet> DotXetDuyets { get; set; }
        public IDbSet<SangKien> SangKiens { get; set; }
        public IDbSet<SangKienFile> SangKienFiles { get; set; }
        public IDbSet<ChamSangKien> ChamSangKiens { get; set; }
        public IDbSet<PhanCongCham> PhanCongChams { get; set; }
        public IDbSet<NguoiThamGia> NguoiThamGias { get; set; }
        public IDbSet<PhongBanThamGiaCuocHop> PhongBanThamGiaCuocHops { get; set; }
        public IDbSet<LinhVucSangKien> LinhVucSangKiens { get; set; }
        public IDbSet<SangKienCNTT> SangKienCNTTs { get; set; }
        public IDbSet<SangKienCNTTFile> SangKienCNTTFiles { get; set; }
        public IDbSet<SoKiHieuVanBan> SoKiHieuVanBans { get; set; }
        public IDbSet<SangKienThanhVien> SangKienThanhViens { get; set; }
        public IDbSet<SangKienThanhVienCNTT> SangKienThanhVienCNTTs { get; set; }
        public IDbSet<SangKienPhanThuong> SangKienPhanThuongs { get; set; }
        public IDbSet<DonViDoan> DonViDoans { get; set; }
        public IDbSet<DonVi> DonVis { get; set; }
        public IDbSet<DoanVien> DoanViens { get; set; }
        public IDbSet<DoanVien_DanToc> DoanVien_DanTocs { get; set; }
        public IDbSet<DoanVien_TaiLieu> DoanVien_TaiLieus { get; set; }
        public IDbSet<DuAn> DuAns { get; set; }
        public IDbSet<DuAnThanhVien> DuAnThanhViens { get; set; }
        public IDbSet<DuAnHangMuc> DuAnHangMucs { get; set; }
        public IDbSet<DuAnTrienKhai> DuAnTrienKhais { get; set; }
        public IDbSet<DuAnTrienKhaiHangMuc> DuAnTrienKhaiHangMucs { get; set; }
        public IDbSet<CongViecChung> CongViecChungs { get; set; }
        public IDbSet<CongViecChungThanhVien> CongViecChungThanhViens { get; set; }
        public IDbSet<DiemDanh> DiemDanhs { get; set; }
        public IDbSet<UserPersonalDriveFolder> UserPersonalDriveFolders { get; set; }
        public IDbSet<UserPersonalDriveFile> UserPersonalDriveFiles { get; set; }
        public IDbSet<UserPersonalDriveFolderShare> UserPersonalDriveFolderShares { get; set; }
        public IDbSet<ZaloToken> ZaloTokens { get; set; }
        public IDbSet<GiaoViecGoiYNoiDung> GiaoViecGoiYNoiDungs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VanBanPhapLuatFile>()
              .HasOptional(t => t.VanBanPhapLuat)
              .WithMany(r => r.VanBanPhapLuatFiles)
              .HasForeignKey(t => t.VanBanPhapLuatId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<QuanLyThongTinTraCuuChiTiet>()
               .HasOptional(t => t.QuanLyThongTinTraCuu)
               .WithMany(r => r.QuanLyThongTinTraCuuChiTiets)
               .HasForeignKey(t => t.QuanLyThongTinTraCuuId)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<QuanLyThongTinTraCuuFile>()
              .HasOptional(t => t.QuanLyThongTinTraCuu)
              .WithMany(r => r.QuanLyThongTinTraCuuFiles)
              .HasForeignKey(t => t.QuanLyThongTinTraCuuId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<NhomNhanViecThanhVien>()
              .HasOptional(t => t.NhomNhanViec)
              .WithMany(r => r.NhomNhanViecThanhViens)
              .HasForeignKey(t => t.NhomNhanViecId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<CongTrinhHuHongChiTiet>()
              .HasOptional(t => t.CongTrinhHuHong)
              .WithMany()
              .HasForeignKey(t => t.CongTrinhHuHongId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<CongTrinhHuHongFile>()
              .HasOptional(t => t.CongTrinhHuHong)
              .WithMany()
              .HasForeignKey(t => t.CongTrinhHuHongId);

            modelBuilder.Entity<GiaoViecThanhVien>()
              .HasOptional(t => t.VaiTroXuLy)
              .WithMany()
              .HasForeignKey(t => t.VaiTroXuLyId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<GiaoViecThanhVienTienTrinh>()
              .HasOptional(t => t.GiaoViecThanhVien)
              .WithMany(r => r.GiaoViecThanhVienTienTrinhs)
              .HasForeignKey(t => t.GiaoViecThanhVienId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<GiaoViecThanhVienTienTrinhFile>()
              .HasOptional(t => t.GiaoViecThanhVienTienTrinh)
              .WithMany(r => r.GiaoViecThanhVienTienTrinhFiles)
              .HasForeignKey(t => t.GiaoViecThanhVienTienTrinhId)
              .WillCascadeOnDelete(false);
            modelBuilder.Entity<TaiLieuCuocHop>()
            .HasOptional(t => t.CuocHop)
            .WithMany(r => r.TaiLieuCuocHops)
            .HasForeignKey(t => t.CuocHopId)
            .WillCascadeOnDelete(false);
            modelBuilder.Entity<TaiLieuDatXe>()
               .HasOptional(t => t.DieuHanhXe)
               .WithMany(r => r.TaiLieuDatXes)
               .HasForeignKey(t => t.DieuHanhXeId)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<LogDieuHanhXe>()
              .HasOptional(t => t.DieuHanhXe)
              .WithMany(r => r.LogDieuHanhXes)
              .HasForeignKey(t => t.DieuHanhXeId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<SangKienFile>()
                .HasRequired(t => t.SangKien)
                .WithMany(r => r.SangKienFiles)
                .HasForeignKey(t => t.SangKienId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SangKien>()
                .Property(t => t.TongDiemDouble);
            modelBuilder.Entity<NguoiThamGia>()
              .HasOptional(t => t.CuocHop)
              .WithMany(r => r.NguoiThamGias)
              .HasForeignKey(t => t.CuocHopId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<NguoiThamGia>()
              .HasOptional(t => t.User)
              .WithMany(r => r.NguoiThamGias)
              .HasForeignKey(t => t.UserId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<PhongBanThamGiaCuocHop>()
              .HasOptional(t => t.CuocHop)
              .WithMany(r => r.PhongBanThamGiaCuocHops)
              .HasForeignKey(t => t.CuocHopId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<PhongBanThamGiaCuocHop>()
              .HasOptional(t => t.PhongBan)
              .WithMany()
              .HasForeignKey(t => t.PhongBanId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<DuAnThanhVien>()
              .HasOptional(t => t.DuAn)
              .WithMany(r => r.DuAnThanhViens)
              .HasForeignKey(t => t.DuAnId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<DuAnThanhVien>()
              .HasOptional(t => t.User)
              .WithMany()
              .HasForeignKey(t => t.UserId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<DuAnTrienKhai>()
              .HasOptional(t => t.DuAn)
              .WithMany()
              .HasForeignKey(t => t.DuAnId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<DuAnTrienKhai>()
              .HasOptional(t => t.DonVi)
              .WithMany()
              .HasForeignKey(t => t.DonViId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<DuAnTrienKhaiHangMuc>()
              .HasOptional(t => t.DuAnTrienKhai)
              .WithMany(r => r.DuAnTrienKhaiHangMucs)
              .HasForeignKey(t => t.DuAnTrienKhaiId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<CongViecChungThanhVien>()
              .HasOptional(t => t.CongViecChung)
              .WithMany(r => r.CongViecChungThanhViens)
              .HasForeignKey(t => t.CongViecChungId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<CongViecChungThanhVien>()
              .HasOptional(t => t.User)
              .WithMany()
              .HasForeignKey(t => t.UserId)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserPersonalDriveFile>()
              .HasOptional(t => t.UserPersonalDriveFolder)
              .WithMany(r => r.UserPersonalDriveFiles)
              .HasForeignKey(t => t.UserPersonalDriveFolderId)
              .WillCascadeOnDelete(false);
        }

        public override int SaveChanges()
        {
            // Lấy danh sách các entity đã thay đổi
            var changedEntities = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                .ToList();

            //foreach (var entry in modifiedEntries)
            //{
            //    IAuditableEntity entity = entry.Entity as IAuditableEntity;
            //    if (entity != null)
            //    {
            //        string identityName = Thread.CurrentPrincipal.Identity.Name;
            //        DateTime now = DateTime.UtcNow;

            //        if (entry.State == System.Data.Entity.EntityState.Added)
            //        {
            //            entity.CreatedBy = identityName;
            //            entity.CreatedDate = now;
            //        }
            //        else
            //        {
            //            base.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
            //            base.Entry(entity).Property(x => x.CreatedDate).IsModified = false;
            //        }

            //        entity.UpdatedBy = identityName;
            //        entity.UpdatedDate = now;
            //    }
            //}

            var result = base.SaveChanges();

            var userChanged = changedEntities.Any(e => e.Entity is User);
            if (userChanged)
            {
                CacheManagerHelper.InitUserCache();
            }

            var vaiTroChanged = changedEntities.Any(e => e.Entity is VaiTro);
            if (vaiTroChanged)
            {
                CacheManagerHelper.InitVaiTroCache();
            }

            var phongBanChanged = changedEntities.Any(e => e.Entity is PhongBan);
            if (phongBanChanged)
            {
                CacheManagerHelper.InitPhongBanCache();
            }

            var chucDanhChanged = changedEntities.Any(e => e.Entity is ChucDanh);
            if (chucDanhChanged)
            {
                CacheManagerHelper.InitChucDanhCache();
            }

            var hinhThucVBChanged = changedEntities.Any(e => e.Entity is HinhThucVanBan);
            if (hinhThucVBChanged)
            {
                CacheManagerHelper.InitHinhThucVanBanCache();
            }

            var coQuanBanHanhChanged = changedEntities.Any(e => e.Entity is CoQuanBanHanh);
            if (coQuanBanHanhChanged)
            {
                CacheManagerHelper.InitCoQuanBanHanhCache();
            }

            var doKhanChanged = changedEntities.Any(e => e.Entity is DoKhan);
            if (doKhanChanged)
            {
                CacheManagerHelper.InitDoKhanCache();
            }

            var dmTraCuuChanged = changedEntities.Any(e => e.Entity is DanhMucTraCuuThongTin);
            if (dmTraCuuChanged)
            {
                CacheManagerHelper.InitDanhMucTraCuuThongTinCache();
            }

            var lyDoChanged = changedEntities.Any(e => e.Entity is LyDo);
            if (lyDoChanged)
            {
                CacheManagerHelper.InitLyDoCache();
            }

            return result;
        }
    }
}
