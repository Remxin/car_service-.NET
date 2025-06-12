import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

interface CreateServiceTaskRequest {
	orderId: number;
	description: string;
	laborCost: number;
}

interface UpdateServiceTaskRequest {
	serviceTaskId: number;
	orderId: number;
	description: string;
	laborCost: number;
}

export const serviceTaskApi = createApi({
	reducerPath: 'serviceTaskApi',
	baseQuery: fetchBaseQuery({
		baseUrl: 'http://localhost:5010/v1/service-tasks',
		prepareHeaders: (headers, { getState }) => {
			const token = (getState() as any).auth.token;
			if (token) headers.set('Authorization', `Bearer ${token}`);
			return headers;
		},
	}),
	tagTypes: ['ServiceTasks'],
	endpoints: (build) => ({
		createServiceTask: build.mutation<{ success: boolean; message: string }, CreateServiceTaskRequest>({
			query: (body) => ({
				url: '/',
				method: 'POST',
				body,
			}),
			invalidatesTags: ['ServiceTasks'],
		}),
		updateServiceTask: build.mutation<{ success: boolean; message: string }, UpdateServiceTaskRequest>({
			query: (body) => ({
				url: '/',
				method: 'PATCH',
				body,
			}),
			invalidatesTags: ['ServiceTasks'],
		}),
		deleteServiceTask: build.mutation<{ success: boolean; message: string }, number>({
			query: (taskId) => ({
				url: `/${taskId}`,
				method: 'DELETE',
			}),
			invalidatesTags: (result, error, id) => [{ type: 'ServiceTasks', id }],
		}),
	}),
});

export const {
	useCreateServiceTaskMutation,
	useUpdateServiceTaskMutation,
	useDeleteServiceTaskMutation,
} = serviceTaskApi;