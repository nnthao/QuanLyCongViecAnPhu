ALTER TABLE GiaoViecThanhVien
ADD NgayChuyenGiao DATETIME NULL;

--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20251115075452');

