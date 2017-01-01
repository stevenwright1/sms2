ALTER TABLE [dbo].[SMS_CONTACT] ADD ORG_NAME_Q NVARCHAR(184) NULL
GO
ALTER TABLE [dbo].[SMS_CONTACT] ADD SID VARCHAR(184) NULL
GO
UPDATE [dbo].[SMS_CONTACT] SET ORG_NAME_Q=ORG_NAME
GO

ALTER TABLE [dbo].[SMS_LOG] ADD ORG_NAME NVARCHAR(100) NULL
GO
ALTER TABLE [dbo].[SMS_LOG] ADD ORG_NAME_Q NVARCHAR(184) NULL
GO

UPDATE [dbo].[SETTINGS] SET [VALUE] = 'Your secure passcode is: {passcode}' 
WHERE [SETTING] = 'SMS_MSG' AND [OBJECT] = 'SMS_SERVICE' AND ([VALUE]='' OR [VALUE]=N' ')
GO

ALTER TABLE [dbo].[SMS_CONTACT] DROP CONSTRAINT [UK_SMS_CONTACT]
GO

ALTER TABLE [dbo].[SMS_CONTACT] ADD CONSTRAINT
[UK_SMS_CONTACT] UNIQUE NONCLUSTERED
(
AD_USERNAME,ORG_NAME,ORG_NAME_Q
) ON [PRIMARY]
GO

UPDATE [dbo].[SETTINGS] SET [VALUE] = '2.0.2' WHERE [SETTING] = 'VERSION'
GO