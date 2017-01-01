CREATE NONCLUSTERED INDEX [ix_usersheetcode_userSheetId_displayed_used] ON [dbo].[UserSheetCode] 
(
	[userSheetId] ASC,
	[displayed] ASC,
	[used] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

INSERT [dbo].[AuthProviders] ([name]) VALUES (N'PINTAN');
GO

UPDATE [dbo].[SETTINGS] SET [VALUE] = '2.0.6' WHERE [SETTING] = 'VERSION'
GO