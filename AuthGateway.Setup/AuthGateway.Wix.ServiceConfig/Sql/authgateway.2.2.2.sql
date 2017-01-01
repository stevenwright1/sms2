﻿BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
GO
ALTER TABLE [SMS_CONTACT] ADD
	[uSNChanged] VARCHAR(10) NOT NULL CONSTRAINT DF_SMS_CONTACT_uSNChanged DEFAULT ''
GO
INSERT INTO [SETTINGS] (SETTING, VALUE, OBJECT) VALUES (N'CONFIG_CHK', N'', N'AUTHGATEWAY');
GO
UPDATE [SETTINGS] SET [VALUE] = '2.2.2' WHERE [SETTING] = 'VERSION'
GO
COMMIT
GO