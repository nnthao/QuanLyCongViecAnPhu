ALTER TABLE GiaoViecThanhVien ADD TrangThai INT NULL;
ALTER TABLE GiaoViecThanhVien ADD NgayXacNhanHoanThanh DATETIME NULL;
ALTER TABLE GiaoViecThanhVien ADD NguoiDanhGiaId INT NULL;
ALTER TABLE GiaoViecThanhVien ADD NgayDanhGia DATETIME NULL;
ALTER TABLE GiaoViecThanhVien ADD KetQuaDanhGia INT NULL;
ALTER TABLE GiaoViecThanhVien ADD YKienDanhGia NVARCHAR(MAX) NULL;

EXEC('UPDATE GiaoViecThanhVien SET TrangThai = 2, KetQuaDanhGia = 1, NgayDanhGia = NgayHoanThanh WHERE IsHoanThanh = 1');
EXEC('UPDATE GiaoViecThanhVien SET TrangThai = 0 WHERE IsHoanThanh IS NULL OR IsHoanThanh = 0');

CREATE TABLE GiaoViecDanhGia (
    Id INT IDENTITY(1,1) NOT NULL,
    GiaoViecId INT NULL,
    GiaoViecThanhVienId INT NOT NULL,
    NguoiDanhGiaId INT NOT NULL,
    NgayDanhGia DATETIME NOT NULL,
    KetQuaDanhGia INT NOT NULL,
    YKienDanhGia NVARCHAR(MAX) NULL,
    IsDeleted BIT NULL DEFAULT 0,
    NgayTao DATETIME NULL,
    PRIMARY KEY (Id)
);

UPDATE VaiTro
SET PhanQuyenKeys = PhanQuyenKeys + ',GiaoViecDanhGia'
WHERE PhanQuyenKeys LIKE '%GiaoViec%' AND PhanQuyenKeys NOT LIKE '%GiaoViecDanhGia%';

INSERT INTO DatabaseVersion(Name) VALUES(N'20261104225258');

