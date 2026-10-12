ALTER TABLE GiaoViec
ADD IsTuGiao BIT NULL;



--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20251124221417');


