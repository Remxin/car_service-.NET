'use client';

import { CheckCircle, XCircle, Timer } from 'lucide-react';
import Link from 'next/link';

interface OrderCardProps {
	id: number;
	status: string;
	vehicle: {
		brand: string;
		model: string;
		year: number;
		vin: string;
	};
	assignedTo: string | null;
	commentCount: number;
	partCount: number;
	createdAt: string;
}

const OrderCard = ({
							  id,
							  status,
							  vehicle,
							  assignedTo,
							  commentCount,
							  partCount,
							  createdAt,
						  }: OrderCardProps) => {
	const statusInfo = {
		NEW: {
			label: 'New',
			icon: <Timer className="w-4 h-4" />,
			color: 'bg-orange-100 text-orange-800',
		},
		IN_PROGRESS: {
			label: 'In progress',
			icon: <Timer className="w-4 h-4 animate-pulse" />,
			color: 'bg-amber-100 text-amber-800',
		},
		GENERATED:{
				label: 'generated',
				icon: <CheckCircle className="w-4 h-4" />,
				color: 'bg-emerald-100 text-emerald-800',
		},
		EXPIRED: {
			label: 'EXPIRED',
			icon: <XCircle className="w-4 h-4" />,
			color: 'bg-rose-100 text-rose-800',
		},
	}[status];

	return (
		<Link href={`/orders/${id}`} className="block hover:shadow-md transition">
		<div className="bg-white hover:shadow-2xl hover:scale-105 hover:bg-orange-300 transition-all duration-300 cursor-pointer shadow-lg border border-zinc-200 rounded-xl p-5 flex flex-col gap-4 hover:shadow-md transition">
			<div className="flex justify-between items-center">
				<div className="text-sm text-zinc-400">ID: {id}</div>
				<span
					className={`flex items-center gap-1 px-2 py-1 text-xs font-medium rounded-full ${statusInfo.color}`}
				>
          {statusInfo.icon}
					{statusInfo.label}
        </span>
			</div>

			<div>
				<h2 className="text-lg font-semibold text-zinc-800">
					{vehicle.brand} {vehicle.model} ({vehicle.year})
				</h2>
				<p className="text-sm text-zinc-500">VIN: {vehicle.vin}</p>
			</div>

			<div className="text-sm text-zinc-600">
				 {assignedTo ? assignedTo : <em>unassigned</em>}
			</div>

			<div className="flex justify-between text-xs text-zinc-500">
				{/*<span>{commentCount} komentarzy</span>*/}
				{/*<span>{partCount} części</span>*/}
				{/*<span>{createdAt}</span>*/}
			</div>
		</div>
			</Link>
	);
}


export default OrderCard;