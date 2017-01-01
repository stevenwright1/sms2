﻿BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
GO

CREATE TABLE [dbo].[AUTH_IMAGE_CATEGORIES](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CATEGORY] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_AUTH_IMAGE_CATEGORIES] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [AUTH_IMAGE_CATEGORIES] (CATEGORY)
VALUES ('people'), ('nature'), ('flowers'), ('animals'), ('business'), ('technology'), ('cars'), ('city'), ('christmas')

GO

DROP TABLE [dbo].[AUTH_IMAGES]
GO

DROP TABLE [dbo].[AUTH_IMAGE_THUMBNAILS]
GO

CREATE TABLE [dbo].[AUTH_IMAGES](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[URL] [nvarchar](1000) NOT NULL,
	[CATEGORY_ID] [int] NOT NULL,
 CONSTRAINT [PK_AUTH_IMAGES] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[AUTH_IMAGES]  WITH CHECK ADD  CONSTRAINT [FK_AUTH_IMAGES_AUTH_IMAGES] FOREIGN KEY([ID])
REFERENCES [dbo].[AUTH_IMAGES] ([ID])
GO

ALTER TABLE [dbo].[AUTH_IMAGES] CHECK CONSTRAINT [FK_AUTH_IMAGES_AUTH_IMAGES]
GO

CREATE TABLE [dbo].[AUTH_IMAGE_THUMBNAILS](
	[IMAGE_ID] [bigint] NOT NULL,
	[THUMBNAIL] [varbinary](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[AUTH_IMAGE_THUMBNAILS]  WITH CHECK ADD  CONSTRAINT [FK_AUTH_IMAGE_THUMBNAILS_AUTH_IMAGES] FOREIGN KEY([IMAGE_ID])
REFERENCES [dbo].[AUTH_IMAGES] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[AUTH_IMAGE_THUMBNAILS] CHECK CONSTRAINT [FK_AUTH_IMAGE_THUMBNAILS_AUTH_IMAGES]
GO

ALTER TABLE [dbo].[SMS_CONTACT]
DROP COLUMN [LEFT_IMAGE]
GO

ALTER TABLE [dbo].[SMS_CONTACT]
DROP COLUMN [RIGHT_IMAGE]
GO

ALTER TABLE [dbo].[SMS_CONTACT] ADD [LEFT_IMAGE] [bigint] NULL
ALTER TABLE [dbo].[SMS_CONTACT] ADD [RIGHT_IMAGE] [bigint] NULL

ALTER TABLE [dbo].[SMS_CONTACT]  WITH CHECK ADD  CONSTRAINT [FK_SMS_CONTACT_AUTH_IMAGES] FOREIGN KEY([LEFT_IMAGE])
REFERENCES [dbo].[AUTH_IMAGES] ([ID])
GO

ALTER TABLE [dbo].[SMS_CONTACT] CHECK CONSTRAINT [FK_SMS_CONTACT_AUTH_IMAGES]
GO

ALTER TABLE [dbo].[SMS_CONTACT]  WITH CHECK ADD  CONSTRAINT [FK_SMS_CONTACT_AUTH_IMAGES1] FOREIGN KEY([RIGHT_IMAGE])
REFERENCES [dbo].[AUTH_IMAGES] ([ID])
GO

ALTER TABLE [dbo].[SMS_CONTACT] CHECK CONSTRAINT [FK_SMS_CONTACT_AUTH_IMAGES1]
GO

UPDATE [SETTINGS] SET [VALUE] = '2.3.9' WHERE [SETTING] = 'VERSION'
GO
COMMIT
GO