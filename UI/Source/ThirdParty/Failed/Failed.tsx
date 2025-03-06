import Icon from 'UI/Icon';

/**
 * Displays a generic error message.
 */
const Failed: React.FC<{}> = () => {
	return(
		<div className="alert alert-danger" role="alert" style={{ textAlign: "center" }}>
			<Icon type="fa-wifi-slash" duotone />
			<p>{`The service is currently unavailable. This may be because your device is currently offline.`}</p>
		</div>
	);
}

export default Failed;