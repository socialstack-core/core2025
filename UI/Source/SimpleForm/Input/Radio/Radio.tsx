import { useId } from 'react';
import Field from 'UI/SimpleForm/Field';
import { getSizeClasses } from 'UI/Functions/Components';

const COMPONENT_PREFIX = 'form__field';

/**
 * Props for the Radio component.
 */
interface RadioProps extends React.InputHTMLAttributes<HTMLInputElement> {
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
	noWrapper?: boolean
}

/**
 * The Radio React component.
 * @param props Radio props.
 */
const Radio: React.FC<RadioProps> = ({ label, id, className, xs, sm, md, lg, xl, wrapperClass, noWrapper, value, onChange, ...rest }) => {

	function renderField() {
		const _id = id || useId();

		// filter out only valid radio attributes
		const radioProps: React.InputHTMLAttributes<HTMLInputElement> =
			Object.fromEntries(
				Object.entries(rest).filter(([key]) =>
					key in document.createElement("input")
				)
			) as React.InputHTMLAttributes<HTMLInputElement>;

		var fieldRadio = `${COMPONENT_PREFIX}-radio`;
		var componentClasses = [fieldRadio, 'visually-hidden'];
		componentClasses = componentClasses.concat(getSizeClasses(fieldRadio, { xs: Boolean(xs), sm: Boolean(sm), md: Boolean(md), lg: Boolean(lg), xl: Boolean(xl) }));

		if (className) {
			componentClasses.push(className);
		}

		return <>
			<input type="radio" id={_id} className={componentClasses.join(' ')} value={value} onChange={(e) => {
				if (onChange instanceof Function) {
					onChange(e);
				}
			}} {...radioProps} />
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

export default Radio;