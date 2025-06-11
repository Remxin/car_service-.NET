'use client';

import { VehicleCard } from '@/components/Vehicle/VehicleCard';
import AddButton from '@/components/AddButton';
import { useState } from 'react';
import { AddVehicleModal } from '@/components/Vehicle/AddVehicleModal';
import AuthGuard from '@/components/AuthGuard';
import { useGetVehiclesQuery } from '@/store/api/vehiclesApi';
import Loader from "@/components/Loader";
import { useDeleteVehicleMutation } from '@/store/api/vehiclesApi';


export default function VehiclesPage() {
	const [open, setOpen] = useState(false);
	const { data: vehicles = [], isLoading, isError } = useGetVehiclesQuery({});
	const [deleteVehicle] = useDeleteVehicleMutation();

	const handleDelete = async (vehicleId: number) => {
		try {
			await deleteVehicle(vehicleId).unwrap();
			console.log(`Vehicle with ID ${vehicleId} deleted successfully.`);
		} catch (error) {
			console.error('Failed to delete vehicle:', error);
		}
	}

	console.log("Vehicles data:", vehicles);

	if (isLoading) {
		return <Loader/>;
	}

	if (isError) {
		return <div>Error loading vehicles.</div>;
	}

	const handleAddClick = () => {
		setOpen(true);
	};

	return (
		<AuthGuard allowedRoles={['admin']}>
			<div className="p-2">
				<div className="flex justify-between items-center mb-6">
					<h1 className="text-4xl font-bold text-zinc-800">Vehicles</h1>
					<AddButton label="Add vehicle" onClick={handleAddClick} />
				</div>
				{isLoading && <p>Loading vehicles...</p>}
				{isError && <p className="text-red-500">Error loading vehicles.</p>}
				<div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-6">
					{vehicles.map((vehicle) => (
						<VehicleCard
							key={vehicle.id}
							{...vehicle}
							onDelete={handleDelete}
						/>
					))}
				</div>
				<AddVehicleModal open={open} onClose={() => setOpen(false)} />
			</div>
		</AuthGuard>
	);
}