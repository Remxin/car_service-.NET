'use client';

import * as Dialog from '@radix-ui/react-dialog';
import { X } from 'lucide-react';
import { useForm } from 'react-hook-form';

export function AddUserModal({ open, onClose }: { open: boolean; onClose: () => void }) {
	const { register, handleSubmit, reset } = useForm();

	const onSubmit = (data: any) => {
		console.log('User added:', data);
		reset();
		onClose();
	};



	return (
		<Dialog.Root open={open} onOpenChange={onClose}>
			<Dialog.Portal>
				<Dialog.Overlay className="fixed inset-0 bg-black/50 animate-in fade-in duration-200" />
				<Dialog.Content className="fixed top-1/2 left-1/2 w-full max-w-md -translate-x-1/2 -translate-y-1/2 bg-white p-6 rounded-xl shadow-lg space-y-4 z-50 animate-in slide-in-from-bottom-10 fade-in duration-200">
					<div className="flex justify-between items-center">
						<Dialog.Title className="text-lg font-bold">Nowy użytkownik</Dialog.Title>
						<Dialog.Close className="text-zinc-500 hover:text-zinc-800">
							<X className="w-5 h-5" />
						</Dialog.Close>
					</div>

					<form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
						<input {...register('name')} placeholder="Imię i nazwisko" className="input" />
						<input {...register('email')} type="email" placeholder="Email" className="input" />
						<select {...register('role')} className="input">
							<option value="admin">Admin</option>
							<option value="mechanic">Mechanik</option>
							<option value="reception">Recepcja</option>
						</select>

						<button className="bg-orange-600 text-white px-4 py-2 rounded-lg hover:bg-orange-700 w-full">
							Dodaj użytkownika
						</button>
					</form>
				</Dialog.Content>
			</Dialog.Portal>
		</Dialog.Root>
	);
}
