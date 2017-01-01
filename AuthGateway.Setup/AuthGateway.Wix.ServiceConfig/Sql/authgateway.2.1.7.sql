/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON

GO

declare @table_name nvarchar(256)
declare @col_name nvarchar(256)
declare @Command  nvarchar(1000)
set @table_name = N'SMS_CONTACT'
set @col_name = N'PINCODE'

select @Command = 'ALTER TABLE ' + @table_name + ' DROP CONSTRAINT ' + d.name
 from sys.tables t   
  join    sys.default_constraints d       
   on d.parent_object_id = t.object_id  
  join    sys.columns c      
   on c.object_id = t.object_id      
    and c.column_id = d.parent_column_id
 where t.name = @table_name
  and c.name = @col_name

execute (@Command)

UPDATE [SMS_CONTACT] SET [PINCODE] = '' WHERE [PINCODE] IS NULL;
ALTER TABLE [SMS_CONTACT] ALTER COLUMN [PINCODE] VARCHAR(40);
GO
UPDATE [SMS_CONTACT] SET [PINCODE] = LTRIM(RTRIM([PINCODE]));
GO

declare @table_name nvarchar(256)
declare @col_name nvarchar(256)
declare @Command  nvarchar(1000)
set @table_name = N'SMS_LOG'
set @col_name = N'STATE'

select @Command = 'ALTER TABLE [' + @table_name + '] DROP CONSTRAINT ' + d.name
 from sys.tables t   
  join    sys.default_constraints d       
   on d.parent_object_id = t.object_id  
  join    sys.columns c      
   on c.object_id = t.object_id      
    and c.column_id = d.parent_column_id
 where t.name = @table_name
  and c.name = @col_name

execute (@Command)
DROP INDEX [IX_SMS_LOG_STATE] ON [SMS_LOG] ;
UPDATE [SMS_LOG] SET [STATE] = '' WHERE [STATE] IS NULL;
ALTER TABLE [SMS_LOG] ALTER COLUMN [STATE] VARCHAR(10);
CREATE NONCLUSTERED INDEX [IX_SMS_LOG_STATE] ON [dbo].[SMS_LOG] 
(
	[STATE] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
GO
UPDATE [SMS_LOG] SET [STATE] = LTRIM(RTRIM([STATE]));
GO

UPDATE [SETTINGS] SET [VALUE] = '2.1.7' WHERE [SETTING] = 'VERSION'
GO

COMMIT
