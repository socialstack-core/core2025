import { useState, useEffect, useRef } from 'react';
import Form from 'UI/Form';
import Fieldset from 'UI/SimpleForm/Fieldset';
import Field from 'UI/SimpleForm/Field';
import Select from 'UI/SimpleForm/Select';
import Input from 'UI/SimpleForm/Input';
import Checkbox from 'UI/SimpleForm/Input/Checkbox';
import Radio from 'UI/SimpleForm/Input/Radio';
import Button from 'UI/Button';
import RequestFullscreen from 'UI/RequestFullscreen';

export default function FormTest(props) {
	const parentRef = useRef();
	//const { } = props;
	//const [variant, setVariant] = useState('primary');
	const [size, setSize] = useState('md');
	//const [outlined, setOutlined] = useState(false);
	//const [rounded, setRounded] = useState(false);
	//const [disabled, setDisabled] = useState(false);
	const [formProps, setFormProps] = useState({});
	const [fieldProps, setFieldProps] = useState({});

	const [title, setTitle] = useState('dr');

	const [date, setDate] = useState();
	const [dateTime, setDateTime] = useState();
	const [month, setMonth] = useState();
	const [time, setTime] = useState();
	const [week, setWeek] = useState();

	const [number, setNumber] = useState();

	const [password, setPassword] = useState();

	const [search, setSearch] = useState();

	const [range, setRange] = useState();

	const [file, setFile] = useState();

	const [color, setColor] = useState();
	const [email, setEmail] = useState();
	const [tel, setTel] = useState();
	const [text, setText] = useState();
	const [url, setUrl] = useState();

	const [check1, setCheck1] = useState();
	const [check2, setCheck2] = useState();

	const [radio, setRadio] = useState();

	useEffect(() => {
		let _props = {
			//variant: variant,
			//ratio: ratio,
			//size: size,
			//outline: outlined || undefined,
			//rounded: rounded || undefined,
			//disabled: disabled || undefined,
		};

		if (size) {
			_props[size] = true;
		}

		setFormProps(_props);
	}, [/*variant,*/ size /*, outlined, rounded, disabled*/]);

	useEffect(() => {
		let _props = {
			//variant: variant,
			//ratio: ratio,
			//size: size,
			//outline: outlined || undefined,
			//rounded: rounded || undefined,
			//disabled: disabled || undefined,
		};

		if (size) {
			_props[size] = true;
		}

		setFieldProps(_props);
	}, [/*variant,*/ size /*, outlined, rounded, disabled*/]);

	return <div className="form-test" ref={parentRef}>
		<RequestFullscreen elementRef={parentRef} />
		<Fieldset className="form-test__options" sm>
			{/*
			<Select label={'Variant'} id="btnVariant" value={variant} onChange={(e) => setVariant(e.target.value)}>
				<option value="">None</option>
				<option value="primary">Primary</option>
				<option value="secondary">Secondary</option>
				<option value="success">Success</option>
				<option value="danger">Danger</option>
				<option value="warning">Warning</option>
				<option value="info">Info</option>
			</Select>
			*/}
			<Select label={'Size'} id="embedSize" value={size} onChange={(e) => setSize(e.target.value)}>
				<option value="xs">Extra small</option>
				<option value="sm">Small</option>
				<option value="md">Medium</option>
				<option value="lg">Large</option>
				<option value="xl">Extra large</option>
			</Select>
			{/*
			<hr/>
			<div className="form__field">
				<input type="checkbox" id="btnOutlined" checked={outlined} onChange={() => setOutlined(!outlined)} />
				<label htmlFor="btnOutlined">
					Outlined
				</label>
			</div>
			<div className="form__field">
				<input type="checkbox" id="btnRounded" checked={rounded} onChange={() => setRounded(!rounded)} />
				<label htmlFor="btnRounded">
					Rounded
				</label>
			</div>
			<div className="form__field">
				<input type="checkbox" id="btnDisabled" checked={disabled} onChange={() => setDisabled(!disabled)} />
				<label htmlFor="btnDisabled">
					Disabled
				</label>
			</div>
				*/}
		</Fieldset>

		<h2 className="component-test-category">Form fields</h2>
		<section className="component-test">
			<Form>
				<Select id={'title'} label={`Title`} value={title} onChange={(e) => {
					setTitle(e.target.value)
				}} help={`Selected: ${title}`} {...fieldProps}>
					<option value="mr">Mr</option>
					<option value="mrs">Mrs</option>
					<option value="ms">Ms</option>
					<option value="dr">Dr</option>
					<option value="rev">Rev</option>
					<option value="other">Other</option>
					<optgroup label={`Another group`}>
						<option value="lorem">Lorem</option>
						<option value="ipsum">Ipsum</option>
						<option value="dolor">Dolor</option>
						<option value="sit">Sit</option>
						<option value="amet" disabled>Amet (disabled)</option>
					</optgroup>
					<optgroup label={`Disabled`} disabled>
						<option value="foo">Foo</option>
						<option value="bar">Bar</option>
						<option value="etc">Etc.</option>
					</optgroup>
				</Select>

				<Fieldset legend={`Date and Time`}>
					<Input type="date" label={`Select a date`} value={date} />
					<Input type="datetime" label={`Select a date and time`} value={dateTime} />
					<Input type="month" label={`Select a month`} value={month} />
					<Input type="time" label={`Select a time`} value={time} />
					<Input type="week" label={`Select a week`} value={number} />
				</Fieldset>

				<Fieldset legend={`Numbers`}>
					<Input type="number" label={`Enter a number`} value={number} />
				</Fieldset>

				<Fieldset legend={`Passwords`}>
					<Input type="password" label={`Password`} value={password} />
				</Fieldset>

				<Fieldset legend={`Search`}>
					<Input type="search" label={`Enter a search query`} value={search} />
				</Fieldset>

				<Fieldset legend={`Range`}>
					<Input type="range" label={`Select a range`} value={range} />
				</Fieldset>

				<Fieldset legend={`File uploads`}>
					<Input type="file" label={`Select a file`} value={file} />
				</Fieldset>

				<Fieldset legend={`Other input types`}>
					<Input type="color" label={`Select a colour`} help={`Click to select`} value={color} />
					<Input type="email" label={`E-mail address`} value={email} />
					<Input type="tel" label={`Telephone number`} value={tel} />
					<Input label={`Enter a string`} value={text} />
					<Input type="url" label={`Web address`} value={url} />
				</Fieldset>

				<Fieldset legend={`Checkboxes`}>
					<Input type="check" label={`Referenced with <input>`} value={check1} />
					<Checkbox label={`Referenced with <Checkbox>`} value={check2} />
				</Fieldset>

				<Fieldset legend={`Radio buttons`}>
					<Input type="radio" label={`Referenced with <input>`} name="radioGroup" value="option1" currentValue={radio} />
					<Radio label={`Referenced with <Radio>`} name="radioGroup" value="option2" currentValue={radio} />
					<Radio label={`Additional option`} name="radioGroup" value="option3" currentValue={radio} />
				</Fieldset>
			</Form>
		</section>

	</div>;
}

FormTest.propTypes = {
};

FormTest.defaultProps = {
}

FormTest.icon='align-center';
