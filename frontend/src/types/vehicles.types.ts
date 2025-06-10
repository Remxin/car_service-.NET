export type Vehicle = {
	id: number;
	brand: string;
	model: string;
	vin: string;
	carImageUrl: string;
	year: number;
	createdAt: string;
	updatedAt: string;
};

export type VehicleQueryParams = {
	page?: number;
	pageSize?: number;
	brand?: string;
	model?: string;
	year?: number;
	vin?: string;
};

export type CreateVehicleRequest = {
	brand: string;
	model: string;
	vin: string;
	carImageUrl: string;
	year: number;
};

export type UpdateVehicleRequest = {
	vehicleId: number;
	brand: string;
	model: string;
	vin: string;
	carImageUrl: string;
	year: number;
};
