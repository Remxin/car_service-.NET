'use client';

import { UserRow } from '@/components/User/UserRow';
import AddButton from "@/components/AddButton";
import { useState } from "react";
import { AddUserModal } from "@/components/User/AddUserModal";

const MOCK_USERS = [
	{
		id: 1,
		name: 'Jan Kowalski',
		email: 'jan.kowalski@example.com',
		role: 'mechanic',
		createdAt: '2025-04-21',
	},
	{
		id: 2,
		name: 'Anna Nowak',
		email: 'anna.nowak@example.com',
		role: 'reception',
		createdAt: '2025-04-22',
	},
	{
		id: 3,
		name: 'Krzysztof Admin',
		email: 'admin@example.com',
		role: 'admin',
		createdAt: '2025-04-20',
	},
];

export default function UsersPage() {
	const [open, setOpen] = useState(false);

	const handleAddClick = () => {
		setOpen(true);
	};


	return (
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
					{ MOCK_USERS.map((user) => (
						<UserRow key={ user.id } { ...user } />
					)) }
					</tbody>
				</table>
			</div>
			<AddUserModal open={open} onClose={() => setOpen(false)} />
		</div>
	);
}

