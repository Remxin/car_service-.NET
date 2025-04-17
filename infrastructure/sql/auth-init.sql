CREATE TABLE [users] (
  [id] int PRIMARY KEY IDENTITY(1, 1),
  [name] varchar(50) NOT NULL,
  [email] varchar(255) UNIQUE NOT NULL,
  [password_hash] varchar(255) NOT NULL,
  [created_at] datetime DEFAULT (current_timestamp)
)
GO

CREATE TABLE [roles] (
  [id] int PRIMARY KEY IDENTITY(1, 1),
  [name] nvarchar(255) UNIQUE NOT NULL,
  [description] nvarchar(255)
)
GO

CREATE TABLE [permissions] (
  [id] int PRIMARY KEY IDENTITY(1, 1),
  [name] nvarchar(255) UNIQUE NOT NULL,
  [description] nvarchar(255)
)
GO

CREATE TABLE [user_roles] (
  [user_id] int,
  [role_id] int,
  PRIMARY KEY ([user_id], [role_id])
)
GO

CREATE TABLE [role_permissions] (
  [role_id] int,
  [permission_id] int,
  PRIMARY KEY ([role_id], [permission_id])
)
GO

ALTER TABLE [user_roles] ADD FOREIGN KEY ([user_id]) REFERENCES [users] ([id])
GO

ALTER TABLE [user_roles] ADD FOREIGN KEY ([role_id]) REFERENCES [roles] ([id])
GO

ALTER TABLE [role_permissions] ADD FOREIGN KEY ([role_id]) REFERENCES [roles] ([id])
GO

ALTER TABLE [role_permissions] ADD FOREIGN KEY ([permission_id]) REFERENCES [permissions] ([id])
GO
