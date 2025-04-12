import Input from 'UI/Input'

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