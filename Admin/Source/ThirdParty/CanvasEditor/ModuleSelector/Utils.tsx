import { getAll as getAllPropTypes, TypeMeta, CodeModuleMeta } from 'Admin/Functions/GetPropTypes';

type ComponentSet = {
	modules: ComponentInfo[],
	directories: ComponentDirectory[],
	directoryLookup: Record<string, ComponentDirectory>
};

type ComponentDirectory = {
	name: string,
	modules: ComponentInfo[],
	path: string[]
};

type ComponentInfo = {
	name: string,
	publicName: string,
	props: CodeModuleMeta,
	directory: ComponentDirectory,
	moduleClass: any /** The react render func, but can be either a class or a function */
};

var cachedComps: ComponentSet | null = null;

export function groupByDirectory(modules: ComponentInfo[]): ComponentDirectory[] {
	var lookup: Record<string, ComponentDirectory> = {};
	var dirs: ComponentDirectory[] = [];

	for (var i = 0; i < modules.length; i++) {

		var module = modules[i];
		var dir = module.directory;
		var dirName = dir.name;
		var lookupDir = lookup[dirName];

		if (!lookupDir) { 
			lookupDir = {
				name: dir.name,
				path: dir.path,
				modules: []
			};
			dirs.push(lookupDir);
			lookup[dirName] = lookupDir;
		}

		lookupDir.modules.push(module);
	}

	return dirs;
}

export function collectModules(componentGroupKeys? : string[]) : Promise<ComponentSet> {
	if (cachedComps){
		return Promise.resolve(cachedComps);
	}

	return getAllPropTypes()
	.then(propTypeCache => {
		cachedComps = constructCache(propTypeCache);
		return cachedComps;
	});
}

function constructCache(propTypeCache : TypeMeta) {
	var modules : ComponentInfo[] = [];
	var directoryLookup: Record<string, ComponentDirectory> = {};
	var directories: ComponentDirectory[] = [];

	// __mm is the superglobal used by socialstack 
	// to hold all available modules.
	for (var modName in window.__mm) {
		// Attempt to get React propTypes.
		var moduleFunc = require(modName).default;

		if (!moduleFunc) {
			continue;
		}

		// modName is e.g. UI/Thing
		
		var modulePropInfo = propTypeCache.codeModules[modName];

		if (!modulePropInfo || !modulePropInfo.propTypes) {
			continue;
		}

		// Remove the filename, and get the super group:
		var nameParts = modName.split('/');
		var name = nameParts.pop();
		var directoryName = nameParts.join('/');

		if (!name) {
			continue;
		}

		var directory = directoryLookup[directoryName];

		if (!directory) {
			directory = { name: directoryName, modules: [], path: nameParts };
			directories.push(directory);
			directoryLookup[directoryName] = directory;
		}

		var moduleInfo: ComponentInfo = {
			name,
			publicName: modName,
			directory,
			props: modulePropInfo,
			moduleClass: moduleFunc
		};

		modules.push(moduleInfo);
	}

	// Sort by name
	modules.sort((a, b) => (a.publicName > b.publicName) ? 1 : ((b.publicName > a.publicName) ? -1 : 0));

	// After the sort, such that directory lists are implicitly sorted too.
	modules.forEach(module => module.directory.modules.push(module));

	return {
		modules,
		directories,
		directoryLookup
	};
}