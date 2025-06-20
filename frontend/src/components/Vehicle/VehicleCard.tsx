'use client';

interface VehicleCardProps {
	id: number;
	brand: string;
	model: string;
	year: number;
	vin: string;
	photoUrl: string;
	onDelete: (vehicleId: number) => void;
}

const DEFAULT_IMAGE_URL = 'https://png.pngtree.com/png-vector/20230206/ourmid/pngtree-orange-car-vector-mockup-png-image_6587139.png';


export function VehicleCard({ id, brand, model, year, vin, photoUrl, onDelete }: VehicleCardProps) {

	console.log("vehicle card render", photoUrl);
	return (
		<div className="bg-white  shadow-lg border border-zinc-200 rounded-xl overflow-hidden">
			<span>ID: {id}</span>
			<img
				src={photoUrl !== "" ? `http://localhost:5009/v1/car-image/${photoUrl}` : DEFAULT_IMAGE_URL}
				alt={`${brand} ${model}`}
				className="w-full h-40 object-contain"
			/>
			<div className="p-4 space-y-2">
				<h2 className="text-lg font-semibold text-zinc-800">
					{brand} {model} ({year})
				</h2>
				<p className="text-sm text-zinc-500">VIN: {vin}</p>
			</div>
			<div className="p-4">
				<button
					onClick={() => onDelete(id)}
					className="w-20 bg-red-600 text-white py-2 px-4 rounded-lg hover:bg-red-700 transition hower: cursor-pointer"
				>
					Delete
				</button>
			</div>
		</div>
	);
}