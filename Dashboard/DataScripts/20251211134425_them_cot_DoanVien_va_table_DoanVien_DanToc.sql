-- Thêm các cột mới vào bảng DoanVien
ALTER TABLE DoanVien
ADD GioiTinh INT NULL,
    QueQuan NVARCHAR(500) NULL,
    DanTocId INT NULL,
    ChuyenMonNghiepVu NVARCHAR(500) NULL,
    LyLuanChinhTri NVARCHAR(500) NULL,
    NgaySinh DATETIME NULL,
    NgayVaoDoan NVARCHAR(500) NULL,
    NgayVaoDang NVARCHAR(500) NULL;

-- Tạo bảng DoanVien_DanToc
CREATE TABLE DoanVien_DanToc (
    Id INT IDENTITY(1,1) NOT NULL,
    Ten NVARCHAR(200) NULL,
    IsDeleted BIT NULL,
    PRIMARY KEY (Id)
);

INSERT INTO DatabaseVersion(Name) VALUES(N'20251211134425');

