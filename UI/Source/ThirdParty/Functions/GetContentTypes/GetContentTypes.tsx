import autoformApi, { ContentType } from 'Api/Autoform';

/* cache */
var cache: ContentType[] | null = null;

/*
* Gets the list of content types on the site. Returns a promise.
*/
export default () => {
	if(cache){
		return Promise.resolve(cache);
	}
	
	return cache = autoformApi.all()
		.then(structure => {
			return cache = structure.contentTypes;
		});
}