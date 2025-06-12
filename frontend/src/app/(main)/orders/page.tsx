'use client';

import OrderCard from '@/components/Order/OrderCard';
import AddButton from '@/components/AddButton';
import AddOrderModal from '@/components/Order/AddOrderModal';
import { useState } from "react";
import AuthGuard from "@/components/AuthGuard";
import { useGetOrdersQuery } from '@/store/api/ordersApi';
import Loader from "@/components/Loader";

export default function OrdersPage() {
	const [open, setOpen] = useState(false);
	const { data: orders, isLoading, error } = useGetOrdersQuery({});


	const handleAddClick = () => {
		setOpen(true);
	};

	if (isLoading) {
		return <Loader/>;
	}

	if (error) {
		return <div>Error loading orders.</div>;
	}

	return (
		<AuthGuard allowedRoles={['admin', 'reception', 'mechanic']}>
			<div className="p-2">
				<div className="flex justify-between items-center mb-6">
					<h1 className="text-4xl font-bold text-zinc-800">Orders</h1>
					<AddButton label="Add order" onClick={handleAddClick} />
				</div>

				<div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-6">
					{orders?.map((order) => (
						<OrderCard
							key={order.id}
							id={order.id}
							status={order.status}
							vehicle={order.vehicle}
							assignedTo={order.assignedMechanicId ? `Mechanic ${order.assignedMechanicId}` : null}
							commentCount={0}
							partCount={0}
							createdAt={new Date(order.createdAt).toLocaleDateString()}
						/>
					))}
				</div>

				<AddOrderModal open={open} onClose={() => setOpen(false)} />
			</div>
		</AuthGuard>
	);
}