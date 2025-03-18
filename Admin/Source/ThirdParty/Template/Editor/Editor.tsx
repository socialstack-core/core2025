import { Template } from "Api/Template";
import { FC, PropsWithChildren, ReactNode, useEffect, useState } from "react";

export type TemplateStructure = {

}

export type EditorProps = {
    value?: Template,
    onChange: (value: TemplateStructure) => void
}

/**
 * This component is used to edit templates in the admin panel. 
 * @see {Api.Templates.Template::BodyJson} - The template object that is used to store the template in the database.
 * @param props 
 * @returns 
 */
const Editor: FC<PropsWithChildren<EditorProps>> = (props: EditorProps): ReactNode => {

    const [editorJson, setEditorJson] = useState<string | null>(null);

    useEffect(() => {
        
        if (!editorJson && props.value && props.value.bodyJson) {
            setEditorJson(props.value.bodyJson);
        }

    }, [editorJson])

    const structure: TemplateStructure = JSON.parse(editorJson || '{}');

    return (
        <div className='template-editor'>
            
        </div>
    );
}

export default Editor;