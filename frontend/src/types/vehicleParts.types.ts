export type VehiclePart = {
	id: number;
	name: string;
	partNumber: string;
	description: string;
	price: number;
	availableQuantity: number;
	createdAt: string;
	updatedAt: string;
};

export type VehiclePartQueryParams = {
	page?: number;
	pageSize?: number;
	name?: string;
	partNumber?: string;
	description?: string;
	price?: number;
	availableQuantity?: number;
};

export type CreateVehiclePartRequest = {
	name: string;
	partNumber: string;
	description: string;
	price: number;
	availableQuantity: number;
};

export type UpdateVehiclePartRequest = {
	vehiclePartId: number;
	name?: string;
	partNumber?: string;
	description?: string;
	price?: number;
	availableQuantity?: number;
};