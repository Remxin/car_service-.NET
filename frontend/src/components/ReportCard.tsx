'use client';

import { FileText, Clock, CheckCircle, XCircle } from 'lucide-react';

interface ReportCardProps {
	id: string;
	status: 'in_progress' | 'completed' | 'failed';
	createdAt: string;
	filename: string;
}

export function ReportCard({ id, status, createdAt, filename }: ReportCardProps) {
	const statusInfo = {
		in_progress: {
			label: 'W trakcie generowania',
			icon: <Clock className="w-4 h-4" />,
			color: 'text-amber-600',
		},
		completed: {
			label: 'Wygenerowany',
			icon: <CheckCircle className="w-4 h-4" />,
			color: 'text-emerald-600',
		},
		failed: {
			label: 'Błąd',
			icon: <XCircle className="w-4 h-4" />,
			color: 'text-rose-600',
		},
	}[status];

	return (
		<div className="bg-white  shadow-sm border border-zinc-200 rounded-xl p-4 flex items-center justify-between  transition">
			<div className="flex items-center gap-4">
				<FileText className="w-6 h-6 text-orange-600" />
				<div>
					<p className="text-sm font-medium text-zinc-800">Raport ID: {id}</p>
					<p className="text-xs text-zinc-500">Utworzono: {createdAt}</p>
				</div>
			</div>
			<div className="flex items-center gap-4">
        <span className={`text-sm flex items-center gap-1 font-medium ${statusInfo.color}`}>
          {statusInfo.icon}
			{statusInfo.label}
        </span>
				{status === 'completed' && (
					<a
						href={`/downloads/${filename}`}
						className="text-sm text-orange-600 underline hover:text-orange-700"
						download
					>
						Pobierz
					</a>
				)}
			</div>
		</div>
	);
}
