BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
GO
DECLARE @OneTimeProviderId INT;
SELECT @OneTimeProviderId = 
	(SELECT TOP 1 [ID] FROM [AuthProviders] WHERE [name] = 'OneTime');
	
INSERT INTO SMS_LOG (AD_USERNAME,PASSCODE,ORG_NAME,ORG_NAME_Q,STATE,DUE_DATE)
(
	SELECT sc.[AD_USERNAME], up.[config], sc.[ORG_NAME], sc.[ORG_NAME], '', (dateadd(minute,(14400),getdate()))
		FROM [UserProviders] up
		INNER JOIN [SMS_CONTACT] sc ON sc.ID = up.userId
		WHERE [authProviderId] = @OneTimeProviderId AND [config] <> ''
);
UPDATE [UserProviders] SET [config] = '' WHERE [authProviderId] = @OneTimeProviderId AND [config] <> ''
GO
UPDATE [SETTINGS] SET [VALUE] = '2.2.4' WHERE [SETTING] = 'VERSION'
GO
COMMIT
GO