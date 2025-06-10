import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { CreateReportRequest, SendReportEmailRequest, ReportStatusResponse } from '@/types/reports.types';


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
} = reportsApi;