import { useEffect, useState } from 'react';


const useApi = <T,>(loader: () => Promise<T>, deps?: React.DependencyList) => {
    const state = useState<T>(() => {
        // Todo: attempt to obtain data from SSR content response
        return null as T;
    });

    const [current, setCurrent] = state;

    useEffect(() => {
        loader()
            .then(val => {
                if (!val) {
                    return;
                }
                setCurrent(val)
            });
    }, deps);

    return state;
};


export default useApi;
