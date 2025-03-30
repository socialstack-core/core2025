import { useState, useEffect } from 'react';
import Form from 'UI/Form';
import Fieldset from 'UI/Form/Fieldset';
import Select from 'UI/Form/Select';
import Input from 'UI/Form/Input';
import Button from 'UI/Button';

export default function CloseButtonTest(props) {
	//const { } = props;
	const [variant, setVariant] = useState();
	const [outlined, setOutlined] = useState(false);
	const [rounded, setRounded] = useState(false);
	const [disabled, setDisabled] = useState(false);
	const [closeProps, setCloseProps] = useState({});

	useEffect(() => {
		setCloseProps({
			variant: variant,
			outline: outlined || undefined,
			rounded: rounded || undefined,
			disabled: disabled || undefined,
		});
	}, [variant, outlined, rounded, disabled]);

	return <div className="close-button-test">
		<Form sm className="button-test__options">
			<Fieldset>
				<Select label={`Variant`} value={variant} onChange={(e) => setVariant(e.target.value)}>
					<option value="">None</option>
					<option value="primary">Primary</option>
					<option value="secondary">Secondary</option>
					<option value="success">Success</option>
					<option value="danger">Danger</option>
					<option value="warning">Warning</option>
					<option value="info">Info</option>
				</Select>
				{/*
				<Select label={`Size`} value={size} onChange={(e) => setSize(e.target.value)}>
					<option value="xs">Extra small</option>
					<option value="sm">Small</option>
					<option value="md">Medium</option>
					<option value="lg">Large</option>
					<option value="xl">Extra large</option>
				</Select>
				*/}
				<Input type="checkbox" label={`Outlined`} checked={outlined} onChange={() => setOutlined(!outlined)} />
				<Input type="checkbox" label={`Rounded`} checked={rounded} onChange={() => setRounded(!rounded)} />
				<Input type="checkbox" label={`Disabled`} checked={disabled} onChange={() => setDisabled(!disabled)} />
			</Fieldset>
		</Form>

		{/*<h2 className="component-test-category">Close buttons</h2>*/}
		<section className="component-test component-spacing">
			<Button close xs {...closeProps} />
			<Button close sm {...closeProps} />
			<Button close md {...closeProps} />
			<Button close lg {...closeProps} />
			<Button close xl {...closeProps} />
		</section>
	</div>;
}

CloseButtonTest.propTypes = {
};

CloseButtonTest.defaultProps = {
}

CloseButtonTest.icon='align-center';
