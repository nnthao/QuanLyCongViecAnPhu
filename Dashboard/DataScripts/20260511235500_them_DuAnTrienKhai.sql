CREATE TABLE DuAnTrienKhai (
    Id INT IDENTITY(1,1) NOT NULL,
    DuAnId INT NULL,
    DonViId INT NULL,
    Ten NVARCHAR(500) NULL,
    MoTa NVARCHAR(MAX) NULL,
    NgayBatDau DATETIME NULL,
    NgayKetThuc DATETIME NULL,
    IsDeleted BIT NULL,
    PRIMARY KEY (Id)
);

CREATE TABLE DuAnTrienKhaiHangMuc (
    Id INT IDENTITY(1,1) NOT NULL,
    DuAnTrienKhaiId INT NULL,
    DuAnHangMucId INT NULL,
    TenHangMuc NVARCHAR(500) NULL,
    MoTa NVARCHAR(MAX) NULL,
    ThuTu INT NULL,
    NgayBatDau DATETIME NULL,
    NgayKetThuc DATETIME NULL,
    TienDo INT NULL,
    GhiChu NVARCHAR(MAX) NULL,
    IsDeleted BIT NULL,
    PRIMARY KEY (Id)
);

INSERT INTO DatabaseVersion(Name) VALUES(N'20260511235500');
