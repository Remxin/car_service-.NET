'use client';

import { useState } from 'react';
import { Clock, CheckCircle, XCircle, FileText, Download } from 'lucide-react';
import {
	useGetReportDownloadLinkQuery,
	useSendReportEmailMutation
} from "@/store/api/reportsApi";
import { Report } from "@/types/reports.types";

interface Props extends Report {}

export function ReportCard({ id, vehicle, mechanic, status, createdAt }: Props) {
	const [showLink, setShowLink] = useState(false);
	const { data: downloadData, refetch } = useGetReportDownloadLinkQuery(id);

	const [sendReportEmail] = useSendReportEmailMutation();

	const statusMap = {
		IN_PROGRESS: {
			label: 'W trakcie generowania',
			icon: <Clock className="w-4 h-4" />,
			color: 'text-amber-600',
		},
		GENERATED: {
			label: 'Wygenerowany',
			icon: <CheckCircle className="w-4 h-4" />,
			color: 'text-emerald-600',
		},
		FAILED: {
			label: 'Błąd',
			icon: <XCircle className="w-4 h-4" />,
			color: 'text-rose-600',
		},
	} as const;

	const info = statusMap[status] || {
		label: 'Nieznany',
		icon: null,
		color: 'text-zinc-600',
	};

	const handleDownload = async () => {
		await refetch();
	};

	const handleSendEmail = async () => {
		await sendReportEmail({ ReportId: id, UsersIds: [mechanic.id] });
		alert('Email sent!');
	};

	return (
		<div className="bg-white shadow-sm border border-zinc-200 rounded-xl p-4 flex items-center justify-between transition">
			<div className="flex items-center gap-4">
				<FileText className="w-6 h-6 text-orange-600" />
				<div>
					<p className="text-sm font-medium text-zinc-800">Report ID: {id}</p>
					<p className="text-xs text-zinc-500">
						Created: {new Date(createdAt.seconds * 1000).toLocaleString()}
					</p>
					<p className="text-xs text-zinc-500">
						Vehicle: {vehicle.brand} {vehicle.model}
					</p>
				</div>
			</div>
			<div className="flex items-center gap-4">
        <span className={`text-sm flex items-center gap-1 font-medium ${info.color}`}>
          {info.icon}
			{info.label}
        </span>
				{status === 'GENERATED' && (
					<>
						<button
							onClick={handleDownload}
							className="p-1 text-orange-600 hover:text-orange-700 hover: cursor-pointer"
							title="Download report"
						>
							<Download className="w-4 h-4" />
						</button>
						{downloadData && (
							<a
								href={downloadData.downloadUrl}
								className="text-sm text-orange-600 underline hover:text-orange-700 hover: cursor-pointer"
								download
							>
								Pobierz
							</a>
						)}
						<button
							onClick={handleSendEmail}
							className="text-sm text-orange-600 underline hover:text-orange-700 hover: cursor-pointer"
						>
							Send Email
						</button>
					</>
				)}
			</div>
		</div>
	);
}
