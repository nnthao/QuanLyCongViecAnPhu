ALTER TABLE SoKiHieuVanBan
ADD Nam INT NULL;

ALTER TABLE SoKiHieuVanBan
ADD HinhThucVanBanId INT NULL;


--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20260204104837');
