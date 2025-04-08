const COMPONENT_PREFIX = 'ui-btn';
const DEFAULT_VARIANT = 'primary';

//import { getSizeClasses } from 'UI/Functions/Components';

export default function Button(props) {
	const {
		variant,
		xs, sm, md, lg, xl,
		outline, outlined,
		close,
		submit,
		disable, disabled,
		round, rounded,
		square, squared,
		children,
		className,
		onClick
	} = props;

	var btnVariant = variant?.toLowerCase() || (close ? undefined : DEFAULT_VARIANT);
	var btnType = submit ? "submit" : "button";

	var componentClasses = [COMPONENT_PREFIX];

	if (btnVariant) {
		componentClasses.push(`${COMPONENT_PREFIX}--${btnVariant}`);
	}

	//componentClasses = componentClasses.concat(getSizeClasses(COMPONENT_PREFIX, props));

	if (outline || outlined) {
		componentClasses.push(`${COMPONENT_PREFIX}--outline`);
	}

	if (close) {
		componentClasses.push(`${COMPONENT_PREFIX}--close`);
	}

	if (round || rounded) {
		componentClasses.push(`${COMPONENT_PREFIX}--rounded`);
	}

	if (square || squared) {
		componentClasses.push(`${COMPONENT_PREFIX}--squared`);
	}

	if (className) {
		componentClasses.push(className);
	}

	let isDisabled = disable || disabled;

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
		<button className={componentClasses.join(' ')} type={btnType} aria-label={close ? `Close` : undefined} disabled={isDisabled ? true : undefined}
			onClick={(e) => {
				if (onClick instanceof Function) {
					onClick(e);
				}
			}}>
			{children}
		</button>
	);
}

Button.propTypes = {
};

Button.defaultProps = {
}
