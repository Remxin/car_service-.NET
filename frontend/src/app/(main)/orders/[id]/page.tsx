'use client';

import { useState } from 'react';
import { useParams } from 'next/navigation';
import { useGetOrderByIdQuery, useDeleteOrderMutation } from '@/store/api/ordersApi';
import { useCreateServiceTaskMutation, useDeleteServiceTaskMutation } from '@/store/api/serviceTaskApi';
import { useCreateServicePartMutation, useDeleteServicePartMutation } from '@/store/api/servicePartApi';
import { useCreateCommentMutation, useDeleteCommentMutation } from '@/store/api/commentsApi';
import Loader from '@/components/Loader';
import { OrderDetailsHeader } from '@/components/Order/OrderDetailsHeader';
import { OrderDetailsSection } from '@/components/Order/OrderDetailsSection';
import { PartItem } from '@/components/Order/PartItem';
import { TaskItem } from '@/components/Order/TaskItem';
import { CommentItem } from '@/components/Order/CommentItem';

export default function OrderDetailsPage() {
	const { id } = useParams();
	const orderId = Number(id);
	const { data, isLoading, error, refetch } = useGetOrderByIdQuery(orderId);
	const [deleteOrder] = useDeleteOrderMutation();
	const [createTask] = useCreateServiceTaskMutation();
	const [deleteTask] = useDeleteServiceTaskMutation();
	const [createPart] = useCreateServicePartMutation();
	const [deletePart] = useDeleteServicePartMutation();
	const [createComment] = useCreateCommentMutation();
	const [deleteComment] = useDeleteCommentMutation();

	const order = data?.serviceCompleteOrder;

	const [newTaskDesc, setNewTaskDesc] = useState('');
	const [newTaskCost, setNewTaskCost] = useState('');
	const [newVehiclePartId, setNewVehiclePartId] = useState('');
	const [newPartQty, setNewPartQty] = useState('');
	const [newComment, setNewComment] = useState('');
	const [showCommentInput, setShowCommentInput] = useState(false);

	const handleDeleteOrder = async () => {
		if (window.confirm('Are you sure you want to delete this order?')) {
			try {
				await deleteOrder(order.id).unwrap();
				window.location.href = '/orders';
			} catch (err) {
				console.error('Failed to delete order:', err);
			}
		}
	};

	const handleAddTask = async () => {
		if (!newTaskDesc.trim()) return;
		await createTask({
			orderId,
			description: newTaskDesc,
			laborCost: parseFloat(newTaskCost) || 0,
		});
		setNewTaskDesc('');
		setNewTaskCost('');
		refetch();
	};


	const handleRemoveTask = async (taskId: number) => {
		await deleteTask(taskId);
		refetch();
	};

	const handleAddPart = async () => {
		if (!newVehiclePartId.trim()) return;
		await createPart({
			orderId,
			vehiclePartId: parseInt(newVehiclePartId),
			quantity: parseInt(newPartQty) || 1,
		});
		setNewVehiclePartId('');
		setNewPartQty('');
		refetch();
	};

	const handleRemovePart = async (partId: number) => {
		await deletePart(partId);
		refetch();
	};

	const handleAddComment = async () => {
		if (!newComment.trim()) return;
		await createComment({
			orderId,
			content: newComment,
		});
		setNewComment('');
		setShowCommentInput(false);
		refetch();
	};

	const handleRemoveComment = async (commentId: number) => {
		await deleteComment(commentId);
		refetch();
	};



	if (isLoading) {
		return <Loader />;
	}
	if (error || !order) {
		return <div>Error loading order details.</div>;
	}

	const handleGenerateReport = () => {
		alert(`Generating report for: ${order.id}`);
	};

	console.log('komenatrze', order.serviceComment);

	return (
		<div className="p-6 space-y-6">
			<OrderDetailsHeader order={order} />

			<OrderDetailsSection title="🔧 Service Tasks">
				{order.serviceTasks.map((task) => (
					<div key={task.id} className="flex justify-between items-center">
						<TaskItem {...task} />
						<button
							onClick={() => handleRemoveTask(task.id)}
							className="text-red-600 text-sm hover:underline"
						>
							Remove
						</button>
					</div>
				))}
				<div className="mt-4 flex flex-col gap-2 w-1/2 w-100">
					<input
						type="text"
						placeholder="Task description"
						value={newTaskDesc}
						onChange={(e) => setNewTaskDesc(e.target.value)}
						className="border px-2 py-1 rounded"
					/>
					<input
						type="number"
						placeholder="Labor cost"
						value={newTaskCost}
						onChange={(e) => setNewTaskCost(e.target.value)}
						className="border px-2 py-1 rounded w-100"
					/>
					<button
						onClick={handleAddTask}
						className="bg-orange-600 text-white px-3 py-1 rounded hover:bg-orange-700 w-30"
					>
						+ Task
					</button>
				</div>
			</OrderDetailsSection>

			<OrderDetailsSection title="⚙️ Used Parts">
				{order.serviceParts.map((part) => (
					<div key={part.id} className="flex justify-between items-center">
						<PartItem {...part} />
						<button
							onClick={() => handleRemovePart(part.id)}
							className="text-red-600 text-sm hover:underline"
						>
							Remove
						</button>
					</div>
				))}
				<div className="mt-4 flex flex-col gap-2 w-1/2">
					<input
						type="text"
						placeholder="Vehicle Part ID"
						value={newVehiclePartId}
						onChange={(e) => setNewVehiclePartId(e.target.value)}
						className="border px-2 py-1 rounded w-100"
					/>
					<input
						type="number"
						placeholder="Quantity"
						value={newPartQty}
						onChange={(e) => setNewPartQty(e.target.value)}
						className="border px-2 py-1 rounded w-100"
					/>
					<button
						onClick={handleAddPart}
						className="bg-orange-600 text-white px-3 py-1 rounded hover:bg-orange-700 w-30"
					>
						+ Part
					</button>
				</div>
			</OrderDetailsSection>

			{/* Comments */}
			<OrderDetailsSection title="💬 Comments">
				{order.serviceComment.map((comment) => (
					<div key={comment.id} className="flex justify-between items-center">
						<CommentItem {...comment} author="" />

					</div>
				))}
				{showCommentInput ? (
					<div className="mt-4 space-y-2">
						<input
							type="text"
							placeholder="Enter a comment..."
							value={newComment}
							onChange={(e) => setNewComment(e.target.value)}
							className="w-full px-3 py-2 border border-zinc-300 rounded-md shadow-sm"
						/>
						<div className="flex justify-end gap-2">
							<button
								onClick={() => setShowCommentInput(false)}
								className="px-3 py-1 text-sm rounded bg-zinc-200 hover:bg-zinc-300"
							>
								Cancel
							</button>
							<button
								onClick={handleAddComment}
								className="px-4 py-1.5 bg-orange-600 text-white text-sm rounded hover:bg-orange-700"
							>
								Send
							</button>
						</div>
					</div>
				) : (
					<div className="flex justify-between items-center mt-6">
						<button
							onClick={() => setShowCommentInput(true)}
							className="inline-flex items-center gap-1 bg-orange-600 text-white text-sm px-3 py-1.5 rounded hover:bg-orange-700"
						>
							<span className="text-lg leading-none">+</span>
							Add comment
						</button>
					</div>
				)}
			</OrderDetailsSection>

			{/* Bottom actions */}
			<div className="pt-6 flex justify-between">
				<button
					onClick={handleGenerateReport}
					className="bg-orange-600 text-white px-5 py-2 rounded-lg hover:bg-orange-700"
				>
					📄 Generate report
				</button>
				<button
					onClick={handleDeleteOrder}
					className="bg-red-600 text-white px-5 py-2 rounded-lg hover:bg-red-700"
				>
					🗑️ Delete Order
				</button>
			</div>
		</div>
	);
}