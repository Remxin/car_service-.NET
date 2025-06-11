import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { CreateReportRequest, SendReportEmailRequest, ReportStatusResponse } from '@/types/reports.types';
import { Order, OrderQueryParams } from "@/types/orders.types";

export const reportsApi = createApi({
	reducerPath: 'reportsApi',
	baseQuery: fetchBaseQuery({
		baseUrl: 'http://localhost:5010/v1/reports',
		prepareHeaders: (headers, { getState }) => {
			const token = (getState() as any).auth.token;
			if (token) headers.set('Authorization', `Bearer ${token}`);
			return headers;
		},
	}),
	tagTypes: ['Reports'],
	endpoints: (build) => ({
		getReports: build.query<Order[], OrderQueryParams>({
			query: (params) => ({
				url: '?Page=1&PageSize=10',
				method: 'GET',
				params,
			}),
			transformResponse: (response: { success: boolean; message: string; serviceOrders: Order[] }) => {
				return response.serviceOrders;
			},
			providesTags: (result) =>
				result
					? result.map((order) => ({ type: 'Reports', id: order.id }))
					: [{ type: 'Reports', id: 'LIST' }],
		}),
		createReport: build.mutation<{ reportId: string }, CreateReportRequest>({
			query: (body) => ({
				url: '/',
				method: 'POST',
				body,
			}),
			invalidatesTags: ['Reports'],
		}),
		getReportDownloadLink: build.query<string, string>({
			query: (reportId) => `/download-link/${reportId}`,
			providesTags: (result, error, id) => [{ type: 'Reports', id }],
		}),
		sendReportEmail: build.mutation<{ success: boolean; message: string }, SendReportEmailRequest>({
			query: (body) => ({
				url: '/send-email',
				method: 'POST',
				body,
			}),
			invalidatesTags: ['Reports'],
		}),
		getReportStatus: build.query<ReportStatusResponse, string>({
			query: (reportId) => `/${reportId}/status`,
			providesTags: (result, error, id) => [{ type: 'Reports', id }],
		}),
	}),
});

export const {
	useCreateReportMutation,
	useGetReportDownloadLinkQuery,
	useSendReportEmailMutation,
	useGetReportStatusQuery,
	useGetReportsQuery,
} = reportsApi;