-- Check structure of existing Users table
SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users';
GO

-- Check the structure of AspNetUsers table
SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'AspNetUsers';
GO

-- Migrate existing users to AspNetUsers
-- Note: Since the password hash in the old system is different than Identity's hash format,
-- users will need to reset their passwords
INSERT INTO AspNetUsers (
    Id, 
    UserName, 
    NormalizedUserName, 
    Email, 
    NormalizedEmail,
    EmailConfirmed,
    PasswordHash,
    SecurityStamp,
    ConcurrencyStamp,
    PhoneNumber,
    PhoneNumberConfirmed,
    TwoFactorEnabled,
    LockoutEnabled,
    AccessFailedCount,
    Name
)
SELECT 
    CONVERT(NVARCHAR(450), u.Id), -- Convert int to string for the Id
    u.Email, -- Use email as username
    UPPER(u.Email), -- Normalized username
    u.Email,
    UPPER(u.Email), -- Normalized email
    1, -- EmailConfirmed = true
    NULL, -- We can't migrate the password hash as the format is different
    NEWID(), -- Generate a new security stamp
    NEWID(), -- Generate a new concurrency stamp
    u.PhoneNumber,
    0, -- PhoneNumberConfirmed = false
    0, -- TwoFactorEnabled = false
    0, -- LockoutEnabled = false
    0, -- AccessFailedCount = 0
    u.Name
FROM Users u
WHERE NOT EXISTS (
    SELECT 1 FROM AspNetUsers au WHERE au.Email = u.Email
);
GO

-- Update foreign keys in Vehicles table to point to new user ids
-- First, create a temporary table to map old IDs to new IDs
CREATE TABLE #UserIdMapping (
    OldId INT,
    NewId NVARCHAR(450)
);

-- Populate the mapping table
INSERT INTO #UserIdMapping (OldId, NewId)
SELECT u.Id, au.Id
FROM Users u
JOIN AspNetUsers au ON au.Email = u.Email;

-- Check the mappings
SELECT * FROM #UserIdMapping;

-- Update the Vehicle table to use the new string IDs
-- Note: This will fail if the column doesn't exist or has the wrong type
-- You may need to modify this based on your actual schema
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Vehicles' AND COLUMN_NAME = 'UserId')
BEGIN
    -- Create a backup of the Vehicles table
    SELECT * INTO Vehicles_Backup FROM Vehicles;
    
    -- Update the UserId column to use the new string IDs
    UPDATE v
    SET v.UserId = m.NewId
    FROM Vehicles v
    JOIN #UserIdMapping m ON v.UserId = CONVERT(NVARCHAR(450), m.OldId);
END

-- Drop the temporary table
DROP TABLE #UserIdMapping;
GO 