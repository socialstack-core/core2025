import Button from 'UI/Button';
import { RefObject } from 'react';

interface RequestFullscreenProps {
	elementRef: RefObject<HTMLElement>;
}

export default function RequestFullscreen({ elementRef }: RequestFullscreenProps) {

	function toggleFullscreen() {
		if (elementRef.current) {
			let element = elementRef.current;

			if (document.fullscreenElement) {
				element.classList.remove('overflow-y--auto');
				document.exitFullscreen();
				return;
			}

			element.classList.add('overflow-y--auto');

			if (element.requestFullscreen) {
				element.requestFullscreen();
			} else if ((element as any).mozRequestFullScreen) {
				(element as any).mozRequestFullScreen();
			} else if ((element as any).webkitRequestFullscreen) {
				(element as any).webkitRequestFullscreen();
			} else if ((element as any).msRequestFullscreen) {
				(element as any).msRequestFullscreen();
			}
		}
	}

	return (
		<Button outline className="request-fullscreen" onClick={() => toggleFullscreen()}>
			<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
				<path d="M3 7V5a2 2 0 0 1 2-2h2" />
				<path d="M17 3h2a2 2 0 0 1 2 2v2" />
				<path d="M21 17v2a2 2 0 0 1-2 2h-2" />
				<path d="M7 21H5a2 2 0 0 1-2-2v-2" />
				<rect width="10" height="8" x="7" y="8" rx="1" />
			</svg>
			{`Toggle fullscreen`}
		</Button>
	);
}
