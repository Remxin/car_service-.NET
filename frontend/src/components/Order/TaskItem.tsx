'use client';

interface TaskItemProps {
	id: number;
	description: string;
	laborCost: number;
}

export function TaskItem({ description, laborCost }: TaskItemProps) {
	return (
		<div className="flex justify-between text-sm text-zinc-700">
			<span>{description}</span>
			<span className="text-orange-600 font-medium">{laborCost.toFixed(2)} zł</span>
		</div>
	);
}
