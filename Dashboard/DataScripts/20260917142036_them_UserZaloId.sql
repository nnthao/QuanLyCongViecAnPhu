ALTER TABLE Users
ADD UserZaloId NVARCHAR(MAX) NULL;

--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20260917142036');
