import autoformApi from 'Api/Autoform';

/* cache */
var cache = null;

/*
* Gets the list of content types on the site. Returns a promise.
*/
export default () => {
	if(cache != null){
		return Promise.resolve(cache);
	}
	
	return cache = autoformApi.all().then(response => {
		return cache = response.json.contentTypes;
	});
}