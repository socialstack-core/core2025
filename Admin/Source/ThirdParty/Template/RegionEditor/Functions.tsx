import { CodeModuleMeta, CodeModuleType, getAll, TemplateModule } from "Admin/Functions/GetPropTypes";
import { CoreRegionConfig, Scalar, TreeComponentItem } from "./RegionEditor";


export const templateConfigToCanvasJson = (config: Record<string, CoreRegionConfig & Record<string, Scalar | TreeComponentItem[]>>, layout: TemplateModule): TreeComponentItem => {

    const tree: TreeComponentItem = {
        t: layout.name,
        d: {},
        r: {}
    }

    Object.keys(config).forEach(prop => {

        const item = config[prop];

        // we need a parent for the collection here, 
        // so we specify Admin/Template/Wrapper, this
        // can be easily filtered out by the collapse function
        tree.r![item.propName] = {
            t: 'Admin/Template/Wrapper',
            d: {},
            c: []
        }

        // the components are already in the right format
        // so we just push them
        item.components!.forEach(component => {

            if (component.t === "Admin/Template/Wrapper") {
                return;
            }
            if (component.t === "Admin/Template/Slot") {
                
                if (!component.d?.multipleAllowed && component.c && component.c.length > 1) {
                    // this shouldn't happen.
                    // every change that happens triggers this function
                    // if an error occurs then it should be shown.
                    throw new Error(`The component ${component.d.$editorLabel ?? component.t} inside ${item.propName} cannot contain more than 1 component`)
                }
    
                if (component.d?.permitted && (component.d.permitted as string[]).length != 0) {
                    // when a permitted array is empty, its an allow all by default. 
                    // when its populated with any items, only the items specified are allowed in there
    
                    const permitted: string[] = component.d.permitted as string[];
                    
                    component.c?.forEach(child => {
                        if (!permitted.includes(child.t)) {
                            throw new Error(`You cannot use the component ${child.d.$editorLabel ?? child.t} inside ${item.propName} > ${component.d.$editorLabel ?? component.t}`)
                        }
                        // for the case of ! not allowing a certain component
                        if (permitted.includes("!" + child.t)) {
                            throw new Error(`You cannot use the component ${child.d.$editorLabel ?? child.t} inside ${item.propName} > ${component.d.$editorLabel ?? component.t}`)
                        }
                    })
                }
            }


            tree.r![item.propName].c?.push(component)
        });
    

    })

    return tree;

}

export const canAddChildren = (componentName:string): Promise<boolean> => {
    return new Promise((resolve, reject) => {

        if (componentName.startsWith("Admin/Template")) {
            resolve(true);
            return;
        }
        getAll().then(codebase => {
            if (!codebase.codeModules[componentName]) {
                return false;
            }

            const moduleTypes: CodeModuleType[] = codebase.codeModules[componentName].types;
            const name:string = componentName.includes('/') ? componentName.split('/').pop()! : componentName;
            const iface = moduleTypes.find(exp => exp.name === 'interface' && exp.instanceName?.includes(name));

            let canAddChild = Boolean(
                iface?.fields?.find(
                    field => field.name === 'children'
                )
            );

            if (!canAddChild) {
                const defExport = moduleTypes.find(child => child.name === 'export' && child.instanceName === 'default');

                if (!defExport?.detail) {
                    resolve(false);
                    return;
                }

                const { detail } = defExport?.detail!;

                const childrenEnabledPropTypes: string[] = [
                    "React.PropsWithChildren", 
                    // add more here.
                ]

                if (!detail || !detail.genericParameters) {
                    resolve(false);
                    return;
                }

                canAddChild = childrenEnabledPropTypes.includes(
                    detail.genericParameters[0].name
                )

            }

            resolve(
                canAddChild
            );

        })
    })
}

export const getPropsForComponent = (type: CodeModuleMeta) => {
    
    const propTypes = type.types.find(type => type.instanceName === 'default');

    if (!propTypes) {
        // fallback on another way, for now, return null.
        return null;
    }

    // check for generic parameters
    if (!propTypes.detail?.detail?.genericParameters && propTypes.detail?.detail?.genericParameters?.length != 0) {
        return null;
    }

    // since the generic parameters exist, we grab the first generic parameter
    // but we need to make sure its of type React.FC
    if (propTypes.detail.detail.instanceName != "React.FC") {
        return null;
    }

    // we're fairly certain we've got a match here
    let ifaceName = propTypes.detail?.detail?.genericParameters[0].instanceName;

    // just incase we dont, cheeky null check.
    if (!ifaceName) {
        return null;
    }

    if (ifaceName == 'React.PropsWithChildren') {
        ifaceName = propTypes.detail?.detail?.genericParameters![0].genericParameters![0].instanceName;
    }

    const iface = type.types.find(type => type.name === 'interface' && type.instanceName === ifaceName);

    // do a null check, there is a possibility the type lives elsewhere outside the script
    // this does need to be handled in future
    if (!iface) {
        return null;
    }

    return iface.fields;
} 