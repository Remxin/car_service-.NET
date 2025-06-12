'use client';

import { useAppSelector } from '@/store/hooks';
import { useRouter }      from 'next/navigation';
import { useEffect, useState, type ReactNode } from 'react';
import type { Role }      from '@/types/auth.types';

type AuthGuardProps = {
	allowedRoles: Role[];
	children: ReactNode;
};

export default function AuthGuard({ allowedRoles, children }: AuthGuardProps) {
	const router = useRouter();
	const token  = useAppSelector(s => s.auth.token);
	const user   = useAppSelector(s => s.auth.user);
	const roles  = user?.roles ?? [];
	const [ready, setReady] = useState(false);

	useEffect(() => {
		if (!token) {
			router.replace('/login');
			return;
		}
		if (!user || roles.length === 0) {
			return;
		}
		if (!roles.some(r => allowedRoles.includes(r))) {
			router.replace('orders');
			return;
		}
		setReady(true);
	}, [token, user, roles, allowedRoles, router]);

	if (!ready) {
		return (
			<div className="flex items-center justify-center h-screen">
				<span>Loading…</span>
			</div>
	);
	}

	return <>{children}</>;
}
