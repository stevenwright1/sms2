BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON

GO
ALTER TABLE [HardToken] ADD [tokentype] NVARCHAR(20) NOT NULL DEFAULT ''
GO
ALTER TABLE [HardToken] ADD [window] NVARCHAR(20) NOT NULL DEFAULT ''
GO

UPDATE [SETTINGS] SET [VALUE] = '2.3.2' WHERE [SETTING] = 'VERSION'
GO

COMMIT