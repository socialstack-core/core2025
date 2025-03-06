/**
 * Props for the Html component.
 */
interface HtmlProps extends React.HTMLAttributes<HTMLSpanElement> {
    /**
     * The HTML to display.
     */
    content: string
}

/**
 * This component displays html. Only ever use this with trusted text.
*/
const Html: React.FC<HtmlProps> = ({content, ...props}) => {
    return <span dangerouslySetInnerHTML={{ __html: content }} {...props} />;
}

export default Html;