export type Timestamp = {
	seconds: number;
	nanos: number;
};

export type Mechanic = {
	id: number;
	name: string;
	email: string;
	createdAt?: Timestamp | null;
};

export type Vehicle = {
	id: number;
	brand: string;
	model: string;
	year: number;
	vin: string;
	photoUrl: string;
	createdAt: Timestamp;
};

export type Report = {
	id: string;
	vehicle: Vehicle;
	mechanic: Mechanic;
	status: 'GENERATED' | 'IN_PROGRESS' | 'FAILED';
	expiresAt: Timestamp;
	createdAt: Timestamp;
};

export type ListReportsResponse = {
	reports: Report[];
};

export type CreateReportRequest = {
	UserId: string;
	OrderId: string;
};

export type ReportDownloadLinkResponse = {
	reportId: string;
	downloadUrl: string;
	expiresAt: Timestamp;
};

export type SendReportEmailRequest = {
	ReportId: string;
	UsersIds: number[];
};