ALTER TABLE LyDo
ADD MaMau NVARCHAR(10) NULL;

--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20251112112745');

