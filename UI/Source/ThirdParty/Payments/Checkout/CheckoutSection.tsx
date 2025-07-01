
interface CheckoutSectionProps {
	title: string;
	enabled?: boolean;
}

const CheckoutSection : React.FC<React.PropsWithChildren<CheckoutSectionProps>> = (props) => {
	
	// Opted for non-collapsible here - collapsibles just added extra clicks
	const {title, children, enabled} = props;
	
	return <div>
		<h2>{title}</h2>
		<div>
			{enabled ? children : `Please complete the steps above first.`}
		</div>
	</div>;
	
}

export default CheckoutSection;