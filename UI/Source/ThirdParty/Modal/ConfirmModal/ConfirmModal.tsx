import { useState, useEffect } from 'react'
import Modal from 'UI/Modal';

export type ConfirmModalProps = {
    title?: string,
    confirmCallback: Function, 
    confirmText?: string, 
    confirmVariant: string,
    cancelCallback: (callbackValue?: boolean) => void, 
    cancelText?: string, 
    cancelVariant?: string
}

const ConfirmModal: React.FC<React.PropsWithChildren<ConfirmModalProps>> = (props: React.PropsWithChildren<ConfirmModalProps>): React.ReactNode => {
	const {
		title,
		confirmCallback, confirmText, confirmVariant,
		cancelCallback, cancelText, cancelVariant
	} = props;

	var confirmClass = 'btn btn-' + (confirmVariant || 'primary');
	var cancelClass = 'btn btn-' + (cancelVariant || 'outline-primary');
	
	return (
		<Modal visible className="confirm-modal" title={title || `Please confirm`} onClose={() => cancelCallback()}>
			{props.children}
			<footer className="confirm-modal__footer">
				<button type="button" className={cancelClass} onClick={() => cancelCallback(false)}>
					{cancelText || `Cancel`}
				</button>
				<button type="button" className={confirmClass} onClick={() => {
					confirmCallback();
					cancelCallback();
				}}>
					{confirmText || `Yes`}
				</button>
			</footer>
		</Modal>
	);
}

export default ConfirmModal;