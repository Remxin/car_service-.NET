import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import type { User } from '@/types/auth.types';

type AuthState = {
	token: string | null;
	user: User | null;
}

const initialState: AuthState = {
	token: typeof window !== 'undefined' ? localStorage.getItem('token') : null,
	user: typeof window !== 'undefined'
		? JSON.parse(localStorage.getItem('user') || 'null')
		: null,
};

const authSlice = createSlice({
	name: 'auth',
	initialState,
	reducers: {
		setCredentials: (state, action: PayloadAction<{ token: string }>) => {
			state.token = action.payload.token;
			localStorage.setItem('token', action.payload.token);
		},
		setUser: (state, action: PayloadAction<User>) => {
			state.user = action.payload;
			localStorage.setItem('user', JSON.stringify(action.payload));
		},
		clearCredentials: (state) => {
			state.token = null;
			state.user = null;
			localStorage.removeItem('token');
			localStorage.removeItem('user');
		},
	},
});

export const { setCredentials, setUser, clearCredentials } = authSlice.actions;
export default authSlice.reducer;
