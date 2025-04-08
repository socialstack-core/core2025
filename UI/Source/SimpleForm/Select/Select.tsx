import { useId } from 'react';
import Field from '../Field';

const COMPONENT_PREFIX = 'ui-form__field';

/**
 * Props for the Footer component.
 */
interface SelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
	/**
	 * associated label
	 */
	label?: string,
	/**
	 * additional classes to be applied to <select> element
	 */
	className?: string,
	/**
	 * true if outlined (false for solid background colour with no border)
	 */
	outlined?: boolean,
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
	 * related help / info message to appear with field
	 */
	help?: string,
	/**
	 * current selected value
	 */
	value?: string,
	/**
	 * callback function triggered on selection change
	 */
	onChange?: Function,
}

/**
 * The Select React component.
 * @param props Select props.
 */
const Select: React.FC<SelectProps> = ({ label, id, className, outlined, children, xs, sm, md, lg, xl, wrapperClass, noWrapper, help, value, onChange, ...rest }) => {

	function renderField() {
		const _id = id || useId();

		// filter out only valid select attributes
		const selectProps: React.SelectHTMLAttributes<HTMLSelectElement> =
			Object.fromEntries(
				Object.entries(rest).filter(([key]) =>
					key in document.createElement("select")
				)
			) as React.SelectHTMLAttributes<HTMLSelectElement>;

		var componentClasses = [`${COMPONENT_PREFIX}-select`];

		const sizes = { xs, sm, md, lg, xl };
		Object.entries(sizes).forEach(([key, value]) => {
			if (value) {
				componentClasses.push(`${COMPONENT_PREFIX}--${key}`);
			}
		});

		if (outlined) {
			componentClasses.push(`${COMPONENT_PREFIX}--outlined`);
		}

		if (className) {
			componentClasses.push(className);
		}

		let helpId = help && help.length ? useId() : undefined;

		return <>
			{label && <>
				<label htmlFor={_id}>
					{label}
				</label>
			</>}
			<select id={_id} className={componentClasses.join(' ')} value={value} onChange={(e) => {
				if (onChange instanceof Function) {
					onChange(e);
				}
			}} aria-describedby={helpId} {...selectProps}>
				{children}
			</select>
			{helpId && <>
				<p className={`${COMPONENT_PREFIX}-help`} id={helpId}>
					{help}
				</p>
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

export default Select;