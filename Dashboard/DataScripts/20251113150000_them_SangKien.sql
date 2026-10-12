-- Create DotXetDuyet table with same structure as DotXetDuyet
CREATE TABLE SangKien (
    Id INT IDENTITY(1,1) NOT NULL,
    DotXetDuyetId INT NOT NULL,
    NamXetDuyet NVARCHAR(MAX) NULL,
    MoTaChiTiet NVARCHAR(MAX) NULL,
    GhiChu NVARCHAR(MAX) NULL,
    NguoiXoaId INT  NULL, 
    NgayTao datetime  NULL,
    NgayXoa datetime  NULL,
    IsDeleted BIT  NULL DEFAULT 0,
    TrangThai NVARCHAR(200) NULL,
    PRIMARY KEY (Id)
);
CREATE TABLE SangKienFile (
    Id INT IDENTITY(1,1) NOT NULL,
    SangkienId INT NOT NULL, 
    TenFile NVARCHAR(255) NULL,  
    DuongDan NVARCHAR(1000) NULL, 
    NgayTao DATETIME NULL DEFAULT GETDATE(),
    NguoiXoaId INT NULL, 
    NgayXoa DATETIME NULL,
    IsDeleted BIT NULL DEFAULT 0,

    PRIMARY KEY (Id)
);
INSERT INTO DatabaseVersion(Name) VALUES(N'20251113150000');
