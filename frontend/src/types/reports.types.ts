export type CreateReportRequest = {
	userId: string;
	orderId: string;
	fromDate: {
		seconds: number;
		nanos: number;
	};
	toDate: {
		seconds: number;
		nanos: number;
	};
};

export type SendReportEmailRequest = {
	reportId: string;
};

export type ReportStatusResponse = {
	status: string;
	progress: number;
};