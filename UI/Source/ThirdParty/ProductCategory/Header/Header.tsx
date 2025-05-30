import productCategoryApi, { ProductCategory } from "Api/ProductCategory";
import CategoryList from 'UI/ProductCategory/List';
import ProductList from 'UI/Product/List';
import Loading from 'UI/Loading';
import productApi from 'Api/Product';
import useApi from "UI/Functions/UseApi";
import Canvas from 'UI/Canvas';
import Image from 'UI/Image';
import defaultImageRef from './image_placeholder.png';

/**
 * Props for the ProductCategory Header component.
 */
interface ProductCategoryHeaderProps {
	// Connected via a graph in the page, which is also where the includes are defined.
	// This component requires at least the following includes:
	// productCategories, productCategories.primaryUrl
	productCategory: ProductCategory
}

/**
 * The ProductCategory Header React component.
 * @param props React props.
 */
const ProductCategoryHeader: React.FC<ProductCategoryHeaderProps> = (props) => {
	const { productCategory } = props;

	if (!productCategory) {
		return;
	}

	return (
		<div className="ui-productcategory-header">
			<h1 className="ui-productcategory-header__title">
				{productCategory.name}
			</h1>
			<div className="ui-productcategory-header__details">

				<Canvas>
					{productCategory.description}
				</Canvas>

				{/* TODO: remove once we have info coming in from productCategory.description */}
				{!productCategory.description && productCategory.parentId != null && <>
					{`:: Product description to appear here ::`}
				</>}

				{!productCategory.description && productCategory.parentId == null && <>
					<p>
						At Acticare we are proud to be one of the biggest suppliers of care home supplies and equipment in the UK. We deliver everything you need to run your care home, from daily consumables to furniture and equipment, directly to your door thanks to our dedicated, nationwide delivery team.
					</p>
					<p>
						Working in a care home can be hectic and stressful, allow us as your supplier to take away one source of stress with reliable, regular deliveries and a friendly customer service team.
					</p>
					<p>
						We operate on an ethos of "client first", this means that we will go of our way to make sure that you get the products that you need, when you need them and not a moment later. Providing the utmost level of care for your residents is your priority - ensuring that you get the support and guidance that you need to do that is <em>our</em> priority.
					</p>
					<p>
						Our promise to is that we always aim to make the lives of care home teams Simpler, Safer and Better every day. With our superior range of products and excellent customer service, we believe we are the best choice of supplier for your care home.
					</p>
					<p>
						Just select the product category that you require from the list below, or alternatively you can get in touch with our dedicated team by email at <a href="mailto:sales@acticareuk.com">sales@acticareuk.com</a> or give us a call on <a href="tel:+1432271271">01432 271271</a>.
					</p>
				</>}
			</div>
			<Image className="ui-productcategory-header__image" size={512} fileRef={productCategory.featureRef || defaultImageRef} />
		</div>
	);
}

export default ProductCategoryHeader;