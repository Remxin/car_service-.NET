CREATE TABLE [vehicles] (
  [id] int PRIMARY KEY IDENTITY(1, 1),
  [brand] nvarchar(255) NOT NULL,
  [model] nvarchar(255) NOT NULL,
  [year] int NOT NULL,
  [vin] varchar(17) UNIQUE NOT NULL,
  [photo_url] nvarchar(255),
  [created_at] datetime DEFAULT (current_timestamp)
)
GO

CREATE TABLE [service_orders] (
  [id] int PRIMARY KEY IDENTITY(1, 1),
  [vehicle_id] int,
  [status] nvarchar(255) NOT NULL,
  [assigned_mechanic_id] int,
  [created_at] datetime DEFAULT (current_timestamp)
)
GO

CREATE TABLE [service_tasks] (
  [id] int PRIMARY KEY IDENTITY(1, 1),
  [order_id] int,
  [description] nvarchar(255) NOT NULL,
  [labor_cost] decimal(10,2) NOT NULL,
  [created_at] datetime DEFAULT (current_timestamp)
)
GO

CREATE TABLE [service_parts] (
  [id] int PRIMARY KEY IDENTITY(1, 1),
  [order_id] int,
  [vehicle_part_id] int,
  [quantity] int NOT NULL
)
GO

CREATE TABLE [vehicle_parts] (
  [id] int PRIMARY KEY IDENTITY(1, 1),
  [name] nvarchar(255) UNIQUE NOT NULL,
  [part_number] nvarchar(255) UNIQUE NOT NULL,
  [description] nvarchar(255),
  [price] decimal(10,2),
  [available_quantity] int
)
GO

CREATE TABLE [service_comments] (
  [id] int PRIMARY KEY IDENTITY(1, 1),
  [order_id] int,
  [content] text,
  [created_at] datetime DEFAULT (current_timestamp)
)
GO

ALTER TABLE [service_orders] ADD FOREIGN KEY ([vehicle_id]) REFERENCES [vehicles] ([id])
GO

ALTER TABLE [service_tasks] ADD FOREIGN KEY ([order_id]) REFERENCES [service_orders] ([id])
GO

ALTER TABLE [service_parts] ADD FOREIGN KEY ([order_id]) REFERENCES [service_orders] ([id])
GO

ALTER TABLE [service_parts] ADD FOREIGN KEY ([vehicle_part_id]) REFERENCES [vehicle_parts] ([id])
GO

ALTER TABLE [service_comments] ADD FOREIGN KEY ([order_id]) REFERENCES [service_orders] ([id])
GO
