import '@/app/globals.css';

export const metadata = {
	title: 'Car Service',
	description: 'Zarządzanie warsztatem samochodowym',
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
	return (
		<html lang="pl">
			<body>{children}</body>
		</html>
	);
}
