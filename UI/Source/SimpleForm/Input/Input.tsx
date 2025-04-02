import Button from 'UI/Button';
import Checkbox from './Checkbox';
import Color from './Color';
import Date from './Date';
import DateTime from './DateTime';
import Email from './Email';
import File from './File';
import Month from './Month';
import Number from './Number';
import Password from './Password';
import Radio from './Radio';
import Range from './Range';
import Search from './Search';
import Tel from './Tel';
import Text from './Text';
import Time from './Time';
import Url from './Url';
import Week from './Week';

/**
 * Props for the Input component.
 */
interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
	/**
	 * associated label
	 */
	label?: string,
	/**
	 * input type
	 */
	type?: string,
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
	help?: string,
}

/**
 * The Input React component.
 * @param props Input props.
 */
const Input: React.FC<InputProps> = (props) => {

	switch (props.type) {
		case 'button':
		case 'btn':
			return <>
				<Button type="button" {...props}>
					{props.value || props.label}
				</Button>
			</>;

		case 'check':
		case 'checkbox':
			return <Checkbox {...props} />;

		case 'color':
		case 'colour':
			return <Color {...props} />;

		case 'date':
			return <Date {...props} />;

		case 'datetime':
		case 'datetime-local':
			return <DateTime {...props} />;

		case 'email':
			return <Email {...props} />;

		case 'file':
			return <File {...props} />;

		//case 'hidden':
		//	return;

		//case 'image':
		//	return;

		case 'month':
			return <Month {...props} />;

		case 'number':
			return <Number {...props} />;

		case 'password':
			return <Password {...props} />;

		case 'radio':
			return <Radio {...props} />;

		case 'range':
			return <Range {...props} />;

		case 'reset':
			return <>
				<Button type="reset" {...props}>
					{props.value || props.label}
				</Button>
			</>;

		case 'search':
			return <Search {...props} />;

		case 'submit':
			return <>
				<Button type="submit" {...props}>
					{props.value || props.label}
				</Button>
			</>;

		case 'tel':
		case 'telephone':
		case 'phone':
		case 'mobile':
			return <Tel {...props} />;

		case 'time':
			return <Time {...props} />;

		case 'url':
			return <Url {...props} />;

		case 'week':
			return <Week {...props} />;

		default:
			return <Text {...props} />;
	}

};

export default Input;