ALTER TABLE GiaoViecThanhVien
ADD NgayNhacHan DATETIME NULL;

--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20251125074010');

