const COMPONENT_PREFIX = 'ui-dialog';

import Button from 'UI/Button';

export default function Header(props) {
	const {
		children,
		title, headingLevel,
		noClose,
		closeCallback, cancelCallback,
		className
	} = props;

	let hasTitle = title && title.length;
	let defaultLevel = hasTitle ? 2 : 0;
	let _headingLevel = headingLevel ? parseInt(headingLevel, 10) : defaultLevel;
	var HeadingTag = `h${_headingLevel}`;

	let componentClasses = [`${COMPONENT_PREFIX}__header`];

	if (className) {
		componentClasses.push(className);
	}

	return (
		<header className={componentClasses.join(' ')}>
			{children}
			{!children && hasTitle && <>
				<HeadingTag className={`${COMPONENT_PREFIX}__title`}>
					{title}
				</HeadingTag>
			</>}
			{!noClose && <>
				<Button close onClick={(e) => {
					if (closeCallback instanceof Function) {
						closeCallback(e);
					}

					if (cancelCallback instanceof Function) {
						cancelCallback(e);
					}
				}} />
			</>}
		</header>
	);
}

Header.propTypes = {
};

Header.defaultProps = {
}
