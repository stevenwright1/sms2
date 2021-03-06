BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON

GO

CREATE TABLE [DOMAINS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[DOMAIN] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_DOMAIN] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[DOMAIN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [ALIASES](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[DOMAIN] [bigint] NOT NULL,
	[ALIAS] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_ALIASES] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ALIASES]  WITH CHECK ADD  CONSTRAINT [FK_ALIASES_DOMAIN] FOREIGN KEY([DOMAIN])
REFERENCES [dbo].[DOMAINS] ([ID])
GO

ALTER TABLE [dbo].[ALIASES] CHECK CONSTRAINT [FK_ALIASES_DOMAIN]
GO

ALTER TABLE [dbo].[SMS_CONTACT] ADD [UPN] [nvarchar](256) NULL

UPDATE [SETTINGS] SET [VALUE] = '2.3.4' WHERE [SETTING] = 'VERSION'
GO

COMMIT