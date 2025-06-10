import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { VehiclePart, CreateVehiclePartRequest, UpdateVehiclePartRequest, VehiclePartQueryParams } from '@/types/vehicleParts.types';


export const vehiclePartsApi = createApi({
	reducerPath: 'vehiclePartsApi',
	baseQuery: fetchBaseQuery({
		baseUrl: 'http://localhost:5010/v1/vehicle-parts',
		prepareHeaders: (headers, { getState }) => {
			const token = (getState() as any).auth.token;
			if (token) headers.set('Authorization', `Bearer ${token}`);
			return headers;
		},
	}),
	tagTypes: ['VehicleParts'],
	endpoints: (build) => ({
		getVehicleParts: build.query<VehiclePart[], VehiclePartQueryParams>({
			query: (params) => ({
				url: '/',
				method: 'GET',
				params,
			}),
			transformResponse: (response: { success: boolean; message: string; vehiclePart: VehiclePart[] }) => {
				return response.vehiclePart || [];
			},
			providesTags: (result) =>
				result ? result.map((part) => ({ type: 'VehicleParts', id: part.id })) : [],
		}),
		getVehiclePartById: build.query<VehiclePart, number>({
			query: (id) => `/${id}`,
			providesTags: (result, error, id) => [{ type: 'VehicleParts', id }],
		}),
		createVehiclePart: build.mutation<VehiclePart, CreateVehiclePartRequest>({
			query: (body) => ({
				url: '/',
				method: 'POST',
				body,
			}),
			invalidatesTags: ['VehicleParts'],
		}),
		updateVehiclePart: build.mutation<VehiclePart, UpdateVehiclePartRequest>({
			query: (body) => ({
				url: '/',
				method: 'PATCH',
				body,
			}),
			invalidatesTags: (result, error, { vehiclePartId }) => [
				{ type: 'VehicleParts', id: vehiclePartId },
			],
		}),
		deleteVehiclePart: build.mutation<{ success: boolean; message: string }, number>({
			query: (partId) => ({
				url: `/${partId}`,
				method: 'DELETE',
			}),
			invalidatesTags: (result, error, id) => [{ type: 'VehicleParts', id }],
		}),
	}),
});

export const {
	useGetVehiclePartsQuery,
	useGetVehiclePartByIdQuery,
	useCreateVehiclePartMutation,
	useUpdateVehiclePartMutation,
	useDeleteVehiclePartMutation,
} = vehiclePartsApi;