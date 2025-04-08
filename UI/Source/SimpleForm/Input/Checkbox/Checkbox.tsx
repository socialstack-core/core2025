import { useId } from 'react';
import Field from 'UI/SimpleForm/Field';
import { getSizeClasses } from 'UI/Functions/Components';

const COMPONENT_PREFIX = 'ui-form__field';

/**
 * Props for the Checkbox component.
 */
interface CheckboxProps extends React.InputHTMLAttributes<HTMLInputElement> {
	/**
	 * associated label
	 */
	label?: string,
	/**
	 * additional classes to be applied to <select> element
	 */
	className?: string,
	/**
	 * render field at extra small size
	 */
	xs?: boolean,
	/**
	 * render field at small size
	 */
	sm?: boolean,
	/**
	 * render field at medium size (default)
	 */
	md?: boolean,
	/**
	 * render field at large size
	 */
	lg?: boolean,
	/**
	 * render field at extra large size
	 */
	xl?: boolean,
	/**
	 * additional classes to be applied to wrapping element
	 */
	wrapperClass?: string,
	/**
	 * disable wrapping element
	 */
	noWrapper?: boolean,
	/**
	 * true if field should be rendered as a switch
	 */
	switch?: boolean
}

/**
 * The Checkbox React component.
 * @param props Checkbox props.
 * 
 * NB: switch aliased to isSwitch to prevent "unexpected keyword" error
 */
const Checkbox: React.FC<CheckboxProps> = ({ label, id, className, xs, sm, md, lg, xl, wrapperClass, noWrapper, switch: isSwitch, value, onChange, ...rest }) => {

	function renderField(): React.ReactNode {
		const _id = id || useId();

		// filter out only valid checkbox attributes
		const checkboxProps: React.InputHTMLAttributes<HTMLInputElement> =
			Object.fromEntries(
				Object.entries(rest).filter(([key]) =>
					key in document.createElement("input")
				)
			) as React.InputHTMLAttributes<HTMLInputElement>;

		// NB: ensure we've not been sent the outdated "datetime" type
		if (checkboxProps.type == "check") {
			checkboxProps.type = "checkbox";
		}

		var fieldCheckbox = `${COMPONENT_PREFIX}-checkbox`;
		var componentClasses = [fieldCheckbox, 'visually-hidden'];
		componentClasses = componentClasses.concat(getSizeClasses(fieldCheckbox, { xs: Boolean(xs), sm: Boolean(sm), md: Boolean(md), lg: Boolean(lg), xl: Boolean(xl) }));

		if (isSwitch) {
			componentClasses.push(`${COMPONENT_PREFIX}-checkbox--switch`);
		}

		if (className) {
			componentClasses.push(className);
		}

		// NB: setting switch attribute as well as class / role, as this will trigger haptics on iOS
		return <>
			<input type="checkbox" id={_id} className={componentClasses.join(' ')} role={isSwitch ? "switch" : undefined} switch={isSwitch ? true : undefined}
				value={value} onChange={(e) => {
				if (onChange instanceof Function) {
					onChange(e);
				}
			}} {...checkboxProps} />
			{label && <>
				<label htmlFor={_id}>
					{label}
				</label>
			</>}
		</>;
	}

	if (noWrapper) {
		return renderField();
	}

	return (
		<Field className={wrapperClass}>
			{renderField()}
		</Field>
	);

};

export default Checkbox;