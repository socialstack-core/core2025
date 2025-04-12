import { Template } from "Api/Template";
import { TreeComponentItem } from "./RegionEditor/RegionEditor";
import { getPropsForComponent } from "./RegionEditor/Functions";
import { getTemplates } from "Admin/Functions/GetPropTypes";


export const validateTemplate = (template: Template, onError?: Function): Promise<Template> => {
    return new Promise((resolve, reject) => {

        if (!template.title || template.title.length == 0) {
                                    
            // lil bit of UX here, show the title page
            onError && onError({
                field: 'title'
            });

            // reject with a nice user friendly message
            return reject({
                type: 'validation',
                detail: 'validation/title/missing',
                message: `This template needs a title`
            })
        }

        if (!template.key || template.key.length == 0) {
                                    
            // lil bit of UX here, show the title page
            onError && onError({
                field: 'key'
            });

            // reject with a nice user friendly message
            return reject({
                type: 'validation',
                detail: 'validation/key/missing',
                message: `This template needs a key`
            })
        }

        if (!template.templateType || template.templateType == 0) {
            // lil bit of UX here, show the template type page
            onError && onError({
                field: 'templateType'
            });

            // reject with a nice user friendly message
            return reject({
                type: 'validation',
                detail: 'validation/templateType/missing',
                message: `This template needs a template type`
            })
        }

        if (!template.baseTemplate || template.baseTemplate.length == 0) {
            // lil bit of UX here, show the title page
            onError && onError({
                field: 'baseTemplate'
            })

            // reject with a nice user friendly message
            return reject({
                type: 'validation',
                detail: 'validation/baseTemplate/missing',
                message: `Please choose a base template`
            })
        }

        if (template.id) {
            // skips the first time validation on templates
            return resolve(template);
        }

        if (!template.bodyJson || template.bodyJson.length == 0) {
            return reject({
                type: 'validation', 
                detail: 'validation/template/incorrectBaseTemplate',
                message: `No changes made against the regions, please configure the template`
            })
        }

        const templateJson: TreeComponentItem = JSON.parse(template.bodyJson && template.bodyJson.length != 0 ? template.bodyJson : '{}') ?? {};


        if (!templateJson.t || templateJson.t.length == 0) {
            return reject({
                type: 'validation', 
                detail: 'validation/template/incorrectBaseTemplate',
                message: `Invalid root template, this base template may no longer exist. `
            })
        }

        getTemplates().then(baseTemplates => {
            const baseTemplate = baseTemplates.find(tpl => tpl.name === templateJson.t)

            if (!baseTemplate) {
                return reject({
                    type: 'validation',
                    detail: 'validation/template/incorrectBaseTemplate',
                    message: `Invalid root template, this base template may no longer exist. `
                })
            }

            const rootTemplateProps = getPropsForComponent(baseTemplate.types)

            if (!rootTemplateProps) {
                return reject({
                    type: 'validation',
                    detail: 'validation/template/incorrectBaseTemplate',
                    message: `This template has no configurable fields`
                })
            }

            // make sure all of the required props exist :P
            for(var prop of rootTemplateProps){
                if (templateJson.r) {
                    if (!templateJson.r[prop.name] && !prop.optional) {
                        return reject({
                            type: 'validation',
                            detail: 'validation/template/requiredField',
                            message: `The placeholder ${prop.name} is not optional, but has no component, please check the entry`
                        })
                    }
                }
            }

            // after all this validation (make sure all fail cases return...)
            resolve(template);

        })

    })
}

export const sortComponentOrder = (paths: string[]): string[] => {
    return [...paths].sort((a, b) => {
        const [groupA, restA] = a.split(/\/(.+)/); // ['UI', 'Paginator']
        const [groupB, restB] = b.split(/\/(.+)/);

        // Prioritize UI before Email
        if (groupA !== groupB) {
            if (groupA === 'UI') return -1;
            if (groupB === 'UI') return 1;
            return groupA.localeCompare(groupB);
        }

        // Same group: sort by the next segment
        return restA.localeCompare(restB);
    });
}