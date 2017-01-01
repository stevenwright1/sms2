SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AuthProviders](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_AuthProviders] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET IDENTITY_INSERT [dbo].[AuthProviders] ON
INSERT [dbo].[AuthProviders] ([id], [name]) VALUES (1, N'CloudSMS')
INSERT [dbo].[AuthProviders] ([id], [name]) VALUES (2, N'OATHCalc')
SET IDENTITY_INSERT [dbo].[AuthProviders] OFF

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[UserProviders](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [bigint] NOT NULL,
	[authProviderId] [int] NOT NULL,
	[active] [tinyint] NOT NULL,
	[selected] [tinyint] NOT NULL,
	[config] [varchar](max) NULL,
 CONSTRAINT [PK_UserProviders] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [IX_UserProviders] UNIQUE NONCLUSTERED 
(
	[userId] ASC,
	[authProviderId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[UserProviders]  WITH CHECK ADD  CONSTRAINT [FK_UserProviders_AuthProviders] FOREIGN KEY([authProviderId])
REFERENCES [dbo].[AuthProviders] ([id])
GO

ALTER TABLE [dbo].[UserProviders] CHECK CONSTRAINT [FK_UserProviders_AuthProviders]
GO

ALTER TABLE [dbo].[UserProviders]  WITH CHECK ADD  CONSTRAINT [FK_UserProviders_SMS_CONTACT] FOREIGN KEY([userId])
REFERENCES [dbo].[SMS_CONTACT] ([ID])
GO

ALTER TABLE [dbo].[UserProviders] CHECK CONSTRAINT [FK_UserProviders_SMS_CONTACT]
GO

ALTER TABLE [dbo].[UserProviders] ADD  CONSTRAINT [DF_UserProviders_active]  DEFAULT ((0)) FOR [active]
GO

ALTER TABLE [dbo].[UserProviders] ADD  CONSTRAINT [DF_UserProviders_selected]  DEFAULT ((0)) FOR [selected]
GO

CREATE NONCLUSTERED INDEX [IX_UserProviders_1] ON [dbo].[UserProviders] 
(
	[userId] ASC,
	[selected] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

UPDATE [dbo].[SETTINGS] SET [VALUE] = '2.0.1' WHERE [SETTING] = 'VERSION'
GO