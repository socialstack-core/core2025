import { useId } from 'react';
import Field from '../Field';
import { getSizeClasses } from 'UI/Functions/Components';

const COMPONENT_PREFIX = 'form__field';

/**
 * Props for the Textarea component.
 */
interface TextareaProps extends React.TextAreaHTMLAttributes<HTMLTextAreaElement> {
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
 * The Textarea React component.
 * @param props Textarea props.
 */
const Textarea: React.FC<TextareaProps > = ({ label, id, className, xs, sm, md, lg, xl, wrapperClass, noWrapper, ...rest }) => {

	function renderField() {
		const _id = id || useId();

		// filter out only valid textarea attributes
		const textProps: React.TextAreaHTMLAttributes<HTMLTextAreaElement> =
			Object.fromEntries(
				Object.entries(rest).filter(([key]) =>
					key in document.createElement("input")
				)
			) as React.TextAreaHTMLAttributes<HTMLTextAreaElement>;

		var fieldTextarea = `${COMPONENT_PREFIX}-textarea`;
		var componentClasses = [fieldTextarea];
		componentClasses = componentClasses.concat(getSizeClasses(fieldTextarea, { xs, sm, md, lg, xl }));

		if (className) {
			componentClasses.push(className);
		}

		return <>
			{label && <>
				<label htmlFor={_id}>
					{label}
				</label>
			</>}
			<textarea id={_id} className={componentClasses.join(' ')} {...textProps} />
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

export default Textarea;