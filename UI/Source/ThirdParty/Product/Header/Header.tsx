import { Product } from 'Api/Product';
import ProductStock from 'UI/Product/Stock';

interface HeaderProps {
	product: Product,
}

/**
 * The Product Header React component.
 * @param props React props.
 */
const Header: React.FC<HeaderProps> = (props) => {
	const { product } = props;

	// TODO
	const isFeatured = true;

	return (
		<div className="ui-product-header">
			{isFeatured && <>
				<div className="ui-product-header__featured">
					<i className="fr fr-star"></i>
					{`Recommended`}
				</div>
			</>}

			<h1 className="ui-product-header__title">
				{product.name}
			</h1>

			<ProductStock product={product} />
		</div>
	);
}

export default Header;