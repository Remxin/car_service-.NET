'use client';

import * as Dialog from '@radix-ui/react-dialog';
import { X } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { useCreateVehiclePartMutation } from '@/store/api/vehiclePartsApi';

interface FormData {
	name: string;
	partNumber: string;
	description: string;
	price: number;
	availableQuantity: number;
}

const AddPartModal = ({ open, onClose }: { open: boolean; onClose: () => void }) => {
	const { register, handleSubmit, reset } = useForm<FormData>();
	const [createVehiclePart, { isLoading, isError, isSuccess }] = useCreateVehiclePartMutation();

	const onSubmit = async (data: FormData) => {
		try {
			console.log('Submitting data:', data);
			await createVehiclePart(data).unwrap();
			reset();
			onClose();
		} catch (error) {
			console.error('Error adding part:', error);
		}
	};

	return (
		<Dialog.Root open={open} onOpenChange={onClose}>
			<Dialog.Portal>
				<Dialog.Overlay className="fixed inset-0 bg-black/50 animate-in fade-in duration-200" />
				<Dialog.Content className="fixed top-1/2 left-1/2 w-full max-w-md -translate-x-1/2 -translate-y-1/2 bg-white p-6 rounded-xl shadow-lg space-y-4 z-50 animate-in slide-in-from-bottom-10 fade-in duration-200">
					<div className="flex justify-between items-center">
						<Dialog.Title className="text-lg font-bold">Nowa część</Dialog.Title>
						<Dialog.Close className="text-zinc-500 hover:text-zinc-800">
							<X className="w-5 h-5" />
						</Dialog.Close>
					</div>

					<form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
						<div className="flex flex-col">
							<label htmlFor="name" className="text-sm font-medium text-zinc-700 mb-1">Nazwa</label>
							<input
								{...register('name', { required: true })}
								id="name"
								className="border border-zinc-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-orange-600"
							/>
						</div>

						<div className="flex flex-col">
							<label htmlFor="partNumber" className="text-sm font-medium text-zinc-700 mb-1">Numer katalogowy</label>
							<input
								{...register('partNumber', { required: true })}
								id="partNumber"
								className="border border-zinc-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-orange-600"
							/>
						</div>

						<div className="flex flex-col">
							<label htmlFor="description" className="text-sm font-medium text-zinc-700 mb-1">Opis</label>
							<textarea
								{...register('description')}
								id="description"
								className="border border-zinc-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-orange-600"
							/>
						</div>

						<div className="flex flex-col">
							<label htmlFor="price" className="text-sm font-medium text-zinc-700 mb-1">Cena</label>
							<input
								type="number"
								{...register('price', { required: true, valueAsNumber: true })}
								id="price"
								className="border border-zinc-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-orange-600"
							/>
						</div>

						<div className="flex flex-col">
							<label htmlFor="availableQuantity" className="text-sm font-medium text-zinc-700 mb-1">Ilość dostępna</label>
							<input
								type="number"
								{...register('availableQuantity', { required: true, valueAsNumber: true })}
								id="availableQuantity"
								className="border border-zinc-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-orange-600"
							/>
						</div>

						<button
							type="submit"
							disabled={isLoading}
							className="bg-orange-600 text-white px-4 py-2 rounded-lg hover:bg-orange-700 w-full"
						>
							{isLoading ? 'Dodawanie...' : 'Dodaj część'}
						</button>
					</form>

					{isError && <p className="text-red-500 text-sm">Błąd podczas dodawania części.</p>}
					{isSuccess && <p className="text-green-500 text-sm">Część została dodana pomyślnie!</p>}
				</Dialog.Content>
			</Dialog.Portal>
		</Dialog.Root>
	);
};

export default AddPartModal;