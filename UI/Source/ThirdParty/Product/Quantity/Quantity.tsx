import { Product } from 'Api/Product';
import { useCart } from 'UI/Payments/CartSession';
import { useRef, useState, useEffect } from "react";
import Button from 'UI/Button';

/**
 * Props for the Quantity component.
 */
interface QuantityProps {
	product: Product,

	/**
	 * optional call to action label
	 */
	ctaLabel?: string,

	/**
	 * optional additional classnames
	 */
	className?: string,
}

/**
 * The Quantity React component.
 * @param props React props.
 */
const Quantity: React.FC<QuantityProps> = (props) => {
	// TODO: min / max / bundle quantity for product
	const min = 1;
	const max = 999;
	const bundle = 1; // if product can only be ordered in multiples of [x]

	const { product, className } = props;
	var { addToCart, getCartQuantity } = useCart();
	const [quantity, setQuantity] = useState(getCartQuantity());
	const [isEditing, setIsEditing] = useState(false);
	//const wrapperRef = useRef(null);
	const inputRef = useRef(null);

	const ctaLabel = props.ctaLabel?.length ? props.ctaLabel : `Add to order`;

	/*
	useEffect(() => {
		const wrapper = wrapperRef.current;

		const handleFocusOut = () => {
			setTimeout(() => {
				if (wrapper && !wrapper.contains(document.activeElement)) {
					setIsEditing(false);
				}
			}, 0);
		};

		if (wrapper) {
			wrapper.addEventListener('focusout', handleFocusOut);
		}

		return () => {
			if (wrapper) {
				wrapper.removeEventListener('focusout', handleFocusOut);
			}
		};
	}, []);
	*/

	let qtyClasses = ['ui-product-qty'];

	if (!quantity) {
		qtyClasses.push("ui-product-qty--none");
	}

	if (isEditing) {
		qtyClasses.push("ui-product-qty--edit");
	}

	if (className) {
		qtyClasses.push(className);
	}

	function addToBasket() {
		increaseQuantity();

		if (inputRef && inputRef.current) {
			inputRef.current.focus();
		}

	}

	function updateQuantity() {
		let newQty = parseInt(inputRef.current.value, 10);

		if (isNaN(newQty) || newQty < 0) {
			newQty = 0;
		}

		addToCart(product.id,
			newQty,
			true
		).then(() => {
			setQuantity(newQty);
			setIsEditing(false);
		});

	}

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
		<div className={qtyClasses.join(' ')}>
			<div className="ui-product-qty__inner">
				{/* nothing in basket?  show "add" button */}
				{!quantity && <>
					<Button sm className="ui-product-qty__add" onClick={() => addToBasket()}>
						{ctaLabel}
					</Button>
				</>}

				{!isEditing && <>
					<Button sm className="ui-product-qty__down" aria-label={`Reduce quantity`} onClick={() => reduceQuantity()}>
						<i className={quantity > 1 ? "fr fr-minus" : "fr fr-trash-alt"}></i>
					</Button>
				</>}

				<div className="ui-product-qty__value-wrapper" /*ref={wrapperRef}*/>
					<input ref={inputRef} type="number" value={quantity} className="ui-product-qty__value" noWrapper
						min="0" max={max} step={bundle} onFocus={() => setIsEditing(true)} />
					<Button sm className="ui-product-qty__update" onClick={() => updateQuantity()}>
						{`Update`}
					</Button>
				</div>

				{!isEditing && <>
					<Button sm className="ui-product-qty__up" aria-label={`Increase quantity`} onClick={() => increaseQuantity()}>
						<i className="fr fr-plus"></i>
					</Button>
				</>}
			</div>
		</div>
	);
}

export default Quantity;