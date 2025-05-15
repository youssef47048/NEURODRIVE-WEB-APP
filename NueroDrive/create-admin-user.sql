-- Create an admin user in AspNetUsers
DECLARE @userId NVARCHAR(450) = NEWID();
DECLARE @securityStamp NVARCHAR(MAX) = NEWID();
DECLARE @concurrencyStamp NVARCHAR(MAX) = NEWID();

-- Insert admin user
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
VALUES (
    @userId, 
    'admin@example.com', 
    'ADMIN@EXAMPLE.COM', 
    'admin@example.com', 
    'ADMIN@EXAMPLE.COM', 
    1, 
    'AQAAAAIAAYagAAAAEG9Mn+m0HCL3GJnGVUKx9gYoHvQvfLNj84jWQfQjeFhZ5cYjdnYNtTcU8JiahE+FXw==', -- This is a hashed password 'Admin123!'
    @securityStamp, 
    @concurrencyStamp, 
    '1234567890', 
    0, 
    0, 
    0, 
    0, 
    'Admin User'
);

-- Create an aspnetusers role
DECLARE @roleId NVARCHAR(450) = NEWID();

INSERT INTO AspNetRoles (
    Id,
    Name,
    NormalizedName,
    ConcurrencyStamp
)
VALUES (
    @roleId,
    'Admin',
    'ADMIN',
    NEWID()
);

-- Assign admin role to the user
INSERT INTO AspNetUserRoles (
    UserId,
    RoleId
)
VALUES (
    @userId,
    @roleId
);

SELECT 'Admin user created with ID: ' + @userId AS Message; 