'use client';

import { UserRow } from '@/components/User/UserRow';
import AddButton from "@/components/AddButton";
import { useState } from "react";
import { AddUserModal } from "@/components/User/AddUserModal";
import AuthGuard from "@/components/AuthGuard";
import { useGetUsersQuery, useAddRoleMutation, useRemoveRoleMutation, useRegisterMutation } from '@/store/api/authApi';
import type { RegisterRequestBody } from '@/types/auth.types';
import Loader from '@/components/Loader';

export default function UsersPage() {
	const {data: users, isLoading} = useGetUsersQuery();
	const [addRole] = useAddRoleMutation();
	const [removeRole] = useRemoveRoleMutation();
	const [register] = useRegisterMutation();
	const [open, setOpen] = useState(false);

	if (isLoading) {
		return <Loader/>;
	}

	const handleAddRole = async (userId: number, roleId: number) => {
		try {
			await addRole({ userId: String(userId), roleId: String(roleId) }).unwrap();
			alert('Rola została dodana!');
		} catch (error) {
			console.error('Nie udało się dodać roli:', error);
			alert('Nie udało się dodać roli.');
		}
	};

	const handleRemoveRole = async (userId: number, roleId: number) => {
		try {
			await removeRole({ userId: String(userId), roleId: String(roleId) }).unwrap();
			alert('Rola została usunięta!');
		} catch (error) {
			console.error('Nie udało się usunąć roli:', error);
			alert('Nie udało się usunąć roli.');
		}
	};

	const handleRegister = async (data: RegisterRequestBody) => {
		try {
			await register(data).unwrap();
			alert('User registered successfully!');
			setOpen(false);
		} catch (error) {
			console.log("data register: ", data);
			console.error('Failed to register user:', error);

			alert('Failed to register user.');
		}
	};

	const handleAddClick = () => {
		setOpen(true);
	};

	console.log('Users:', users);


	return (
		<AuthGuard allowedRoles={['admin']}>
		<div className="p-2">
			<div className="flex justify-between items-center mb-6">
				<h1 className="text-4xl font-bold text-zinc-800">Menage users</h1>
				<AddButton label="Add user" onClick={ handleAddClick }/>
			</div>
			<div className="overflow-x-auto border border-zinc-200 rounded-lg shadow-sm bg-white rounded-lg shadow-sm">
				<table className="min-w-full text-sm">
					<thead className="bg-zinc-200 border-b border-zinc-200">
					<tr>
						<th className="px-4 py-3 text-left text-zinc-500 font-medium">Name</th>
						<th className="px-4 py-3 text-left text-zinc-500 font-medium">Email</th>
						<th className="px-4 py-3 text-left text-zinc-500 font-medium">Role</th>
						<th className="px-4 py-3 text-left text-zinc-500 font-medium">Created at</th>
						<th className="px-4 py-3"></th>
					</tr>
					</thead>
					<tbody>
					{users?.map((user) => (
						<UserRow
							key={user.id}
							id={user.id}
							name={user.name}
							email={user.email}
							roles={user.roles}
							createdAt={new Date(user.createdAt.seconds * 1000).toLocaleDateString()} // Format createdAt
							removeRole={handleRemoveRole}
							addRole={handleAddRole}
						/>
					))}
					</tbody>
				</table>
			</div>
			<AddUserModal open={open} onClose={() => setOpen(false)} onAddUser={handleRegister}/>
		</div>
		</AuthGuard>
	);
}

