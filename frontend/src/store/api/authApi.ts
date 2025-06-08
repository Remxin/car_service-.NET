// src/store/api/authApi.ts
import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export interface UserWithRoles {
	id: number;
	email: string;
	firstName: string;
	lastName: string;
	roles: string[];
}

export interface LoginRequestBody {
	email: string;
	password: string;
}
export interface LoginResponse {
	success: boolean;
	message: string;
	token?: string;
}

export interface RegisterRequestBody {
	email: string;
	password: string;
	firstName: string;
	lastName: string;
}

export interface AddRoleBody {
	userId: number;
	roleId: number;
	token: string;
}
export interface RemoveRoleBody {
	userId: number;
	roleId: number;
	token: string;
}

export interface VerifyResponse {
	isValid: boolean;
	message: string;
}

export const authApi = createApi({
	reducerPath: 'authApi',
	baseQuery: fetchBaseQuery({
		baseUrl: 'http://localhost:5010/v1/users',
		prepareHeaders: (headers, { getState }) => {
			const token = (getState() as any).auth.token;
			if (token) headers.set('Authorization', `Bearer ${token}`);
			return headers;
		},
	}),
	tagTypes: ['Users'],
	endpoints: (build) => ({
		getUsers: build.query<UserWithRoles[], void>({
			query: () => `/`,
			providesTags: (result = []) =>
				result.map((u) => ({ type: 'Users' as const, id: u.id })),
		}),
		login: build.mutation<LoginResponse, LoginRequestBody>({
			query: (body) => ({
				url: 'login',
				method: 'POST',
				body,
			}),
		}),
		register: build.mutation<LoginResponse, RegisterRequestBody>({
			query: (body) => ({
				url: 'register',
				method: 'POST',
				body,
			}),
		}),
		// dodaj rolę
		addRole: build.mutation<{ success: boolean; message: string }, AddRoleBody>({
			query: (body) => ({
				url: 'add-role',
				method: 'POST',
				body,
			}),
			invalidatesTags: ['Users'],
		}),
		// usuń rolę
		removeRole: build.mutation<{ success: boolean; message: string }, RemoveRoleBody>({
			query: (body) => ({
				url: 'remove-role',
				method: 'POST',
				body,
			}),
			invalidatesTags: ['Users'],
		}),
		// weryfikacja tokena
		verify: build.mutation<VerifyResponse, void>({
			query: () => ({
				url: 'verify',
				method: 'POST',
			}),
		}),
	}),
});

export const {
	useGetUsersQuery,
	useLoginMutation,
	useRegisterMutation,
	useAddRoleMutation,
	useRemoveRoleMutation,
	useVerifyMutation,
} = authApi;
