/**
 * Obtains server set config with the given case insensitive name.
 * @param name
 * @returns
 */
export default function getConfig<T>(name: string): T | null {
	var cfg = window.__cfg ? window.__cfg[name.toLowerCase()] : null;
	return cfg as T;
}