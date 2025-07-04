import { ApiList } from 'UI/Functions/WebRequest';
import Tile from 'Admin/Tile';
import Alert from 'UI/Alert';
import {isoConvert} from 'UI/Functions/DateTools';
import { useEffect, useState } from 'react';
import StdOutApi from 'Api/StdOutController';

export type ModWindowScope = typeof window & {
    scrollMaxY: number
}

export type StdOutMessage = {
    entry: string,
    trace?: string
}

export type StdEntry = {
    createdUtc: Date,
    messages: StdOutMessage[],
    tag: string,
    type: string
}

/**
 * Server stdout.
 */
const StdOut: React.FC<{}> = (): React.ReactNode => {
    
    var [log, setLog] = useState([]);
    
    useEffect(() => {
        var update = () => {
            StdOutApi.getLog({}).then(res => {

                var response = JSON.parse(res);

                var results = response.results.reverse();
                
                results.forEach((res: StdEntry) => {
                    res.createdUtc = isoConvert(res.createdUtc);
                });
                
                console.log(results);
                setLog(results as any);
                
                if(window.scrollY >= ((window as ModWindowScope).scrollMaxY - 100)){
                    window.scroll(0,(window as ModWindowScope).scrollMaxY);
                }
            });
        };
        
        update();
        
        setInterval(update, 3000);
    },[]);
    
    var renderTag = (msg: StdEntry) => {
        var className = 'tag-' + msg.type;
        var text = '';
        
        switch(msg.type){
            case 'ok':
                text='OK';
            break;
            case 'info':
                text ='INFO';
            break;
            case 'warn':
                text='WARN';
            break;
            case 'error':
                text='ERROR';
            break;
            case 'fatal':
                text='FATAL';
            break;
        }
        
        return <span className={'tag-' + msg.type}>{text} {msg.createdUtc.toLocaleString()}</span>;
    };
    
    return <div>
        <Tile>
            <Alert type='info'>
                The following log is partially realtime. It will poll the current server for its latest log entries every 3 seconds.
            </Alert>
        </Tile>
        <div className="dashboards-stdout">
            {log.map((entry: StdEntry) => {
                
                return <div>
                    {renderTag(entry)}
                    {entry.messages.map((message: StdOutMessage) => {
                        if(message.trace){
                            return <>
                                <p>{message.entry}</p>
                                <pre>{message.trace}</pre>
                            </>;
                        }
                        return <p>{message.entry}</p>;
                    })}
                </div>;
                
            })}
        </div>
    </div>;
}

export default StdOut;