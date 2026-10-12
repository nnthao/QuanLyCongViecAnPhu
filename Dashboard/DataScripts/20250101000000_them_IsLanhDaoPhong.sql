ALTER TABLE Users
ADD IsLanhDaoPhong BIT NULL;

--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20250101000000');

