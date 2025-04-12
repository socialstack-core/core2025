import TemplateConfig from "Admin/Template/TemplateConfig";
import { Template } from "Api/Template"
import Input from "UI/Input";
import TemplateTypeSelector from "../TemplateTypeSelector";
import { Scalar } from "Admin/Template/RegionEditor";
import Default from "Admin/Layouts/Default";
import { useState, useEffect } from "react";
import Form from "UI/Form";
import { validateTemplate } from "../Functions";

export type TemplateEditorProps = {
    formAction: (template: Template) => Promise<Template>,
    existing?: Template
}



const TemplateEditor: React.FC<TemplateEditorProps> = (props: TemplateEditorProps): React.ReactElement => {

    const formSections: React.ReactElement[] = [
        // first panel
        <div className='main-information'>
    
            <Input
                type='text'
                name='title'
                label='Title'
                defaultValue={props.existing?.title}
            />
            <Input
                type='textarea'
                name='description'
                label='Description'
                defaultValue={props.existing?.description}
            />
            <Input
                type='text'
                name='key'
                label='Key'
                defaultValue={props.existing?.key}
            />
            <TemplateTypeSelector selected={props.existing?.templateType} name='templateType' label={`Template type`}/>
    
        </div> , 
        // template config
        <TemplateConfig existing={props.existing}/>
    ];


    const [currentSection, setSection] = useState(0)

    useEffect(() => {
        if (currentSection < 0) {
            setSection(0);
            return;
        }
        if (currentSection >= formSections.length) {
            setSection(formSections.length - 1);
            return;
        }
    }, [currentSection])

    return (
        <Default>
            <div className='container template-create'>
                <div className='row'>
                    <div className='col col-md-6'>
                        <h4>{props.existing ? `Edit Template '${props.existing.title}'` : `Create a new Template`}</h4>
                        <Form
                            action={props.formAction}
                            onValues={(template: Template) => {
                                return validateTemplate(template, (errorInfo: Record<string, Scalar>) => {
                                    const { field } = errorInfo;

                                    switch(field) {
                                        case "title":
                                        case "templateType":
                                            setSection(0);
                                        break;
                                        case "baseTemplate":
                                            setSection(1);
                                        break;
                                    }
                                })
                            }}
                        >
                            {props.existing && <input type='hidden' name='id' value={props.existing.id}/>}
                            {formSections.map((section, idx) => {
                                return (
                                    <div className={'section-container' + (idx === currentSection ? ' active' : '')}>
                                        {section}
                                    </div>
                                )
                            })}
                            
                            {
                                // this displays a back button, to allow previous parts of the form to be edited.
                                currentSection > 0 &&
                                <button
                                    key='back-btn'
                                    className='btn btn-primary'
                                    onClick={() => setSection(currentSection - 1)}
                                    type='button'
                                >
                                    {`Back`}
                                </button>
                            }
                            {
                                // if the section is the last section, allow the form to be submitted
                                // otherwise show a next button. 
                                currentSection == formSections.length - 1 ?
                                <button 
                                    className='btn btn-primary'
                                    key='create-btn'
                                >
                                    {`Create template`}
                                </button> :
                                <button 
                                    type='button' 
                                    onClick={() => setSection(currentSection + 1)} 
                                    className='btn btn-primary'
                                    key='next-btn'
                                >
                                    {`Continue`}
                                </button>
                        }
                        </Form>
                    </div>
                </div>
            </div>
        </Default>
    )

}

export default TemplateEditor;