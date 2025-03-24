const BUTTON_PREFIX = 'btn';
const DEFAULT_VARIANT = 'primary';

export default function Button(props) {
	const { variant, outline, close, submit, disabled, children } = props;

	var btnVariant = variant?.toLowerCase() || (close ? undefined : DEFAULT_VARIANT);
	var btnType = submit ? "submit" : "button";

	var componentClasses = [BUTTON_PREFIX];

	if (btnVariant) {
		componentClasses.push(`${BUTTON_PREFIX}--${btnVariant}`);
	}

	if (outline) {
		componentClasses.push(`${BUTTON_PREFIX}--outline`);
	}

	if (close) {
		componentClasses.push(`${BUTTON_PREFIX}--close`);
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
		<button className={componentClasses.join(' ')} type={btnType} aria-label={close ? `Close` : undefined} disabled={disabled ? true : undefined}>
			{children}
		</button>
	);
}

Button.propTypes = {
};

Button.defaultProps = {
}
