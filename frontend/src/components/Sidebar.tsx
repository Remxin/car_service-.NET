'use client';
import '../app/globals.css';


import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useMemo } from 'react';

type Role = 'admin' | 'mechanic' | 'reception';

interface SidebarProps {
	role: Role;
}

const Sidebar = ({ role }: SidebarProps) => {
	const pathname = usePathname();

	const isActive = (href: string) =>
		pathname.startsWith(href) ? 'text-orange-500 font-semibold' : 'text-zinc-300';

	const links = useMemo(() => {
		const base = [
			{ href: '/orders', label: 'orders' },
			{ href: '/parts', label: 'parts' },
		];

		if (role === 'admin') {
			return [
				...base,
				{ href: '/reports', label: 'reports' },
				{ href: '/vehicles', label: 'vehicles' },
				{ href: '/users', label: 'manage users' },
			];
		}

		return base;
	}, [role]);

	return (
		<aside className="w-50 h-full bg-zinc-900 text-zinc-100 p-6 space-y-5 fixed top-0 left-0">
			{links.map((link) => (
				<Link
					key={link.href}
					href={link.href}
					className={`block capitalize hover:text-orange-400 transition ${isActive(link.href)}`}
				>
					{link.label}
				</Link>
			))}
		</aside>
	);
};


export default Sidebar;
