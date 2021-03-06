BEGIN TRANSACTION
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

CREATE TABLE [dbo].[AE_HEARTBEAT](
	[DATETIME] [datetime] NOT NULL,
	[AE_INSTANCE] [nvarchar](34) NOT NULL,
	[PREFERENCE] [int] NOT NULL,
	[POLLING_MASTER] [bit] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[AE_HEARTBEAT] ADD  CONSTRAINT [DF_AE_HEARTBEAT_POLLING_MASTER]  DEFAULT ((0)) FOR [POLLING_MASTER]
GO

CREATE TABLE [dbo].[AE_COMMANDS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[DATETIME] [datetime] NOT NULL,
	[RECIPIENT_SERVER] [nvarchar](34) NOT NULL,
	[COMMAND] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_AE_COMMANDS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

UPDATE [SETTINGS] SET [VALUE] = '2.4.1' WHERE [SETTING] = 'VERSION'
GO
COMMIT
GO
