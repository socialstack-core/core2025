import { useEffect, useState } from 'react';


const useApi = <T,>(loader: () => Promise<T>, deps?: React.DependencyList) => {
    const state = useState<T>(() => {
        // Todo: attempt to obtain data from SSR content response
        return null as T;
    });

    const [current, setCurrent] = state;

    const dependencies:React.DependencyList = deps ?? [];

    useEffect(() => {
        loader()
            .then(val => {
                if (!val) {
                    return;
                }
                setCurrent(val)
            });

    // [Lint disabled here] 
    // Reason: the dependency array expects an array literal, we're
    // passing dependencies here from the method which
    // renders this impossible.
    // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [loader, setCurrent, ...dependencies]);

    return state;
};


export default useApi;
