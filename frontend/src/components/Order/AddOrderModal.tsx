'use client';

import * as Dialog from '@radix-ui/react-dialog';
import { X } from 'lucide-react';
import { useForm } from 'react-hook-form';

interface Props {
	open: boolean;
	onClose: () => void;
}

const AddOrderModal = ({ open, onClose }: Props) => {
	const { register, handleSubmit, reset } = useForm();

	const onSubmit = (data: any) => {
		console.log('Order added:', data);
		reset();
		onClose();
	};

	return (
		<Dialog.Root open={open} onOpenChange={onClose}>
			<Dialog.Portal>
				<Dialog.Overlay className="fixed inset-0 bg-black/50" />
				<Dialog.Content className=" fixed top-1/2 left-1/2 w-full max-w-md -translate-x-1/2 -translate-y-1/2 bg-white p-6 rounded-xl shadow-lg space-y-4 z-50">
					<div className="flex justify-between items-center">
						<Dialog.Title className="text-lg font-bold text-zinc-800">
							Nowe zlecenie
						</Dialog.Title>
						<Dialog.Close className="text-zinc-500 hover:text-zinc-800">
							<X className="w-5 h-5" />
						</Dialog.Close>
					</div>

					<form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
						<div>
							<label className="block text-sm font-medium text-zinc-600">
								ID pojazdu
							</label>
							<input
								{...register('vehicleId')}
								className="w-full mt-1 px-3 py-2 border border-zinc-300 rounded-md shadow-sm"
							/>
						</div>

						<div>
							<label className="block text-sm font-medium text-zinc-600">
								Status
							</label>
							<select
								{...register('status')}
								className="w-full mt-1 px-3 py-2 border border-zinc-300 rounded-md shadow-sm"
							>
								<option value="new">Nowe</option>
								<option value="in_progress">W trakcie</option>
								<option value="completed">Zakończone</option>
							</select>
						</div>

						<div>
							<label className="block text-sm font-medium text-zinc-600">
								ID mechanika
							</label>
							<input
								{...register('mechanicId')}
								className="w-full mt-1 px-3 py-2 border border-zinc-300 rounded-md shadow-sm"
							/>
						</div>

						<div className="flex justify-end">
							<button
								type="submit"
								className="bg-orange-600 text-white px-4 py-2 rounded-lg hover:bg-orange-700 transition"
							>
								Dodaj zlecenie
							</button>
						</div>
					</form>
				</Dialog.Content>
			</Dialog.Portal>
		</Dialog.Root>
	);
}

export default AddOrderModal;
