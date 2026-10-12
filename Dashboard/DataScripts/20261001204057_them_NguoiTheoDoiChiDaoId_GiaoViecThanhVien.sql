ALTER TABLE GiaoViecThanhVien
ADD NguoiTheoDoiChiDaoId INT NULL;


--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20261001204057');
