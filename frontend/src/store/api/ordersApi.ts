import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { Order, CreateOrderRequest, OrderQueryParams } from '@/types/orders.types';


export const ordersApi = createApi({
	reducerPath: 'ordersApi',
	baseQuery: fetchBaseQuery({
		baseUrl: 'http://localhost:5010/v1/orders',
		prepareHeaders: (headers, { getState }) => {
			const token = (getState() as any).auth.token;
			if (token) headers.set('Authorization', `Bearer ${token}`);
			return headers;
		},
	}),
	tagTypes: ['Orders'],
	endpoints: (build) => ({
		getOrders: build.query<Order[], OrderQueryParams>({
			query: (params) => ({
				url: '?Page=1&PageSize=10',
				method: 'GET',
				params,
			}),
			transformResponse: (response: { success: boolean; message: string; serviceOrders: Order[] }) => {
				return response.serviceOrders; // Extract the orders array
			},
			providesTags: (result) =>
				result ? result.map((order) => ({ type: 'Orders', id: order.id })) : [],
		}),
		getOrderById: build.query<Order, number>({
			query: (orderId) => `/${orderId}`,
			providesTags: (result, error, id) => [{ type: 'Orders', id }],
		}),
		createOrder: build.mutation<Order, CreateOrderRequest>({
			query: (body) => ({
				url: '/',
				method: 'POST',
				body,
			}),
			invalidatesTags: ['Orders'],
		}),
		deleteOrder: build.mutation<{ success: boolean; message: string }, number>({
			query: (orderId) => ({
				url: `/${orderId}`,
				method: 'DELETE',
			}),
			invalidatesTags: (result, error, id) => [{ type: 'Orders', id }],
		}),
	}),
});

export const {
	useGetOrdersQuery,
	useGetOrderByIdQuery,
	useCreateOrderMutation,
	useDeleteOrderMutation,
} = ordersApi;