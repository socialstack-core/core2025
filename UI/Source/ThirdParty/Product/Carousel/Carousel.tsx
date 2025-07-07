import { Product } from 'Api/Product';
import { useEffect, useId } from "react";
import Image from 'UI/Image';
import defaultImageRef from './image_placeholder.png';

/**
 * Props for the Carousel component.
 */
interface CarouselProps {
	/**
	 * The content to display in this signpost.
	 */
	product: Product,
}

/**
 * The Carousel React component.
 * @param props React props.
 */
const Carousel: React.FC<CarouselProps> = (props) => {
	const { product } = props;
	let id = "carousel";

	useEffect(() => {
		//id = useId();
	}, []);

	const hasRelatedImages = product.productImages?.length > 0;

	let productImagesClasses = ["ui-product-images"];

	if (!hasRelatedImages) {
		productImagesClasses.push("ui-product-images--single");
	}

	function handleLightboxClick(e) {

		// close if clicking the background and not the image
		if (e.target == e.currentTarget) {
			e.target.parentElement.removeAttribute("open");
		}

	}

	return (
		<div className={productImagesClasses.join(' ')}>
			{hasRelatedImages && <>
				{/* hidden radio buttons (these drive image selection) */}
				<input type="radio" name="carousel" id={`${id}_1`} checked />
				{product.productImages?.map((ref, i) => {
					return <input type="radio" name="carousel" id={`${id}_${i+2}`} />
				})}

				{/* thumbnail images */}
				<div className="ui-product-images__thumbnails">
					<label htmlFor={`${id}_1`} className="ui-product-images__thumbnail">
						<Image size={100} fileRef={product.featureRef || defaultImageRef} />
					</label>
					{product.productImages?.map((productImage, i) => {
						return <>
							<label htmlFor={`${id}_${i + 2}`} className="ui-product-images__thumbnail">
								<Image size={100} fileRef={productImage.ref || defaultImageRef} />
							</label>
						</>;
					})}
				</div>

			</>}

			{/* larger image preview with full-size preview on click */}
			<div className="ui-product-images__slides">
				<details className="ui-product-images__slide">
					<summary>
						<Image size={512} fileRef={product.featureRef || defaultImageRef} />
					</summary>
					<div className="ui-product-images__slide-content" onClick={(e) => handleLightboxClick(e)}>
						<Image size={1024} fileRef={product.featureRef || defaultImageRef} lazyLoad={true} />
					</div>
				</details>
				{product.productImages?.map((productImage, i) => {
					return <>
						<details className="ui-product-images__slide">
							<summary>
								<Image size={512} fileRef={productImage.ref || defaultImageRef} />
							</summary>
							<div className="ui-product-images__slide-content" onClick={(e) => handleLightboxClick(e)}>
								<Image size={1024} fileRef={productImage.ref || defaultImageRef} lazyLoad={true} />
							</div>
						</details>
					</>;
				})}
			</div>
		</div>
	);
}

export default Carousel;