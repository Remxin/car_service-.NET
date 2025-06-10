'use client';

import PartCard from '@/components/Part/PartCard';
import AddButton from '@/components/AddButton';
import { useState } from 'react';
import AddPartModal from '@/components/Part/AddPartModal';
import AuthGuard from '@/components/AuthGuard';
import { useGetVehiclePartsQuery } from '@/store/api/vehiclePartsApi';
import Loader from "@/components/Loader";

export default function PartsPage() {
	const [open, setOpen] = useState(false);
	const { data: parts, isLoading, error } = useGetVehiclePartsQuery({});

	const handleAddClick = () => {
		setOpen(true);
	};

	if (isLoading) {
		return <Loader/>;
	}

	if (error) {
		return <div>Error loading parts.</div>;
	}

	return (
		<AuthGuard allowedRoles={['admin', 'mechanic', 'reception']}>
			<div className="p-2">
				<div className="flex justify-between items-center mb-6">
					<h1 className="text-4xl font-bold text-zinc-800">Parts</h1>
					<AddButton label="Add part" onClick={handleAddClick} />
				</div>
				<div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-6">
					{parts?.map((part) => (
						<PartCard key={part.id} {...part} />
					))}
				</div>

				<AddPartModal open={open} onClose={() => setOpen(false)} />
			</div>
		</AuthGuard>
	);
}