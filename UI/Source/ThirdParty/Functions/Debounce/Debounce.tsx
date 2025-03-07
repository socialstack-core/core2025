export default class Debounce<T> {

	delay: number;
	timer: number;
	onRun: (item: T) => void;

	constructor(func: (item: T) => void, delay : number) {
		this.onRun = func;
		this.delay = delay || 250;
		this.timer = 0;
	}
	
	handle(args : T){
		this.timer && clearTimeout(this.timer);

		this.timer = setTimeout(() => {
			 this.onRun(args);
		}, this.delay);
	}
}