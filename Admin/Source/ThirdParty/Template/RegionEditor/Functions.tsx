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
            tree.r![item.propName].c?.push(component)
        });
    

    })

    return tree;

}