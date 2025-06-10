export type ServiceComment = {
	id: number;
	orderId: number;
	content: string;
	createdAt: string;
	updatedAt: string;
};

export type CreateCommentRequest = {
	orderId: number;
	content: string;
};

export type UpdateCommentRequest = {
	serviceCommentId: number;
	orderId: number;
	content: string;
};