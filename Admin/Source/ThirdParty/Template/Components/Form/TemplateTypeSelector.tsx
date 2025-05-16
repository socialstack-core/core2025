import Input from 'UI/Input'

/**
 * TemplateTypeSelector is a React functional component that renders a select input for choosing a template type.
 * The component allows the user to select between different template types like 'Web', 'Email', and 'PDF'.
 * It takes in props for customization such as the name of the input, its label, and the currently selected template type.
 * 
 * @component
 * 
 * @param {TemplateTypeSelectorProps} props - The props for configuring the TemplateTypeSelector component.
 * @param {string} props.name - The name attribute for the `<select>` element. This is used for form submission or identification of the field.
 * @param {string} props.label - The label to be displayed alongside the select input. It will appear as a part of the input's label element.
 * @param {number} [props.selected] - The currently selected template type. It can be `0` (default), `1` (Web), `2` (Email), or `3` (PDF). If not provided, the default value of `0` will be used.
 * 
 * @returns {React.ReactNode} - A rendered select input field wrapped inside a custom `Input` component.
 * 
 * @example
 * // Example usage of TemplateTypeSelector
 * <TemplateTypeSelector
 *   name="templateType"
 *   label="Template Type"
 *   selected={1}
 * />
 */
type TemplateTypeSelectorProps = {
    name: string,
    label: string,
    selected?: number
}

const TemplateTypeSelector: React.FC<TemplateTypeSelectorProps> = (props: TemplateTypeSelectorProps): React.ReactNode => (
        <Input
            type={'select'}
            name={props.name}
            label={props.label}
        >
            <option selected={props.selected === 0} value={0}>{`Choose ${props.label}`}</option>
            <option selected={props.selected == 1} value={1}>Web</option>
            <option selected={props.selected == 2} value={2}>Email</option>
            <option selected={props.selected == 3} value={3}>PDF</option>
        </Input>
)

export default TemplateTypeSelector;
