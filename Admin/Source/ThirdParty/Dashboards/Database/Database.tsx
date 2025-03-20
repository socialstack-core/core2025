import Tile from "Admin/Tile";
import { useState } from "react"
import Alert from "UI/Alert";
import Form from "UI/Form";
import Input from "UI/Input"

export type ModWindowScope = typeof window & {
    recordSets: RecordSet[]
}

export type RecordSet = {
    fields: string[],
    results: Record<string,string|boolean|number>[]
}

const renderResults = (sets:RecordSet[]) => {
		
    return sets.map((set: RecordSet) => {
        
        return <table className="table result-set table-striped">
            <thead>
                {set.fields.map((fieldName: string) => <th>{fieldName}</th>)}
            </thead>
            <tbody>
            {set.results.map((resultRow: Record<string,any>) => <tr>{resultRow.map((field: string) => <td>{field}</td>)}</tr>)}
            </tbody>
        </table>;
        
    });
    
};

const renderRuns = (runs: any) => {
        
    return runs.map((run: any) => {
        
        return <div className="db-run">
            <p>
                {run.affectedRows >= 0 ? `Rows affected: ${run.affectedRows}` : ''}
            </p>
            {run.error ? <Alert type='error'>{`${run.error}`}</Alert> : (
                
                run.sets && run.sets.length > 0 ? renderResults(run.sets) : `OK, No result sets in this run.`
                
            )}
        </div>;
        
    });
    
};
    

const Database: React.FC<{}> = (): React.ReactNode => {
    
    const [runs, setRuns] = useState([]);

    return (
        <div className="dashboards-database">
            <Tile>
                <Alert type='info'>
                    Tip: Use your browsers JS console to perform analysis. Within a session, every result set will be available via <b>window.recordSets</b>
                </Alert>
                <Form 
                    action={() => Promise.resolve()} 
                    loadingMessage={`Running query..`}
                    submitLabel={`Execute Query`}
                    onSuccess={response => {

                        var modWin: ModWindowScope = window as ModWindowScope;
                        
                        if(!modWin.recordSets){
                            modWin.recordSets = [];
                        }
                        
                        // if(response.sets){
                        //     modWin.recordSets = modWin.recordSets.concat(response.sets);
                        // }
                        
                        var newRuns = runs.slice();
                        newRuns.unshift(response as never);
                        setRuns(newRuns);
                    }}
                >
                    <Input 
                        type='textarea' 
                        contentType='application/sql' 
                        name='query' 
                    />
                </Form>
            </Tile>
            <Tile>
                {runs.length ? renderRuns(runs) : <center>{`No results yet`}</center>}
            </Tile>
        </div>
    )

}

export default Database;