import { CodeModuleType, CodeModuleTypeField, getTemplates, TemplateModule } from "Admin/Functions/GetPropTypes";
import TemplateApi, { Template } from "Api/Template";
import { FormEventHandler, useEffect, useState } from "react";
import { ApiList } from "UI/Functions/WebRequest";
import Input from "UI/Input";
import RegionEditor from "Admin/Template/RegionEditor";
import { DocumentRootTreeNode } from "../RegionEditor/types";

export type TemplateConfigProps = {
    existing?: Template
}

const TemplateConfig: React.FC<TemplateConfigProps> = (props: TemplateConfigProps): React.ReactElement => {

    // holds the base templates/layouts from the filesystem
    const [layoutTemplates, setLayoutTemplates] = useState<TemplateModule[] | null>(null);
    
    // the chosen layout, used to query templates that share the same layout.
    const [chosenLayout, setChosenLayout] = useState<TemplateModule | null>()

    // when a filesystem layout is chosen, this then populates this with a list of templates
    // will be empty until templates are actually created.
    const [possibleTemplates, setPossibleTemplates] = useState<Template[] | null>();

    // when a db template is selected it will hold here.
    const [chosenTemplate, setChosenTemplate] = useState<Template | null>()

    // hold the template in here, onChange pass up to the parent so
    // the JSON can be serialized before putting in the POST data.
    // when a template is chosen from "Inherits", this should override 
    // this value.
    const [templateJson, setTemplateJson] = useState<DocumentRootTreeNode>();

    // when the template is updated, this is used for the key
    const [updateNo, setUpdateNo] = useState(0)


    // load in templates if they aren't loaded in, runs once.
    useEffect(() => {

        if (!layoutTemplates) {
            getTemplates().then(templates => setLayoutTemplates(templates))
        } 
        if (props.existing) {
            // if we're loading an existing one in, mind aswell pass this in :P
            setChosenLayout(layoutTemplates?.find(tpl => tpl.name === props.existing?.baseTemplate))
        }

    }, [layoutTemplates, props.existing])

    // when a chosen layout is selected, we need to pull any templates that share
    // the same base template, this removes any ambiguity.
    useEffect(() => {

        if (chosenLayout) {

            const where: Partial<Record<keyof(Template), string | number | boolean>> = {};

            if (chosenLayout.name.length != 0) {
                where.baseTemplate = chosenLayout.name;
            }
            TemplateApi.list({ query: "BaseTemplate = ?", args: [chosenLayout.name] })
                       .then((possibleOptions: ApiList<Template>) => {
                            
                            // pass in a list of possible templates that share the same base template.
                            setPossibleTemplates(possibleOptions.results)

                            // set the default template JSON
                            const d: Record<string, any> = {}

                            chosenLayout.types.types.some((type: CodeModuleType) => {
                                if (type.name === 'interface') {
                                    type.fields?.forEach((field) => {
                                        d[field.name] = null;
                                    })
                                }
                            })

                            setTemplateJson({
                                t: chosenLayout.name,
                                d,
                                r: {}
                            })
                       })
                       .catch((err) => {
                            // TODO: Do something.
                            console.error(err)
                       })
        } 

    }, [chosenLayout]);

    useEffect(() => {

        if (props.existing) {
            // this actually allows it to load in its existing values
            setChosenTemplate(possibleTemplates?.find(tpl => tpl.id === props.existing?.id))
        }
            
    }, [possibleTemplates])

    useEffect(() => {
        // when the update occurs, set the template JSON
        // when a user chooses a different template, it then in turns
        // updates this.
        setTemplateJson(JSON.parse(chosenTemplate?.bodyJson ?? '{}') ?? {})

    }, [chosenTemplate])

    if (!layoutTemplates || (chosenLayout && !possibleTemplates)) {
        // needs better handling
        return <p>Loading</p>
    }

    return (
        <div className='template-config'>
            {!props.existing && <Input
                type='select'
                name='baseTemplate'
                label={'Choose base layout'}
                onInput={(ev) => {
                    setChosenLayout(layoutTemplates.find(template => template.name === (ev.target as HTMLSelectElement).value))
                    setChosenTemplate({ id: 0 } as Template)
                }}
            >
                <option value={''}>{`Choose base layout`}</option>
                {layoutTemplates.map(template => {
                    return (
                        <option selected={template.name == chosenLayout?.name} value={template.name}>{template.name.replace("UI/Templates/", "")}</option>
                    )
                })}
            </Input>}
            {chosenLayout && !props.existing && 
                <Input
                    type='select'
                    name='templateParent'
                    label={`Inherits`}
                    key={chosenTemplate?.id}
                    onInput={(ev) => {
                        setChosenTemplate(possibleTemplates?.find(template => template.id === parseInt((ev.target as HTMLSelectElement).value)))
                    }}
                >
                    <option value={0}>{`None (Blank)`}</option>
                    {possibleTemplates?.map(template => {
                        return (
                            <option selected={chosenTemplate?.id === template.id} value={template.id}>{template.title}</option>
                        )
                    })}
                </Input>
            }
            <input type='hidden' name='bodyJson' value={JSON.stringify(templateJson ?? {})}/>
            {chosenLayout && templateJson && 
                <div className='component-config'>
                    <h4>Regions</h4>
                    <RegionEditor
                        templateDocument={templateJson ?? {}}
                        extends={chosenTemplate == null ? undefined : chosenTemplate}
                        layoutFile={chosenLayout}
                        onChange={(previous, latest) => {

                            console.log({ previous, latest })

                            setTemplateJson(latest);
                            setUpdateNo(updateNo + 1)
                        }}
                        key={'update-' + updateNo}
                    />
                </div>
            }
        </div>
    )

}

export default TemplateConfig;