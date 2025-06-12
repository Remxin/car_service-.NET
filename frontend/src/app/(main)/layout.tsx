import Sidebar from '@/components/Sidebar';

export default function MainLayout({ children }: { children: React.ReactNode }) {

    return (
        <div className="flex">
            <Sidebar />
            <main className="ml-50 h-screen p-8 w-full bg-zinc-50">{children}</main>
        </div>
    );
}

