'use client';


import PartCard  from '@/components/Part/PartCard';
import AddButton from "@/components/AddButton";
import { useState } from "react";
import AddPartModal from "@/components/Part/AddPartModal";
import AuthGuard from "@/components/AuthGuard";


const MOCK_PARTS = [
	{
		id: 1,
		name: 'Filtr oleju',
		partNumber: 'OL-F123',
		description: 'Filtr oleju do silników benzynowych',
		price: 39.99,
		availableQuantity: 25,
	},
	{
		id: 2,
		name: 'Klocki hamulcowe',
		partNumber: 'BR-456',
		description: 'Zestaw przednich klocków hamulcowych',
		price: 129.99,
		availableQuantity: 12,
	},
	{
		id: 3,
		name: 'Amortyzator',
		partNumber: 'SH-789',
		description: 'Amortyzator tylny do BMW E90',
		price: 249.00,
		availableQuantity: 4,
	},
	{
		id: 4,
		name: 'Żarówka H7',
		partNumber: 'LT-H7',
		description: 'Żarówka halogenowa H7 55W',
		price: 14.50,
		availableQuantity: 100,
	},
];

export default function PartsPage() {
	const [open, setOpen] = useState(false);

	const handleAddClick = () => {
		setOpen(true);
	};


	return (
		<AuthGuard allowedRoles={['admin', 'mechanic', 'reception']}>
		<div className="p-2">
			<div className="flex justify-between items-center mb-6">
				<h1 className="text-4xl font-bold text-zinc-800">Parts</h1>
				<AddButton label="Add part" onClick={ handleAddClick }/>
			</div>
			<div className="grid grid-cols-1 sm:grid-cols-2 xl:grid-cols-3 gap-6">
				{ MOCK_PARTS.map((part) => (
					<PartCard key={ part.id } { ...part } />
				)) }
			</div>

			<AddPartModal open={open} onClose={() => setOpen(false)} />
		</div>
		</AuthGuard>
	);
}
