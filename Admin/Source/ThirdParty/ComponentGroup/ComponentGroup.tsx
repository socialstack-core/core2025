import { CodeModuleMeta, getAll, TypeMeta } from "Admin/Functions/GetPropTypes";
import { useEffect, useState } from "react";


const Editor: React.FC = (props:any) => {

    // this holds all the components that follow certain criteria.
    const [components, setComponents] = useState<string[] | null>(null)

    useEffect(() => {
        // we need the components available to actually choose one. 
        if (!components) {

            getAll().then((results:TypeMeta) => {
                const { codeModules } = results;

                setComponents(Object.keys(codeModules).filter(
                    mod => {
                        if (mod.startsWith("Admin/") || mod.startsWith("Api/") || mod.startsWith("UI/Templates") || mod.startsWith("UI/Functions") || mod.startsWith("Email/Templates")) {
                            return;
                        }
                        return mod;
                    }
                ));
            });
        }

    }, [components])
    console.log(components)
    return (
        <div className='component-group-editor'>

            <div className='mb-3'>
                <label htmlFor="form-field-4" className="form-label">{props.label}</label>
                <input type='hidden' value={props.value} name={props.name}/>

                <div className='component-list'>
                    {/* {components} */}
                </div>
            </div>
        </div>
    )
}

export default Editor;