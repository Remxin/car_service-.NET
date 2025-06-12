'use client';
import '../app/globals.css';

import Link from 'next/link';
import { usePathname, useRouter } from 'next/navigation';
import { useMemo } from 'react';
import { clearCredentials } from '@/store/authSlice';
import { useAppDispatch, useAppSelector } from '@/store/hooks';
import { LogOut } from 'lucide-react';
import type { Role } from '@/types/auth.types';

const Sidebar = () => {
	const pathname = usePathname();
	const dispatch = useAppDispatch();
	const router = useRouter();
	const user = useAppSelector(state => state.auth.user);
	const roles: Role[] = user?.roles ?? [];

	const isActive = (href: string) =>
		pathname.startsWith(href) ? 'text-orange-500 font-semibold' : 'text-zinc-300';

	console.log('Sidebar rendered with roles:', roles);

	const links = useMemo(() => {
		const base = [
			{ href: '/orders', label: 'orders' },
			{ href: '/parts',  label: 'parts'    },
		];
		if (roles.includes('admin')) {
			return [
				...base,
				{ href: '/reports',  label: 'reports'    },
				{ href: '/vehicles', label: 'vehicles'    },
				{ href: '/users',    label: 'manage users' },
			];
		}
		return base;
	}, [roles]);

	const handleLogout = () => {
		dispatch(clearCredentials());
		router.replace('/login');
	};

	return (
		<aside className="w-50 h-full bg-zinc-900 text-zinc-100 p-6 flex flex-col justify-between pb-20 fixed top-0 left-0">
			<div className="space-y-5">
				{links.map(link => (
					<Link
						key={link.href}
						href={link.href}
						className={`block capitalize hover:text-orange-400 transition ${isActive(link.href)}`}
					>
						{link.label}
					</Link>
				))}
			</div>

			<button
				onClick={handleLogout}
				className="flex items-center gap-2 text-red-400 hover:text-red-300 transition mt-10 hover:cursor-pointer"
			>
				<LogOut size={18}/>
				Log out
			</button>
		</aside>
	);
};

export default Sidebar;
