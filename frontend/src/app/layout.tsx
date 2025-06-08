import '@/app/globals.css';
import ClientProviders from "@/app/ClientProviders";

export const metadata = {
	title: 'Car Service',
	description: 'Zarządzanie warsztatem samochodowym',
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
	return (
		<html lang="pl">
			<body>
			<ClientProviders>{children}</ClientProviders>
			</body>
		</html>
	);
}
