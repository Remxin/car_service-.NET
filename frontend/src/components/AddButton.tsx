'use client';

import { Plus } from 'lucide-react';

interface AddButtonProps {
	label: string;
	onClick: () => void;
}

const AddButton = ({ label, onClick }: AddButtonProps) => {
	return (
		<button
			onClick={onClick}
			className=" hover:cursor-pointer inline-flex items-center gap-2 bg-orange-600 text-white px-4 py-2 rounded-lg hover:bg-orange-700 transition text-sm font-medium active:scale-95 focus:outline-none focus:ring-2 focus:ring-orange-500"
		>
			<Plus className="w-4 h-4" />
			{label}
		</button>
	);
}


export default AddButton;