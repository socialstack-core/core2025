const HEADING_DEFAULT_LEVEL = 2;

type HeadingLevel = 1 | 2 | 3 | 4 | 5 | 6;

interface HeadingProps {
	/**
	 * heading level (1-6)
	 */
	level?: HeadingLevel,
	/**
	 * optional additional class name(s)
	 */
	className?: string,
	/**
	 * optionally animate heading into place on scroll
	 */
	animation?: AnimationProps
}

const Heading: React.FC<React.PropsWithChildren<HeadingProps>> = (props) => {
	const { level, className, animation, children } = props;

	var Tag = `h${level || HEADING_DEFAULT_LEVEL}` as React.ElementType;
	var componentClasses = ['heading'];

	if (className) {
		componentClasses.push(className);
	}

	let anim : string | undefined = animation ? (animation.type === 'none' ? undefined : animation.type) : undefined;

	if (anim) {

		switch (anim) {
			case 'fade':
			case 'flip':
			case 'slide':
				// possible for animation.direction to be undefined? may need to set a default direction here
				anim = `${anim}-${animation?.direction}`;
				break;

			// TODO
			//case 'fade-in':
			//case 'fade-out':

			case 'zoom-in':
			case 'zoom-out':
				// no direction required, use as-is
				break;

		}

	}

	return <Tag className={componentClasses.join(' ')} data-aos={anim}>
		{children}
	</Tag>;
}

export default Heading;