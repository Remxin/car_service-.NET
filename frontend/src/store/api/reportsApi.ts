import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import {
	Report,
	ListReportsResponse,
	CreateReportRequest,
	ReportDownloadLinkResponse,
	SendReportEmailRequest
} from "@/types/reports.types";

export const reportsApi = createApi({
	reducerPath: 'reportsApi',
	baseQuery: fetchBaseQuery({
		baseUrl: 'http://localhost:5010/v1/reports',
		prepareHeaders: (headers, { getState }) => {
			const token = (getState() as any).auth?.token;
			if (token) headers.set('Authorization', `Bearer ${token}`);
			return headers;
		},
	}),
	tagTypes: ['Reports'],
	endpoints: (build) => ({
		getReports: build.query<Report[], void>({
			query: () => '?Page=1&PageSize=10',
			transformResponse: (response: ListReportsResponse) => response.reports,
			providesTags: (result) =>
				result
					? [
						...result.map((report) => ({ type: 'Reports' as const, id: report.id })),
						{ type: 'Reports', id: 'LIST' },
					]
					: [{ type: 'Reports', id: 'LIST' }],
		}),
		createReport: build.mutation<{ reportId: string }, CreateReportRequest>({
			query: (body) => ({
				url: '',
				method: 'POST',
				body,
			}),
			invalidatesTags: ['Reports'],
		}),
		getReportDownloadLink: build.query<ReportDownloadLinkResponse, string>({
			query: (reportId) => `download-link/${reportId}`,
			providesTags: (result, error, id) => [{ type: 'Reports', id }],
		}),
		sendReportEmail: build.mutation<{ success: boolean; message: string }, SendReportEmailRequest>({
			query: (body) => ({
				url: 'send-email',
				method: 'POST',
				body,
			}),
			invalidatesTags: ['Reports'],
		}),
		getReportStatus: build.query<{ status: string; progress: number }, string>({
			query: (reportId) => `${reportId}/status`,
			providesTags: (result, error, id) => [{ type: 'Reports', id }],
		}),
	}),
});

export const {
	useGetReportsQuery,
	useCreateReportMutation,
	useGetReportDownloadLinkQuery,
	useSendReportEmailMutation,
	useGetReportStatusQuery,
} = reportsApi