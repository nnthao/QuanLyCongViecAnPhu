-- Create SangKienPhanThuong table with same structure as SangKienPhanThuong
CREATE TABLE SangKienPhanThuong (
    Id INT IDENTITY(1,1) NOT NULL,
    DotXetDuyetId INT NOT NULL, -- FK to DotXetDuyet
    GiaiThuongXepLoai INT NULL,
    TienThuongXepLoai INT NULL,
    NgayTao datetime  NULL,
    NguoiXoaId INT NULL,
    NgayXoa datetime  NULL,
    IsDeleted BIT  NULL DEFAULT 0,
    PRIMARY KEY (Id)
);
INSERT INTO DatabaseVersion(Name) VALUES(N'20251126130000');
