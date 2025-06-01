import Image from 'UI/Image';
import Link from 'UI/Link';
import defaultImageRef from './image_placeholder.png';

/**
 * Props for the Promotion component.
 */
interface PromotionProps {
	/**
	 * associated promotion image
	 */
	promoRef?: FileRef,

	/**
	 * promotion title
	 */
	title: string,

	/**
	 * promotion description
	 */
	description: string,

	/**
	 * call to action text
	 */
	cta?: string,

	/**
	 * call to action link
	 */
	url: string,
}

/**
 * The Promotion React component.
 * @param props React props.
 */
const Promotion: React.FC<PromotionProps> = (props) => {
	const { promoRef, title, description, cta, url } = props;

	return (
		<aside className="ui-promotion">
			<Image size={400} fileRef={promoRef || defaultImageRef} />
			<h4 className="ui-promotion__title">
				{title}
			</h4>
			<p className="ui-promotion__description">
				{description}
			</p>
			<Link href={url} className="btn btn-secondary ui-promotion__cta">
				{cta || `Shop now!`}
			</Link>
		</aside>
	);
}

export default Promotion;