import { CodeModuleMeta, CodeModuleType, getAll, TemplateModule } from "Admin/Functions/GetPropTypes";
import { RegionCanvasTreeNode, RegionConfiguration } from "./types";

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
                    "PropsWithChildren"
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

/**
 * This takes multiple configs and bundles them together in an overlaying manner.
 * @param baseConfig 
 * @param overlayingConfig 
 * @returns 
 */
export const compileRegionConfig = (baseConfig: RegionConfiguration | null = null, overlayingConfig: RegionConfiguration): RegionConfiguration => {

    return {
        /**
         * isLocked can be set by the parent template locking, or by the current template. This differs from isLockedByParent 
         * as isLockedByParent only marks if the isLocked field is a result of the parent template.
        */
        isLocked: baseConfig?.isLocked ?? overlayingConfig.isLocked ?? false,
        /**
         * Templates can have > 1 inheritence chain length, which means we need to check if the parent template of the parent has locked this,
         * the parent, or simply just the editor.
         */
        isLockedByParent: baseConfig?.isLocked ?? baseConfig?.isLockedByParent ?? false,
        /**
         * The editor label should be set in the baseConfig, but baseConfig can be null. If a name
         * cannot be resolved, we fallback to 'Unnamed region'.
         */
        editorLabel: baseConfig?.editorLabel ?? overlayingConfig.editorLabel ?? `Unnamed region`,
        /**
         * When left empty, allowed components is limited strictly to the children allowed to the current role.
         * the restriction on these get tighter the further down the chain we go.
         */
        allowedComponents: baseConfig?.allowedComponents ?? overlayingConfig.allowedComponents ?? [],
        /**
         * Does this region support more than one child?
         */
        childrenSupported: baseConfig?.childrenSupported ?? overlayingConfig.childrenSupported ?? false,
        /**
         * Are children allowed to be added to this region?
         */
        childrenAllowed: baseConfig?.childrenAllowed ?? overlayingConfig.childrenAllowed ?? true,
        /**
         * If multiple children are allowed, this will be true. If not, then false.
         */
        multipleChildrenAllowed: baseConfig?.multipleChildrenAllowed ?? overlayingConfig.multipleChildrenAllowed ?? true,

        /**
         * Is a region or child optional?
         */
        isOptional: baseConfig?.isOptional ?? overlayingConfig.isOptional ?? false
    }

}