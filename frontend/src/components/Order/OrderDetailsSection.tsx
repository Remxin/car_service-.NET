'use client';

interface Props {
	title: string;
	children: React.ReactNode;
}

export function OrderDetailsSection({ title, children }: Props) {
	return (
		<section className="bg-white border border-zinc-200 rounded-xl p-6 space-y-4 shadow-sm">
			<h3 className="text-lg font-semibold text-zinc-800">{title}</h3>
			<div className="space-y-2">{children}</div>
		</section>
	);
}
