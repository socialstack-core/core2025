import { Product } from 'Api/Product';
import Image from 'UI/Image';
import Link from 'UI/Link';
import Button from 'UI/Button';
import Quantity from 'UI/Product/Quantity';
//import defaultImageRef from './image_placeholder.png';
import ProductPrice, { CurrencyAmount } from 'UI/Product/Price';
import ProductStock from 'UI/Product/Stock';
import { useCart } from 'UI/Payments/CartSession';

/**
 * Props for the Signpost component.
 */
interface SignpostProps {
	/**
	 * set true to hide quantity controls
	 */
	hideQuantity?: boolean,

	/**
	 * set true to disable link to product info
	 */
	disableLink?: boolean,

	/**
	 * Line item price (in basket)
	 */
	priceOverride?: CurrencyAmount,

	/**
	 * The content to display in this signpost.
	 */
	content: Product,

	/**
	 * label for featured products
	 */
	featuredLabel?: string,

	/**
	 * allow removal from basket
	 */
	showRemove?: boolean

	/**
	 * 
	 */
	readOnly?: boolean,

	/**
	 * 
	 */
	qtyOverride?: Number,
}

/**
 * The Signpost React component.
 * @param props React props.
 */
const Signpost: React.FC<SignpostProps> = (props) => {
	const { disableLink, content, hideQuantity, priceOverride, showRemove, readOnly, qtyOverride } = props;
	var { addToCart } = useCart();

	if (!content) {
		return;
	}

	// TODO: need an isFeatured flag per product
	let isFeatured = true;
	let featuredLabel = props.featuredLabel || `Recommended`;

	// TODO: need an isApproved flag
	let isApproved = true;

	// retrieve associated category name
	let categoryName: string | null = null;

	if (content.primaryCategory) {
		categoryName = content.primaryCategory.name;
	} else if (content.productCategories?.length) {
		categoryName = content.productCategories[0].name;
	}

	// TODO: determine when product has options
	let hasOptions = false;

	function renderInner() {
		return <>
			<div className="ui-product-signpost__image">
				{/* optional featured product header */}
				{isFeatured && <>
					<span className="ui-product-signpost__featured">
						<i className="fr fr-star"></i>
						{featuredLabel}
					</span>
				</>}

				{/* product image
				  * reference 512px image as we have 3 views:
				  * list: ~150px
				  * small thumbnails: ~150px
				  * large thumbnails: ~322px
				  * 
				  * NB: while "no-image" is invalid, this will trigger a broken image;
				  * UI/Image spots this and replaces this with a placeholder image via CSS
				  */}
				<Image size={512} fileRef={content.featureRef || "no-image"} />

				{/* thumbs up icon */}
				{isApproved && <>
					<div className="ui-product-signpost__approved">
						<i className="fr fr-thumbs-up"></i>
						<span className="sr-only">
							{`Approved`}
						</span>
					</div>
				</>}
			</div>

			{/* category */}
			{categoryName && categoryName.length > 0 && <>
				<span className="ui-product-signpost__category">
					<i className="fr fr-tag"></i>
					<span>
						{categoryName}
					</span>
				</span>
			</>}

			{/* product name */}
			<span className="ui-product-signpost__name">
				{content.name}
			</span>

			{/* price */}
			<ProductPrice product={content} override={priceOverride} />

			{/* stock info */}
			<ProductStock product={content} />
		</>;
	}

	var classNames = ['ui-product-signpost'];

	if (disableLink) {
		classNames.push('ui-product-signpost--disabled');
	}

	return (
		<div className={classNames.join(' ')}>
			<div className="ui-product-signpost__outer">
				{disableLink && <>
					<div className="ui-product-signpost__inner">
						{renderInner()}
					</div>
				</>}

				{!disableLink && <>
					<Link className="ui-product-signpost__inner" href={content.primaryUrl || `/product/${content.slug}`}>
						{renderInner()}
					</Link>
				</>}

				{/* quantity controls */}
				{!hideQuantity && <>
					<Quantity product={content} qtyOverride={qtyOverride} readOnly={readOnly} />
				</>}

				{/* remove (used when viewed within basket) */}
				{showRemove && <>
					<Button xs outlined variant="danger" className="ui-product-signpost__remove" title={`Remove`} onClick={() => addToCart(content.id, 0)}>
						<i className='fr fr-trash-alt' />
					</Button>
				</>}
			</div>
		</div>
	);
}

export default Signpost;