'use client';

interface PartCardProps {
	id: number;
	name: string;
	partNumber: string;
	description: string;
	price: number;
	availableQuantity: number;
}

const PartCard = ({
							 name,
							 partNumber,
							 description,
							 price,
							 availableQuantity,
						 }: PartCardProps)=>  {
	return (
		<div className="bg-white shadow-lg  border border-zinc-200 rounded-xl p-5 flex flex-col gap-2 ">
			<h2 className="text-lg font-semibold text-zinc-800">{name}</h2>
			<p className="text-sm text-zinc-500">{partNumber}</p>
			<p className="text-sm text-zinc-600">{description}</p>

			<div className="mt-3 flex justify-between items-center text-sm">
				<span className="text-orange-600 font-semibold">{price.toFixed(2)} zł</span>
				<span className="text-zinc-500">{availableQuantity} szt.</span>
			</div>
		</div>
	);
}

export default PartCard;