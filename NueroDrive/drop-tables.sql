-- Drop all foreign key constraints first
DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql = @sql + N'
ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' + QUOTENAME(OBJECT_NAME(parent_object_id)) + 
' DROP CONSTRAINT ' + QUOTENAME(name) + ';'
FROM sys.foreign_keys;

EXEC sp_executesql @sql;

-- Now drop all tables
SET @sql = N'';

SELECT @sql = @sql + N'
DROP TABLE ' + QUOTENAME(SCHEMA_NAME(schema_id)) + '.' + QUOTENAME(name) + ';'
FROM sys.tables
WHERE name NOT LIKE '%deploy%' AND name <> '__EFMigrationsHistory';

-- Finally drop the migrations history table
SET @sql = @sql + N'
DROP TABLE __EFMigrationsHistory;';

EXEC sp_executesql @sql; 