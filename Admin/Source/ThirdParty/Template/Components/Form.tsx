import TemplateApi, { Template } from "Api/Template"
import { useEffect, useState } from "react";
import Form from "UI/Form";
import { validateTemplate } from "./Form/functions";
import Input from "UI/Input";
import Container from "UI/Container";
import Column from "UI/Column";
import TemplateTypeSelector from "./Form/TemplateTypeSelector";
import { getTemplates, TemplateModule } from "Admin/Functions/GetPropTypes";
import { ApiList } from "UI/Functions/WebRequest";
import Button from "UI/Button";
import Editor from "Admin/Template/Components/Editor";
import Alert from "UI/Alert";

/**
 * Props for the TemplateForm component. This allows the form to either create a new template or edit an existing one.
 * 
 * @typedef {Object} TemplateFormProps
 * @property {Template} [existing] - The existing template to edit. If provided, the form will be pre-filled with this template's data.
 */
export type TemplateFormProps = {
    existing?: Template
}

/**
 * TemplateForm is a React functional component used to create or edit a template.
 * The form includes various fields to input template metadata and an optional template designer for customizing the template body.
 * 
 * @component
 * 
 * @param {TemplateFormProps} props - The props passed into the TemplateForm component.
 * @param {Template} [props.existing] - An existing template to pre-fill the form for editing. If not provided, the form is for creating a new template.
 * 
 * @returns {React.ReactNode} - A JSX element representing the template creation/editing form.
 * 
 * @example
 * // Example usage of TemplateForm
 * <TemplateForm existing={template} />
 */
