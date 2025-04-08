import Checkbox from 'UI/SimpleForm/Input/Checkbox';

/**
 * Props for the Checkbox component.
 */
interface SwitchProps extends React.InputHTMLAttributes<HTMLInputElement> {
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
 * The Switch React component.
 * @param props Switch props.
 */
const Switch: React.FC<SwitchProps> = (props) => {
	return <Checkbox switch {...props} />;
};

export default Switch;