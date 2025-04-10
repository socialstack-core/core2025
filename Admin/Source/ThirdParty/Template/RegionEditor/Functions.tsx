import { TemplateModule } from "Admin/Functions/GetPropTypes";
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

            if (!component.d?.allowMultiple && component.c && component.c.length > 1) {
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

            tree.r![item.propName].c?.push(component)
        });
    

    })

    return tree;

}