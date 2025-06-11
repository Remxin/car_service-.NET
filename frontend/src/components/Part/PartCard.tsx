'use client';

interface PartCardProps {
	id: number;
	name: string;
	partNumber: string;
	description: string;
	price: number;
	availableQuantity: number;
	onDelete: (id: number) => void;
}

const PartCard = ({
					  id,
					  name,
					  partNumber,
					  description,
					  price,
					  availableQuantity,
					  onDelete,
				  }: PartCardProps) => {
	return (
		<div className="bg-white shadow-lg border border-zinc-200 rounded-xl p-5 flex flex-col gap-4">
			<div className="flex justify-between items-center">
				<h2 className="text-lg font-semibold text-zinc-800">{ name }</h2>
				<div className="text-sm text-zinc-400">ID: { id }</div>
			</div>
			<p className="text-sm text-zinc-500">{ partNumber }</p>
			<p className="text-sm text-zinc-600">{ description }</p>

			<div className="mt-3 flex justify-between items-center text-sm">
				<span className="text-orange-600 font-semibold">{ price.toFixed(2) } zł</span>
				<span className="text-zinc-500">{ availableQuantity } szt.</span>
			</div>

			<button
				onClick={ () => onDelete(id) }
				className="w-30 mt-4 bg-red-600 text-white py-2 px-4 rounded-lg hover:bg-red-700 transition"
			>
				Delete
			</button>
		</div>
	);
};

export default PartCard;