-- Tạo bảng DoanVien_TaiLieu
CREATE TABLE DoanVien_TaiLieu (
    Id INT IDENTITY(1,1) NOT NULL,
    TenFile NVARCHAR(1000) NULL,
    DuongDan NVARCHAR(MAX) NULL,
    NgayTao DATETIME NULL,
    IsDeleted BIT NULL,
    PRIMARY KEY (Id)
);

INSERT INTO DatabaseVersion(Name) VALUES(N'20251212153717');

