import AutoEdit from "Admin/Layouts/AutoEdit";
import CreatePage from "Admin/Page/Create";

const PageEditor: React.FC = () => {

    const path = location.pathname.split('/').filter(part => part.length != 0);

    const id = parseInt(
        path[path.indexOf('page') + 1]
    );

    const isEditPage = !Number.isNaN(id);

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