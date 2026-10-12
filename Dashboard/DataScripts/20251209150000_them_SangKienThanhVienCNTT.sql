-- Create SangKienThanhVien table with same structure as SangKienThanhVien
CREATE TABLE SangKienThanhVienCNTT (
    Id INT IDENTITY(1,1) NOT NULL,
    SangKienCNTTId INT NOT NULL, -- FK to SangKien
    UserId INT NOT NULL,     -- FK to User
    TiLeDongGop DECIMAL(5,2) NOT NULL, -- 0.00 - 100.00
    VaiTro NVARCHAR(MAX) NULL, -- "chu tri,thanh vien
    GhiChu NVARCHAR(MAX) NULL,
    NgayTao datetime  NULL,
    NguoiXoaId INT NULL,
    NgayXoa datetime  NULL,
    IsDeleted BIT  NULL DEFAULT 0,
    PRIMARY KEY (Id)
);
INSERT INTO DatabaseVersion(Name) VALUES(N'20251209150000');
