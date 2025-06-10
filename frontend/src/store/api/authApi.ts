import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import type { Role } from '@/types/auth.types';
import { User, RemoveRoleBody, AddRoleBody, RegisterRequestBody, LoginRequestBody, LoginResponse, VerifyResponse } from '@/types/auth.types';

export const authApi = createApi({
	reducerPath: 'authApi',
	baseQuery: fetchBaseQuery({
		baseUrl: 'http://localhost:5010/v1/users',
		prepareHeaders: (headers, { getState }) => {
			const token = (getState() as any).auth.token;
			console.log('Token:', token);
			if (token) headers.set('Authorization', `Bearer ${token}`);
			return headers;
		},
	}),
	tagTypes: ['Users'],
	endpoints: (build) => ({
		getUsers: build.query<User[], void>({
			query: () => `/`,
			transformResponse: (response: { usersWithRoles: { user: User; roleName: Role[] }[] }) => {
				return response.usersWithRoles.map(({ user, roleName }) => ({
					...user,
					roles: roleName,
				}));
			},
			providesTags: (result) =>
				result ? result.map((u) => ({ type: 'Users', id: u.id })) : [],
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
		addRole: build.mutation<{ success: boolean; message: string }, AddRoleBody>({
			query: (body) => ({
				url: 'add-role',
				method: 'POST',
				body,
			}),
			invalidatesTags: ['Users'],
		}),
		removeRole: build.mutation<{ success: boolean; message: string }, RemoveRoleBody>({
			query: (body) => ({
				url: 'remove-role',
				method: 'POST',
				body,
			}),
			invalidatesTags: ['Users'],
		}),
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
