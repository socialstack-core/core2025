export default (value: string): PublicError | undefined => {

    /**
     * The JSON structure of an empty document is as shown
     * {
     *   "c": {
     *     "t": "p",
     *     "c": {
     *       "t": "br"
     *     },
     *     "d": {}
     *   }
     * }
     *
     * When the canvas is empty, it serializes to this structure.
     * We want to treat this as invalid/empty content.
     */

    // 1. Check for empty or whitespace-only string
    if (!value || !value.trim()) {
        return {
            type: 'field/required',
            message: 'Canvas content is required'
        };
    }

    // 2. Attempt to parse the string as JSON
    let parsed;
    try {
        parsed = JSON.parse(value);
    } catch {
        // If parsing fails, it's invalid canvas content format
        return {
            type: 'field/required',
            message: 'Incorrect canvas content format'
        };
    }

    // 3. Check if parsed JSON matches the empty canvas structure
    if (
        parsed?.c?.t === 'p' &&
        parsed?.c?.c?.t === 'br'
    ) {
        return {
            type: 'field/required',
            message: `Canvas is empty, but it's a required field`
        };
    }

    // 5. Otherwise, content is valid
    return undefined;
}
