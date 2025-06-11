import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

interface CreateServicePartRequest {
	orderId: number;
	name: string;
	quantity: number;
	price: number;
}

interface UpdateServicePartRequest {
	servicePartId: number;
	orderId: number;
	name: string;
	quantity: number;
	price: number;
}

export const servicePartApi = createApi({
	reducerPath: 'servicePartApi',
	baseQuery: fetchBaseQuery({
		baseUrl: 'http://localhost:5010/v1/service-parts',
		prepareHeaders: (headers, { getState }) => {
			const token = (getState() as any).auth.token;
			if (token) headers.set('Authorization', `Bearer ${token}`);
			return headers;
		},
	}),
	tagTypes: ['ServiceParts'],
	endpoints: (build) => ({
		createServicePart: build.mutation<{ success: boolean; message: string }, CreateServicePartRequest>({
			query: (body) => ({
				url: '/',
				method: 'POST',
				body,
			}),
			invalidatesTags: ['ServiceParts'],
		}),
		updateServicePart: build.mutation<{ success: boolean; message: string }, UpdateServicePartRequest>({
			query: (body) => ({
				url: '/',
				method: 'PATCH',
				body,
			}),
			invalidatesTags: ['ServiceParts'],
		}),
		deleteServicePart: build.mutation<{ success: boolean; message: string }, number>({
			query: (partId) => ({
				url: `/${partId}`,
				method: 'DELETE',
			}),
			invalidatesTags: (result, error, id) => [{ type: 'ServiceParts', id }],
		}),
	}),
});

export const {
	useCreateServicePartMutation,
	useUpdateServicePartMutation,
	useDeleteServicePartMutation,
} = servicePartApi;