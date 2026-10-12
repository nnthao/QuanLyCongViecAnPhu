ALTER TABLE GiaoViecThanhVien
ADD GiaoViecThanhVienChuyenGiaoId INT NULL;

--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20251114151949');

