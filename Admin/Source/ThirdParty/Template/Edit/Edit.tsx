import TemplateEditor from "Admin/Template/Editor";
import TemplateApi, { Template } from "Api/Template";
import { useEffect, useState } from "react";

const showError = () => {
    // TODO: handle this better.... 
    return <p>An Error occured</p>
}

const EditTemplate: React.FC = (props: any): React.ReactNode => {

    const pathItems = location.pathname.replaceAll("//", "/").split('/')
    const targetIdx = pathItems.indexOf('template');

    if (targetIdx < 0 || !pathItems[targetIdx + 1]) {
        return showError();
    }

    // grab the ID from the URL
    const id: number = parseInt(pathItems[targetIdx + 1]);

    if (Number.isNaN(id)) {
        return showError();
    }

    const [template, setTemplate] = useState<Template | undefined>();

    useEffect(() => {
        if (!template) {
            TemplateApi.load(id)
                       .then((tpl: Template) => setTemplate(tpl))
                       .catch(() => console.error('Failed to load template'))
        }
    }, [template])

    if (!template) {
        return <p>Loading</p>
    }

    return (
        <TemplateEditor
            formAction={TemplateApi.update}
            existing={template}
        />
    )
}

export default EditTemplate;  