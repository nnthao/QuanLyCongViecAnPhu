ALTER TABLE PhongBan
ADD NguoiTheoDoiChiDaoId INT NULL;

--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20261001154634');
