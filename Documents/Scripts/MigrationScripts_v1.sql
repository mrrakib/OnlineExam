TRUNCATE TABLE [dbo].[__MigrationHistory]

ALTER Table Users
Add IsActive bit Not Null Default(0)