'use client';

import { Pencil } from 'lucide-react';
import { useState } from 'react';

interface UserRowProps {
	id: number;
	name: string;
	email: string;
	role: 'admin' | 'mechanic' | 'reception';
	createdAt: string;
}

export function UserRow({ id, name, email, role, createdAt }: UserRowProps) {
	const [isEditing, setIsEditing] = useState(false);
	const [newRole, setNewRole] = useState(role);

	const roleColor = {
		admin: 'text-orange-600',
		mechanic: 'text-blue-600',
		reception: 'text-emerald-600',
	}[role];

	const handleSave = () => {
		console.log(`Zmieniono rolę użytkownika ${id} na: ${newRole}`);
		setIsEditing(false);
	};

	const handleRemove = () => {
		console.log(`Usunięto rolę użytkownika ${id}`);
		//setNewRole('');
		setIsEditing(false);
	};

	return (
		<>
			<tr className="hover:bg-orange-300 transition">
				<td className="px-4 py-3 font-medium text-zinc-800">{name}</td>
				<td className="px-4 py-3 text-zinc-600">{email}</td>
				<td className={`px-4 py-3 font-medium capitalize ${roleColor}`}>{role}</td>
				<td className="px-4 py-3 text-zinc-500">{createdAt}</td>
				<td className="px-4 py-3 text-right space-x-2">
					<button
						onClick={() => setIsEditing(true)}
						className="text-zinc-500 hover:text-orange-600 transition"
					>
						<Pencil className="w-4 h-4" />
					</button>
				</td>
			</tr>

			{isEditing && (
				<tr>
					<td colSpan={5} className="bg-zinc-100 p-4">
						<div className="flex items-center gap-4">
							<select
								value={newRole}
								onChange={(e) => setNewRole(e.target.value as UserRowProps['role'])}
								className="border border-zinc-300 rounded px-2 py-1"
							>
								<option value="admin">Admin</option>
								<option value="mechanic">Mechanic</option>
								<option value="reception">Reception</option>
							</select>
							<button
								onClick={handleSave}
								className="bg-orange-600 text-white px-3 py-1 rounded hover:bg-orange-700"
							>
								Zapisz
							</button>
							<button
								onClick={handleRemove}
								className="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600"
							>
								Usuń rolę
							</button>
							<button
								onClick={() => setIsEditing(false)}
								className="text-zinc-500 hover:text-zinc-800"
							>
								Anuluj
							</button>
						</div>
					</td>
				</tr>
			)}
		</>
	);
}
