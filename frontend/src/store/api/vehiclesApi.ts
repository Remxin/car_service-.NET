import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import {Vehicle, CreateVehicleRequest, UpdateVehicleRequest, VehicleQueryParams } from '@/types/vehicles.types';

export const vehiclesApi = createApi({
	reducerPath: 'vehiclesApi',
	baseQuery: fetchBaseQuery({
		baseUrl: 'http://localhost:5010/v1/vehicles',
		prepareHeaders: (headers, { getState }) => {
			const token = (getState() as any).auth.token;
			if (token) headers.set('Authorization', `Bearer ${token}`);
			return headers;
		},
	}),
	tagTypes: ['Vehicles'],
	endpoints: (build) => ({
		getVehicles: build.query<Vehicle[], VehicleQueryParams>({
			query: (params) => ({
				url: '?Page=1&PageSize=10',
				method: 'GET',
				params,
			}),
			transformResponse: (response: { success: boolean; message: string; vehicles: Vehicle[] }) => {
				return response.vehicles || [];
			},
			providesTags: (result) =>
				result ? result.map((vehicle) => ({ type: 'Vehicles', id: vehicle.id })) : [],
		}),
		getVehicleById: build.query<Vehicle, number>({
			query: (vehicleId) => `/${vehicleId}`,
			providesTags: (result, error, id) => [{ type: 'Vehicles', id }],
		}),
		createVehicle: build.mutation<Vehicle, CreateVehicleRequest>({
			query: (body) => ({
				url: '/',
				method: 'POST',
				body,
			}),
			invalidatesTags: ['Vehicles'],
		}),
		updateVehicle: build.mutation<Vehicle, UpdateVehicleRequest>({
			query: (body) => ({
				url: '/',
				method: 'PATCH',
				body,
			}),
			invalidatesTags: (result, error, { vehicleId }) => [{ type: 'Vehicles', id: vehicleId }],
		}),
		deleteVehicle: build.mutation<{ success: boolean; message: string }, number>({
			query: (vehicleId) => ({
				url: `/${vehicleId}`,
				method: 'DELETE',
			}),
			invalidatesTags: (result, error, id) => [{ type: 'Vehicles', id }],
		}),
	}),
});

export const {
	useGetVehiclesQuery,
	useGetVehicleByIdQuery,
	useCreateVehicleMutation,
	useUpdateVehicleMutation,
	useDeleteVehicleMutation,
} = vehiclesApi;