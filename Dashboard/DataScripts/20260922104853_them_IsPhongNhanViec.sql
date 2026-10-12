ALTER TABLE PhongBan
ADD IsPhongNhanViec BIT NULL;



--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20260922104853');
