import React, { useEffect, useState } from "react";
import { Product } from 'Api/Product';
import AttributeSelectors from 'UI/Product/Variants/AttributeSelectors';

/**
 * Props for the Variants component.
 */
interface VariantsProps {
	/**
	 * Optional section title to display above the variants.
	 */
	title?: string;

	/**
	 * The product containing variants to display.
	 */
	product: Product;
}

// ------------------------
// Type Definitions
// ------------------------

interface AttributeValueEntry {
  value: string;
  variantIds: number[];
}

type AttributeMatrix = Record<string, AttributeValueEntry[]>;


/**
 * The Attributes React component.
 * Renders a table of product attributes grouped by attribute name.
 *
 * @param props React component props.
 */
const Variants: React.FC<VariantsProps> = ({ title, product }) => {
	const { variants } = product;
	const [attributeMatrix, setAttributeMatrix] = useState<AttributeMatrix>({});

	useEffect(() => {
		if (variants && variants.length > 0) { 
			setAttributeMatrix(buildAttributeMatrix(variants));
		}
	},[]);

    function buildAttributeMatrix<AttributeMatrix>(variants) {

		console.log(variants);
		const matrix = {};

		for (const variant of variants) {
			const variantId = variant.id;

			for (const attr of variant.attributes) {
				const key = attr.attribute.key;
				const value = attr.value;

				if (!matrix[key]) {
					matrix[key] = [];
				}

				// Check if this value already exists under this attribute key
				let valueEntry = matrix[key].find(entry => entry.value === value);

				if (!valueEntry) {
					valueEntry = { value, variantIds: [] };
					matrix[key].push(valueEntry);
				}

				// Add variantId if not already added
				if (!valueEntry.variantIds.includes(variantId)) {
					valueEntry.variantIds.push(variantId);
				}
			}
		}

		return matrix;
	}


	return (
		<>
			{attributeMatrix && 
			<>
				{/* Optional title header */}
				{title && (
					<h2 className="ui-product-view__subtitle">
						{title}
					</h2>
				)}
				<AttributeSelectors attributeMatrix={attributeMatrix} />
			</>
			}
		</>
	);
};

export default Variants;
