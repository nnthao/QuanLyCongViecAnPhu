ALTER TABLE VaiTroXuLy
ADD IsCanDinhKemFileKhiXacNhanHoanThanh BIT NULL;

--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20261008183716');
