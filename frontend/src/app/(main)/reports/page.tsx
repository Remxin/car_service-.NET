'use client';

import { ReportCard } from '@/components/ReportCard';
import AuthGuard from '@/components/AuthGuard';
import Loader from '@/components/Loader';
import { useGetReportsQuery } from '@/store/api/reportsApi';

export default function ReportsPage() {
	const { data: reports, isLoading, isError } = useGetReportsQuery();

	if (isLoading) {
		return <Loader />;
	}

	if (isError) {
		return <p className="text-red-500">Error loading reports.</p>;
	}

	return (
		<AuthGuard allowedRoles={['admin']}>
			<div className="p-2">
				<h1 className="text-4xl font-bold text-zinc-800 mb-6">Reports</h1>
				<div className="space-y-4">
					{reports?.map((report) => (
						<ReportCard key={report.id} {...report} />
					))}
				</div>
			</div>
		</AuthGuard>
	);
}
