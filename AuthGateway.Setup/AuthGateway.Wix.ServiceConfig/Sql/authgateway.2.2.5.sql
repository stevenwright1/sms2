BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
GO

CREATE TABLE [Message](
[id] [int] IDENTITY(1,1) NOT NULL,
[label] [nvarchar](200) NOT NULL,
[text] [nvarchar](max) NOT NULL,
[replacement] [nvarchar](max) NOT NULL,
[order] [int] NOT NULL,
 CONSTRAINT [PK_MESSAGE] PRIMARY KEY CLUSTERED 
([id] ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [Message] ADD
	[title] VARCHAR(200) NOT NULL CONSTRAINT DF_Message_title DEFAULT ''

GO

INSERT INTO [Message]([label], [text], [replacement], [order])
VALUES (
'Passcode SMS Message',
'Your secure passcode is: {passcode}',
'{passcode} : Will be replaced with the current passcode, if this is not found it will be added at the end'
+ CHAR(10) + '{username} : Will be replaced with the username associated with the mobile'
+ CHAR(10) + '{fullname} : Will be replaced with the full user name',
1 )

GO

INSERT INTO [Message]([label], [title], [text], [replacement], [order]) VALUES (
'OATH Setup E-mail',
'WrightCCS - SMS2 - QR Configuration image',
'This is your SMS2 configuration code, scan it using Google Authenticator:<br/> <img src="cid:{attachment}" />'
+ CHAR(10) + '<br/>If the QR doesn''t work, you can use this URL: {url}'
+ CHAR(10) + '<br/>Or create the account manually using this shared secret: {sharedsecret}'
,
'{attachment} : Embedded image name to be shown in the e-mail body'
+ CHAR(10) + '{url} : URL that was used to generate the QR code'
+ CHAR(10) + '{sharedsecret} : The secret used to generate the tokens'
+ CHAR(10) + '{username} : Will be replaced with the username associated with the mobile'
+ CHAR(10) + '{fullname} : Will be replaced with the full user name'
, 2 )
GO

INSERT INTO [Message]([label], [title], [text], [replacement], [order]) VALUES (
'OneTime Setup E-mail',
'WrightCCS - Token',
'Your account has been configured with a OneTime secure passcode: {passcode}',
'{passcode} : Will be replaced with the current passcode, if this is not found it will be added at the end'
+ CHAR(10) + '{username} : Will be replaced with the username associated with the mobile'
+ CHAR(10) + '{fullname} : Will be replaced with the full user name',
3)
GO

INSERT INTO [Message]([label], [title], [text], [replacement], [order]) VALUES (
'PIN/TAN Setup E-mail',
'WrightCCS - SMS2 - Authentication sheets',
'Attached you can find your SMS2 authentication sheets', 
'',
4)
GO

UPDATE [SETTINGS] SET [VALUE] = '2.2.5' WHERE [SETTING] = 'VERSION'

GO
COMMIT

GO