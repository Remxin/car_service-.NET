'use client';

interface OrderDetailsHeaderProps {
	order: {
		id: string;
		createdAt: string;
		status: 'new' | 'in_progress' | 'completed' | 'canceled';
		mechanic: string;
		vehicle: {
			brand: string;
			model: string;
			year: number;
			vin: string;
			photoUrl: string;
		};
	};
}

export function OrderDetailsHeader({ order }: OrderDetailsHeaderProps) {
	const statusStyles: Record<string, string> = {
		new: 'bg-orange-100 text-orange-800',
		in_progress: 'bg-amber-100 text-amber-800',
		completed: 'bg-emerald-100 text-emerald-800',
		canceled: 'bg-rose-100 text-rose-800',
	};

	return (
		<div className="bg-white border border-zinc-200 rounded-xl p-6 flex flex-col sm:flex-row gap-6 shadow-sm">
			<img
				src={order.vehicle.photoUrl}
				alt="Pojazd"
				className="w-full sm:w-48 h-32 object-cover rounded-lg"
			/>
			<div className="flex-1 space-y-2">
				<div className="flex justify-between items-center">
					<h2 className="text-xl font-bold text-zinc-800">Zlecenie {order.id}</h2>
					<span className={`px-2 py-1 rounded-full text-sm font-medium ${statusStyles[order.status]}`}>
            {order.status.replace('_', ' ')}
          </span>
				</div>
				<p className="text-sm text-zinc-500">Dodano: {order.createdAt}</p>
				<p className="text-sm text-zinc-500">Mechanik: {order.mechanic}</p>
				<p className="text-sm text-zinc-600">
					{order.vehicle.brand} {order.vehicle.model} ({order.vehicle.year}) – VIN: {order.vehicle.vin}
				</p>
			</div>
		</div>
	);
}
