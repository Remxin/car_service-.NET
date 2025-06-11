'use client';

import * as Dialog from '@radix-ui/react-dialog';
import { X } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { useCreateVehicleMutation } from '@/store/api/vehiclesApi';
import { CreateVehicleRequest } from '@/types/vehicles.types';
import { useState } from 'react';

export function AddVehicleModal({
									open,
									onClose,
								}: {
	open: boolean;
	onClose: () => void;
}) {
	const DEFAULT_IMAGE_URL = 'https://png.pngtree.com/png-vector/20230206/ourmid/pngtree-orange-car-vector-mockup-png-image_6587139.png';

	const { register, handleSubmit, reset, setValue, getValues } =
		useForm<CreateVehicleRequest>({
			defaultValues: {
				brand: '',
				model: '',
				vin: '',
				year: undefined,
				carImageUrl: DEFAULT_IMAGE_URL,
			},
		});

	const [createVehicle, { isLoading, isError, isSuccess }] =
		useCreateVehicleMutation();

	const [uploading, setUploading] = useState(false);

	const handleImageUpload = async (file: File) => {
		setUploading(true);
		try {
			const formData = new FormData();
			formData.append('file', file);

			const response = await fetch('http://localhost:5010/v1/uploads', {
				method: 'POST',
				body: formData,
			});

			if (!response.ok) {
				throw new Error(`Upload failed: ${response.status}`);
			}

			const data = await response.json();
			setValue(
				'carImageUrl',
				data.url || DEFAULT_IMAGE_URL
			);
		} catch (err) {
			console.error('Error uploading image:', err);
			setValue('carImageUrl', DEFAULT_IMAGE_URL);
		} finally {
			setUploading(false);
		}
	};

	const onSubmit = async (data: CreateVehicleRequest) => {
		if (!data.carImageUrl || data.carImageUrl === '') {
			data.carImageUrl = DEFAULT_IMAGE_URL;
		}

		try {
			await createVehicle(data).unwrap();
			reset();
			onClose();
		} catch (err) {
			console.error('Error adding vehicle:', err);
		}
	};

	return (
		<Dialog.Root open={open} onOpenChange={onClose}>
			<Dialog.Overlay className="fixed inset-0 bg-black/50" />

			<Dialog.Content className="fixed top-1/2 left-1/2 w-full max-w-md -translate-x-1/2 -translate-y-1/2
                                  bg-white p-6 rounded-xl shadow-lg z-50">
				<div className="flex justify-between items-center mb-4">
					<Dialog.Title className="text-lg font-bold">Nowy pojazd</Dialog.Title>
					<Dialog.Close>
						<X className="w-5 h-5 text-zinc-500 hover:text-zinc-800" />
					</Dialog.Close>
				</div>

				<form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
					<div>
						<label htmlFor="brand" className="block text-sm font-medium text-zinc-700">
							Marka
						</label>
						<input
							id="brand"
							{...register('brand', { required: true })}
							className="mt-1 block w-full border border-zinc-300 rounded-lg px-3 py-2"
						/>
					</div>

					<div>
						<label htmlFor="model" className="block text-sm font-medium text-zinc-700">
							Model
						</label>
						<input
							id="model"
							{...register('model', { required: true })}
							className="mt-1 block w-full border border-zinc-300 rounded-lg px-3 py-2"
						/>
					</div>

					<div>
						<label htmlFor="year" className="block text-sm font-medium text-zinc-700">
							Rok
						</label>
						<input
							id="year"
							type="number"
							{...register('year', { required: true, valueAsNumber: true })}
							className="mt-1 block w-full border border-zinc-300 rounded-lg px-3 py-2"
						/>
					</div>

					<div>
						<label htmlFor="vin" className="block text-sm font-medium text-zinc-700">
							VIN
						</label>
						<input
							id="vin"
							{...register('vin', { required: true })}
							className="mt-1 block w-full border border-zinc-300 rounded-lg px-3 py-2"
						/>
					</div>

					<div>
						<label htmlFor="carImage" className="block text-sm font-medium text-zinc-700">
							Zdjęcie pojazdu
						</label>
						<input
							id="carImage"
							type="file"
							accept="image/*"
							onChange={(e) => {
								if (e.target.files?.[0]) {
									handleImageUpload(e.target.files[0]);
								}
							}}
							className="mt-1 block w-full border border-zinc-300 rounded-lg px-3 py-2"
						/>
						{uploading && (
							<p className="text-sm text-orange-600">Uploading image...</p>  // info o uploadzie
						)}
					</div>

					<button
						type="submit"
						disabled={isLoading || uploading}
						className="w-full bg-orange-600 text-white px-4 py-2 rounded-lg hover:bg-orange-700"
					>
						{isLoading ? 'Dodawanie...' : 'Dodaj pojazd'}
					</button>
				</form>

				{isError && (
					<p className="mt-2 text-sm text-red-500">
						Błąd podczas dodawania pojazdu.
					</p>
				)}
				{isSuccess && (
					<p className="mt-2 text-sm text-green-500">
						Pojazd został dodany pomyślnie!
					</p>
				)}
			</Dialog.Content>
		</Dialog.Root>
	);
}
