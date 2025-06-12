interface CommentItemProps {
	id: number;
	author: string;
	content: string;
}

export function CommentItem({ author, content }: CommentItemProps) {
	return (
		<div className="border border-zinc-100 rounded-md px-3 py-2 text-sm bg-zinc-50">
			<span className="font-medium text-orange-600">{author}</span> {content}
		</div>
	);
}