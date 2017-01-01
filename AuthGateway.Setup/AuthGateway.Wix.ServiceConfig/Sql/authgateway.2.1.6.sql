/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON

GO

INSERT [dbo].[AuthProviders] ([name]) VALUES (N'Static');
GO

UPDATE [SETTINGS] SET [VALUE] = '2.1.6' WHERE [SETTING] = 'VERSION'
GO

COMMIT
