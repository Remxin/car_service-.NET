-- Create roles
INSERT INTO roles (name, description) VALUES
('admin', 'Administrator with full access'),
('receptionist', 'Receptionist who can view parts and manage service orders'),
('mechanic', 'Mechanic who can view and update assigned service orders and tasks');

-- Create permissions
INSERT INTO permissions (name, description) VALUES
('view_vehicle_parts', 'Permission to view vehicle parts'),
('list_vehicle_parts', 'Permission to list vehicle parts'),
('view_service_order', 'Permission to view service orders'),
('create_service_order', 'Permission to create new service orders'),
('update_service_order', 'Permission to update service orders'),
('view_service_task', 'Permission to view service tasks'),
('create_service_task', 'Permission to create service tasks'),
('add_service_comment', 'Permission to add comments to service orders'),
('view_service_comment', 'Permission to view comments on service orders'),
('manage_users', 'Permission to manage users and roles'),
('generate_reports', 'Permission to generate reports'),
('list_reports', 'Permission to list reports'),
('add_vehicles', 'Permission to add vehicles'),
('list_vehicles', 'Permission to list vehicles');

INSERT INTO role_permissions (role_id, permission_id) VALUES
(2, (SELECT id FROM permissions WHERE name = 'list_vehicle_parts')),
(2, (SELECT id FROM permissions WHERE name = 'view_vehicle_parts')),
(2, (SELECT id FROM permissions WHERE name = 'view_service_order')),
(2, (SELECT id FROM permissions WHERE name = 'create_service_order')),
(2, (SELECT id FROM permissions WHERE name = 'add_service_comment')),
(2, (SELECT id FROM permissions WHERE name = 'view_service_comment')),
(2,  (SELECT id FROM permissions WHERE name = 'add_vehicles')),
(2,  (SELECT id FROM permissions WHERE name = 'list_vehicles'));

INSERT INTO role_permissions (role_id, permission_id) VALUES
(3, (SELECT id FROM permissions WHERE name = 'view_vehicle_parts')),
(3, (SELECT id FROM permissions WHERE name = 'view_service_order')),
(3, (SELECT id FROM permissions WHERE name = 'update_service_order')),
(3, (SELECT id FROM permissions WHERE name = 'view_service_task')),
(3, (SELECT id FROM permissions WHERE name = 'create_service_task')),
(3, (SELECT id FROM permissions WHERE name = 'add_service_comment')),
(3, (SELECT id FROM permissions WHERE name = 'view_service_comment'));

INSERT INTO role_permissions (role_id, permission_id)
SELECT 1, id FROM permissions;

INSERT INTO users(name, email, password_hash) VALUES ('admin', 'test@email.com', 'AQAAAAIAAYagAAAAEM5OhJFUaS7ZiTjO1LIjWmJ2LrfZE2+2tsoeikO26DNVQLxQmKpTO92r227RwL0YLA==');
INSERT INTO user_roles(user_id, role_id) VALUES(1, 1);
