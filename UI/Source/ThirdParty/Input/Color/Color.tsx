import Default, { DefaultInputType } from 'UI/Input/Default';
import { useState } from 'react';

type ColorInputType = DefaultInputType & {
	allowTransparency?: boolean
};

// Registering 'color' as being available
declare global {
	interface InputPropsRegistry {
		'color': ColorInputType;
	}
}

const Color: React.FC<CustomInputTypeProps<"color">> = (props) => {

	const [renderCount, setRenderCount] = useState<number>(0);
	const { field, icon, helpFieldId, onInputRef, inputRef, validationFailure } = props;
	const { allowTransparency, onChange, className, ...attribs } = field;
	var input = inputRef as HTMLInputElement;

	// Hack to allow alpha values in color input
	const onTransparentChange = (e: React.FormEvent<HTMLInputElement>) => {
		if (input && e && e.target) {
			var target = e.target as HTMLInputElement;

			if (target.checked) {
				if (input.value && input.value.length == 7) {
					input.value += "00";
				}
			} else {
				if (input.value && input.value.length == 9) {
					input.value = input.value.slice(0, -2);
				}
			}

			setRenderCount(renderCount+1);
		}
	}

	const colorValue = input && input.value
		? input.value
		: field.defaultValue;
	const isTransparent = colorValue ? ((colorValue as string).length == 9) : false;

	let fieldMarkup: React.ReactNode;

	if (isTransparent) {
		fieldMarkup = (
			<input
				ref={(el: HTMLInputElement) => onInputRef && onInputRef(el)}
				className={(className || "form-control") + (validationFailure ? ' is-invalid' : '')}
				aria-describedby={helpFieldId}
				type="text"
				onInput={onChange}
				style={{ display: "none" }}
				{...attribs}
			/>
		);
	} else {
		fieldMarkup = <Default type="color" config={props} field={field} />;
	}

	return <div className="input-wrapper color-input">
		{fieldMarkup}
		{(allowTransparency || isTransparent) &&
			<>
				<label className="transparent-label">Transparent</label>
				<input
					type="checkbox"
					name="isTransparent"
					checked={isTransparent}
					onInput={onTransparentChange}
				/>
			</>
		}
		{icon}
	</div>;
}

window.inputTypes['color'] = Color;
export default Color;