﻿BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [AE_HEARTBEAT] ADD [IMAGES_PREFERENCE] [int] NOT NULL DEFAULT 2
GO

ALTER TABLE [AE_HEARTBEAT] ADD [IMAGES_POLLING_MASTER] [bit] NOT NULL DEFAULT 0
GO


UPDATE [SETTINGS] SET [VALUE] = '2.4.2' WHERE [SETTING] = 'VERSION'
GO
COMMIT
GO
