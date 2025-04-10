const COMPONENT_PREFIX = 'ui-dialog';
//const DEFAULT_VARIANT = 'primary';

import { useEffect, useRef } from 'react';
import Header from './Header';
import Body from './Body';
import Footer from './Footer';
// import { getSizeClasses } from 'UI/Functions/Components';

export default function Dialog(props) {
	const {
		//variant,
		xs, sm, md, lg, xl,
		//outline, outlined,
		//close,
		//submit,
		//disable, disabled,
		//round, rounded,
		headerChildren,
		title,
		headingLevel,
		noClose,
		id,
		children,
		footerChildren,
		className,
		confirmProps, confirmLabel, confirmCallback,
		cancelProps, cancelLabel, cancelCallback
	} = props;
	const dialogRef = useRef();

	useEffect(() => {

		if (!dialogRef.current) {
			return;
		}

		let dialog = dialogRef.current;
		const oldShow = dialog.show;
		const oldShowModal = dialog.showModal;

		dialog.show = () => {
			oldShow.call(dialog);
			dialog.classList.add('show');
			dialog.addEventListener('transitionend', openEndHandler);
		}

		dialog.showModal = () => {
			oldShowModal.call(dialog);
			dialog.classList.add('show');
			dialog.addEventListener('transitionend', openEndHandler);
		}

		function openEndHandler(e) {

			if (e.target == dialog) {
				dialog.classList.add('shown');
				dialog.removeEventListener('transitionend', openEndHandler);
			}

		}

		function closeEndHandler(e) {

			if (e.target == dialog) {
				oldClose.call(dialog);
				dialog.removeEventListener('transitionend', closeEndHandler);
			}

		}

		const oldClose = dialog.close;

		dialog.close = () => {
			dialog.classList.remove('show');
			dialog.addEventListener('transitionend', closeEndHandler);
		}

		dialog.addEventListener("close", () => {
			dialog.classList.remove('show');
			dialog.classList.remove('shown');
		});


	}, []);

	//var btnVariant = variant?.toLowerCase() || (close ? undefined : DEFAULT_VARIANT);
	//var btnType = submit ? "submit" : "button";

	var componentClasses = [COMPONENT_PREFIX];

	/*
	if (btnVariant) {
		componentClasses.push(`${BUTTON_PREFIX}--${btnVariant}`);
	}
	*/

	componentClasses = componentClasses.concat(getSizeClasses(COMPONENT_PREFIX, props));

	/*
	if (outline || outlined) {
		componentClasses.push(`${BUTTON_PREFIX}--outline`);
	}

	if (close) {
		componentClasses.push(`${BUTTON_PREFIX}--close`);
	}

	if (round || rounded) {
		componentClasses.push(`${BUTTON_PREFIX}--rounded`);
	}

	let isDisabled = disable || disabled;
	*/

	if (className) {
		componentClasses.push(className);
	}

	function close() {

		if (dialogRef?.current) {
			dialogRef.current.close();
		}

	}

	return (
		<dialog ref={dialogRef} id={id} className={componentClasses.join(' ')}>
			<Header title={title} headingLevel={headingLevel} noClose={noClose} closeCallback={() => close()} cancelCallback={cancelCallback}>
				{headerChildren}
			</Header>
			<Body>
				{children}
			</Body>
			<Footer closeCallback={() => close()}
					confirmProps={confirmProps} confirmLabel={confirmLabel} confirmCallback={confirmCallback}
					cancelProps={cancelProps} cancelLabel={cancelLabel} cancelCallback={cancelCallback}>
				{footerChildren}
			</Footer>
		</dialog>
	);
}

Dialog.propTypes = {
};

Dialog.defaultProps = {
}
