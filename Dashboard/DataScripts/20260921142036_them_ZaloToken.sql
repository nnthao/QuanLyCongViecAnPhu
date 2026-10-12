CREATE TABLE ZaloToken
(Id int IDENTITY NOT NULL, 
AccessToken nvarchar(MAX) NULL,
RefreshToken nvarchar(MAX) NULL,
PRIMARY KEY (Id));

--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20260921142036');
