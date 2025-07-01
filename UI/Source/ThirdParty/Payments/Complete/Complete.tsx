import Alert from 'UI/Alert';
import { useSession } from 'UI/Session';
import { useRouter } from 'UI/Router';
import { useEffect } from 'react';

/**
 * Props for the Complete component.
 */
interface CompleteProps {
	noSessionUpdate?: boolean;
}

/**
 * The Cart React component.
 * @param props React props.
 */
const Complete: React.FC<CompleteProps> = (props) => {
	const { setPage, pageState } = useRouter();
	const { query } = pageState;
	var { sessionReload } = useSession();

	useEffect(() => {
		
		if(!props.noSessionUpdate){
			// Force a session refresh. This is because a payment may have been for a subscription which affects the session state.
			sessionReload && sessionReload();
		}
		
	}, []);
	
	switch (query.get('status')) {
		case 'success':
			return <div className="payment-complete">
				<Alert variant='success'>
					<h2 className="stripe-complete-intent__title">
						{`Order Successful`}
					</h2>
					<p>
						{`Thank you for your order.`}
					</p>
				</Alert>
			</div>;

		case 'pending':
			return <div className="payment-complete">
				<Alert variant='info'>
					<h2 className="stripe-complete-intent__title">
						{`Purchase Pending`}
					</h2>
					<p>
						{`It looks like your details are still processing. We'll update you when processing is complete.`}
					</p>
				</Alert>
			</div>;

		case 'failed':
			return <div className="payment-complete">
				<Alert variant='danger'>
					<h2 className="stripe-complete-intent__title">
						{`Purchase Failed`}
					</h2>
					<p>
						{`Failed to process payment details. Please`} <a href='/cart/checkout' className="alert-link">{`click here`}</a> {`to try another payment method.`}
					</p>
				</Alert>
			</div>;

		case 'card-update.success':
			return <div className="payment-complete">
				<Alert variant='success'>
					<h2 className="stripe-complete-intent__title">
						{`Card Update Complete`}
					</h2>
				</Alert>
			</div>;
		case 'card-update.failed':
			return <div className="payment-complete">
				<Alert variant='danger'>
					<h2 className="stripe-complete-intent__title">
						{`Card Update Failed`}
					</h2>
					<p>
						{`Failed to process payment details. Please go back and try another payment method.`}
					</p>
				</Alert>
			</div>;

		default:
			// invalid
			return `Unknown payment status`;
		break;
    }
}

export default Complete;