import { Product } from 'Api/Product';
import Canvas from 'UI/Canvas';
import Quantity from 'UI/Product/Quantity';
import ProductQuantity from 'UI/Payments/ProductQuantity';
import ProductCarousel from 'UI/Product/Carousel';
import ProductSpecifications from 'UI/Product/Specifications';
import Promotion from 'UI/Promotion';
import ProductAbout from 'UI/Product/About';
import ProductAttributes from 'UI/Product/Attributes';
import ProductPrice from 'UI/Product/Price';
import ProductStock from 'UI/Product/Stock';
import ProductSubtitle from 'UI/Product/Subtitle';

/**
 * Props for the View component.
 */
interface ViewProps {
	/**
	 * Must have included at least productCategories, firstCategory, firstCategory.categoryBreadcrumb
	 * Usually provided by a graph.
	 */
	product: Product
}

/**
 * The View React component.
 * @param props React props.
 */
const View: React.FC<ViewProps> = (props) => {
	const { product } = props;

	return (
		<div className="ui-product-view">
			<h1 className="ui-product-view__title">
				{product.name}
			</h1>
			<div className="ui-product-view__internal">
				<div className="ui-product-view__sidebar">
					{/* product images */}
					<ProductCarousel product={product} />

					{/* specs / downloads */}
					<ProductSpecifications />

					{/* promo */}
					<Promotion
						title={`Get 10% Off Our Bedroom Bestsellers`}
						description={`Save now on top-rated beds and accessories - Limited time offer`}
						url={`#`} />
				</div>

				<div className="ui-product-view__content">
					{/* approved / featured */}
					<header className="ui-product-view__content-header">
						<span className="ui-product-view__approved">
							{`Approved`}
							<i className="fr fr-thumbs-up"></i>
						</span>
						<span className="ui-product-view__featured">
							<i className="fr fr-star"></i>
							{`Featured product`}
						</span>
					</header>

					{/* price */}
					<ProductPrice product={product} />

					{/* stock info */}
					<ProductStock product={product} />

					{/* add to basket */}
					{/* NB: updated UI available as UI/Product/Quantity; needs combining with existing UI/Payments/ProductQuantity */}
					{/* <Quantity /> */}
					<ProductQuantity product={product} quantity={1} addText={`Add to basket`} allowMultiple={true} goStraightToCart={true} />

					{/* about */}
					<ProductAbout title={`About this product`} product={product} />

					{/* product attributes */} 
					<ProductAttributes title={`Product details`} product={product} />

					{/* FAQ */}
					<ProductSubtitle subtitle={`Frequently asked questions`} />
					{`:: TODO ::`}

					{/* frequently bought with */}
					<ProductSubtitle subtitle={`Frequently bought with this product`} />
					{`:: TODO ::`}

					{/* related products */}
					<ProductSubtitle subtitle={`Similar products`} />
					{`:: TODO ::`}
				</div>
			</div>
		</div>
	);
}

export default View;