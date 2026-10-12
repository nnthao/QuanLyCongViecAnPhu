ALTER TABLE GiaoViecThanhVien
ADD VaiTroXuLyId INT NULL;

--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20260916212036');
