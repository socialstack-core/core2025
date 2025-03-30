import { useState, useEffect, useRef } from 'react';
import Form from 'UI/Form';
import Fieldset from 'UI/Form/Fieldset';
import Select from 'UI/Form/Select';
import Input from 'UI/Form/Input';
import Button from 'UI/Button';
import RequestFullscreen from 'UI/RequestFullscreen';

export default function ButtonTest(props) {
	//const { } = props;
	const parentRef = useRef();
	//const [variant, setVariant] = useState('primary');
	const [size, setSize] = useState('md');
	const [outlined, setOutlined] = useState(false);
	const [rounded, setRounded] = useState(false);
	const [disabled, setDisabled] = useState(false);
	const [buttonProps, setButtonProps] = useState({});

	useEffect(() => {
		let _props = {
			//variant: variant,
			//size: size,
			outline: outlined || undefined,
			rounded: rounded || undefined,
			disabled: disabled || undefined,
		};

		if (size) {
			_props[size] = true;
		}

		setButtonProps(_props);
	}, [/*variant,*/ size, outlined, rounded, disabled]);

	return <div className="button-test" ref={parentRef}>
		<RequestFullscreen elementRef={parentRef} />
		<Form sm className="button-test__options">
			<Fieldset>
				{/*
			<Select label={`Variant`} id="btnVariant" value={variant} onChange={(e) => setVariant(e.target.value)}>
				<option value="">None</option>
				<option value="primary">Primary</option>
				<option value="secondary">Secondary</option>
				<option value="success">Success</option>
				<option value="danger">Danger</option>
				<option value="warning">Warning</option>
				<option value="info">Info</option>
			</Select>
			*/}
				<Select label={`Size`} id="btnSize" value={size} onChange={(e) => setSize(e.target.value)}>
					<option value="xs">Extra small</option>
					<option value="sm">Small</option>
					<option value="md">Medium</option>
					<option value="lg">Large</option>
					<option value="xl">Extra large</option>
				</Select>
				<Input type="checkbox" label={`Outlined`} checked={outlined} onChange={() => setOutlined(!outlined)} />
				<Input type="checkbox" label={`Rounded`} checked={rounded} onChange={() => setRounded(!rounded)} />
				<Input type="checkbox" label={`Disabled`} checked={disabled} onChange={() => setDisabled(!disabled)} />
			</Fieldset>
		</Form>

		<h2 className="component-test-category">Button variants</h2>
		<section className="component-test component-spacing">
			<Button variant="primary" {...buttonProps}>
				Primary
			</Button>
			<Button variant="secondary" {...buttonProps}>
				Secondary
			</Button>
			<Button variant="success" {...buttonProps}>
				Success
			</Button>
			<Button variant="danger" {...buttonProps}>
				Danger
			</Button>
			<Button variant="warning" {...buttonProps}>
				Warning
			</Button>
			<Button variant="info" {...buttonProps}>
				Info
			</Button>
		</section>

	</div>;
}

ButtonTest.propTypes = {
};

ButtonTest.defaultProps = {
}

ButtonTest.icon='align-center';
