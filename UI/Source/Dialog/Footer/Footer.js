const COMPONENT_PREFIX = 'dialog';

import Button from 'UI/Button';

export default function Footer(props) {
	const {
		children,
		closeCallback,
		confirmProps, //confirmLabel, confirmCallback,
		cancelProps, //cancelLabel, cancelCallback,
		className
	} = props;

	let componentClasses = [`${COMPONENT_PREFIX}__footer`];

	if (className) {
		componentClasses.push(className);
	}

	let footerClasses = componentClasses.join(' ');

	if (children) {
		return (
			<footer className={footerClasses}>
				{children}
			</footer>
		);
	}

	//let hasConfirm = confirmLabel && confirmLabel.length;
	//let hasCancel = cancelLabel && cancelLabel.length;
	let hasConfirm = confirmProps?.label;
	let hasCancel = cancelProps?.label;

	if (!hasConfirm && !hasCancel) {
		return;
	}

	return (
		<footer className={footerClasses}>
			{hasCancel && <>
				<Button {...cancelProps} onClick={(e) => {
					if (closeCallback instanceof Function) {
						closeCallback(e);
					}
					if (cancelProps?.callback instanceof Function) {
						cancelProps.callback(e);
					}
				}}>
					{cancelProps.label}
				</Button>
			</>}
			{hasConfirm && <>
				<Button {...confirmProps} onClick={(e) => {
					if (closeCallback instanceof Function) {
						closeCallback(e);
					}
					if (confirmProps?.callback instanceof Function) {
						confirmProps.callback(e);
					}
				}}>
					{confirmProps.label}
				</Button>
			</>}
		</footer>
	);

}

Footer.propTypes = {
};

Footer.defaultProps = {
}
