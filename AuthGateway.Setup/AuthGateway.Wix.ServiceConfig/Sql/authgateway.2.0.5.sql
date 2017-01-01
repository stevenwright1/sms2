﻿--CREATE UNIQUE NONCLUSTERED INDEX [UX_UserSheet_Active_UserId] ON [dbo].[UserSheet] 
--(
--	[active] ASC,
--	[userId] ASC
--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

INSERT [dbo].[SETTINGS] ([SETTING], [VALUE], [OBJECT]) VALUES (N'PASSCODE_LEN', N'6', N'PINTAN')
GO

UPDATE [dbo].[SETTINGS] SET [VALUE] = '2.0.5' WHERE [SETTING] = 'VERSION'
GO