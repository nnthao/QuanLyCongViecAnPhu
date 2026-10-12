ALTER TABLE PhongBan
ADD ThuTu INT NULL;

--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20251111201959');

