import { ReportCard } from '@/components/ReportCard';
import AuthGuard from '@/components/AuthGuard';

const MOCK_REPORTS = [
	{
		id: 'REP-001',
		status: 'completed',
		createdAt: '2025-06-01',
		filename: 'report_ORD001.pdf',
	},
	{
		id: 'REP-002',
		status: 'in_progress',
		createdAt: '2025-06-02',
		filename: '',
	},
	{
		id: 'REP-003',
		status: 'failed',
		createdAt: '2025-06-03',
		filename: '',
	},
];

export default function ReportsPage() {

	return (
		<AuthGuard allowedRoles={['admin']}>
		<div className="p-2">
			<h1 className="text-4xl font-bold text-zinc-800 mb-6">Raporty</h1>
			<div className="space-y-4">
				{MOCK_REPORTS.map((report) => (
					<ReportCard key={report.id} {...report} />
				))}
			</div>
		</div>
		</AuthGuard>
	);
}
