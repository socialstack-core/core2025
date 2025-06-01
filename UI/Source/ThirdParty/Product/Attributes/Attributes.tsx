import { Product } from 'Api/Product';

/**
 * Props for the Attributes component.
 */
interface AttributesProps {
	/**
	 * associated title
	 */
	title?: string,

	/**
	 * The content to display
	 */
	product: Product,
}

/**
 * The Attributes React component.
 * @param props React props.
 */
const Attributes: React.FC<AttributesProps> = (props) => {
	const { title, product } = props;

	// TODO: define attributes
	const attributes = [
		{
			'key': `Dimensions`,
			'value': `...`
		},
		{
			'key': `Adjustable height`,
			'value': `...`
		}
	];

	if (!attributes?.length) {
		return;
	}

	return <>
		{title?.length && <>
			<h2 className="ui-product-view__subtitle">
				{title}
			</h2>
		</>}
		<table className="table table-bordered ui-product-view__attributes">
			<tbody>
				{attributes.map(attrib => {
					return <tr>
						<th scope="row">
							{attrib.key}
						</th>
						<td>
							{attrib.value}
						</td>
					</tr>;
				})}
			</tbody>
		</table>
	</>;
}

export default Attributes;