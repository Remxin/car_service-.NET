'use client';

import * as Dialog from '@radix-ui/react-dialog';
import { X } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { RegisterRequestBody } from "@/types/auth.types";

export function AddUserModal({ open, onClose, onAddUser }: { open: boolean; onClose: () => void; onAddUser: (data: RegisterRequestBody) => void }) {
	const { register, handleSubmit, reset } = useForm<RegisterRequestBody>();

	const onSubmit = (data: RegisterRequestBody) => {
		onAddUser(data);
		onClose();
		reset();
	};

	return (
		<Dialog.Root open={open} onOpenChange={onClose}>
			<Dialog.Portal>
				<Dialog.Overlay className="fixed inset-0 bg-black/50 animate-in fade-in duration-200" />
				<Dialog.Content className="fixed top-1/2 left-1/2 w-full max-w-md -translate-x-1/2 -translate-y-1/2 bg-white p-6 rounded-xl shadow-lg space-y-4 z-50 animate-in slide-in-from-bottom-10 fade-in duration-200">
					<div className="flex justify-between items-center">
						<Dialog.Title className="text-lg font-bold">New user</Dialog.Title>
						<Dialog.Close className="text-zinc-500 hover:text-zinc-800">
							<X className="w-5 h-5" />
						</Dialog.Close>
					</div>

					<form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
						<div className="flex flex-col">
							<label htmlFor="firstName" className="text-sm font-medium text-zinc-700 mb-1">Imię</label>
							<input
								{...register('firstName')}
								id="firstName"
								placeholder="Imię"
								className="border border-zinc-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-orange-600"
							/>
						</div>

						<div className="flex flex-col">
							<label htmlFor="lastName" className="text-sm font-medium text-zinc-700 mb-1">Nazwisko</label>
							<input
								{...register('lastName')}
								id="lastName"
								placeholder="Nazwisko"
								className="border border-zinc-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-orange-600"
							/>
						</div>

						<div className="flex flex-col">
							<label htmlFor="email" className="text-sm font-medium text-zinc-700 mb-1">Email</label>
							<input
								{...register('email')}
								id="email"
								type="email"
								placeholder="Email"
								className="border border-zinc-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-orange-600"
							/>
						</div>

						<div className="flex flex-col">
							<label htmlFor="password" className="text-sm font-medium text-zinc-700 mb-1">Hasło</label>
							<input
								{...register('password')}
								id="password"
								type="password"
								placeholder="Hasło"
								className="border border-zinc-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-orange-600"
							/>
						</div>

						<button className="bg-orange-600 text-white px-4 py-2 rounded-lg hover:bg-orange-700 w-full">
							Dodaj użytkownika
						</button>
					</form>
				</Dialog.Content>
			</Dialog.Portal>
		</Dialog.Root>
	);
}