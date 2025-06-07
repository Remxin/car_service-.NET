'use client';

import { Pencil } from 'lucide-react';

interface UserRowProps {
	id: number;
	name: string;
	email: string;
	role: 'admin' | 'mechanic' | 'reception';
	createdAt: string;
}

export function UserRow({ name, email, role, createdAt }: UserRowProps) {
	const roleColor = {
		admin: 'text-orange-600',
		mechanic: 'text-blue-600',
		reception: 'text-emerald-600',
	}[role];

	return (
		<tr className="hover:bg-orange-300 transition">
			<td className="px-4 py-3 font-medium text-zinc-800">{name}</td>
			<td className="px-4 py-3 text-zinc-600">{email}</td>
			<td className={`px-4 py-3 font-medium capitalize ${roleColor}`}>{role}</td>
			<td className="px-4 py-3 text-zinc-500">{createdAt}</td>
			<td className="px-4 py-3 text-right space-x-2">
				<button className="text-zinc-500 hover:text-orange-600">
					<Pencil className="w-4 h-4" />
				</button>
			</td>
		</tr>
	);
}
