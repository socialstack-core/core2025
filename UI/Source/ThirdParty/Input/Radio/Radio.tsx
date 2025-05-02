import { DefaultInputType } from 'UI/Input/Default';
import { useId } from 'react';

type RadioInputType = DefaultInputType & {
	solid?: boolean,
	value?: boolean,
	defaultValue?: boolean
}

// Registering 'radio' as being available
declare global {
	interface InputPropsRegistry {
		'radio': RadioInputType;
	}
}

const Radio: React.FC<CustomInputTypeProps<"radio">> = (props) => {
	const { field, validationFailure, label, onInputRef, helpFieldId } = props;
	const { className, solid, onChange, ...attribs } = field;
	const id = attribs.id || useId();

	const classes = className ? className.split(" ") : [];

	if (validationFailure) {
		classes.push("is-invalid");
	}

	classes.unshift("form-check");

	// NB: don't auto-apply a bottom margin as radio buttons will be rendered in groups and should remain in close proximity
	// TODO: apply bottom margin to wrapping radio-group element
	/*
	// ensure we include a standard bottom margin if one hasn't been supplied
	// (only apply this on the last item in the group)
	if (!className.find(element => element.startsWith("mb-"))) {
		className.push("mb-3");
	}
	*/

	var radioClass = classes.join(" ");
	var inputClass = "form-check-input" + (solid ? " form-check-input--solid" : "");

	const value = (!!field.value) || field.checked;
	const defaultValue = (!!field.defaultValue) || field.defaultChecked;

	return (
		<div className={attribs.readOnly ? '' : radioClass}>
			{attribs.readOnly ? (
				(value === undefined ? defaultValue : value) ? <b>Yes (readonly) </b> : <b>No (readonly) </b>
			) : 
				<input
					ref={(el: HTMLInputElement) => onInputRef && onInputRef(el)}
					className={inputClass}
					aria-describedby={helpFieldId}
					type="radio"
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

window.inputTypes['radio'] = Radio;
export default Radio;

