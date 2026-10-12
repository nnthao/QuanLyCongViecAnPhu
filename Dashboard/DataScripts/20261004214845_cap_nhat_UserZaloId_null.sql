UPDATE Users 
SET UserZaloId = NULL 
WHERE LOWER(UserZaloId) = 'null' OR LTRIM(RTRIM(UserZaloId)) = '';

UPDATE Users 
SET TelegramId = NULL 
WHERE LOWER(TelegramId) = 'null' OR LTRIM(RTRIM(TelegramId)) = '';

--- Add version script to DatabaseVersion ----
INSERT INTO DatabaseVersion(Name) VALUES(N'20261004214845');
