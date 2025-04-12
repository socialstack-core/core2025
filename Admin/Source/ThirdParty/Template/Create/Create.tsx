import TemplateEditor from "Admin/Template/Editor";
import TemplateApi from "Api/Template";


const CreateTemplate: React.FC = (props: any): React.ReactNode => {
    return (
        <TemplateEditor
            formAction={TemplateApi.create}
        />
    )
}

export default CreateTemplate;  