import { MediaRef } from 'Api/Upload';
import Link from 'UI/Link';

export type ImageSize      = "original" | number | string; // add more as necessary
export type ImageAlignment = "None" | "Left" | "Right" | "Center";

export type ImageProps = {
    onClick?: React.MouseEventHandler<HTMLImageElement>, 
    fileRef: FileRef, 
    linkUrl?: string, 
    size?: ImageSize, 
    fullWidth?: boolean, 
    float?: ImageAlignment, 
    className?: string,
	animation?: string, 
    animationDirection?: string, 
    animationDuration?: number,
	width?: number, 
    height?: number, 
    title?: string
}


const Image: React.FC<ImageProps> = (props: ImageProps): React.ReactNode => {
    
    const {
        fileRef,
        width, 
        height
    } = props;


    // join class names as opposed to += 
    const classNames:  string[]  = [props.className ?? ''];
    let animation: string | null = null; 

    if (fileRef === null) {
        // no file ref, so don't render the image.
        console.error('[ERROR] Image: No fileRef provided.');
        return;
    }

    // adds an alignment CSS class to the image
    if (props.float && props.float !== "None") {
        // avoided the horrible switch case that existed before.
        classNames.push("image-" + props.float.toLowerCase());
    }

    // any animation functionality.
    if (props.animation) {
        animation = props.animation;
        animation += "-" + (props.animationDirection ?? "up");
    }

    if (props.fullWidth || props.size === "original" && width && !isNaN(width)) {
        classNames.push("image-wide");
    }

    const src = typeof fileRef === 'string' ? fileRef : (fileRef as MediaRef).url;

    if (!props.linkUrl) {
        return (
            <div 
                className={classNames.join(" ")}
                onClick={props.onClick}
                data-aos={animation}
                data-aos-once={Boolean(animation)}
                data-aos-duration={props.animationDuration || 400}
            >
                <img
                    src={src}
                    alt={props.title}
                    width={width}
                    height={height}
                />
            </div>
        )
    }
    return (
        <Link
            href={props.linkUrl}
        >
            <div 
                className={classNames.join(" ")}
                onClick={props.onClick} 
            >
                <img
                    src={src}
                    alt={props.title}
                    width={width}
                    height={height}
                />
            </div>
        </Link>
    )
}

export default Image;