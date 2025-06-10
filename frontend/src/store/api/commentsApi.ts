import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import type { ServiceComment, CreateCommentRequest, UpdateCommentRequest } from '@/types/comments.types';


export const commentsApi = createApi({
	reducerPath: 'commentsApi',
	baseQuery: fetchBaseQuery({
		baseUrl: 'http://localhost:5010/v1/service-comments',
		prepareHeaders: (headers, { getState }) => {
			const token = (getState() as any).auth.token;
			if (token) headers.set('Authorization', `Bearer ${token}`);
			return headers;
		},
	}),
	tagTypes: ['ServiceComments'],
	endpoints: (build) => ({
		createComment: build.mutation<ServiceComment, CreateCommentRequest>({
			query: (body) => ({
				url: '/',
				method: 'POST',
				body,
			}),
			invalidatesTags: ['ServiceComments'],
		}),
		updateComment: build.mutation<ServiceComment, UpdateCommentRequest>({
			query: (body) => ({
				url: '/',
				method: 'PATCH',
				body,
			}),
			invalidatesTags: (result, error, { serviceCommentId }) => [
				{ type: 'ServiceComments', id: serviceCommentId },
			],
		}),
		deleteComment: build.mutation<{ success: boolean; message: string }, number>({
			query: (commentId) => ({
				url: `/${commentId}`,
				method: 'DELETE',
			}),
			invalidatesTags: (result, error, id) => [{ type: 'ServiceComments', id }],
		}),
	}),
});

export const {
	useCreateCommentMutation,
	useUpdateCommentMutation,
	useDeleteCommentMutation,
} = commentsApi;