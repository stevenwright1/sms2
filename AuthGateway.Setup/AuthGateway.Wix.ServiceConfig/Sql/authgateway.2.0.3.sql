ALTER TABLE [dbo].[SMS_CONTACT] ADD AUTHENABLED [bit] NULL
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_UserProviders_SMS_CONTACT]') AND parent_object_id = OBJECT_ID(N'[dbo].[UserProviders]'))
ALTER TABLE [dbo].[UserProviders] DROP CONSTRAINT [FK_UserProviders_SMS_CONTACT]
GO

ALTER TABLE [dbo].[UserProviders]  WITH CHECK ADD  CONSTRAINT [FK_UserProviders_SMS_CONTACT] FOREIGN KEY([userId])
REFERENCES [dbo].[SMS_CONTACT] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UserProviders] CHECK CONSTRAINT [FK_UserProviders_SMS_CONTACT]
GO

UPDATE [dbo].[SETTINGS] SET [VALUE] = '2.0.3' WHERE [SETTING] = 'VERSION'
GO