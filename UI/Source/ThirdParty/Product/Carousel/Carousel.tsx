import { Product } from 'Api/Product';
import { useId } from "react";
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
	const id = useId();

	// TODO: determine related images for product
	const relatedImages = [];
	/* uncomment for test
	const relatedImages = [
		"public:7F9316427F97C7546866B2DE191C5C90/80.jpg",
		"public:7F9316427F97C7546866B2DE191C5C90/81.jpg",
		"public:7F9316427F97C7546866B2DE191C5C90/82.jpg",
		"public:7F9316427F97C7546866B2DE191C5C90/83.jpg",
	];
	*/

	const hasRelatedImages = relatedImages?.length > 0;

	let productImagesClasses = ["ui-product-images"];

	if (!hasRelatedImages) {
		productImagesClasses.push("ui-product-images--single");
	}

	return (
		<div className={productImagesClasses.join(' ')}>
			{hasRelatedImages && <>
				{/* hidden radio buttons (these drive image selection) */}
				<input type="radio" name="carousel" id={`${id}_1`} checked />
				{relatedImages.map((ref, i) => {
					return <input type="radio" name="carousel" id={`${id}_${i+2}`} />
				})}

				{/* thumbnail images */}
				<div className="ui-product-images__thumbnails">
					<label htmlFor={`${id}_1`} className="ui-product-images__thumbnail">
						<Image size={100} fileRef={product.featureRef || defaultImageRef} />
					</label>
					{relatedImages.map((ref, i) => {
						return <>
							<label htmlFor={`${id}_${i + 2}`} className="ui-product-images__thumbnail">
								<Image size={100} fileRef={ref || defaultImageRef} />
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
					<div className="ui-product-images__slide-content">
						<Image size={1024} fileRef={product.featureRef || defaultImageRef} lazyLoad={true} />
					</div>
				</details>
				{relatedImages.map((ref, i) => {
					return <>
						<details className="ui-product-images__slide">
							<summary>
								<Image size={512} fileRef={ref || defaultImageRef} />
							</summary>
							<div className="ui-product-images__slide-content">
								<Image size={1024} fileRef={ref || defaultImageRef} lazyLoad={true} />
							</div>
						</details>
					</>;
				})}
			</div>
		</div>
	);
}

export default Carousel;