import ProductTable from 'UI/Payments/ProductTable';
import { Purchase } from 'Api/Purchase';

/**
 * Props for the BasicInstruction component.
 */
interface BasicInstructionProps {
	purchase: Purchase
}

/**
 * The BasicInstruction React component.
 * @param props React props.
 */
const View: React.FC<BasicInstructionProps> = (props) => {

	const {
		purchase
	} = props;
	
	// Purchase is a mandatory prop (if trying to view one that can't be seen, the page 404s) but just in case:
	if(purchase == null){
		return `Purchase not found`;
	}
	
	return <>
		<h1>
			{`Details about your purchase`}
		</h1>
		<ProductTable readonly shoppingCart={{items:purchase.productQuantities}} />
	</>;
}

export default View;