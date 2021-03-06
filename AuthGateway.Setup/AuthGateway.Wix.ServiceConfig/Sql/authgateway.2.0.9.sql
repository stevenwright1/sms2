/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE [SMS_CONTACT] ADD
	[userStatus] tinyint NOT NULL DEFAULT 0
GO

UPDATE
	[SMS_CONTACT]
SET
	[userStatus] = 1
WHERE
	[USER_STATUS] = 'True'

GO

ALTER TABLE 
	[SMS_CONTACT] 
DROP CONSTRAINT 
	[DF_SMS_CONTACT_USER_STATUS] 
GO


ALTER TABLE
	[SMS_CONTACT]
DROP COLUMN
	[USER_STATUS]

GO

UPDATE
	[SMS_CONTACT]
SET
	[ORG_NAME] = [ORG_NAME_Q]

ALTER TABLE [SMS_CONTACT] DROP CONSTRAINT [UK_SMS_CONTACT]
GO

ALTER TABLE
	[SMS_CONTACT]
DROP COLUMN
	[ORG_NAME_Q]

ALTER TABLE [SMS_CONTACT] ADD CONSTRAINT
[UK_SMS_CONTACT] UNIQUE NONCLUSTERED
(
AD_USERNAME,ORG_NAME
) ON [PRIMARY]
GO


CREATE NONCLUSTERED INDEX [IX_SMS_CONTACT_ORG_NAME] ON [SMS_CONTACT] 
(
	[ORG_NAME] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]

GO



UPDATE [SETTINGS] SET [VALUE] = '2.0.9' WHERE [SETTING] = 'VERSION'
GO

COMMIT
