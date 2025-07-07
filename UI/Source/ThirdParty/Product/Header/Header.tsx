import { Product } from 'Api/Product';
import ProductStock from 'UI/Product/Stock';

interface HeaderProps {
	product: Product,
	currentVariant?: Product
}

/**
 * The Product Header React component.
 * @param props React props.
 */
const Header: React.FC<HeaderProps> = (props) => {
	const { product, currentVariant } = props;

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
				{currentVariant?.name || product.name}
			</h1>
			
			<ProductStock product={currentVariant || product} />
		</div>
	);
}

export default Header;