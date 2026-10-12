CREATE TABLE SoKiHieuVanBan (
    Id INT IDENTITY(1,1) NOT NULL,
    SoKiHieu INT NULL,
    UserId INT NULL,
    NgayLay DATETIME NULL,
    PRIMARY KEY (Id)
);

--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20251120000000');

