import AutoEdit from "Admin/Layouts/AutoEdit";
import CreatePage from "Admin/Page/Create";
import { useTokens } from "UI/Token";

const PageEditor: React.FC = (props: any) => {

    const id = useTokens(props.id)

    const isEditPage = id.length != 0;

    if (isEditPage) {

        return (
            <AutoEdit
                contentType= "Page"
                singular= "Page"
                id= {id.toString()}
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