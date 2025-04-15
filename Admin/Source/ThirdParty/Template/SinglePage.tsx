import CreateTemplate from "Admin/Template/Create";
import EditTemplate from "Admin/Template/Edit";

const SinglePage: React.FC<{}> = (): React.ReactElement => {

    const url = location.pathname.split('/');

    if (url.pop()?.toLowerCase() === 'add')
    {
        return (
            <CreateTemplate />
        )
    }

    return <EditTemplate />

}

export default SinglePage;