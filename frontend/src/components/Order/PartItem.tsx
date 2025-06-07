'use client';

interface PartItemProps {
	id: number;
	name: string;
	quantity: number;
}

export function PartItem({ name, quantity }: PartItemProps) {
	return (
		<div className="flex justify-between text-sm text-zinc-700">
			<span>{name}</span>
			<span className="text-zinc-500">{quantity} szt.</span>
		</div>
	);
}
