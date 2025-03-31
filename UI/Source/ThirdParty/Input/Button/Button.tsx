type ButtonInputType = React.ButtonHTMLAttributes<HTMLButtonElement>;

// Registering 'button' as being available
declare global {
	interface InputPropsRegistry {
		'button': ButtonInputType;
		'submit': ButtonInputType;
		'reset': ButtonInputType;
	}
}

type ButtonProps = {
	type: 'button' | 'submit' | 'reset',
	field: ButtonInputType,
	config: CustomInputTypePropsBase
};

const Button: React.FC<ButtonProps> = (props) => {
	const { config, field, type } = props;
	const { icon, label } = config;
	const { className, children, ...attribs } = field;

	return (
		<button
			type={type}
			className={className || "btn btn-primary"}
			{...attribs}
		>
			{
				icon
			}
			{label || children || "Submit"}
		</button>
	);
	
}

export default Button;
window.inputTypes['button'] = (props: CustomInputTypeProps<"button">) => <Button type="button" field={props.field} config={props} />;
window.inputTypes['submit'] = (props: CustomInputTypeProps<"submit">) => <Button type="submit" field={props.field} config={props} />;
window.inputTypes['reset'] = (props: CustomInputTypeProps<"reset">) => <Button type="reset" field={props.field} config={props} />;