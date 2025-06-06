// Define distinct types for each form mode
import { Content } from "Api/Content";

/**
 * Represents the type of an auto form page.
 * - `list`: A page that lists items.
 * - `create`: A form to create a new item.
 * - `update`: A form to update an existing item.
 */
export type AutoFormType = 'list' | 'create' | 'update';

/**
 * Describes the shape of a button extension that can be injected into an AutoForm.
 */
export type AutoFormButtonExtension = {
    /** The label to display on the button. */
    label: string;

    /**
     * Optional click handler for the button.
     * @param content The content object, strongly typed to `Content<uint>`.
     * @param setPage Optional page navigation function.
     */
    onClick?: (content: Content<uint>, setPage?: (url: string) => void) => void;

    /** Optional href to turn the button into a link. */
    href?: string;

    /** Optional class name for styling the button. */
    className?: string;
};

/**
 * Base structure shared across all form types that can have button extensions.
 */
export type AutoFormSharedItems = {
    /** Optional list of custom buttons to render on the form. */
    buttons?: AutoFormButtonExtension[];
};

/**
 * Structure specific to `create` forms, extending shared items.
 * Add additional `create`-only extension properties here in future.
 */
export type AutoFormCreateItems = AutoFormSharedItems & {
    // Add any extensible items here
};

/**
 * Structure specific to `update` forms, extending shared items.
 * Add additional `update`-only extension properties here in future.
 */
export type AutoFormUpdateItems = AutoFormSharedItems & {
    // Add any extensible items here
};

/**
 * Structure specific to `list` views, extending shared items.
 * Add additional `list`-only extension properties here in future.
 */
export type AutoFormListItems = AutoFormSharedItems & {
    // Add any extensible items here
};

/**
 * A mapping object that associates each AutoFormType with its corresponding extension structure.
 * Ensures each form type gets the correct set of extensible properties.
 */
export type AutoFormExtensionMap = {
    create: AutoFormCreateItems;
    update: AutoFormUpdateItems;
    list: AutoFormListItems;
};

/**
 * Class responsible for managing form-specific extensions (e.g., buttons) for each content type and form type.
 *
 * @example
 * ```ts
 * AutoFormExtensions.addAutoFormButton('Article', 'create', {
 *   label: 'Preview',
 *   onClick: (content) => console.log(content)
 * });
 * ```
 *
 * @remarks
 * - Ensures type safety across dynamic extension registration.
 * - Encourages modular form customization with zero coupling to form logic.
 */
class AutoFormExtensions {
    /**
     * Internal registry for all content-specific form extensions.
     * Maps a content type (e.g., 'Article') to its per-form-type extensions.
     */
    private extensions: Record<string, AutoFormExtensionMap> = {};

    /**
     * Adds a custom button to a specific form type for a given content type.
     *
     * @param contentType The name/key of the content type (e.g., 'Article').
     * @param pageType The type of the form page (create, update, or list).
     * @param button The button extension to add.
     */
    public addAutoFormButton(contentType: string, pageType: AutoFormType, button: AutoFormButtonExtension) {
        if (!this.extensions[contentType]) {
            this.extensions[contentType] = {
                list: {},
                create: {},
                update: {}
            };
        }

        if (!Array.isArray(this.extensions[contentType][pageType].buttons)) {
            this.extensions[contentType][pageType].buttons = [];
        }

        this.extensions[contentType][pageType].buttons!.push(button);
    }

    /**
     * Retrieves all buttons for a specific form type and content type.
     *
     * @param contentType The name/key of the content type (e.g., 'Article').
     * @param pageType The type of the form page (create, update, or list).
     * @returns An array of button extensions for the specified form.
     */
    public getAutoFormButtons(contentType: string, pageType: AutoFormType): AutoFormButtonExtension[] {
        if (!this.extensions[contentType]) {
            this.extensions[contentType] = {
                list: {},
                create: {},
                update: {}
            };
        }

        if (!Array.isArray(this.extensions[contentType][pageType].buttons)) {
            this.extensions[contentType][pageType].buttons = [];
        }

        return this.extensions[contentType][pageType].buttons!;
    }
}

export default new AutoFormExtensions();
