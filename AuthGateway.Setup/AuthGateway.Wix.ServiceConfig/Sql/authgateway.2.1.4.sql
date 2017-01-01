﻿BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
GO
ALTER TABLE SMS_CONTACT ADD
	locked bit NOT NULL CONSTRAINT DF_SMS_CONTACT_locked DEFAULT 0
GO
UPDATE [SETTINGS] SET [VALUE] = '2.1.4' WHERE [SETTING] = 'VERSION'
GO
COMMIT
GO