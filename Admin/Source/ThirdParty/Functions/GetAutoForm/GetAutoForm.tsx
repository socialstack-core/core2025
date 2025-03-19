import autoFormApi, { AutoFormInfo } from 'Api/AutoForm';

interface CachedForm {
	form: AutoFormInfo,
	canvas: string
}

/* Autoform cache */
var cache: Record<string, Record<string, CachedForm>> = {};

/*
* Gets info for a particular autoform. Returns a promise.
* Type is the autoform type, usually "content" or e.g. "config".
* Name is the form name, e.g. "content","user"
*/
export default (type: string, name : string) => {
	if(!cache[type]){
		cache[type] = {} as Record<string, CachedForm>;
	}
	
	name = name.toLowerCase();
	
	if(cache[type][name]){
		return Promise.resolve(cache[type][name]);
	}

	return autoFormApi.get(type, name).then(form => {
		var cacheInfo: CachedForm = {
			form, canvas: JSON.stringify({ content: form.fields })
		};
		cache[type][name] = cacheInfo;
		return cacheInfo;
	});
}

var gotAll = false;

/*
export function getAllContentTypes() {
	
	if(gotAll){
		return Promise.resolve(cache.content);
	}
	
	return autoFormApi.allContentForms().then(response => {
		gotAll = true;
		
		cache.content = {};
		var byEndpoint = {};
		var forms = response.forms;
		
		forms.forEach(form => {
			byEndpoint[form.endpoint] = form;
		});
		
		var types = response.contentTypes;
		
		types.forEach(type => {
			var lcName = type.name.toLowerCase();
			var form = byEndpoint['v1/' + lcName];
			
			if(!form){
				return;
			}
			
			cache.content[lcName] = {form, name: type.name, canvas: JSON.stringify({content: form.fields})};
		});
		
		return cache.content;
	});
	
}
*/