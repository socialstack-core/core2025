import roleApi from 'Api/Role';

/* cache */
var cache = null;

/*
* Gets the list of content types on the site. Returns a promise.
*/
export default (capName, session) => {
	if(session.loadingUser){
		return session.loadingUser.then(() => loadCached(capName, session));
	}
	
	return loadCached(capName, session);
}

function loadCached(capName, session) {
	if(cache != null){
		return Promise.resolve(cache).then(caps => !!caps[capName]);
	}
	
	var roleId = session.user ? session.user.role : 0;
	
	return cache = roleApi.get(roleId).then(role => {
		cache = {};
		role.capabilities.forEach(c => {
			cache[c.name] = c;
		});
		return cache;
	}).then(caps => !!caps[capName]);
}