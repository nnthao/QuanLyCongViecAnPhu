-- Create DotXetDuyet table with same structure as DotXetDuyet
CREATE TABLE DotXetDuyet (
    Id INT IDENTITY(1,1) NOT NULL,
    TenDotXetDuyet NVARCHAR(MAX) NULL,
    NamXetDuyet NVARCHAR(MAX) NULL,
    GhiChu NVARCHAR(MAX) NULL,
    NguoiXoaId INT  NULL, 
    NgayTao datetime  NULL,
    NgayXoa datetime  NULL,
    IsDeleted BIT  NULL DEFAULT 0,
    TrangThai NVARCHAR(200) NULL,

    PRIMARY KEY (Id)
);
ALTER TABLE dbo.DotXetDuyet ADD TuNgay datetime  NULL;
ALTER TABLE dbo.DotXetDuyet ADD DenNgay datetime  NULL;
INSERT INTO DatabaseVersion(Name) VALUES(N'20251113130000');
