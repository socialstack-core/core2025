import { Product } from 'Api/Product';
import Image from 'UI/Image';
import Link from 'UI/Link';
import Input from 'UI/Input';
import Quantity from 'UI/Product/Quantity';
import defaultImageRef from './image_placeholder.png';

/**
 * Props for the Signpost component.
 */
interface SignpostProps {
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
	const { content } = props;

	// TODO: need an isFeatured flag per product
	let isFeatured = true;

	// TODO: need a flag per product to tie to "thumbs up" icon
	let isLiked = true;
	const thumbsUpLabel = <>
		<i className="fr fr-thumbs-up"></i>
		<span className="sr-only">
			{`Like`}
		</span>
	</>;

	// TODO: retrieve associated category name
	let categoryName = `Category name`;

	// TODO: retrieve associated price (see priceId)
	// NB: also needs to account for:
	// - global incl / excl VAT setting (see header)
	// - multiple options (set to lowest price so this can be displayed as "From £X.XX")
	let price = 85.02;

	// TODO: optional previous price
	let oldPrice = 96.99;

	// TODO: determine when product has options
	let hasOptions = false;
	let options = 12;
	let approved = 4;

	let GBPound = new Intl.NumberFormat('en-GB', {
		style: 'currency',
		currency: 'GBP',
	});

	function addToOrder() {
		// TODO
	}

	function chooseOptions() {
		// TODO
	}

	return (
		<div className="ui-product-signpost">
			<Link href={content.primaryUrl}>
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
					<Input type="checkbox" className="ui-product-signpost__liked" label={thumbsUpLabel} onClick={(e) => {
						e.stopPropagation()
					}} value={isLiked} noWrapper />
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
				<span className="ui-product-signpost__price">
					{hasOptions && <span>{`From`}</span>}
					{GBPound.format(price)}
					{oldPrice && <span>{`Was ${GBPound.format(oldPrice) }`}</span>}
				</span>

				{/* stock info */}
				{!hasOptions && <>
					<span className="ui-product-signpost__stock">
						<i className="fr fr-barcode"></i>
						<span>
							{content.sku}
						</span>
						<span>
							&mdash;
						</span>
						{content.stock > 0 && `${content.stock} in stock`}
						{!content.stock && `OUT OF STOCK`}
					</span>
				</>}

				{hasOptions && <>
					<span className="ui-product-signpost__options">
						{`${options} options - ${approved} approved`}
					</span>
				</>}

			</Link>

			{/* quantity controls */}
			{!hasOptions && <>
				<Quantity />
			</>}

			{!hasOptions && <>
				<button type="button" className="btn btn-secondary ui-product-signpost__add" onClick={() => addToOrder()}>
					{`Add to order`}
				</button>
			</>}

			{hasOptions && <>
				<button type="button" className="btn btn-secondary ui-product-signpost__choose" onClick={() => chooseOptions()}>
					{`Choose options`}
				</button>
			</>}

		</div>
	);
}

export default Signpost;