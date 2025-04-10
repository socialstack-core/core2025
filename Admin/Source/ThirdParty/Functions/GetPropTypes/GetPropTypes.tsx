import { getJson } from 'UI/Functions/WebRequest';

export interface TypeMeta {
    /**
     * The code modules in the meta, indexed by module path.
     */
    codeModules: Record<string, CodeModuleMeta>
}

/**
 * A particular type definition within a code module.
 */
export interface CodeModuleType {
    /**
     * What this type is. 'string', 'function' etc.
     */
    name: string;

    /**
     * If it's a function, this is e.g. "React.FC". 
     * It's the name of this specific instance of the thing.
     */
    instanceName?: string;

    /**
     * True if this is a built in type such as string, boolean etc.
     */
    builtIn?: boolean;

    /**
     * If this is an interface or type (builtIn is false), the set of fields on the type.
     */
    fields?: CodeModuleTypeField[];

    /**
     * If name=variable or it's an export, detail represents the type that the var is being last set to.
     */
    detail?: CodeModuleType;

    /**
     * Generic parameters if this type has any.
     */
    genericParameters?: CodeModuleType[];

    /**
     * If this type extends other types, the set of those types.
     */
    extends?: CodeModuleType[];
}

/**
 * A field on an interface or type.
 */
export interface CodeModuleTypeField {
    /**
     * The name of this field.
     */
    name: string;

    /**
     * True if field is optional.
     */
    optional: boolean;

    /**
     * The type of this field.
     */
    fieldType: CodeModuleType;
}

export interface CodeModuleMeta {
    /**
     * The raw types in this file. Defines both the exported things and the raw interfaces and type defs.
     */
    types: CodeModuleType[];

    /**
     * The established propType set. Null if this module does not appear to export a react function.
     */
    propTypes?: Record<string, PropTypeMeta>;
}

export interface PropTypeMeta {
    /**
     * The field type.
     */
    type: CodeModuleType
}

function expandVariable(type: CodeModuleType) {
    if (type.name == 'variable') {
        if (!type.detail) {
            return null;
        }

        return expandVariable(type.detail);
    }

    return type;
}

function setupModuleMeta(meta: TypeMeta, moduleName: string, module : CodeModuleMeta) {

    // Get the export called 'default'.
    var defaultExport = module.types.find(t => t.name == 'export' && t.instanceName == 'default');

    if (!defaultExport || !defaultExport.detail) {
        module.propTypes = undefined;
        return;
    }

    var type = expandVariable(defaultExport.detail);

    if (!type) {
        module.propTypes = undefined;
        return;
    }

    // To collect the proptypes, we're looking for type
    // to be a function called 'FC' or 'React.FC' with 1 arg.
    if (
        !(type.name == 'function' || type.name == 'identifier') ||
        !(type.instanceName == 'FC' || type.instanceName == 'React.FC')
        || !type.genericParameters || !type.genericParameters.length
    ) {
        module.propTypes = undefined;
        return;
    }

    // It can potentially have other types, like React.PropsWithChildren, nested in the generic params.
    var result: Record<string, PropTypeMeta> = {};
    expandPropTypes(meta, module, type, result);
    module.propTypes = result;
}

/**
 * Expands a type via its generic types and also any types it extends.
 * @param type
 */
function expandPropTypes(meta: TypeMeta, module: CodeModuleMeta, type: CodeModuleType, propTypes: Record<string, PropTypeMeta>) {
    if (!type) {
        return;
    }

    if ((type.name == 'function' || type.name == 'identifier') && type.genericParameters?.length) {
        if (type.instanceName == 'React.PropsWithChildren' || type.instanceName == 'PropsWithChildren') {

            // Add children prop:
            propTypes['children'] = {
                type: {
                    name: 'identifier',
                    instanceName: 'React.ReactNode'
                }
            };

        }

        // Expand the next set:
        expandPropTypes(meta, module, type.genericParameters[0], propTypes);
    } else if (type.name == 'identifier' && type.instanceName) {

        // Lookup this type locally first. If it gets no hits, then
        // we'll find it globally instead.
        var interfaceType = module.types.find(t => (t.name == 'class' || t.name == 'interface') && t.instanceName == type.instanceName);

        if (!interfaceType) {
            interfaceType = findTypeGlobally(type.instanceName, meta);
        }

        if (!interfaceType) {
            return;
        }

        // Read its fields next and add each to the propTypes set.
        expandPropTypes(meta, module, interfaceType, propTypes);

    } else if (type.name == 'class' || type.name == 'interface') {

        // If it extends anything, add those first.
        if (type.extends) {
            for (var i = 0; i < type.extends.length; i++) {
                expandPropTypes(meta, module, type.extends[i], propTypes);
            }
        }

        // Add the explicit fields next.
        if (type.fields) {
            for (var i = 0; i < type.fields.length; i++) {
                var field = type.fields[i];

                var fieldType = field.fieldType;

                if (fieldType.name == 'identifier' && fieldType.instanceName) {
                    // Locate this type and ref it instead.
                    var fieldTypeObj = module.types.find(t => (t.name == 'class' || t.name == 'interface' || t.name == 'union') && t.instanceName == fieldType.instanceName);

                    if (!fieldTypeObj) {
                        fieldTypeObj = findTypeGlobally(fieldType.instanceName, meta);
                    }

                    if (fieldTypeObj) {
                        fieldType = fieldTypeObj;
                    }
                }

                propTypes[field.name] = {
                    type: fieldType
                } as PropTypeMeta;
            }
        }

    }

}

/**
 * Searches for a type by name globally.
 */
function findTypeGlobally(name: string, meta: TypeMeta) {
    for (var k in meta.codeModules) {
        var module = meta.codeModules[k];
        var interfaceType = module.types.find(t => (t.name == 'class' || t.name == 'interface') && t.instanceName == name);
        if (interfaceType) {
            return interfaceType;
        }
    }

    return undefined;
}

/**
 * Loads the type-meta cache.
 * @returns
 */
function loadCache() {
    if (_cache) {
        return Promise.resolve(_cache);
    }

    if (!_cacheLoader) {
        _cacheLoader = getJson<TypeMeta>("/pack/type-meta.json")
        .then(json => {
            for (var k in json.codeModules) {
                var module = json.codeModules[k];
                setupModuleMeta(json, k, module);

                if (k == 'UI/Image') {
                    console.log(module);
                }
            }
            _cache = json;
            return json;
        });
    }

    return _cacheLoader;
}

let _cacheLoader: Promise<TypeMeta>;
let _cache : TypeMeta | null = null;

/**
 * Loads the prop type data. Generally call this once when e.g. the canvas editor starts up.
 * @returns
 */
const getAll = () => loadCache();

export {
    getAll
}

/**
 * Gets the prop types for a named component from the given cache set, which you get once from waiting for getAll.
 * Returns null if there was no match.
 */
export const getPropTypes = (name: string, cache: TypeMeta): Record<string, PropTypeMeta> | null => {
    var module = cache.codeModules[name];

    if (!module || !module.propTypes) {
        return null;
    }

    return module.propTypes;
};

export type TemplateModule = {
    name:string,
    types: CodeModuleMeta
}

export const getTemplates = async (): Promise<TemplateModule[]> => {
    return new Promise((res, rej) => {
        
        getAll().then((result) => {

            const { codeModules } = result;

            const resolution: TemplateModule[] = [];

            Object.keys(codeModules)
                    .filter(key => key.startsWith('UI/Templates'))
                    .forEach(key => {
                        resolution.push({
                            name: key,
                            types: codeModules[key]
                        })
                    })
            res(resolution)
        })
        .catch(rej)

    })
} 