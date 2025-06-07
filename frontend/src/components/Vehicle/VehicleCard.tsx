'use client';

interface VehicleCardProps {
	id: number;
	brand: string;
	model: string;
	year: number;
	vin: string;
	photoUrl: string;
}

export function VehicleCard({ brand, model, year, vin, photoUrl }: VehicleCardProps) {
	return (
		<div className="bg-white hover:shadow-2xl hover:scale-105 hover:bg-orange-300 transition-all duration-300 cursor-pointer shadow-lg border border-zinc-200 rounded-xl overflow-hidden  hover:shadow-md transition">
			<img
				src={photoUrl}
				alt={`${brand} ${model}`}
				className="w-full h-40 object-cover"
			/>
			<div className="p-4 space-y-2">
				<h2 className="text-lg font-semibold text-zinc-800">
					{brand} {model} ({year})
				</h2>
				<p className="text-sm text-zinc-500">VIN: {vin}</p>
			</div>
		</div>
	);
}
