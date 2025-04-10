import { CodeModuleType, CodeModuleTypeField, getTemplates, TemplateModule } from "Admin/Functions/GetPropTypes";
import TemplateApi, { Template } from "Api/Template";
import { FormEventHandler, useEffect, useState } from "react";
import { ApiList } from "UI/Functions/WebRequest";
import Input from "UI/Input";
import RegionEditor, { Scalar, TreeComponentItem } from "Admin/Template/RegionEditor";


const TemplateConfig: React.FC = (props: {}): React.ReactElement => {

    // holds the base templates/layouts from the filesystem
    const [layoutTemplates, setLayoutTemplates] = useState<TemplateModule[] | null>(null);
    
    // the chosen layout, used to query templates that share the same layout.
    const [chosenLayout, setChosen] = useState<TemplateModule | null>()

    // when a filesystem layout is chosen, this then populates this with a list of templates
    // will be empty until templates are actually created.
    const [possibleTemplates, setPossibleTemplates] = useState<Template[] | null>();

    // when a db template is selected it will hold here.
    const [chosenTemplate, setChosenTemplate] = useState<Template | null>()

    // hold the template in here, onChange pass up to the parent so
    // the JSON can be serialized before putting in the POST data.
    // when a template is chosen from "Inherits", this should override 
    // this value.
    const [templateJson, setTemplateJson] = useState<TreeComponentItem | null>();


    // load in templates if they aren't loaded in, runs once.
    useEffect(() => {

        if (!layoutTemplates) {
            getTemplates().then(templates => setLayoutTemplates(templates))
        }

    }, [layoutTemplates])

    // when a chosen layout is selected, we need to pull any templates that share
    // the same base template, this removes any ambiguity.
    useEffect(() => {

        if (chosenLayout) {

            const where: Partial<Record<keyof(Template), string | number | boolean>> = {};

            if (chosenLayout.name.length != 0) {
                where.baseTemplate = chosenLayout.name;
            }
            TemplateApi.list(where)
                       .then((possibleOptions: ApiList<Template>) => {
                            
                            // pass in a list of possible templates that share the same base template.
                            setPossibleTemplates(possibleOptions.results)

                            // set the default template JSON
                            const d: Record<string, Scalar> = {}

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
                                c: []
                            })
                       })
                       .catch((err) => {
                            // TODO: Do something.
                            console.error(err)
                       })
        } 

    }, [chosenLayout]);

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

    console.log(chosenLayout)

    return (
        <div className='template-config'>
            <Input
                type='select'
                name='baseTemplate'
                label={'Choose base layout'}
                onInput={(ev) => {
                    setChosen(layoutTemplates.find(template => template.name === (ev.target as HTMLSelectElement).value))
                }}
            >
                <option value={''}>{`Choose base layout`}</option>
                {layoutTemplates.map(template => {
                    return (
                        <option selected={template.name == chosenLayout?.name} value={template.name}>{template.name.replace("UI/Templates/", "")}</option>
                    )
                })}
            </Input>
            {chosenLayout && 
                <Input
                    type='select'
                    name='templateParent'
                    label={`Inherits`}
                    onInput={(ev) => {
                        setChosenTemplate(possibleTemplates?.find(template => template.title === ((ev.target as HTMLSelectElement).value)))
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
            {chosenLayout && templateJson && 
                <div className='component-config'>
                    <h4>Regions</h4>
                    <RegionEditor
                        templateJson={templateJson}
                        currentLayout={chosenLayout}
                        currentTemplate={chosenTemplate ?? undefined}
                    />
                </div>
            }
            {chosenLayout && 
                <div className='configurable-items'>
                    <h3>Configuration</h3>
                    {(chosenLayout as any).types.types[0].fields.map((layoutConfig: CodeModuleTypeField) => {
                        if (['React.ReactNode', 'React.ReactElement'].includes(layoutConfig?.fieldType?.instanceName!))
                        {
                            return;
                        }
                        return <h3>{layoutConfig.name}</h3>
                    })}
                </div>
            }
        </div>
    )

}

export default TemplateConfig;