import AutoEdit from "Admin/Layouts/AutoEdit";
import CreatePage from "Admin/Page/Create";
import { useTokens } from "UI/Token";

const PageEditor: React.FC = (props: any) => {
	const { content } = props;
    const isEditPage = !!content;

    if (isEditPage) {

        return (
            <AutoEdit
                contentType= "Page"
                singular= "Page"
                content= {content}
                plural= "pages"
            />
        )
    }
	
    return (
        <div className='page-editor'>
            <CreatePage />
        </div>
    )

}

export default PageEditor;