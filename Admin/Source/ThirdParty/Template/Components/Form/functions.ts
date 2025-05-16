import { Template } from "Api/Template";

/**
 * Validates a template object, ensuring it contains the required fields and that its structure is correct.
 * The function performs checks for several mandatory fields such as `title`, `key`, `templateType`, and `baseTemplate`.
 * It also validates the `bodyJson` property if the template has an `id`. In case of invalid or missing fields, 
 * it rejects the promise with a user-friendly error message and optionally triggers an `onError` callback.
 * 
 * @param {Template} template - The template object to be validated. It contains the properties to be checked such as title, key, templateType, etc.
 * @param {Function} [onError] - An optional callback function that is invoked when an error is found. It can be used for showing UX feedback, such as highlighting missing fields.
 * @returns {Promise<Template>} - A promise that resolves if the template passes validation or rejects with an error message if any required field is missing or invalid.
 * 
 * @throws {Object} - If validation fails, the function will reject the promise with an error object that includes:
 *   - `type`: The type of error (e.g., `'validation'`).
 *   - `detail`: A string representing a more specific error detail (e.g., `'validation/title/missing'`).
 *   - `message`: A user-friendly error message that explains what is wrong with the template.
 */
export const validateTemplate = (template: Template, onError?: Function): Promise<Template> => {
    return new Promise((resolve, reject) => {

        // Validate the title field
        if (!template.title || template.title.length == 0) {
            // UX: Highlight the title field in case of error
            onError && onError({
                field: 'title'
            });

            // Reject with a user-friendly message
            return reject({
                type: 'validation',
                detail: 'validation/title/missing',
                message: `This template needs a title`
            });
        }

        // Validate the key field
        if (!template.key || template.key.length == 0) {
            // UX: Highlight the key field in case of error
            onError && onError({
                field: 'key'
            });

            // Reject with a user-friendly message
            return reject({
                type: 'validation',
                detail: 'validation/key/missing',
                message: `This template needs a key`
            });
        }

        // Validate the templateType field
        if (!template.templateType || template.templateType == 0) {
            // UX: Highlight the templateType field in case of error
            onError && onError({
                field: 'templateType'
            });

            // Reject with a user-friendly message
            return reject({
                type: 'validation',
                detail: 'validation/templateType/missing',
                message: `This template needs a template type`
            });
        }

        // Validate the baseTemplate field
        if (!template.baseTemplate || template.baseTemplate.length == 0) {
            // UX: Highlight the baseTemplate field in case of error
            onError && onError({
                field: 'baseTemplate'
            });

            // Reject with a user-friendly message
            return reject({
                type: 'validation',
                detail: 'validation/baseTemplate/missing',
                message: `Please choose a base template`
            });
        }

        // Skip validation for templates that already have an id (i.e., not a new template)
        if (template.id) {
            return resolve(template);
        }

        // Validate the bodyJson field for templates without an id
        if (!template.bodyJson || template.bodyJson.length == 0) {
            return reject({
                type: 'validation', 
                detail: 'validation/template/incorrectBaseTemplate',
                message: `No changes made against the regions, please configure the template`
            });
        }

        // Parse the bodyJson field, falling back to an empty object if invalid
        const templateJson = JSON.parse(template.bodyJson && template.bodyJson.length != 0 ? template.bodyJson : '{}') ?? {};

        // Validate that the root (r) property exists in the parsed templateJson
        if (!templateJson.r) {
            return reject({
                type: 'validation', 
                detail: 'validation/template/incorrectBaseTemplate',
                message: `Invalid root template, No roots have been set.`
            });
        }

        // If all validations pass, resolve the template
        resolve(template);
    });
};
