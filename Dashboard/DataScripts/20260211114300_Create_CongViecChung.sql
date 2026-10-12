

CREATE TABLE CongViecChung (
    id INT IDENTITY(1,1) PRIMARY KEY,
    ten NVARCHAR(MAX),
    ngayTao DATETIME DEFAULT GETDATE(),
    isDeleted BIT DEFAULT 0
);


CREATE TABLE CongViecChungThanhVien (
    id INT IDENTITY(1,1) PRIMARY KEY,
    congViecChungId INT,
    userId INT,
    isDeleted BIT DEFAULT 0
);

INSERT INTO DatabaseVersion(Name) VALUES(N'20260211114300');
