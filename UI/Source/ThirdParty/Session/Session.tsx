import webRequest, {expandIncludes} from "UI/Functions/WebRequest";
import { createContext, useContext, useState } from 'react';
import userApi from 'Api/User';

let initState : Session | undefined = undefined;

if(window.gsInit){
	initState = window.gsInit;
	
	for(var k in initState){
		// initState[k] = expandIncludes(initState[k]);
	}
	
	initState.loadingUser = undefined;
}

interface SessionContext {
	session: Session,
	sessionReload?: () => Promise<Session>,
	setSession: (s:Session) => void
}

const sessionCtx = createContext<SessionContext>({
	session: {},
	setSession: (s) => { }
});

export function useSession(){
	return useContext(sessionCtx);
}

interface SessionProviderProps {
	initialState?: Session
}

export const Provider: React.FC<React.PropsWithChildren<SessionProviderProps>> = (props) => {

	let dispatchWithEvent = (updatedVal: Session, diff?: boolean) => {
		if (diff) {
			updatedVal = { ...session, ...updatedVal };
		}

		for (var k in updatedVal) {
			updatedVal[k] = expandIncludes(updatedVal[k]);
		}

		var e = new CustomEvent('xsession', {
			detail: {
				state: updatedVal,
				setSession
			}
		});

		document.dispatchEvent && document.dispatchEvent(e);
		setSession(updatedVal);
		return updatedVal;
	}
	
	let sessionReload = () => userApi.self()
		.then((response) => {
			var state: Session = (response?.json) ? {
				...response.json, loadingUser: undefined
			} as Session : {
				loadingUser: undefined
			};
			dispatchWithEvent(state);
			return state;
		}).catch(() => dispatchWithEvent({
			loadingUser: undefined
		}));
	
	const [session, setSession] = useState(() : Session => {
		var newSession = props.initialState || initState;

		if (!newSession) {
			newSession = {
				loadingUser: sessionReload()
			};
		}

		return newSession;
	});
  
	return (
		<sessionCtx.Provider
			value={{
				session,
				sessionReload,
				setSession: dispatchWithEvent
			}}
		>
			{props.children}
		</sessionCtx.Provider>
	);
};
