import { parse } from 'UI/FileRef';

interface IconBaseProps {
	/**
	 * Use the light variation.
	 */
	light?: boolean,

	/**
	 * Use the solid variation.
	 */
	solid?: boolean,

	/**
	 * Use the duotone variation.
	 */
	duotone?: boolean,

	/**
	 * Use the regular variation.
	 */
	regular?: boolean,

	/**
	 * Use the brand variation.
	 */
	brand?: boolean,

	/**
	 * True if this icon should be a fixed width.
	 */
	fixedWidth?: boolean,

	/**
	 * True if the icon should spin.
	 */
	spin?: boolean,

	/**
	 * Flip horizontally
	 */
	horizontalFlip?: boolean,

	/**
	 * Character, used internally by the compiler.
	 */
	c?:string
}

/**
 * Props for the Icon component.
 */
type IconProps = IconBaseProps & {
	/**
	 * You MUST use a constant string. UI/Icon is a first-class component in the compile process.
	 * The compiler will use "type" to identify in-use icons and strip accordingly.
	 * This is the type of icon you want to display, such as type="fa-bullhorn". 
	 * Don't target this type with CSS: instead, target ui-icon if you need to do so.
	 */
	type: string,
}

const Icon: React.FC<IconProps> = (props) => {
	const { type, light, solid, duotone, regular, brand, fixedWidth, spin, horizontalFlip, c } = props;
	
	var variant = 'fa';
	
	if(light){
		variant = 'fal';
	}else if(duotone){
		variant = 'fad';
	}else if(brand){
		variant = 'fab';
	}else if(regular){
		variant = 'far';
	}else if(solid){
		variant = 'fas';
	}
	
	var className = variant + " ui-icon";

	if (!c) {
		// May need to reconsider this if people specifically target an icon using these class names.
		// They should generally target ui-icon instead though.
		// It exists such that static icons, which use a tiny highly accelerated subset of icons, 
		// don't end up being given 2 characters when both the accelerated set and the 
		// entire set are present (like on the admin panel).
		className += " " + type;
	}

	if (fixedWidth) {
		className += " fa-fw";
    }

	if (spin) {
		className += " fa-spin";
	}

	if (horizontalFlip) {
		className += " fa-flip-horizontal";
	}

	// NB: count removal: if you need it, use a wrapping component instead
	// due to the special case that Icon is to the compiler.

	return <i className={className}>{c}</i>;
}

export default Icon;

type IconRefProps = IconBaseProps & {
	fileRef: FileRef
};

export const IconRef : React.FC<IconRefProps> = (props) => {
	const { fileRef, ...iconProps } = props;

	// future version where the inline icons are actually stripped -
	// this one would not do so and instead refs the main font files with all.
	var fullRef = parse(fileRef);

	return <Icon {...iconProps} type={fullRef?.basepath || ''} />;
};