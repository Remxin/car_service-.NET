export type Order = {
	id: number;
	vehicleId: number;
	status: string;
	assignedMechanicId: number | null;
	createdAt: string;
	updatedAt: string;
	vehicle: {
		vin: string;
		brand: string;
		model: string;
		year: number;
	};
};

export type OrderQueryParams = {
	page?: number;
	pageSize?: number;
	vehicleVin?: string;
	vehicleBrand?: string;
	vehicleModel?: string;
	vehicleYear?: number;
	createdAfter?: string;
	createdBefore?: string;
};

export type CreateOrderRequest = {
	vehicleId: number;
	status: string;
	assignedMechanicId: number;
};