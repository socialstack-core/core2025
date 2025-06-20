import { useRouter } from 'UI/Router';
import useApi from 'UI/Functions/UseApi';
import searchApi from 'Api/ProductSearchController';
import productApi from 'Api/Product';
import ProductList from 'UI/Product/List';
import Loading from 'UI/Loading';
import { ProductCategory } from 'Api/ProductCategory';
import { ProductAttributeValue } from 'Api/ProductAttributeValue';
import { ProductAttribute } from 'Api/ProductAttribute';

/**
 * Props for the Search component.
 */
interface SearchProps {
	/**
	 * An example optional fileRef prop.
	 */
	// logoRef?: FileRef
}

interface ProductCategoryFacet {
	count: int;
	category: ProductCategory
}

interface AttributeValueFacet {
	count: int;
	value: ProductAttributeValue
}

interface AttributeFacetGroup {
	attribute: ProductAttribute,
	facetValues: AttributeValueFacet[]
}

/**
 * The Search React component.
 * @param props React props.
 */
const Search: React.FC<SearchProps> = (props) => {

	const { pageState } = useRouter();
	const { query } = pageState;
	var initialSearch = query?.get("q") || "";
	var initialPageStr = query?.get("page") || "";
	var initialPageOffset = (parseInt(initialPageStr) || 1) - 1;

	const [resultSet] = useApi(() => searchApi.faceted({
		query: initialSearch,
		pageOffset: initialPageOffset as int,
	}, [
		// Any normal includes you might need here
		// 'price',

		// Plus then includes on the facets (the attribute and category selectors)
		'secondary.productCategoryFacets.category',
		'secondary.attributeValueFacets.value.attribute.attributeGroup'
	]), [initialSearch]);

	const showSearchResults = () => {

		if (!resultSet) {
			return <Loading />;
		}

		const facets = resultSet.secondary;
		const { attributeValueFacets, productCategoryFacets } = facets;

		const categoryFacets = (productCategoryFacets?.results || []) as ProductCategoryFacet[];
		const attributeFacets = (attributeValueFacets?.results || []) as AttributeValueFacet[];

		// Group attribute facets ("blue (12)") up by the attribute ("Colour") they are for.
		// Could group again by attribute.attributeGroup ("Material & Design") if necessary.

		var attributeMap = new Map<uint, AttributeFacetGroup>();

		attributeFacets.forEach(facet => {
			if (!(facet.value?.attribute)) {
				return;
			}

			var attribId = facet.value.productAttributeId;
			var grouping = attributeMap.get(attribId);

			if (!grouping) {
				grouping = {
					attribute: facet.value.attribute,
					facetValues: []
				} as AttributeFacetGroup;

				attributeMap.set(attribId, grouping);
			}

			grouping.facetValues.push(facet);
		});

		var attributeFacetGroups = Array.from(attributeMap.values());
		console.log(attributeFacetGroups);

		return <>
			<div>
				<h3>{`Categories`}</h3>
				{
					categoryFacets.map(facet => {

						if (!facet.category) {
							return null;
						}

						// can include primaryUrl etc on facets as well if needed
						// here though it is (probably, I haven't looked at the designs recently) exclusively a button
						// which then restricts the search.

						return <p>
							{facet.category.name + ' (' + facet.count + ')'}
						</p>;

					})
				}
				<h3>{`Attributes`}</h3>
				{
					attributeFacetGroups.map(facet => {

						// can include primaryUrl etc on facets as well if needed
						// here though it is (probably, I haven't looked at the designs recently) exclusively a button
						// which then restricts the search.

						return <>
							<h4>{
								facet.attribute.name
							}</h4>
							{
								facet.facetValues.map(facetVal => {

									return <p>
										{facetVal.value.value + ' (' + facetVal.count + ')'}
									</p>;

								})
							}
						</>;
					})
				}
			</div>
			<ProductList content={resultSet.results} />
		</>;
	};

	return (
		<div className="ui-product-search">
			{showSearchResults()}
		</div>
	);
}

export default Search;