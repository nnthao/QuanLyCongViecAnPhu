ALTER TABLE GiaoViecThanhVien
ADD SoGioUocTinh INT NULL;

--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20251119075855');

