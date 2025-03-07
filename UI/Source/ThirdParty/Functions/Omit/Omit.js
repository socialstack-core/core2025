export default function (object, keyOrKeys, noDefaults) {

	// Omit is frequently used to leave out particular fields, but object destructuring is now widely supported and effectively means the JS engine can do it
	// whilst also simultaneously preserving type safety.
	//
	// Previous usage: omit(props, 'b')
	// Destructure instead: {b, ...props}
	throw new Error('Use object destructuring instead of Omit. Omit will shortly be deleted. Find this error for deets!');
}