import { Product } from 'Api/Product';
import Image from 'UI/Image';
import Link from 'UI/Link';
import Quantity from 'UI/Product/Quantity';
import defaultImageRef from './image_placeholder.png';
import ProductPrice from 'UI/Product/Price';
import ProductStock from 'UI/Product/Stock';

/**
 * Props for the Signpost component.
 */
interface SignpostProps {
	/**
	 * set true to hide quantity controls
	 */
	hideQuantity?: boolean,

	/**
	 * set true to hide add to order / choose options controls
	 */
	hideOrder?: boolean,

	/**
	 * set true to disable link to product info
	 */
	disableLink?: boolean,

	/**
	 * quantity within basket
	 */
	quantity?: number,

	/**
	 * The content to display in this signpost.
	 */
	content: Product,
}

/**
 * The Signpost React component.
 * @param props React props.
 */
const Signpost: React.FC<SignpostProps> = (props) => {
	const { disableLink, quantity, content, hideQuantity, hideOrder } = props;

	// TODO: need an isFeatured flag per product
	let isFeatured = true;

	// TODO: need an isApproved flag
	let isApproved = true;

	// TODO: retrieve associated category name
	// productCategories array?
	let categoryName = `Category name`;

	// TODO: determine when product has options
	let hasOptions = false;

	function addToOrder() {
		// TODO
	}

	function chooseOptions() {
		// TODO
	}

	function renderInternal() {
		return <>
			<header className="ui-product-signpost__header">
				{/* optional featured product header */}
				{isFeatured && <>
					<span className="ui-product-signpost__featured">
						<i className="fr fr-star"></i>
						{`Featured product`}
					</span>
				</>}

				{/* product image */}
				<Image size={200} fileRef={content.featureRef || defaultImageRef} />

				{/* thumbs up icon */}
				{isApproved && <>
					<div className="ui-product-signpost__approved">
						<i className="fr fr-thumbs-up"></i>
						<span className="sr-only">
							{`Approved`}
						</span>
					</div>
				</>}
			</header>

			{/* category */}
			<span className="ui-product-signpost__category">
				<i className="fr fr-tag"></i>
				{categoryName}
			</span>

			{/* product name */}
			<span className="ui-product-signpost__name">
				{content.name}
			</span>

			{/* price */}
			<ProductPrice product={content} />

			{/* stock info */}
			<ProductStock product={content} />
		</>;
	}

	return (
		<div className={disableLink ? "ui-product-signpost ui-product-signpost--disabled" : "ui-product-signpost"}>
			{disableLink && <>
				<div className="ui-product-signpost__internal">
					{renderInternal()}
				</div>
			</>}
			{!disableLink && <>
				<Link className="ui-product-signpost__internal" href={content.primaryUrl || `/product/${content.slug}`}>
					{renderInternal()}
				</Link>
			</>}

			{/* quantity controls */}
			{!hideQuantity && !hasOptions && <>
				<Quantity inBasket={quantity} />
			</>}

			{!hideOrder && !hasOptions && <>
				<button type="button" className="btn btn-secondary ui-product-signpost__add" onClick={() => addToOrder()}>
					{`Add to order`}
				</button>
			</>}

			{!hideOrder && hasOptions && <>
				<button type="button" className="btn btn-secondary ui-product-signpost__choose" onClick={() => chooseOptions()}>
					{`Choose options`}
				</button>
			</>}

		</div>
	);
}

export default Signpost;