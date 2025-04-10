import Default from "Admin/Layouts/Default"
import TemplateApi from "Api/Template";
import Form from "UI/Form";
import Input from "UI/Input";
import { useEffect, useState } from "react";
import TemplateTypeSelector from "../TemplateTypeSelector";
import TemplateConfig from "Admin/Template/TemplateConfig";

const formSections: React.ReactElement[] = [
    // first panel
    <div className='main-information'>

        <Input
            type='text'
            name='title'
            label='Title'
        />
        <Input
            type='textarea'
            name='description'
            label='Description'
        />
        <TemplateTypeSelector name='templateType' label={`Template type`}/>

    </div> , 
    // template config
    <TemplateConfig />
];

const CreateTemplate: React.FC = (props: any): React.ReactNode => {

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
                        <h4>{`Create a new Template`}</h4>
                        <Form
                            action={TemplateApi.create}
                        >
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

// This is deprecated, will be removed when the propTypes functionality is dropped.
CreateTemplate.propTypes = {};

export default CreateTemplate;  