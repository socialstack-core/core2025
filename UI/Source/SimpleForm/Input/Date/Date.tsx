import { useId } from 'react';
import Field from 'UI/SimpleForm/Field';
import { getSizeClasses } from 'UI/Functions/Components';

const COMPONENT_PREFIX = 'form__field';

/**
 * Props for the Date component.
 */
interface DateProps extends React.InputHTMLAttributes<HTMLInputElement> {
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
	xl?: boolean;
	/**
	 * additional classes to be applied to wrapping element
	 */
	wrapperClass?: string;
	/**
	 * disable wrapping element
	 */
	noWrapper?: boolean;
}

/**
 * The Date React component.
 * @param props Date props.
 */
const Date: React.FC<DateProps > = ({ label, id, className, xs, sm, md, lg, xl, wrapperClass, noWrapper, ...rest }) => {

	function renderField() {
		const _id = id || useId();

		// filter out only valid date attributes
		const dateProps: React.InputHTMLAttributes<HTMLInputElement> =
			Object.fromEntries(
				Object.entries(rest).filter(([key]) =>
					key in document.createElement("input")
				)
			) as React.InputHTMLAttributes<HTMLInputElement>;

		var fieldDate = `${COMPONENT_PREFIX}-date`;
		var componentClasses = [fieldDate];
		componentClasses = componentClasses.concat(getSizeClasses(fieldDate, { xs: Boolean(xs), sm: Boolean(sm), md: Boolean(md), lg: Boolean(lg), xl: Boolean(xl) }));

		if (className) {
			componentClasses.push(className);
		}

		return <>
			{label && <>
				<label htmlFor={_id}>
					{label}
				</label>
			</>}
			<input type="date" id={_id} className={componentClasses.join(' ')} {...dateProps} />
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

export default Date;