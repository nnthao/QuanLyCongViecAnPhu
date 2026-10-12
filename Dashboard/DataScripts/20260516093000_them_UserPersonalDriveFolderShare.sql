CREATE TABLE UserPersonalDriveFolderShare (
    Id INT IDENTITY(1,1) NOT NULL,
    UserPersonalDriveFolderId INT NOT NULL,
    UserId INT NOT NULL,
    SharedByUserId INT NOT NULL,
    CreatedAt DATETIME NULL DEFAULT GETDATE(),
    IsDeleted BIT NULL DEFAULT 0,
    PRIMARY KEY (Id)
);

INSERT INTO DatabaseVersion(Name) VALUES(N'20260516093000');
