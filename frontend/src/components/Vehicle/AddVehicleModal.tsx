'use client';

import * as Dialog from '@radix-ui/react-dialog';
import { X } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { useCreateVehicleMutation } from '@/store/api/vehiclesApi';
import { CreateVehicleRequest } from '@/types/vehicles.types';
import { useState } from 'react';

export function AddVehicleModal({ open, onClose }: { open: boolean; onClose: () => void }) {
	const { register, handleSubmit, reset, setValue } = useForm<CreateVehicleRequest>();
	const [createVehicle, { isLoading, isError, isSuccess }] = useCreateVehicleMutation();
	const [uploading, setUploading] = useState(false);

	const handleImageUpload = async (file: File) => {
		setUploading(true);
		try {
			// Replace this with your actual image upload logic
			const formData = new FormData();
			formData.append('file', file);

			const response = await fetch('http://localhost:5010/v1/uploads', {
				method: 'POST',
				body: formData,
			});

			if (!response.ok) {
				throw new Error('Failed to upload image');
			}

			const data = await response.json();
			setValue('carImageUrl', data.url); // Set the uploaded image URL in the form
		} catch (error) {
			console.error('Error uploading image:', error);
		} finally {
			setUploading(false);
		}
	};

	const onSubmit = async (data: CreateVehicleRequest) => {
		try {
			await createVehicle(data).unwrap();
			reset();
			onClose();
		} catch (error) {
			console.error('Error adding vehicle:', error);
		}
	};

	return (
		<Dialog.Root open={open} onOpenChange={onClose}>
			<Dialog.Portal>
				<Dialog.Overlay className="fixed inset-0 bg-black/50 animate-in fade-in duration-200" />
				<Dialog.Content className="fixed top-1/2 left-1/2 w-full max-w-md -translate-x-1/2 -translate-y-1/2 bg-white p-6 rounded-xl shadow-lg space-y-4 z-50 animate-in slide-in-from-bottom-10 fade-in duration-200">
					<div className="flex justify-between items-center">
						<Dialog.Title className="text-lg font-bold">Nowy pojazd</Dialog.Title>
						<Dialog.Close className="text-zinc-500 hover:text-zinc-800">
							<X className="w-5 h-5" />
						</Dialog.Close>
					</div>

					<form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
						<div className="flex flex-col">
							<label htmlFor="brand" className="text-sm font-medium text-zinc-700 mb-1">Marka</label>
							<input
								{...register('brand', { required: true })}
								id="brand"
								className="border border-zinc-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-orange-600"
							/>
						</div>

						<div className="flex flex-col">
							<label htmlFor="model" className="text-sm font-medium text-zinc-700 mb-1">Model</label>
							<input
								{...register('model', { required: true })}
								id="model"
								className="border border-zinc-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-orange-600"
							/>
						</div>

						<div className="flex flex-col">
							<label htmlFor="year" className="text-sm font-medium text-zinc-700 mb-1">Rok</label>
							<input
								type="number"
								{...register('year', { required: true, valueAsNumber: true })}
								id="year"
								className="border border-zinc-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-orange-600"
							/>
						</div>

						<div className="flex flex-col">
							<label htmlFor="vin" className="text-sm font-medium text-zinc-700 mb-1">VIN</label>
							<input
								{...register('vin', { required: true })}
								id="vin"
								className="border border-zinc-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-orange-600"
							/>
						</div>

						<div className="flex flex-col">
							<label htmlFor="carImage" className="text-sm font-medium text-zinc-700 mb-1">Zdjęcie pojazdu</label>
							<input
								type="file"
								id="carImage"
								accept="image/*"
								onChange={(e) => {
									if (e.target.files && e.target.files[0]) {
										handleImageUpload(e.target.files[0]);
									}
								}}
								className="border border-zinc-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-orange-600"
							/>
							{uploading && <p className="text-sm text-orange-600">Uploading image...</p>}
						</div>

						<button
							type="submit"
							disabled={isLoading || uploading}
							className="bg-orange-600 text-white px-4 py-2 rounded-lg hover:bg-orange-700 w-full"
						>
							{isLoading ? 'Dodawanie...' : 'Dodaj pojazd'}
						</button>
					</form>

					{isError && <p className="text-red-500 text-sm">Błąd podczas dodawania pojazdu.</p>}
					{isSuccess && <p className="text-green-500 text-sm">Pojazd został dodany pomyślnie!</p>}
				</Dialog.Content>
			</Dialog.Portal>
		</Dialog.Root>
	);
}