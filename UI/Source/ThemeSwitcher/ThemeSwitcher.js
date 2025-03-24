import { useState, useEffect } from 'react'; 
import store from 'UI/Functions/Store';

export default function ThemeSwitcher(props) {
	const THEME_KEY = 'core-test-theme';
	const [mode, setMode] = useState(store.get(THEME_KEY) || 'system');

	useEffect(() => {
		store.set(THEME_KEY, mode);

		let html = document.querySelector("html");

		if (html) {

			switch (mode) {
				case 'light':
					html.classList.remove("dark-mode");
					html.classList.add("light-mode");
					break;

				case 'dark':
					html.classList.remove("light-mode");
					html.classList.add("dark-mode");
					break;

				default:
					html.classList.remove("light-mode");
					html.classList.remove("dark-mode");
					break;
			}

		}

	}, [mode]);

	return (
		<menu className="theme-switcher">
			<li>
				<button type="button" className={`btn btn--primary ${mode !== 'light' ? 'btn--outline' : ''}`} onClick={() => setMode('light')}>
					<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
						<circle cx="12" cy="12" r="4" />
						<path d="M12 2v2" />
						<path d="M12 20v2" />
						<path d="m4.93 4.93 1.41 1.41" />
						<path d="m17.66 17.66 1.41 1.41" />
						<path d="M2 12h2" />
						<path d="M20 12h2" />
						<path d="m6.34 17.66-1.41 1.41" />
						<path d="m19.07 4.93-1.41 1.41" />
					</svg>
				</button>
				<button type="button" className={`btn btn--primary ${mode !== 'dark' ? 'btn--outline' : ''}`} onClick={() => setMode('dark')}>
					<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
						<path d="M12 3a6 6 0 0 0 9 9 9 9 0 1 1-9-9Z" />
					</svg>
				</button>
				<button type="button" className={`btn btn--primary ${mode !== 'system' ? 'btn--outline' : ''}`} onClick={() => setMode('system')}>
					<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
						<circle cx="12" cy="12" r="10" />
						<path d="M12 18a6 6 0 0 0 0-12v12z" />
					</svg>
				</button>
			</li>
		</menu>
	);
}
