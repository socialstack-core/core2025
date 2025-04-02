import { useState, useEffect, useRef } from 'react';
import Form from 'UI/Form';
import Fieldset from 'UI/SimpleForm/Fieldset';
import Select from 'UI/SimpleForm/Select';
import Button from 'UI/Button';
import Dialog from 'UI/Dialog';

export default function DialogTest(props) {
	//const { } = props;
	//const [variant, setVariant] = useState('primary');
	const [size, setSize] = useState('md');
	//const [outlined, setOutlined] = useState(false);
	//const [rounded, setRounded] = useState(false);
	//const [disabled, setDisabled] = useState(false);
	const [dialogProps, setDialogProps] = useState({});
	const modalRef = useRef();

	useEffect(() => {
		let _props = {
			//variant: variant,
			//size: size,
			//outline: outlined || undefined,
			//rounded: rounded || undefined,
			//disabled: disabled || undefined,
		};

		if (size) {
			_props[size] = true;
		}

		setDialogProps(_props);
	}, [/*variant,*/ size /*, outlined, rounded, disabled*/]);

	function triggerModal() {

		if (modalRef?.current) {
			modalRef.current.base.showModal();
		}

	}

	function triggerNonModal() {

		if (modalRef?.current) {
			modalRef.current.base.show();
		}

	}

	return <div className="dialog-test">
		<Fieldset className="dialog-test__options">
			{/*
			<Select label={`Variant`} value={variant} onChange={(e) => setVariant(e.target.value)}>
				<option value="">None</option>
				<option value="primary">Primary</option>
				<option value="secondary">Secondary</option>
				<option value="success">Success</option>
				<option value="danger">Danger</option>
				<option value="warning">Warning</option>
				<option value="info">Info</option>
			</Select>
				*/}
			<Select label={`Size`} value={size} onChange={(e) => setSize(e.target.value)}>
				<option value="xs">Extra small</option>
				<option value="sm">Small</option>
				<option value="md">Medium</option>
				<option value="lg">Large</option>
				<option value="xl">Extra large</option>
			</Select>
		</Fieldset>

		{/*<h2 className="component-test-category">Button variants</h2>*/}
		<section className="component-test component-spacing">
			<Button variant="primary" onClick={() => triggerModal()}>
				Trigger modal
			</Button>
			<Button variant="primary" outline onClick={() => triggerNonModal()}>
				Trigger non-modal
			</Button>

			<Dialog ref={modalRef} id="modal" title="Modal dialog test"
				confirmProps={{
					label: 'OK',
					variant: 'primary',
					outline: false,
					callback: () => alert('OK pressed')
				}}
				cancelProps={{
					label: 'Cancel',
					variant: 'secondary',
					outline: true,
					callback: () => alert('dialog cancelled')
				}}
				//confirmCallback={() => alert('OK pressed')}
				//cancelCallback={() => alert('dialog cancelled')}
			>
				<p>
					Lorem ipsum dolor sit amet consectetur, adipisicing elit. In, natus eius. Odio at doloribus, laborum iure dolore eius quidem et incidunt veritatis illum animi reprehenderit iusto labore vero ea quibusdam!
				</p>
				<p>
					Lorem ipsum, dolor sit amet consectetur adipisicing elit. Nesciunt, ipsum reprehenderit. Magni optio, saepe debitis iusto unde voluptate numquam inventore deserunt, repellendus eveniet tempora voluptates ipsam adipisci at, sunt blanditiis.
				</p>
			</Dialog>
		</section>

	</div>;
}

DialogTest.propTypes = {
};

DialogTest.defaultProps = {
}

DialogTest.icon='align-center';
