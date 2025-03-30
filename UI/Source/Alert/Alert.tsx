import { useState } from 'react';
import Button from 'UI/Button';
import Icon from 'UI/Icon';

const COMPONENT_PREFIX = 'alert';
const DEFAULT_VARIANT = 'info';

export type AlertType = 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info';

interface AlertProps {
	/**
	 * determines appearance (info/ error/ primary / secondary etc.)
	 */
	variant?: AlertType,
	/**
	 * optional title
	 */
	title?: string,
	/**
	 * HTML tag to use for title (defaults to <strong>)
	 */
	titleTag?: HTMLElement,
	/**
	 * set true to hide icon
	 */
	hideIcon?: boolean,
	/**
	 * Optionally provide a custom Icon instance.
	 */
	customIcon?: React.ReactElement,
	/**
	 * set true to display close button
	 */
	dismissable?: boolean,
	/**
	 * The alert type
	 */
	type?: string,
	/**
	 * render at extra small size
	 */
	xs?: boolean,
	/**
	 * render at small size
	 */
	sm?: boolean,
	/**
	 * render at medium size (default)
	 */
	md?: boolean,
	/**
	 * render at large size
	 */
	lg?: boolean,
	/**
	 * render at extra large size
	 */
	xl?: boolean,
	/**
	 * optional additional classes
	 */
	className?: string,
}

/**
 * Alert component
 */
const Alert: React.FC<React.PropsWithChildren<AlertProps>> = ({ variant, title, titleTag, hideIcon, customIcon, dismissable, isDismissable, type, xs, sm, md, lg, xl, children }) => {
	const [showAlert, setShowAlert] = useState(true);
	let TitleTag = titleTag || 'strong';
	let alertVariant = variant?.toLowerCase() || DEFAULT_VARIANT;
	let icon: React.ReactNode = undefined;

	/****************
	 * NB: new markup currently injects a suitable icon via CSS;
	 *     needs revisiting for custom icon support
	 ****************/

	// resolve default icon class
	switch (alertVariant) {
		//case 'primary':
		//break;

		//case 'secondary':
		//break;

		case 'success':
			icon = <Icon type="fa-check-circle" light />;
			break;

		case 'danger':
			icon = <Icon type="fa-times-circle" light />;
			break;

		case 'warning':
			icon = <Icon type="fa-exclamation-triangle" light />;
			break;

		case 'info':
			icon = <Icon type="fa-info-circle" light />;
			break;

		//case 'light':
		//break;

		//case 'dark':
		//break;
	}

	if (customIcon) {
		icon = customIcon;
	}

	var componentClasses = [COMPONENT_PREFIX];
	componentClasses.push(`${COMPONENT_PREFIX}--${alertVariant}`);

	let _dismissable = dismissable || isDismissable;

	if (_dismissable) {
		componentClasses.push(`${COMPONENT_PREFIX}--dismissable`);
	}

	if (hideIcon) {
		componentClasses.push(`${COMPONENT_PREFIX}--no-icon`);
	}

	if (!showAlert) {
		return;
	}

	return (
		<div className={componentClasses.join(' ')} role="alert">
			<div className={`${COMPONENT_PREFIX}__notch`}></div>
			{_dismissable && <>
				<Button close xs className={`${COMPONENT_PREFIX}__close`} onClick={() => setShowAlert(false)} />
			</>}
			{title && <>
				<TitleTag className={`${COMPONENT_PREFIX}__title`}>
					{title}
				</TitleTag>
			</>}
			{children}
		</div>
	);
}

export default Alert;