const TemplateForm: React.FC<TemplateFormProps> = (props: TemplateFormProps): React.ReactNode => {

    // State to toggle the visibility of the template designer
    const [usingTemplateDesigner, setShowTemplateDesigner] = useState<boolean>(false);

    // State to store UI template collection
    const [uiTemplateCollection, setUITemplateCollection] = useState<TemplateModule[]>();

    // State to store the selected UI template
    const [chosenUiTemplate, setChosenUiTemplate] = useState<TemplateModule>();

    // State to store parent database templates related to the selected UI template
    const [parentDbTemplateCollection, setParentDbTemplateCollection] = useState<Template[]>();

    // State to store the currently selected parent template
    const [parentDbTemplate, setParentDbTemplate] = useState<Template>();

    // State to store error messages
    const [error, setError] = useState();

    // Fetch UI templates when the component mounts
    useEffect(() => {
        if (!Array.isArray(uiTemplateCollection)) {
            getTemplates().then(setUITemplateCollection).catch(setError);
        }
    }, [uiTemplateCollection]);

    // If an existing template is provided, update the selected UI template and parent template
    useEffect(() => {
        if (uiTemplateCollection && props.existing) {
            setChosenUiTemplate(uiTemplateCollection.find(tpl => tpl.name === props.existing?.baseTemplate));
        }
        if (props.existing && props.existing.templateParent && parentDbTemplateCollection) {
            setParentDbTemplate(parentDbTemplateCollection.find(parent => parent.id === props.existing?.templateParent));
        }
    }, [props.existing, uiTemplateCollection]);

    // Fetch parent database templates for the selected UI template
    useEffect(() => {
        if (chosenUiTemplate) {
            TemplateApi.list({
                query: "baseTemplate = ? and id != ?",
                args: [chosenUiTemplate.name, props.existing?.id ?? 0]
            })
            .then((result: ApiList<Template>) => {
                setParentDbTemplateCollection(result.results);
            })
            .catch(setError);
        }
    }, [chosenUiTemplate, props.existing]);

    return (
        <Container>
            <Column size={'6'}>
                <Form
                    className="template-form"
                    action={
                        props.existing && props.existing.id ? 
                            // If editing an existing template, call update
                            (fields: Template) => TemplateApi.update(props.existing!.id, fields) :
                            // Otherwise, create a new template
                            TemplateApi.create
                    }
                    onValues={(values: Template) => {
                        return new Promise<Template>((resolve) => {
                            console.log(JSON.parse(values.bodyJson!));
                            validateTemplate(values, (error: any) => {
                                if (error.field === 'editor') {
                                    setShowTemplateDesigner(true);
                                } else {
                                    setShowTemplateDesigner(false);
                                }
                            })
                            .then(resolve)
                            .catch((error) => {
                                setError(error.message);
                            });
                        });
                    }}
                >
                    {error && <Alert variant="danger">{error}</Alert>}
                    
                    {/* Template Metadata Fields */}
                    <div className={'field ' + (usingTemplateDesigner ? 'hide' : '')}>
                        <Input type="text" name="title" label="Template title" defaultValue={props.existing?.title} />
                    </div>
                    <div className={'field ' + (usingTemplateDesigner ? 'hide' : '')}>
                        <Input type="textarea" name="description" label="Template description" defaultValue={props.existing?.description} />
                    </div>
                    <div className={'field ' + (usingTemplateDesigner ? 'hide' : '')}>
                        <Input type="text" name="key" label="Template key" defaultValue={props.existing?.key} />
                    </div>
                    <div className={'field ' + (usingTemplateDesigner ? 'hide' : '')}>
                        <TemplateTypeSelector name="templateType" label="Template type" selected={props.existing?.templateType} />
                    </div>

                    {/* Base Template Selection */}
                    <div className={'field ' + (!usingTemplateDesigner ? 'hide' : '')}>
                        <Input 
                            type="select"
                            name="baseTemplate"
                            label="Base template"
                            defaultValue={props.existing?.baseTemplate}
                            onChange={(ev) => {
                                const target = ev.target as HTMLSelectElement;
                                setChosenUiTemplate(uiTemplateCollection?.find(tpl => tpl.name === target.value));
                            }}
                        >
                            <option value="">{`Choose template`}</option>
                            {uiTemplateCollection?.map(template => (
                                <option value={template.name} key={template.name}>{template.name.substring(template.name.lastIndexOf('/') + 1)}</option>
                            ))}
                        </Input>
                    </div>

                    {/* Parent Template Selection */}
                    {parentDbTemplateCollection && (
                        <div className={'field ' + (!usingTemplateDesigner ? 'hide' : '')}>
                            <Input 
                                type="select"
                                name="templateParent"
                                label="Parent Template"
                                defaultValue={props.existing?.templateParent}
                                onChange={(ev) => {
                                    const target = ev.target as HTMLSelectElement;
                                    setParentDbTemplate(parentDbTemplateCollection.find(parent => parent.id === parseInt(target.value)));
                                }}
                            >
                                <option value="">{`Choose template`}</option>
                                {parentDbTemplateCollection?.map(template => (
                                    <option value={template.id} key={template.id}>{template.title}</option>
                                ))}
                            </Input>
                        </div>
                    )}

                    {/* Template Designer */}
                    {chosenUiTemplate && usingTemplateDesigner && (
                        <Editor
                            uiTemplateFile={chosenUiTemplate}
                            dbParentTemplate={parentDbTemplate}
                            value={props.existing?.bodyJson}
                            key={chosenUiTemplate.name + "-" + (parentDbTemplate?.id ?? 0)}
                        />
                    )}

                    {/* Action Buttons */}
                    {
                        usingTemplateDesigner ? (
                            <>
                                <Button buttonType="button" onClick={() => setShowTemplateDesigner(false)}>
                                    {`Back to form`}
                                </Button>
                                <Button buttonType="submit">
                                    {props.existing ? `Save changes` : `Create template`}
                                </Button>
                            </>
                        ) : (
                            <Button buttonType="button" onClick={() => setShowTemplateDesigner(true)}>
                                {`Next: Template Designer`}
                            </Button>
                        )
                    }
                </Form>
            </Column>
        </Container>
    );
}

export default TemplateForm;
