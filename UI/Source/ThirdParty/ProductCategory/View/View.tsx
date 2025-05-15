import productCategoryApi, { ProductCategory } from "Api/ProductCategory";
import CategoryList from 'UI/ProductCategory/List';
import ProductList from 'UI/Product/List';
import Loading from 'UI/Loading';
import productApi from 'Api/Product';
import useApi from "UI/Functions/UseApi";

/**
 * Props for the View component.
 */
interface ViewProps {
	// Connected via a graph in the page, which is also where the includes are defined.
	// This component requires at least the following includes:
	// productCategories, productCategories.primaryUrl
	productCategory: ProductCategory
}

/**
 * The View React component.
 * @param props React props.
 */
const View: React.FC<ViewProps> = (props) => {

	const { productCategory } = props;

	// SSR compliant useApi call:
	const [products] = useApi(() => {
		return productApi.list({
			query: 'ProductCategories=?',
			args: [productCategory.id]
		}, [
			productApi.includes!.primaryurl
		])
	}, [
		productCategory
	]);

	const [productCategories] = useApi(() => {
		return productCategoryApi.list({
			query: 'ParentId=?',
			args: [productCategory.id]
		}, [
			productCategoryApi.includes!.primaryurl
		])
	}, [
		productCategory
	]);

	if (!products || !productCategories) {
		return <Loading />;
	}

	return (
		<div className="ui-productcategory-view">
			{ /* Categories nested inside this category */ }
			<CategoryList content={productCategories.results} />

			{ /* Products inside this category */}
			<ProductList content={products.results} />
		</div>
	);
}

export default View;