import { Product } from 'Api/Product';
import ProductStock from 'UI/Product/Stock';
import ProductVariants from 'UI/Product/Variants';

interface HeaderProps {
	product: Product,
	currentVariant?: Product,
	onSelectVariant?: (variant?: Product) => void
}

/**
 * The Product Header React component.
 * @param props React props.
 */
const Header: React.FC<HeaderProps> = (props) => {
	const { product, onSelectVariant, currentVariant } = props;

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
			
			{product.variants && product.variants.length > 0 && <>
				<ProductVariants product={product} currentVariant={currentVariant} onChange={onSelectVariant} />
			</>}
			
			<ProductStock product={currentVariant || product} />
		</div>
	);
}

export default Header;