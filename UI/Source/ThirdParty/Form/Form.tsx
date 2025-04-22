import submitForm from 'UI/Functions/SubmitForm';
import Spacer from 'UI/Spacer';
import Alert from 'UI/Alert';
import Loading from 'UI/Loading';
import Input from 'UI/Input';
import { useState, useEffect, useRef } from 'react'; 

/**
 * Props for the Form component.
 */
interface FormProps<ResponseType, FieldType> extends React.HTMLAttributes<HTMLFormElement> {
	action: (fields: FieldType) => Promise<ResponseType>,
	resetOnSubmit?:boolean,
	failedMessage?: React.ReactNode,
	loadingMessage?: string,
	successMessage?: React.ReactNode,
	submitEnabled?: boolean,
	submitLabel?: string,
	onSuccess?: (response: ResponseType) => void,
	onFailed?: (e: PublicError) => void,
	onValues?: (values: FieldType) => FieldType | Promise<FieldType>,
	onSubmitted?: (values: FieldType) => void
}

/**
 * Wraps <form> in order to automatically manage setting up default values.
 * You can also directly use form and the Functions/SubmitForm method if you want - use of this component is optional.
 * This component is best used with UI/Input.
 */
const Form = <ResponseType extends any, FieldType extends any>(props: FormProps<ResponseType, FieldType>) => {
	const {
		action,
		children,
		failedMessage,
		loadingMessage,
		successMessage,
		submitEnabled,
		submitLabel,
		resetOnSubmit,
		onSubmitted,
		onSuccess,
		onFailed,
		onValues,
		...attribs
	} = props;

	const formRef = useRef<HTMLFormElement>(null);
	const [loading, setLoading] = useState(false);
	const [success, setSuccess] = useState(false);
	const [failed, setFailed] = useState <PublicError | null>(null);

	useEffect(() => {
		setFailed(null);
		setSuccess(false);
		setLoading(false);
	}, [action]);

	const onSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
		e.preventDefault(); // Prevent default form submission
		setLoading(true);
	
		try {
			const rawVals = await submitForm(e);
			let values = rawVals as FieldType;
	
			if (onValues) {
				const result = onValues(values);
				values = result instanceof Promise ? await result : result;
			}
	
			onSubmitted && onSubmitted(values);

			const response = await action(values);
	
			if (resetOnSubmit) {
				formRef.current?.reset();
			}
	
			setLoading(false);
			setFailed(null);
			setSuccess(true);
			onSuccess && onSuccess(response);
		} catch (e: any) {
			
			const error: PublicError = (e?.message)
				? (e as PublicError)
				: {
					type: 'validation',
					message: `Unable to send this form - please check your answers`,
					detail: e
				};
			console.error(e);
			setLoading(false);
			setFailed(error);
			setSuccess(false);
			onFailed && onFailed(error);
		}
	
		return false;
	};
	

	let failureMessage = failed ? (failed.message || failedMessage) : undefined;
	var showFormResponse = !!(loadingMessage || submitLabel || failedMessage);
	var submitDisabled = loading || (submitEnabled !== undefined && submitEnabled != true);
	
	return (
		<form
			onSubmit={onSubmit}
			ref={formRef}
			method={"post"}
			{...attribs}
		>
			{children}
			{showFormResponse && (
				<div className="form-response">
					<Spacer />
					{
						failureMessage && (
							<div className="form-failed">
								<Alert variant="danger">
									{failureMessage}
								</Alert>
								<Spacer />
							</div>
						)
					}
					{
						success && successMessage && (
							<div className="form-success">
								<Alert variant="success">
									{successMessage}
								</Alert>
								<Spacer />
							</div>
						)
					}
					{
						submitLabel && <Input type="submit" label={submitLabel} disabled={submitDisabled} />
					}
					{
						loading && loadingMessage && (
							<div className="form-loading">
								<Spacer />
								<Loading message={loadingMessage}/>
							</div>
						)
					}
				</div>
			)}
		</form>
	);
	
}

export default Form;