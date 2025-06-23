/**
 * Props for the button component.
 */
interface ButtonProps extends React.HTMLAttributes<HTMLElement>  {

	/**
	 * True if the button is disabled.
	 */
	disabled?: boolean,

	/**
	 * The type of tag to use on the button itself.
	 */
	tag?: React.ElementType,

	/**
	 * optional additional class name(s)
	 */
	className?: string,

	/**
	 * The button type.
	 */
	buttonType?: 'button' | 'reset' | 'submit',

	/**
	 * Optional href if it is a link
	 */
	href?: string,

	/**
	 * True if the button should be the extra small style.
	 */
	xs?: boolean,

	/**
	 * True if the button should be the small style.
	 */
	sm?: boolean,

	/**
	 * True if the button should be the regular style.
	 */
	md?: boolean,

	/**
	 * True if the button should be the large style.
	 */
	lg?: boolean,

	/**
	 * True if the button should be the extra large style.
	 */
	xl?: boolean,

	/**
	 * True if the button should wrap text inside it.
	 */
	allowWrap?: boolean,

	/**
	 * True if the button should be the outlined style.
	 */
	outlined?: boolean,

	/**
	 * The style variant, "primary", "secondary" etc. "primary" assumed.
	 */
	variant?: string
}

/**
 * Button component
 * ref: https://getbootstrap.com/docs/5.0/components/buttons/
 */
const Button: React.FC<React.PropsWithChildren<ButtonProps>> = ({
	children, className, tag, buttonType, href, variant,
	disabled, outlined, allowWrap, xs, sm, md, lg, xl, ...props
}) => {
	var classes = className ? className.split(" ") : [];

	var Tag = tag ? tag : "button";

	switch (Tag) {
		case 'button':
		case 'a':
		case 'input':
			break;

		default:
			Tag = "button";
			break;
	}

	if (href) {
		Tag = "a";
	}

	if (!buttonType) {
		buttonType = "button";
	}

	switch (buttonType) {
		case 'button':
		case 'submit':
		case 'reset':
			break;

		default:
			buttonType = "button";
			break;
	}

	if (!variant) {
		variant = "primary";
	}

	if (allowWrap) {
		classes.unshift("btn--wrapped");
	}

	// sizing
	if (xs) {
		classes.unshift("btn-xs");
	}

	if (sm) {
		classes.unshift("btn-sm");
	}

	if (md) {
		classes.unshift("btn-md");
	}

	if (lg) {
		classes.unshift("btn-lg");
	}

	if (xl) {
		classes.unshift("btn-xl");
	}

	classes.unshift("btn-" + (outlined ? "outline-" : "") + variant);
	classes.unshift("ui-btn");
	classes.unshift("btn");

	var btnClass = classes.join(" ");

	return (
		<Tag className={btnClass}
			disabled={Tag != "a" && disabled ? true : undefined}
			inert={Tag == "a" && disabled ? true : undefined}
			aria-disabled={disabled ? "true" : undefined}
			tabIndex={disabled ? -1 : undefined}
			href={Tag == "a" ? href : undefined}
			type={Tag == "a" ? undefined : buttonType}
			role={Tag == "a" ? "button" : undefined}
			{...props}
		>
			{children}
		</Tag>
	);
}

export default Button;
