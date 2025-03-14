/**
 * Used to construct a form field set and returns it in a promise.
 * If the fields were unable to be constructed because they contain validation errors 
 * then the promise will reject with a PublicError stating the validation failure itself.
 * @param e
 * @returns A promise which is rejected if any of the fields have validation errors.
 */
const SubmitForm = (e: React.FormEvent<HTMLFormElement>) => {
	e.preventDefault();
	
	var fields = (e.target as HTMLFormElement)?.elements;
	var values: Record<string, any> = {};
	var valuePromises = [];
	
	for (var i = 0; i < fields.length; i++) {
		valuePromises.push(collectFieldValue(fields[i], values));
	}

	var submitterName: string = (e as any).submitter?.name;

	if(submitterName){
		values[submitterName] = (e as any).submitter.value;
	}
	
	return Promise.all(valuePromises)
		.then(() => values);
}

/**
 * Collects the given form field's value, putting it in to the given values set.
 * Handles a custom field function called onGetValue such that a form can contain types beyond strings only.
 * @param field
 * @param values
 * @returns A promise which is rejected if any of the fields have validation errors.
 */
const collectFieldValue = (field: any, values: Record<string, any>) : Promise<void> => {
	return new Promise((success, reject) => {
		if (!field.name || field.type == 'submit' || (field.type == 'radio' && !field.checked)) {
			// Not a field we care about.
			success();
			return;
		}

		var name: string = field.name as string;

		if (field.onValidationCheck) {
			var valError = field.onValidationCheck(field) as PublicError;

			if (valError) {
				reject(valError);
				return;
			}
		}

		var value = field.type == 'checkbox' ? field.checked : field.value;

		if (field.onGetValue) {
			value = field.onGetValue(value, field);

			if (value && value.then && typeof value.then === 'function') {
				// It's a promise.
				// Must wait for all of these before proceeding.
				value.then((val: string) => {
					field.value = val;
					values[name] = val;
					success();
				});
			} else {
				field.value = value;
			}
		}

		values[name] = value;
		success();
	});
}

export default SubmitForm;
