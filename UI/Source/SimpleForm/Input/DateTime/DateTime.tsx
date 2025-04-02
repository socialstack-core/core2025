import { useId } from 'react';
import Field from '../../Field';
import { getSizeClasses } from 'UI/Functions/Components';

const COMPONENT_PREFIX = 'form__field';

/**
 * Props for the DateTime component.
 */
interface DateTimeProps extends React.InputHTMLAttributes<HTMLInputElement> {
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
 * The DateTime React component.
 * @param props DateTime props.
 */
const DateTime: React.FC<DateTimeProps> = ({ label, id, className, xs, sm, md, lg, xl, wrapperClass, noWrapper, ...rest }) => {

	function renderField() {
		const _id = id || useId();

		// filter out only valid datetime attributes
		let dateTimeProps: React.InputHTMLAttributes<HTMLInputElement> =
			Object.fromEntries(
				Object.entries(rest).filter(([key]) =>
					key in document.createElement("input")
				)
			) as React.InputHTMLAttributes<HTMLInputElement>;

		// NB: ensure we've not been sent the outdated "datetime" type
		if (dateTimeProps.type == "datetime") {
			dateTimeProps.type = "datetime-local";
		}

		var fieldDateTime = `${COMPONENT_PREFIX}-datetime`;
		var componentClasses = [fieldDateTime];
		componentClasses = componentClasses.concat(getSizeClasses(fieldDateTime, { xs, sm, md, lg, xl }));

		if (className) {
			componentClasses.push(className);
		}

		return <>
			{label && <>
				<label htmlFor={_id}>
					{label}
				</label>
			</>}
			<input data-test="FOOOO" type="datetime-local" id={_id} className={componentClasses.join(' ')} {...dateTimeProps} />
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

export default DateTime;