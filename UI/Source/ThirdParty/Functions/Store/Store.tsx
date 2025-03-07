var store = window.localStorage;

/**
 * Gets the given object by textual key name from the store.
 * @param key
 * @returns
 */
function get<T>(key : string){
	var val = store.getItem(key);
	return val ? JSON.parse(val) as T : null;
}

/**
 * Sets the given value in to the store by JSON stringifying it.
 * @param key
 * @param value
 */
function set(key : string, value : any){
	store.setItem(key, JSON.stringify(value));
}

/**
 * Removes the given key from the store.
 * @param key
 */
function remove(key : string){
	store.removeItem(key);
}

export default {get, set, remove};