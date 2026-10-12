-- Create linhvucsangkien table with same structure as linhvucsangkien
CREATE TABLE LinhVucSangKien(
    Id INT IDENTITY(1,1) NOT NULL,
    TenLinhVuc NVARCHAR(MAX) NULL,
    IsDeleted BIT  NULL DEFAULT 0,
    PRIMARY KEY (Id)
);
INSERT INTO DatabaseVersion(Name) VALUES(N'20251121140000');
