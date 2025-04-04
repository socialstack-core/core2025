import { useId } from 'react';
import { getSizeClasses } from 'UI/Functions/Components';

const COMPONENT_PREFIX = 'ui-form';

/**
 * Props for the Form component.
 */
interface FormProps extends React.FormHTMLAttributes<HTMLFormElement> {
	/**
	 * render at extra small size
	 */
	xs?: boolean,
	/**
	 * render at small size
	 */
	sm?: boolean,
	/**
	 * render at medium size (default)
	 */
	md?: boolean,
	/**
	 * render at large size
	 */
	lg?: boolean,
	/**
	 * render at extra large size
	 */
	xl?: boolean,
	/**
	 * additional classes to be applied to <form> element
	 */
	className?: string,
}

/**
 * The Form React component.
 * @param props Form props.
 */
const Form: React.FC<FormProps> = ({ id, className, xs, sm, md, lg, xl, children, ...rest }) => {
	const _id = id || useId();

	// filter out only valid form attributes
	const formProps: React.FormHTMLAttributes<HTMLFormElement> =
		Object.fromEntries(
			Object.entries(rest).filter(([key]) =>
				key in document.createElement("input")
			)
		) as React.FormHTMLAttributes<HTMLFormElement>;

	var componentClasses = [COMPONENT_PREFIX];
	componentClasses = componentClasses.concat(getSizeClasses(COMPONENT_PREFIX, { xs, sm, md, lg, xl }));

	if (className) {
		componentClasses.push(className);
	}

	return <>
		<form id={_id} className={componentClasses.join(' ')} {...formProps}>
			{children}
		</form>
	</>;
};

export default Form;