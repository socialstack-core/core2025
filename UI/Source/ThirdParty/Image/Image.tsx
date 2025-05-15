import Link from 'UI/Link';
import { getUrl } from 'UI/FileRef';

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

    var imgUrl = getUrl(props.fileRef);

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
                    src={imgUrl}
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
                    src={imgUrl}
                    alt={props.title}
                    width={width}
                    height={height}
                />
            </div>
        </Link>
    )
}

export default Image;


// TODO: Rework all of this originally from getRef.
// GetRef is now back to its roots and just returns URLs - aka <Image> is expected to render images
// directly rather than asking getRef to maybe do it, and 
//having an awkward time passing various props through getRef to accomodate it.

/**
 * 
 * import { decode, drawImageDataOnNewCanvas } from 'UI/Functions/Blurhash';
import getConfig from 'UI/Config';

const BLURHASH_WIDTH = 64;
const BLURHASH_HEIGHT = 36;
const MIN_SRCSET_WIDTH = 256;


function displayImage(url, options) {
/**
* 
if (video) {
var videoSize = options.size || 256;
return <>
    <video className="responsive-media__video" src={url}
        width={options.isPortrait ? undefined : videoSize} height={options.isPortrait ? videoSize : undefined}
        loading={options.lazyLoad == false ? undefined : "lazy"} controls {...options.attribs} />
</>;
}

*
var imgSize = options.size || undefined;
var renderedWidth = imgSize || (options.args && options.args.w ? options.args.w : undefined);
var ImgWrapperTag = options.portraitRef ? 'picture' : 'div';
var hasWrapper = options.forceWrapper || false;
var wrapperStyle : React.CSSProperties = {};
 
if (options.fx && options.fy && !(options.fx == 50 && options.fy == 50)) {
wrapperStyle.backgroundPosition = `${options.fx}% ${options.fy}%`;
}

// check - blurhash available?
if (options.blurhash && !window.SERVER) {
var imgData = decode(options.blurhash, options.blurhash_width, options.blurhash_height);
var canvas = drawImageDataOnNewCanvas(imgData, options.blurhash_width, options.blurhash_height);
wrapperStyle.backgroundImage = `url(${canvas.toDataURL()})`;
hasWrapper = true;
}

// if we need to support art direction (e.g. portrait / landscape versions of the same image)
if (options.portraitRef) {
hasWrapper = true;
}

var width = options.isPortrait ? undefined : renderedWidth;
var height = options.isPortrait ? renderedWidth : undefined;

if (options.forceWidth) {
width = options.forceWidth;
}

if (options.forceHeight) {
height = options.forceHeight;
}

if (width) {
wrapperStyle.width = `${width}px`;
}

if (height) {
wrapperStyle.height = `${height}px`;
}

if (options.figure) {
ImgWrapperTag = 'figure';
hasWrapper = true;
}

var imgWidth = hasWrapper ? undefined : width;
var imgHeight = hasWrapper ? undefined : height;

var authorText = (options.author && options.author.length > 0) ? options.author : '';

var altText = options.alt;
if (altText == undefined || altText.length == 0) {
if (options.attrib && options.attribs.alt != undefined && options.attribs.alt.length > 0) {
    altText = options.attribs.alt;
} else if (options.altText != undefined && options.altText.length > 0) {
    altText = options.altText;
    if (authorText.length > 0) {
        altText += ` by ` + authorText;
    }
}
}

if (options.forceWidth) {
imgWidth = width;
}

if (options.forceHeight) {
imgHeight = height;
}

if ((options.portraitRef && options.portraitSrcset && options.responsiveSizes) || (options.landscapeSrcset && options.responsiveSizes)) {
ImgWrapperTag = "picture";
}

// setup structured data for the image based on the author
var structuredDataHeader = (authorText && authorText.length > 0) ? { "itemscope": "", itemtype: "https://schema.org/ImageObject" } : {};
var structuredDataItem = (authorText && authorText.length > 0) ? { itemprop: "contentUrl" } : {};

/*
var imageClass = ['responsive-media__image'];

if (options.attribs && options.attribs.className) {
imageClass.push(options.attribs.className);
}

var img = <img className={imageClass.join(' ')} src={url} srcset={hasWrapper ? undefined : options.landscapeSrcset}
*
	
var img = <img className="responsive-media__image" src={url} srcset={hasWrapper ? undefined : options.landscapeSrcset}
style={options.fx && options.fy && !(options.fx == 50 && options.fy == 50) ? { 'object-position': `${options.fx}% ${options.fy}%` } : undefined}
width={imgWidth}
height={imgHeight}
fetchPriority={options.fetchPriority}
loading={options.lazyLoad == false ? undefined : "lazy"}
{...options.attribs}
alt={altText}
onError={options.hideOnError ? (e) => {
    e.currentTarget.style.display = 'none';
} : undefined}
{...structuredDataItem}
/>;

if (hasWrapper) {
var wrapperClass = ['responsive-media__wrapper'];

if (options.attribs && options.attribs.wrapperClassName) {
    wrapperClass.push(options.attribs.wrapperClassName);
}

return (
    // support art direction / blurhash background
    <ImgWrapperTag {...structuredDataHeader} className={wrapperClass.join(' ')} style={wrapperStyle}>
        {options.portraitRef && options.portraitSrcset && options.responsiveSizes && <>
            <source
                media="(orientation: portrait)"
                srcset={options.portraitSrcset}
                sizes={options.responsiveSizes}
            />
        </>}
        {options.landscapeSrcset && options.responsiveSizes && <>
            <source
                media={options.portraitRef ? "(orientation: landscape)" : undefined}
                srcset={options.landscapeSrcset}
                sizes={options.responsiveSizes}
            />
        </>}
        {img}
        {options.figure && options.figCaption && <figcaption className="responsive-media__caption">
            {options.figCaption}
            {structuredDataHeader && structuredDataHeader.itemtype &&
            <>
                <span itemprop="creator" itemtype="https://schema.org/Person" itemscope>
                    <meta itemprop="name" content={authorText} />
                </span>
                <p itemprop="copyrightNotice">© {new Date().getFullYear()} - <span itemprop="creditText">{authorText}</span></p>
            </>
        }
        </figcaption>}
    </ImgWrapperTag>
);
}

// plain image with author data
if (structuredDataHeader && structuredDataHeader.itemtype)
{
return (
    <div {...structuredDataHeader}>
        {img}
        <span itemprop="creator" itemtype="https://schema.org/Person" itemscope>
            <meta itemprop="name" content={authorText} />
        </span>
        <span style='display:none;' itemprop="copyrightNotice">{authorText}</span>
        <span style='display:none;' itemprop="creditText">{authorText}</span>
    </div>
);
}

// basic image
return img;
}



// portrait image (or video) check
// (used within handler to determine which of height/width attributes are set)
if (options.portraitCheck && isImage && !isIcon &&
r.args && r.args.w && r.args.h && r.args.w < r.args.h) {
options.isPortrait = true;
}

// image handling
if (isImage && !isIcon && r.fileType && r.fileType.toLowerCase() != 'svg') {
options.responsiveSizes = getSizes(options, r);
options.landscapeSrcset = getSrcset(options, r);

if (options.portraitRef) {
    var rp = getRef.parse(options.portraitRef);
    options.portraitSrcset = getSrcset(options.portraitRef, options, rp);
}

// focal point check
if (!(r.focalX == 50 && r.focalY == 50)) {
    options.fx = r.focalX;
    options.fy = r.focalY;
}

// blurhash check
if (r.args && r.args.b) {
    options.blurhash = r.args.b;
    options.blurhash_width = Math.min(r.args.w || BLURHASH_WIDTH, BLURHASH_WIDTH);
    options.blurhash_height = Math.min(r.args.h || BLURHASH_HEIGHT, BLURHASH_HEIGHT);
}

// author
if (r.author.length > 0) {
    options.author = r.author;
}

// alternate name
if (r.altText.length > 0) {
    options.altText = r.altText;
}
}



function getSupportedSizes() {
var uploaderConfig = getConfig<UploaderConfig>('uploader');
var sizes = uploaderConfig?.imageSizes;
return sizes?.filter(size => size >= 256);
}

function getSrcset(options, r) {

if (!r) {
return '';
}

var supportedSizes = getSupportedSizes();
var width = options.size || ((r.args && r.args.w) ? r.args.w : undefined);

if (!width) {
width = supportedSizes[supportedSizes.length - 1];
}

var srcset = [];

supportedSizes.forEach(size => {

if (size >= MIN_SRCSET_WIDTH && size <= width) {
    var url = r.handler(r.basepath, { size: size, url: true }, r);
    srcset.push(`${url} ${size}w`);
}

});

return srcset.length < 2 ? undefined : srcset.join(',');
}

function getSizes(options, r) {
var supportedSizes = getSupportedSizes();
var width = options.size || ((r.args && r.args.w) ? r.args.w : undefined);

if (!width) {
width = supportedSizes[supportedSizes.length - 1];
}

var sizes = [];

supportedSizes.forEach((size, i) => {

if (size >= MIN_SRCSET_WIDTH && size <= width) {
    sizes.push(i == supportedSizes.length - 1 ?
        `${size}px` :
        `(max-width: ${size}px) ${size}px`);
}

});

return sizes.length < 2 ? undefined : sizes.join(',');
}

 * 
 */