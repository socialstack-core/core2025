import { getSizeClasses } from 'UI/Functions/Components';

const COMPONENT_PREFIX = 'form__fieldset';
//const DEFAULT_VARIANT = 'primary';

export default function Fieldset(props) {
	const {
		//variant,
		xs, sm, md, lg, xl,
		//outline, outlined,
		//close,
		//submit,
		//disable, disabled,
		//round, rounded,
		legend,
		children,
		className
	} = props;

	let hasLegend = legend && legend.length;

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
		<fieldset className={componentClasses.join(' ')}>
			{hasLegend && <>
				<legend>
					{legend}
				</legend>
			</>}
			{children}
		</fieldset>
	);
}

Fieldset.propTypes = {
};

Fieldset.defaultProps = {
}
