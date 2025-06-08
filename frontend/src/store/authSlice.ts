import { createSlice, PayloadAction } from '@reduxjs/toolkit';

interface AuthState {
	token: string | null;
	isAuthenticated: boolean;
}

const initialState: AuthState = {
	token: typeof window !== 'undefined' ? localStorage.getItem('token') : null,
	isAuthenticated: !!(typeof window !== 'undefined' && localStorage.getItem('token')),
};

const authSlice = createSlice({
	name: 'auth',
	initialState,
	reducers: {
		setCredentials: (state, action: PayloadAction<{ token: string }>) => {
			state.token = action.payload.token;
			state.isAuthenticated = true;
			localStorage.setItem('token', action.payload.token);
		},
		clearCredentials: (state) => {
			state.token = null;
			state.isAuthenticated = false;
			localStorage.removeItem('token');
		},
	},
});

export const { setCredentials, clearCredentials } = authSlice.actions;
export default authSlice.reducer;
