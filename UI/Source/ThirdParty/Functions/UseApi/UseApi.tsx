import { useEffect, useState } from 'react';


const useApi = <T,>(loader: () => Promise<T>, deps?: React.DependencyList) => {
    const state = useState<T>(() => {
        // SSR hydration logic placeholder
        return null as T;
    });

    const [current, setCurrent] = state;

    useEffect(() => {
        let isCurrent = true;

        loader().then(val => {
            if (!val || !isCurrent) return;
            setCurrent(val);
        });

        return () => {
            isCurrent = false;
        };
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, deps);

    return state;
};


export default useApi;
