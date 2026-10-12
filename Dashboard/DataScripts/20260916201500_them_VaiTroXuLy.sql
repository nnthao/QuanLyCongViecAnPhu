CREATE TABLE VaiTroXuLy (
    Id INT IDENTITY(1,1) NOT NULL,
    Ten NVARCHAR(500) NULL,
    ThuTu INT NULL,
    IsDeleted BIT NULL DEFAULT 0,
    PRIMARY KEY (Id)
);

INSERT INTO VaiTroXuLy (Ten, ThuTu, IsDeleted) VALUES (N'Xử lý chính', 1, 0);
INSERT INTO VaiTroXuLy (Ten, ThuTu, IsDeleted) VALUES (N'Phối hợp', 2, 0);
INSERT INTO VaiTroXuLy (Ten, ThuTu, IsDeleted) VALUES (N'Theo dõi', 3, 0);
INSERT INTO VaiTroXuLy (Ten, ThuTu, IsDeleted) VALUES (N'Tham mưu', 4, 0);
INSERT INTO VaiTroXuLy (Ten, ThuTu, IsDeleted) VALUES (N'Chỉ đạo', 5, 0);

INSERT INTO DatabaseVersion(Name) VALUES(N'20260916201500');
