import { Product } from 'Api/Product';
import {useEffect, useMemo, useRef} from "react";
import Image from 'UI/Image';
import defaultImageRef from './image_placeholder.png';

export type CarouselItem = {
	
	// to compare against products
	id: uint,
	
	// holds the image ref
	featureRef?: string
} 

/**
 * Props for the Carousel component.
 */
interface CarouselProps {
	/**
	 * The content to display in this signpost.
	 */
	product: Product,

	/**
	 * A possibly selected variant
	 */
	currentVariant?: Product

	/**
	 * When a thumb is changed.
	 * @param item
	 */
	onThumbSelected?: (item: CarouselItem) => void

	/**
	 * Passed through via props.
	 */
	selectedThumbnail?: CarouselItem
}

const Carousel: React.FC<CarouselProps> = ({ product, currentVariant, onThumbSelected, selectedThumbnail }: CarouselProps) => {
	var activeProduct = currentVariant || product;
	
	// we use useMemo here as this is derived from props, doesn't need
	// any state, and should only mutate when changing products.
	const allItems: CarouselItem[] = useMemo(() => {
		
		// if product.variants is empty
		// nullish coalesce (i.e. fallback)
		// so we're always guaranteed an array
		// which means .map is always valid.
		const mainImages = (product.variants ?? [])
			// we want essentially 2 things from each variant, we want its ID and we want its featureRef. 
			.map((variant) => ({ id: variant.id, featureRef: variant.featureRef }))
			// filter out any item where featureRef is falsy
			// i.e. empty string
			// i.e. null
			// i.e. undefined
			.filter(item => Boolean(item.featureRef));
		
		const additionalImages = ((product.productImages ?? []).map(item => ({ id: product.id, featureRef: item.ref })))
			.filter(item => Boolean(item.featureRef))
		
		return [...mainImages, ...additionalImages]
		// product is its only dependency.
	}, [product])
	
	// this ref exists to add a scroll into view
	// call to the highlighted product, this scrolls
	// the pane towards whichever variant is selected. 
	const activeThumbnailRef = useRef<HTMLDivElement>(null);
	
	useEffect(() => {
		if (activeThumbnailRef.current) {
			
			// the selected thumbnail when
			// selected from the matrix may be
			// at the end of the thumbnail list,
			// hidden by the containers constrained size
			// so here we scroll to the selected 
			// thumbnail.
			activeThumbnailRef.current.scrollIntoView({
				behavior: 'smooth', 
				block: 'nearest',  
				inline: 'nearest',
			})
		}
	}, [activeThumbnailRef.current])
	
	// taken from the original (unmodified), this removes the "open" attribute on the backing overlay
	const handleLightboxClick = (e: React.MouseEvent<HTMLDivElement>) => {
		if (e.target == e.currentTarget) {
			(e.target as HTMLDivElement).parentElement?.removeAttribute("open");
		}
	}
	
	// here we cascade back, 
	// in the parent component
	// when the attribute matrix is edited
	// it clears the "selectedThumbnail" state 
	// that gets passed to the corresponding prop. 
	// in that instance, it cascades down to 
	// current variant should one be selected,
	// this will definitely be true after the attribute matrix is selected. 
	// the only case where none of these are populated is where the 
	// page initially loads, which shows the default product image.
	const highlightedVariant = selectedThumbnail ?? currentVariant ?? product;
	
	// repeated helper closure, takes the currently selected picture
	// and renders it, when a featureRef isn't present, it renders
	// a default image. Mostly unmodified from its previous state
	// except it returns a full element as opposed to a fragment. 
	const renderImage = (item: CarouselItem) => {
		
		if (item.featureRef) {
			return (
				<details className="ui-product-images__slide">
					<summary>
						<Image size={512} fileRef={item.featureRef} />
					</summary>
					<div className="ui-product-images__slide-content" onClick={(e) => handleLightboxClick(e)}>
						<Image size={1024} fileRef={item.featureRef} lazyLoad={true} />
					</div>
				</details>
			);
		}

		return <Image size={512} fileRef={defaultImageRef} className="ui-product-images__slide ui-product-images__slide--empty" />;
	}

	const hasRelatedImages = allItems.length != 0;
	let productImagesClasses = ["ui-product-images"];

	if (!hasRelatedImages) {
		productImagesClasses.push("ui-product-images--single");
	}

	return (
		<div className={productImagesClasses.join(' ')}>
			{hasRelatedImages && <>
				{/* hidden radio buttons (these drive image selection) */}
				<input type="radio" name="carousel" id={`carousel_1`} checked />
				{allItems.map((_, i) => {
					return <input type="radio" name="carousel" id={`carousel_${i+2}`} />
				})}
			
				{/* thumbnail images */}
				<div className="ui-product-images__thumbnails">
					{/* 
						* this is the product's feature ref
						* and exists purely for the actual "product" not
						* a variant. 
					*/}
					<label 
						onClick={() => onThumbSelected && onThumbSelected(product)} 
						htmlFor={`current_1`} 
						className={"ui-product-images__thumbnail " + (highlightedVariant.id === product.id ? 'active' : '')}
						ref={highlightedVariant.id === product.id ? activeThumbnailRef : undefined}
					>
						<Image 
							size={100} 
							fileRef={product.featureRef || defaultImageRef} 
						/>
					</label>
					{/* variant iteration, this iterates a collection of carousel items that belong purely to variants */}
					{allItems.map((item, i) => {
						// when clicked, this sets the parent state to selected variant. 
						return (
							<label 
								onClick={() => onThumbSelected && onThumbSelected(item)} 
								htmlFor={`current_${i + 2}`} 
								className={"ui-product-images__thumbnail " + (highlightedVariant?.id === item.id ? 'active' : '')}
								ref={highlightedVariant.id === item.id ? activeThumbnailRef : undefined}
							>
								<Image size={100} fileRef={item.featureRef || defaultImageRef} />
							</label>
						);
					})}
				</div>
			</>}

			<div className="ui-product-images__slides">
				{renderImage(highlightedVariant)}
			</div>
		</div>
	)
	
}


export default Carousel;