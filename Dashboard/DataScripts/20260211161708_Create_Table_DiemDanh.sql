-- Create Table DiemDanh
CREATE TABLE DiemDanh (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT,
    NgayNghi DATETIME,
    LoaiNghi INT, -- 1: Sang, 2: Chieu, 3: CaNgay
    LyDo NVARCHAR(MAX),
    IsDeleted BIT DEFAULT 0
);

INSERT INTO DatabaseVersion(Name) VALUES('20260211161708');
