import { useState } from "react";

/**
 * Props for the Quantity component.
 */
interface QuantityProps {
	/**
	 * minimum quantity
	 */
	min?: number,

	/**
	 * maximum quantity (base on stock level?)
	 */
	max?: number
}

/**
 * The Quantity React component.
 * @param props React props.
 */
const Quantity: React.FC<QuantityProps> = (props) => {
	const min = props.min || 1;
	const { max } = props;

	// TODO: determine how many of this product are currently in the basket
	const [quantity, setQuantity] = useState(min);

	function reduceQuantity() {
		let newQty = (quantity == min) ? 0 : quantity - 1;

		if (newQty < 0) {
			newQty = 0;
		}

		setQuantity(newQty);
	}

	function increaseQuantity() {
		let newQty = quantity + 1;

		if (max && newQty > max) {
			newQty = max;
		}

		setQuantity(newQty);
	}

	return (
		<div className="ui-product-qty">
			<button type="button" className="btn ui-product-qty__down" aria-label={`Reduce quantity`} onClick={() => reduceQuantity()}>
				<i className="fr fr-minus"></i>
			</button>
			{quantity}
			<button type="button" className="btn ui-product-qty__up" aria-label={`Reduce quantity`} onClick={() => increaseQuantity()}>
				<i className="fr fr-plus"></i>
			</button>
		</div>
	);
}

export default Quantity;