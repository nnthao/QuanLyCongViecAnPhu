--Phan cong xem user nao se cham sang kien nay


CREATE TABLE PhanCongCham (
    Id INT IDENTITY(1,1) NOT NULL,
    SangkienId INT NOT NULL,
    UserId INT NOT NULL,   -- admin duoc phan cong
    NguoiXoaId INT  NULL, 
    NgayTao datetime  NULL,
    NgayXoa datetime  NULL,
    IsDeleted BIT  NULL DEFAULT 0,
    TrangThai NVARCHAR(200) NULL,

    PRIMARY KEY (Id)
);
INSERT INTO DatabaseVersion(Name) VALUES(N'20251120150000');