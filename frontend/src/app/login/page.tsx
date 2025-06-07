'use client';

import { useState } from 'react';
import { Mail, Lock } from 'lucide-react'; // zainstaluj: npm install lucide-react
import { useRouter } from 'next/navigation';

export default function LoginPage() {
	const [email, setEmail] = useState('');
	const [password, setPassword] = useState('');
	const router = useRouter();

	const handleSubmit = (e: React.FormEvent) => {
		e.preventDefault();
		router.push('/orders');
	};

	return (
		<div className="relative min-h-screen flex items-center justify-center bg-[url('../assets/loginBackground.jpg')] bg-cover bg-center bg-no-repeat">
			<div className="absolute inset-0 bg-black/60" aria-hidden="true" />

			<form
				onSubmit={handleSubmit}
				className="relative z-10 backdrop-blur-md bg-white shadow-2xl rounded-2xl px-10 py-8 w-full max-w-md space-y-6 animate-fade-in"
			>
				<h1 className="text-3xl font-bold text-center text-zinc-800">Welcome back 👋</h1>
				<p className="text-center text-zinc-600 text-sm">Please enter your credentials</p>

				<div className="space-y-4">
					<div>
						<label htmlFor="email" className="block text-sm font-medium text-zinc-700 mb-1">
							Email address
						</label>
						<div className="relative">
							<Mail className="absolute left-3 top-1/2 -translate-y-1/2 text-zinc-400 w-5 h-5" />
							<input
								type="email"
								id="email"
								className="pl-10 pr-3 py-2 w-full border border-zinc-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-orange-400 transition"
								value={email}
								onChange={(e) => setEmail(e.target.value)}
								required
							/>
						</div>
					</div>

					<div>
						<label htmlFor="password" className="block text-sm font-medium text-zinc-700 mb-1">
							Password
						</label>
						<div className="relative">
							<Lock className="absolute left-3 top-1/2 -translate-y-1/2 text-zinc-400 w-5 h-5" />
							<input
								type="password"
								id="password"
								className=" pl-10 pr-3 py-2 w-full border border-zinc-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-orange-400 transition"
								value={password}
								onChange={(e) => setPassword(e.target.value)}
								required
							/>
						</div>
					</div>

					<button
						type="submit"
						className="w-full mt-10 bg-orange-600 text-white py-2 rounded-lg hover:bg-orange-700 transition active:scale-95 focus:ring-2 focus:ring-orange-400 focus:outline-none"
					>
						Log in
					</button>
				</div>
			</form>
		</div>
	);
}
