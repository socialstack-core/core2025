import Input from 'UI/Input'

type TemplateTypeSelectorProps = {
    name: string,
    label: string
}

const TemplateTypeSelector: React.FC<TemplateTypeSelectorProps> = (props: TemplateTypeSelectorProps): React.ReactNode => (
        <Input
            type={'select'}
            name={props.name}
            label={props.label}
        >
            <option value={0}>{`Choose ${props.label}`}</option>
            <option value={1}>Web</option>
            <option value={2}>Email</option>
            <option value={3}>PDF</option>
        </Input>
)



export default TemplateTypeSelector;