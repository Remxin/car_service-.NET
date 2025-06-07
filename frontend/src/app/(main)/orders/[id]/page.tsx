'use client';

import { useState } from 'react';
import { OrderDetailsHeader } from '@/components/Order/OrderDetailsHeader';
import { OrderDetailsSection } from '@/components/Order/OrderDetailsSection';
import { PartItem } from '@/components/Order/PartItem';
import { TaskItem } from '@/components/Order/TaskItem';
import { CommentItem } from '@/components/Order/CommentItem';

const MOCK_ORDER = {
	id: 'ORD-001',
	createdAt: '2025-06-01',
	status: 'in_progress',
	mechanic: 'Jan Kowalski',
	vehicle: {
		brand: 'BMW',
		model: 'X5',
		year: 2020,
		vin: 'WBAXX11060DT12345',
		photoUrl: '/images/bmw.jpg',
	},
	tasks: [
		{ id: 1, description: 'Wymiana oleju', laborCost: 100 },
		{ id: 2, description: 'Sprawdzenie hamulców', laborCost: 50 },
	],
	parts: [
		{ id: 1, name: 'Filtr oleju', quantity: 1 },
		{ id: 2, name: 'Klocki hamulcowe', quantity: 2 },
	],
	comments: [
		{ id: 1, author: 'Anna', content: 'Klient prosił o szybszy termin' },
		{ id: 2, author: 'Mechanik', content: 'Części dostarczone' },
	],
};

export default function OrderDetailsPage() {
	const [comments, setComments] = useState(MOCK_ORDER.comments);
	const [showInput, setShowInput] = useState(false);
	const [newComment, setNewComment] = useState('');

	const handleGenerateReport = () => {
		alert(`Generowanie raportu dla: ${MOCK_ORDER.id}`);
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
			<OrderDetailsHeader order={MOCK_ORDER} />

			<OrderDetailsSection title="🔧 Zadania serwisowe">
				{MOCK_ORDER.tasks.map((task) => (
					<TaskItem key={task.id} {...task} />
				))}
			</OrderDetailsSection>

			<OrderDetailsSection title="⚙️ Użyte części">
				{MOCK_ORDER.parts.map((part) => (
					<PartItem key={part.id} {...part} />
				))}
			</OrderDetailsSection>

			<OrderDetailsSection title="💬 Komentarze">
				{comments.map((comment) => (
					<CommentItem key={comment.id} {...comment} />
				))}

				{showInput ? (
					<div className="mt-4 space-y-2">
						<input
							type="text"
							placeholder="Wpisz komentarz..."
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
