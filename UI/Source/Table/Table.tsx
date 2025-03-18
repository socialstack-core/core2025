import Loop, { LoopProps } from 'UI/Loop';
import { AutoApi, ApiIncludes } from 'Api/ApiEndpoints';
import { VersionedContent } from 'Api/Content';

/**
 * Props for the Table component.
 */
interface TableProps<T, I> extends LoopProps<T, I> {
	/**
	 * Optionally used to render your table's header.
	 * @returns
	 */
	onHeader?: () => React.ReactNode
}

/**
 * The Table React component. Each child function should return a <tr> with the desired column arrangement inside it.
 * @param props React props.
 */
const Table = <T extends VersionedContent, I extends ApiIncludes>(props: TableProps<T, I>) => {

	const {
		onHeader,
		...loopProps
	} = props;

	return (
		<table>
			{onHeader && <thead>
				{onHeader()}
			</thead>}
			<tbody>
				<Loop {...loopProps}>
					{loopProps.children}
				</Loop>
			</tbody>
		</table>
	);
}

export default Table;