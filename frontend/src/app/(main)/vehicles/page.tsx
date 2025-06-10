'use client';


import { VehicleCard } from '@/components/Vehicle/VehicleCard';
import AddButton from "@/components/AddButton";
import { useState } from "react";
import { AddVehicleModal } from "@/components/Vehicle/AddVehicleModal";
import AuthGuard from "@/components/AuthGuard";

const MOCK_VEHICLES = [
	{
		id: 1,
		brand: 'BMW',
		model: 'X5',
		year: 2020,
		vin: 'WBAXX11060DT12345',
		photoUrl: '/images/bmw.jpg',
	},
	{
		id: 2,
		brand: 'Audi',
		model: 'A4',
		year: 2019,
		vin: 'WAUZZZF40KA012345',
		photoUrl: '/images/audi.jpg',
	},
	{
		id: 3,
		brand: 'Toyota',
		model: 'Corolla',
		year: 2018,
		vin: 'JTDBR32E520052134',
		photoUrl: '/images/toyota.jpg',
	},
	{
		id: 4,
		brand: 'Ford',
		model: 'Focus',
		year: 2017,
		vin: 'WF0DP3TH4H4123456',
		photoUrl: '/images/ford.jpg',
	},
];

export default function VehiclesPage() {
	const [open, setOpen] = useState(false);

	const handleAddClick = () => {
		setOpen(true);
	};

	return (
		<AuthGuard allowedRoles={['admin']}>
		<div className="p-2">
			<div className="flex justify-between items-center mb-6">
				<h1 className="text-4xl font-bold text-zinc-800">Vehicles</h1>
				<AddButton label="Add vehicle" onClick={ handleAddClick }/>
			</div>
			<div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-6">
				{ MOCK_VEHICLES.map((vehicle) => (
					<VehicleCard key={ vehicle.id } { ...vehicle } />
				)) }
			</div>
			<AddVehicleModal open={open} onClose={() => setOpen(false)} />
		</div>
		</AuthGuard>
	);
}
