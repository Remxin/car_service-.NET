export type Role = 'admin' | 'mechanic' | 'reception';

export type User = {
	id: number;
	name: string;
	email: string;
	created_at: {
		seconds: string;
		nanos: number;
	};
	roles: Role[];
};
export type LoginRequestBody = {
	email: string;
	password: string;
};

export type LoginResponse = {
	success: boolean;
	message: string;
	token?: string;
};

export type RegisterRequestBody = {
	email: string;
	password: string;
	firstName: string;
	lastName: string;
};

export type AddRoleBody = {
	userId: number;
	roleId: number;
	token: string;
};

export type RemoveRoleBody = {
	userId: number;
	roleId: number;
	token: string;
};

export type VerifyResponse = {
	isValid: boolean;
	message: string;
	user: User;
	roles: Role[];
};
