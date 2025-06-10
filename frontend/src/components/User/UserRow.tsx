import { Pencil } from 'lucide-react';
import { useState } from 'react';
import { Role } from "@/types/auth.types";

interface UserRowProps {
	id: number;
	name: string;
	email: string;
	roles: Role[];
	createdAt: string;
	addRole: (userId: number, roleId: number) => void;
	removeRole: (userId: number, roleId: number) => void;
}

export function UserRow(props: UserRowProps) {
	const { id, name, email, roles, createdAt, addRole, removeRole } = props;
	const [isEditing, setIsEditing] = useState(false);
	const [selectedRoleId, setSelectedRoleId] = useState<number | null>(null);

	const handleAddRole = () => {
		if (selectedRoleId !== null) {
			addRole(id, selectedRoleId);
			setSelectedRoleId(null);
		}
	};

	const handleRemoveRole = (roleId: number) => {
		removeRole(id, roleId);
	};

	return (
		<>
			<tr className="hover:bg-orange-300 transition">
				<td className="px-4 py-3 font-medium text-zinc-800">{name}</td>
				<td className="px-4 py-3 text-zinc-600">{email}</td>
				<td className="px-4 py-3 font-medium capitalize">
					{roles.map((role) => (
						<span
							key={role.id}
							className="mr-2 text-gray-600"
						>
              					{role.name}
							<button
								onClick={() => handleRemoveRole(role.id)}
								className="ml-2 text-red-500 hover:text-red-700"
							>
                x
              </button>
            </span>
					))}
				</td>
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
								value={selectedRoleId || ''}
								onChange={(e) => setSelectedRoleId(Number(e.target.value))}
								className="border border-zinc-300 rounded px-2 py-1"
							>
								<option value="" disabled>
									Wybierz rolę
								</option>
								<option value={1}>Admin</option>
								<option value={2}>Mechanic</option>
								<option value={3}>Reception</option>
							</select>
							<button
								onClick={handleAddRole}
								className="bg-orange-600 text-white px-3 py-1 rounded hover:bg-orange-700"
							>
								Dodaj rolę
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