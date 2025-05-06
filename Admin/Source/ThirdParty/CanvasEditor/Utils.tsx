import { CodeModuleMeta, isJsx } from 'Admin/Functions/GetPropTypes';

type RootProp = {
	name: string
};

/**
 * Gets the list of props which are roots from the given prop type info.
 * @param type
 * @returns
 */
export function getRootInfo(type: CodeModuleMeta): RootProp[] {

	if (!type || !type.propTypes)
	{
		return [];
	}
	var {propTypes} = type;
	
	if(!propTypes){
		return [];
	}
	
	var rootInfo = [];
	
	for(var name in propTypes){
		var info = propTypes[name];

		if (isJsx(info)) {
			rootInfo.push({ name } as RootProp);
		}
	}
	
	return rootInfo;
}