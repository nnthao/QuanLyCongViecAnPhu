-- Create DotXetDuyet table with same structure as DotXetDuyet
CREATE TABLE ChamSangKien (
    Id INT IDENTITY(1,1) NOT NULL,
    UserId INT NOT NULL, --nguoi cham sang kien
    SangkienId INT NOT NULL, ---id cua sang kien
    MoTaChiTiet NVARCHAR(MAX) NULL,
    KetQuaChiTiet NVARCHAR(MAX) NULL,--dạng json lưu kết quả từng giám khảo
    GhiChu NVARCHAR(MAX) NULL,
    NguoiXoaId INT  NULL, 
    NgayTao datetime  NULL,
    NgayXoa datetime  NULL,
    IsDeleted BIT  NULL DEFAULT 0,
    TrangThai NVARCHAR(200) NULL,
    PRIMARY KEY (Id)
);
INSERT INTO DatabaseVersion(Name) VALUES(N'2025111708000');
