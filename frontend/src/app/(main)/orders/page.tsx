'use client';

import OrderCard  from '@/components/Order/OrderCard';
import AddButton from '@/components/AddButton';
import AddOrderModal from '@/components/Order/AddOrderModal';
import { useState } from "react";
import AuthGuard from "@/components/AuthGuard";

const MOCK_ORDERS = [
	{
		id: 'ORD-001',
		status: 'new',
		vehicle: {
			brand: 'BMW',
			model: 'X5',
			year: 2020,
			vin: 'WBAXX11060DT12345',
		},
		assignedTo: null,
		commentCount: 2,
		partCount: 4,
		createdAt: '2025-06-01',
	},
	{
		id: 'ORD-002',
		status: 'in_progress',
		vehicle: {
			brand: 'Audi',
			model: 'A4',
			year: 2019,
			vin: 'WAUZZZF40KA012345',
		},
		assignedTo: 'Jan Kowalski',
		commentCount: 5,
		partCount: 3,
		createdAt: '2025-06-02',
	},
	{
		id: 'ORD-003',
		status: 'completed',
		vehicle: {
			brand: 'Toyota',
			model: 'Corolla',
			year: 2018,
			vin: 'JTDBR32E520052134',
		},
		assignedTo: 'Anna Nowak',
		commentCount: 1,
		partCount: 2,
		createdAt: '2025-06-03',
	},
	{
		id: 'ORD-004',
		status: 'canceled',
		vehicle: {
			brand: 'Ford',
			model: 'Focus',
			year: 2017,
			vin: 'WF0DP3TH4H4123456',
		},
		assignedTo: null,
		commentCount: 0,
		partCount: 0,
		createdAt: '2025-06-04',
	},
];

export default function OrdersPage() {
	const [open, setOpen] = useState(false);

	const handleAddClick = () => {
		setOpen(true);
	};

	return (
	<AuthGuard allowedRoles={['admin','reception', 'mechanic']}>
		<div className="p-2">
			<div className="flex justify-between items-center mb-6">
				<h1 className="text-4xl font-bold text-zinc-800">Orders</h1>
				<AddButton label="Add order" onClick={handleAddClick} />
			</div>

			<div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-6">
				{MOCK_ORDERS.map((order) => (
					<OrderCard key={order.id} {...order} />
				))}
			</div>

			<AddOrderModal open={open} onClose={() => setOpen(false)} />
		</div>
	</AuthGuard>
	);
}

