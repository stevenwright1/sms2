BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
GO
UPDATE [Message] 
SET
	[text] = 'Your secure passcode is: {passcode} Please contact your local Service Desk should you require assistance'
WHERE 
	[label] = 'Passcode SMS Message'
	OR [label] = 'Passcode E-mail Message'
GO
UPDATE [SETTINGS] SET [VALUE] = '2.3.6' WHERE [SETTING] = 'VERSION'
GO
COMMIT
GO
