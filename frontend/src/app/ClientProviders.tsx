'use client';
import { ReactNode, useEffect } from 'react';
import { Provider } from 'react-redux';
import { store } from '@/store/store';
import { useVerifyMutation } from '@/store/api/authApi';
import { setUser, clearCredentials } from '@/store/authSlice';
import { useAppDispatch } from '@/store/hooks';

function VerifyListener({ children }: { children: ReactNode }) {
	const dispatch = useAppDispatch();
	const [verify] = useVerifyMutation();

	useEffect(() => {
		const token = store.getState().auth.token;
		if (!token) return;
		verify().unwrap().then(res => {
			if (res.isValid && res.user) {
				dispatch(setUser({
					...res.user,
					roles: res.roles,
				}));
			} else {
				dispatch(clearCredentials());
			}
		}).catch(() => {
			dispatch(clearCredentials());
		});
	}, [dispatch, verify]);

	return <>{children}</>;
}

export default function ClientProviders({ children }: { children: ReactNode }) {
	return (
		<Provider store={store}>
			<VerifyListener>{children}</VerifyListener>
		</Provider>
	);
}
