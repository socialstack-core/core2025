import { getSizeClasses } from 'UI/Functions/Components';

const COMPONENT_PREFIX = 'ui-form__field';
//const DEFAULT_VARIANT = 'primary';

export type FieldProps = {
    xs?: boolean,
    sm?: boolean,
    md?: boolean,
    lg?: boolean,
    xl?: boolean,
    children?: React.ReactNode | React.ReactNode[],
    className?: string
}

export default function Field(props: FieldProps) {
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


	componentClasses = componentClasses.concat(getSizeClasses(COMPONENT_PREFIX, {
        xs: Boolean(xs), sm: Boolean(sm), md: Boolean(md), lg: Boolean(lg), xl: Boolean(xl)
    }));

	if (className) {
		componentClasses.push(className);
	}

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
