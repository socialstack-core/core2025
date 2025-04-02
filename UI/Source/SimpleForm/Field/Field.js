import { getSizeClasses } from 'UI/Functions/Components';

const COMPONENT_PREFIX = 'form__field';
//const DEFAULT_VARIANT = 'primary';

export default function Field(props) {
	const {
		//variant,
		xs, sm, md, lg, xl,
		//outline, outlined,
		//close,
		//submit,
		//disable, disabled,
		//round, rounded,
		children,
		className
	} = props;

	var componentClasses = [COMPONENT_PREFIX];

	//if (btnVariant) {
	//	componentClasses.push(`${COMPONENT_PREFIX}--${btnVariant}`);
	//}

	componentClasses = componentClasses.concat(getSizeClasses(COMPONENT_PREFIX, props));

	if (className) {
		componentClasses.push(className);
	}

	//let isDisabled = disable || disabled;

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
		<div className={componentClasses.join(' ')}>
			{children}
		</div>
	);
}

Field.propTypes = {
};

Field.defaultProps = {
}
