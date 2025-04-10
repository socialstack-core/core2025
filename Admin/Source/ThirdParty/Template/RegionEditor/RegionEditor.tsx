import { CodeModuleTypeField, TemplateModule } from "Admin/Functions/GetPropTypes";
import ComponentSelector from "Admin/Template/ComponentSelector";
import { Template } from "Api/Template";
import { createRef, useState } from "react";
import Alert from "UI/Alert";


export type Scalar = string | number | boolean | null | undefined;

export type TreeComponentItem = {
    // the component type (i.e UI/Functions/SomeComponent)
    t: string,
    // the initialising props, don't change this to "any" or "unknown", just 
    // adjoin it to the accepted V types below
    d: Record<string, Scalar>,
    // children in here.
    c: TreeComponentItem[]
}

export type RegionEditorProps = {
    templateJson: TreeComponentItem,
    currentTemplate?: Template,
    currentLayout: TemplateModule,
    onChange?: (newTree: TreeComponentItem) => void
}

/*
* When choosing ReactNode props, this should re-create the whole tree but only
* affect the changed item, lets say a user changes a header element, anything else
* should be left alone. Any descendants of the header will then be removed. Although
* the header property should be assigned to the prop, it will of course have to be added to the
* children tree. 
*/

const RegionEditor: React.FC<RegionEditorProps> = (props: RegionEditorProps): React.ReactNode => {

    const { currentLayout, currentTemplate, templateJson } = props;
    const { types } = currentLayout.types;

    // make sure we can find the props:MyProps type first, if we can't show an error
    const layoutProps = types.find(type => type.instanceName?.includes('Props') && type.name == 'interface');

    if (!layoutProps) {
        return (
            <Alert type='error'>{`Invalid template: Cannot find a props type in this template`}</Alert>
        )
    }

    // get the fields, from here we iterate over them
    // and then these directly reflect on the template
    const { fields } = layoutProps;

    // this holds the config for all fields, when this is updated
    // the full template is updated.
    const [fullConfig, setFullConfig] = useState<Record<string, CoreRegionConfig & Record<string, Scalar | TreeComponentItem[]>>>({});

    const onChange = (update: CoreRegionConfig & Record<string, Scalar | TreeComponentItem[]>) => {
        fullConfig[update.propName] = {...update};

        setFullConfig({...fullConfig});

        console.log(fullConfig)
    }

    if (currentTemplate) {
        // fill the config with values based on the current template. 
        // we know the templates share the same baseTemplate so 
        // we can pull in the existing templates information

        fields?.forEach(field => {
            if (templateJson.d[field.name]) {

                // props['footer'] = UI/Blocks/Footer
                const componentInfo = templateJson.d[field.name] as string;

                fullConfig[field.name] = {
                    // this is set to true due to the fact templateJson.d[field.name] exists, if it isn't used
                    // it remains empty.
                    enabled: true,
                    // keep everything the same, see below.
                    propName: field.name,
                    // sets the component name (ex. UI/Functions/SomeComponent)
                    components: []
                }
                
                // make sure it runs the same way a normal run happens
                onChange(fullConfig[field.name])
            }
        })
    }

    return (
        <div className='region-editor'>
            {fields?.map(field => field.fieldType.instanceName === 'React.ReactNode' && <RegionLevelEditor config={fullConfig[field.name]} onChange={onChange} field={field} />)}
        </div>
    )

}

type RegionLevelEditorProps = {
    field: CodeModuleTypeField,
    config: CoreRegionConfig & Record<string, any>, 
    onChange: (config: CoreRegionConfig & Record<string, Scalar>) => void
}

type CoreRegionConfig = {
    enabled: boolean,
    propName: string,
    components?: TreeComponentItem[],
    isLockedByParent?: boolean
}

const RegionLevelEditor: React.FC<RegionLevelEditorProps> = (props: RegionLevelEditorProps): React.ReactElement => {

    // holds the state as to whether the select component modal should show
    // this one specifically exists to top level props component selection
    const [showSelectComponentModal, setShowSelectComponentModal] = useState(false)

    const { field } = props;
    let { config } = props;

    // takes the checked state whenever emitChange is called, as opposed to storing the value
    // and having to sync states.
    const enabledRef: React.RefObject<HTMLInputElement | null> = createRef<HTMLInputElement>();

    // this config is a sub item of the actual full config. This is a per-component
    // level, this isn't the full template config that will properly create the template.
    if (!config) {
        config = {
            enabled: !field.optional,
            propName: field.name
        }
    }

    // When a change is made, this needs to be called, this then
    // passes back the changes to the parent component (which should only be RegionEditor)
    // the parent component will then pass the new config back to here where it will render accordingly.
    const emitChange = () => {

        // get the checked status before passing to parent handler.
        props.onChange(config);
    }

    return (
        <div className='region'>
            <div className='enablement'>
                <input 
                    type='checkbox' 
                    ref={enabledRef} 
                    onClick={() => {

                        // when the input element has mounted, and is checked
                        // check if there is a component, if there isn't 
                        // the user should be prompted to choose one,
                        // if one isn't chosen, this should be reset back to false
                        if (enabledRef.current && enabledRef.current.checked) {
                            if (!config.component) {
                                setShowSelectComponentModal(true)
                                return;
                            }
                        }

                        // if the checkbox is de-checked, this should remove any
                        // configuration tied to it.
                        if (enabledRef.current && !enabledRef.current.checked) {
                            config.component = undefined;
                            config.enabled = false;
                            config.componentProps = {};

                            emitChange();
                        }
                    }} 
                    checked={Boolean(config['enabled'])}
                />
            </div>
            <div 
                className='main-info'
                onClick={() => {
                    setShowSelectComponentModal(true)
                }}
            >
                <h3>{field.name}</h3>
                <button  
                    type={'button'}
                >+</button>
            </div>
            <div className='child-regions'>
                {config.components?.map((component) => {
                    return (
                        <div className='child-region'>
                            {`Component: ${component.t}`}
                        </div>
                    )
                })}
            </div>
            {
                showSelectComponentModal && (
                    <ComponentSelector
                        title={`Choose component for ${field.name}`}
                        onClose={() => {
                            if (!config.components || config.components.length == 0) {
                                config.enabled = false;
                                config.component = undefined;
                            }
                            emitChange();
                            setShowSelectComponentModal(false)
                        }}
                        onComponentSelected={(component) => {

                            config.enabled = true;

                            if (!config.components) {
                                config.components = [];
                            }
                            config.components.push({
                                t: component,
                                d: {},
                                c: []
                            })

                            emitChange();
                            setShowSelectComponentModal(false)
                        }}
                    />
                )
            }
        </div>
    )

}

export default RegionEditor;