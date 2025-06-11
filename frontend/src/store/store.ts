import { configureStore } from '@reduxjs/toolkit';
import authReducer from './authSlice';
import { authApi } from './api/authApi';
import { ordersApi } from './api/ordersApi';
import { vehiclesApi } from './api/vehiclesApi';
import { vehiclePartsApi } from './api/vehiclePartsApi';
import { reportsApi } from './api/reportsApi';
import { commentsApi } from './api/commentsApi';
import { serviceTaskApi } from './api/serviceTaskApi';
import { servicePartApi } from './api/servicePartApi';

export const store = configureStore({
	reducer: {
		auth: authReducer,
		[authApi.reducerPath]: authApi.reducer,
		[ordersApi.reducerPath]: ordersApi.reducer,
		[vehiclesApi.reducerPath]: vehiclesApi.reducer,
		[vehiclePartsApi.reducerPath]: vehiclePartsApi.reducer,
		[reportsApi.reducerPath]: reportsApi.reducer,
		[commentsApi.reducerPath]: commentsApi.reducer,
		[serviceTaskApi.reducerPath]: serviceTaskApi.reducer,
		[servicePartApi.reducerPath]: servicePartApi.reducer,
	},
	middleware: (getDefault) =>
		getDefault().concat(
			authApi.middleware,
			ordersApi.middleware,
			vehiclesApi.middleware,
			vehiclePartsApi.middleware,
			reportsApi.middleware,
			commentsApi.middleware,
			serviceTaskApi.middleware,
			servicePartApi.middleware
		),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;