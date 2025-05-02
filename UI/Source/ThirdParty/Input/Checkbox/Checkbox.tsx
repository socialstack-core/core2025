import { DefaultInputType } from 'UI/Input/Default';
import { useId } from 'react';

type CheckboxInputType = DefaultInputType & {
	isSwitch?: boolean,
	solid?: boolean,
	value?: boolean,
	defaultValue?: boolean
};

// Registering 'checkbox' as being available
declare global {
	interface InputPropsRegistry {
		'checkbox': CheckboxInputType;
		'bool': CheckboxInputType;
		'boolean': CheckboxInputType;
	}
}

interface CheckboxProps {
	field: CheckboxInputType,
	config: CustomInputTypePropsBase
}

const Checkbox: React.FC<CheckboxProps> = (props) => {
	const { field, config } = props;
	const { label, validationFailure, onInputRef, helpFieldId } = config;
	let { isSwitch, solid, className, onChange, style, ...attribs } = field;
	const id = attribs.id || useId();
	
	const classes = className ? className.split(" ") : [];

	if (isSwitch) {
		classes.unshift("form-switch");
	}

	if (validationFailure) {
		classes.push("is-invalid");
	}

	classes.unshift("form-check");

	/* wrapped by UI/Input
	// ensure we include a standard bottom margin if one hasn't been supplied
	if (!classes.find(element => element.startsWith("mb-"))) {
		classes.push("mb-3");
	}
	*/

	var checkClass = classes.join(" ");
	var inputClass = "form-check-input" + (solid ? " form-check-input--solid" : "");

	const value = (!!field.value) || field.checked;
	const defaultValue = (!!field.defaultValue) || field.defaultChecked;

	return (
		<div className={field.readOnly ? '' : checkClass} style={style}>
			{field.readOnly ? (
				(value === undefined ? defaultValue : value) ? <b>{`Yes (readonly) `}</b> : <b>{`No (readonly) `}</b>
			) : <input
					ref={(el) => onInputRef && onInputRef(el as HTMLElement)}
					className={inputClass}
					aria-describedby={helpFieldId}
					type="checkbox"
					onInput={onChange}
					{...attribs}
					id={id}
					checked={value}
					defaultChecked={defaultValue}
			/>}
			<label className="form-check-label" htmlFor={id}>
				{label}
			</label>
		</div>
	);
}

window.inputTypes['checkbox'] = (props: CustomInputTypeProps<"checkbox">) => <Checkbox field={props.field} config={props} />;
window.inputTypes['bool'] = (props: CustomInputTypeProps<"bool">) => <Checkbox field={props.field} config={props} />;
window.inputTypes['boolean'] = (props: CustomInputTypeProps<"boolean">) => <Checkbox field={props.field} config={props} />;