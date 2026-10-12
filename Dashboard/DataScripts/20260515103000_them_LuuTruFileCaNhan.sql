CREATE TABLE UserPersonalDriveFolder (
    Id INT IDENTITY(1,1) NOT NULL,
    NguoiTaoId INT NOT NULL,
    ParentFolderId INT NULL,
    Name NVARCHAR(500) NOT NULL,
    FolderName NVARCHAR(500) NOT NULL,
    CreatedAt DATETIME NULL DEFAULT GETDATE(),
    IsDeleted BIT NULL DEFAULT 0,
    PRIMARY KEY (Id)
);

CREATE TABLE UserPersonalDriveFile (
    Id INT IDENTITY(1,1) NOT NULL,
    NguoiTaoId INT NOT NULL,
    UserPersonalDriveFolderId INT NULL,
    FileName NVARCHAR(500) NULL,
    DuongDan NVARCHAR(MAX) NULL,
    NgayTao DATETIME NULL DEFAULT GETDATE(),
    IsDeleted BIT NULL DEFAULT 0,
    PRIMARY KEY (Id)
);

INSERT INTO DatabaseVersion(Name) VALUES(N'20260515103000');
