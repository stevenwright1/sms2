﻿BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
GO

ALTER TABLE [dbo].[AUTH_IMAGES]  WITH CHECK ADD  CONSTRAINT [FK_AUTH_IMAGES_AUTH_IMAGE_CATEGORIES] FOREIGN KEY([CATEGORY_ID])
REFERENCES [dbo].[AUTH_IMAGE_CATEGORIES] ([ID])
GO

ALTER TABLE [dbo].[AUTH_IMAGES] CHECK CONSTRAINT [FK_AUTH_IMAGES_AUTH_IMAGE_CATEGORIES]
GO

UPDATE [SETTINGS] SET [VALUE] = '2.4.0' WHERE [SETTING] = 'VERSION'
GO
COMMIT
GO
