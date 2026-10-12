ALTER TABLE VaiTroXuLy
ADD IsThongKeGiaoViec BIT NULL;

--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20261008175537');
