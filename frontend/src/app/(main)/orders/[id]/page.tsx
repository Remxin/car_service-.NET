'use client';

import { useState } from 'react';
import { useGetOrderByIdQuery } from '@/store/api/ordersApi';
import { useParams } from 'next/navigation';
import { OrderDetailsHeader } from '@/components/Order/OrderDetailsHeader';
import { OrderDetailsSection } from '@/components/Order/OrderDetailsSection';
import { PartItem } from '@/components/Order/PartItem';
import { TaskItem } from '@/components/Order/TaskItem';
import { CommentItem } from '@/components/Order/CommentItem';
import Loader from '@/components/Loader';

export default function OrderDetailsPage() {
	const { id } = useParams();
	const { data: order, isLoading, error } = useGetOrderByIdQuery(Number(id));
	const [comments, setComments] = useState(order?.comments || []);
	const [showInput, setShowInput] = useState(false);
	const [newComment, setNewComment] = useState('');

	if (isLoading) {
		return <Loader />;
	}

	if (error || !order) {
		return <div>Error loading order details.</div>;
	}

	const handleGenerateReport = () => {
		alert(`Generating report for: ${order.id}`);
	};

	const handleAddComment = () => {
		if (!newComment.trim()) return;

		const nextComment = {
			id: Date.now(),
			author: 'You',
			content: newComment,
		};

		setComments((prev) => [...prev, nextComment]);
		setNewComment('');
		setShowInput(false);
	};

	return (
		<div className="p-6 space-y-6">
			<OrderDetailsHeader order={order} />

			<OrderDetailsSection title="🔧 Service Tasks">
				{order.tasks.map((task) => (
					<TaskItem key={task.id} {...task} />
				))}
			</OrderDetailsSection>

			<OrderDetailsSection title="⚙️ Used Parts">
				{order.parts.map((part) => (
					<PartItem key={part.id} {...part} />
				))}
			</OrderDetailsSection>

			<OrderDetailsSection title="💬 Comments">
				{comments.map((comment) => (
					<CommentItem key={comment.id} {...comment} />
				))}

				{showInput ? (
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
								onClick={() => setShowInput(false)}
								className="px-3 py-1 text-sm rounded bg-zinc-200 hover:bg-zinc-300 transition"
							>
								Cancel
							</button>
							<button
								onClick={handleAddComment}
								className="px-4 py-1.5 bg-orange-600 text-white text-sm rounded hover:bg-orange-700 transition"
							>
								Send
							</button>
						</div>
					</div>
				) : (
					<div className="flex justify-between items-center mt-6">
						<button
							onClick={() => setShowInput(true)}
							className="inline-flex items-center gap-1 bg-orange-600 text-white text-sm px-3 py-1.5 rounded-md hover:bg-orange-700 transition"
						>
							<span className="text-lg leading-none">+</span>
							Add comment
						</button>
					</div>
				)}
			</OrderDetailsSection>

			<div className="pt-6 flex justify-end">
				<button
					onClick={handleGenerateReport}
					className="bg-orange-600 text-white px-5 py-2 rounded-lg hover:bg-orange-700 transition font-medium"
				>
					📄 Generate report
				</button>
			</div>
		</div>
	);
}