ALTER TABLE PhongBan
ADD PhongBanChaId INT NULL;

--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20251111201245');

