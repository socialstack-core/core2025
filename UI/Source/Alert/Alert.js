const ALERT_PREFIX = 'alert';
const DEFAULT_VARIANT = 'info';

export default function Alert(props) {
	const { variant, title, titleTag, isDismissable, children } = props;
	let TitleTag = titleTag || 'strong';

	var alertVariant = variant?.toLowerCase() || DEFAULT_VARIANT;

	var componentClasses = [ALERT_PREFIX];
	componentClasses.push(`${ALERT_PREFIX}--${alertVariant}`);

	if (isDismissable) {
		componentClasses.push(`${ALERT_PREFIX}--dismissable`);
	}

	/* runs only after component initialisation (comparable to legacy componentDidMount lifecycle method)
	useEffect(() => {
		// ...
	}, []);
	*/

	/* runs after both component initialisation and each update (comparable to legacy componentDidMount / componentDidUpdate lifecycle methods)
	useEffect(() => {
		// ...
	});
	*/

	return (
		<div className={componentClasses.join(' ')} role="alert">
			<div className="alert__notch"></div>
			{title && <TitleTag className="alert__title">
				{title}
			</TitleTag>}
			{children}
		</div>
	);
}

Alert.propTypes = {
};

Alert.defaultProps = {
}
