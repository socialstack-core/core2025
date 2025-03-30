import { useEffect, useRef, useId } from 'react';
import Field from '../../Field';
import { getSizeClasses, getValidProps } from 'UI/Functions/Components';

const COMPONENT_PREFIX = 'form__field';
const DEFAULT_COLOR = '#000';

/**
 * Props for the Color component.
 */
interface ColorProps extends React.InputHTMLAttributes<HTMLInputElement> {
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
	 * related help / info message to appear with field
	 */
	help?: string
}

/**
 * The Color React component.
 * @param props Color props.
 */
const Color: React.FC<ColorProps > = ({ label, id, className, xs, sm, md, lg, xl, wrapperClass, noWrapper, value, onChange, onInput, help, ...rest }) => {
	const fieldRef = useRef();

	useEffect(() => {

		/*
		const handleColorChange = (e) => {
			fieldRef.current.style.setProperty("--contrast-text", getContrastingColor(e.target.value));
		};
		*/

		if (fieldRef?.current) {
			fieldRef.current.dataset.value = value || DEFAULT_COLOR;
			fieldRef.current.style.setProperty("--contrast-text", getContrastingColor(value || DEFAULT_COLOR));
			//fieldRef.current.addEventListener('input', handleColorChange);
		}

		return () => {
			/*
			if (fieldRef?.current) {
				fieldRef.current.removeEventListener('input', handleColorChange);
			}
			*/
		};

	});

	function getContrastingColor(hex) {

		if (!hex || !hex.length) {
			return '#fff';
		}

		// Remove the '#' if present
		hex = hex.replace('#', '');

		// Convert to RGB components
		let r = parseInt(hex.substring(0, 2), 16);
		let g = parseInt(hex.substring(2, 4), 16);
		let b = parseInt(hex.substring(4, 6), 16);

		if (isNaN(r) || isNaN(g) || isNaN(b)) {
			return '#fff';
		}

		// Calculate relative luminance (per W3C formula)
		let luminance = (0.2126 * r + 0.7152 * g + 0.0722 * b);

		// Return black or white depending on luminance
		return luminance > 128 ? '#000' : '#fff';
	}

	function renderField() {
		const _id = id || useId();

		// TODO: check getValidProps

		// filter out only valid color attributes
		const colorProps: React.InputHTMLAttributes<HTMLInputElement> =
			Object.fromEntries(
				Object.entries(rest).filter(([key]) =>
					key in document.createElement("input")
				)
			) as React.InputHTMLAttributes<HTMLInputElement>;

		var fieldColor = `${COMPONENT_PREFIX}-color`;
		var componentClasses = [fieldColor];
		componentClasses = componentClasses.concat(getSizeClasses(fieldColor, { xs, sm, md, lg, xl }));

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
			<input type="color" ref={fieldRef} id={_id} className={componentClasses.join(' ')} xdata-value={value} value={value} onChange={(e) => {
				if (onChange instanceof Function) {
					onChange(e);
				}
			}} onInput={(e) => {

				if (fieldRef?.current) {
					fieldRef.current.dataset.value = e.target.value;
					fieldRef.current.style.setProperty("--contrast-text", getContrastingColor(e.target.value));
					//fieldRef.current.addEventListener('input', handleColorChange);
				}

				if (onInput instanceof Function) {
					onInput(e);
				}
				}} aria-describedby={helpId} {...colorProps} />
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

export default Color;