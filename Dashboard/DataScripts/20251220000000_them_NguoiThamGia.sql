-- Tạo bảng NguoiThamGia
CREATE TABLE NguoiThamGia (
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [CuocHopId] INT NOT NULL,
    [UserId] INT NOT NULL
);


-- Add version script to DatabaseVersion
INSERT INTO DatabaseVersion(Name) VALUES(N'20251220000000');

