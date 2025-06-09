'use client'

import { useAppSelector } from '@/store/hooks';
import { useRouter }      from 'next/navigation';
import { useEffect }      from 'react';
import type { Role }      from '@/types/auth.types';

export function useAuthGuard(allowed: Role[]) {
	const router = useRouter();
	const token  = useAppSelector(s => s.auth.token);
	const user   = useAppSelector(s => s.auth.user);

	const roles: Role[] = Array.isArray(user?.roles)
		? (user.roles as Role[])
		: [];

	useEffect(() => {
		if (!token) {
			router.replace('/login');
			return;
		}
		if (!roles.some(r => allowed.includes(r))) {
			router.replace('/orders');
		}
	}, [token, roles, router, allowed]);
}
