BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
GO
UPDATE [Message] SET [order] = 5 WHERE [order] = 4;
GO
UPDATE [Message] SET [order] = 4 WHERE [order] = 3;
GO
UPDATE [Message] SET [order] = 3 WHERE [order] = 2;
GO
INSERT INTO [Message]([label], [text], [replacement], [order])
VALUES (
'Passcode E-mail Message',
'Your secure One Time Token is: {passcode}',
'{passcode} : Will be replaced with the current OTP, if this is not found it will be added at the end'
+ CHAR(10) + '{username} : Will be replaced with the username'
+ CHAR(10) + '{fullname} : Will be replaced with the full user name',
2 );
GO
UPDATE [SETTINGS] SET [VALUE] = '2.2.7' WHERE [SETTING] = 'VERSION'
GO
COMMIT
GO