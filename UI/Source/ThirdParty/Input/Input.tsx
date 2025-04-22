import Default from 'UI/Input/Default';
import { useState, useEffect } from 'react';

var gId = 1;
var inputTypes = window.inputTypes;

type InputProps<T extends keyof InputPropsRegistry> = InputPropsRegistry[T] & {
	type: T;
	onValidationFailure?: (er: PublicError) => React.ReactNode;
	noWrapper?: boolean;
	validate?: string[];
	groupClassName?: string;
	labelPosition?: 'above' | 'below';
	validateErrorLocation?: 'above' | 'below';
	helpPosition?: 'above' | 'below';
	contentType?: string;
	inline?: boolean;
	autoFocus?: boolean;
	hideRequiredStar?: boolean;
	label?: React.ReactNode;
	help?: React.ReactNode;
	icon?: React.ReactNode;
	onInputRef?: (el: HTMLElement) => void;
	onBlur?: (e: React.FocusEvent) => void;
	onChange?: (e: React.ChangeEvent) => void;
};

/**
 * Helps eliminate a significant amount of boilerplate around <input>, <textarea> and <select> elements.
 * Note that you can still use them directly if you want.
 * You can also use validate={[..]} and provide either a module name from the 
 * UI/Functions/Validation set of modules or a function which receives the value to validate and returns nothing or an error object.
 * See e.g. UI/Functions/Validation/Required/Required.tsx for the structure of a custom method.
 */
const Input = <T extends keyof InputPropsRegistry>(props: InputProps<T>) => {

	let {
		type,
		onValidationFailure,
		noWrapper,
		validate,
		groupClassName,
		labelPosition,
		validateErrorLocation,
		contentType,
		helpPosition,
		inline,
		autoFocus,
		hideRequiredStar,
		label,
		help,
		icon,
		onBlur,
		onChange,
		onInputRef,
		...customAttribs // InputProps
	} = props;

	let field = customAttribs as InputPropsRegistry[T];

	let {
		id
	} = customAttribs;

	const [fieldId] = useState(() => 'form-field-' + (gId++));
	const [validationFailure, setValidationFailure] = useState<PublicError | null>(null);
	const [inputRef, setInputRef] = useState<HTMLElement | null>(null);

	if (!id) {
		id = fieldId;
	}

	const helpFieldId = id + "-help";
	
	useEffect(() => {
		if (autoFocus && inputRef) {
			inputRef.focus();
		}
	}, []);

	if (!labelPosition) {
		labelPosition = 'above';
	}
	
	if (!helpPosition) {
		helpPosition = 'above';
	}

	if (!validateErrorLocation) {
		validateErrorLocation = 'below';
	}

	const renderLabel = (pos: string) => {
		if (labelPosition != pos) {
			// Don't render here.
			return null;
		}

		return <label htmlFor={id} className="form-label">
			{label}
			{!hideRequiredStar && validate && validate.indexOf("Required")!=-1 && <span className="is-required-field"></span>}
		</label>;
	}

	const renderHelp = (pos: string) => {
		if (!help || helpPosition != pos) {
			// Don't render here.
			return null;
		}

		return <div id={helpFieldId} className={"form-text form-text-" + pos}>
			{help}
		</div>;
	}

	const renderMessages = (pos: string) => {
		if (!validationFailure || validateErrorLocation != pos) {
			// Don't render here.
			return null;
		}

		return <div className="validation-error">
			{onValidationFailure ? onValidationFailure(validationFailure) : validationFailure.message}
		</div>;
	}

	const updateValidation = () : PublicError | null => {
		var invalid = validationError();
		setValidationFailure(invalid);
		return invalid;
	}

	const revalidate = () => {
		updateValidation();
	}

	const onChangeWithVal = (e : React.ChangeEvent) => {
		onChange && onChange(e);
		if (e.defaultPrevented) {
			return;
		}

		// Validation check
		revalidate();
	}

	const onBlurWithVal = (e : React.FocusEvent) => {
		onBlur && onBlur(e);
		if (e.defaultPrevented) {
			return;
		}

		// Validation check
		revalidate();
	}

	const validationError = () : PublicError | null => {
		var validations = validate;

		if (!validations) {
			return null;
		}
	
		if (!Array.isArray(validations)) {
			// Make it one:
			validations = [validations];
		}
		
		var field = inputRef as HTMLInputElement; // (can also be a textarea or a select)
		if(!field){
			return null;
		}
		
		var v = field.type == 'checkbox' ? field.checked : field.value;
		var vFail : PublicError | null = null;

		for (var i = 0; i < validations.length; i++) {
			// If it's a string, include the module.
			// Otherwise it's assumed to be a function that we directly run.
			var valType = validations[i];

			if (!valType) {
				continue;
			}

			switch (typeof valType) {
				case "string":
					var mtd = require("UI/Functions/Validation/" + valType).default;
					vFail = mtd(v) as PublicError | null;
					break;
				default:
					console.log("Invalid validation type: ", validations, valType, i);
					break;
			}
			
			if (vFail) {
				return vFail;
			}
		}
		
		return null;
	}

	const setRef = (ref: HTMLElement) => {
		setInputRef(ref);
		onInputRef && onInputRef(ref);
		
		if(ref){
			(ref as any).onValidationCheck = updateValidation;
		}
	}

	const renderField = () => {
		return <>
			{renderLabel('above')}
			{renderHelp('above')}
			{renderMessages('above')}
			{renderInput()}
			{renderLabel('below')}
			{renderHelp('below')}
			{renderMessages('below')}
		</>;
	}

	const renderInput = () => {
		var Handler = inputTypes[type] as React.FC<CustomInputTypeProps<T>>;

		if (!Handler) {
			Handler = inputTypes['text'] as React.FC<CustomInputTypeProps<T>>;
		}

		field.onChange = onChangeWithVal;
		field.onBlur = onBlurWithVal;

		return <Handler
			validationFailure={validationFailure}
			helpFieldId={helpFieldId}
			label={label}
			help={help}
			icon={icon}
			field={field}
			inputRef={inputRef}
			onInputRef={setRef}
			onChange={props.onChange}
		/>;
	}

	if (inline) {
		return renderInput();
	}

	if (noWrapper) {
		return renderField();
	}

	var groupClass = groupClassName ? "mb-3 " + groupClassName : "mb-3";

	if (labelPosition == 'below') {
		groupClass = 'form-floating ' + groupClass;
	}

	return (
		<div className={groupClass}>
			{renderField()}
		</div>
	);
}

export default Input;