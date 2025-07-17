import { ApiList } from 'UI/Functions/WebRequest';
import Tile from 'Admin/Tile';
import Alert from 'UI/Alert';
import {isoConvert} from 'UI/Functions/DateTools';
import { useEffect, useState } from 'react';
import StdOutApi from 'Api/StdOutController';
import Input from "UI/Input";

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
    
    const [logLevelEnablement, setLogLevelEnablement] = useState<string[]>(['error', 'warn', 'info', 'ok']);
    const [disableReactWarnings, setDisableReactWarnings] = useState<boolean>(false);
    const [fromDate, setFromDate] = useState<string>('');  // ISO string or empty
    const [toDate, setToDate] = useState<string>('');      // ISO string or empty
    const [filterQuery, setFilterQuery] = useState<string>(''); // empty is falsy :)
    const [exceptionsOnly, setExceptionsOnly] = useState<boolean>(false);
    
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
    
    const toggleLogEnablement = (key: string, checked: boolean) => {
        if (checked) {
            setLogLevelEnablement([...logLevelEnablement, key]);
        }
        else
        {
            setLogLevelEnablement([...logLevelEnablement.filter((item) => item !== key)]);
        }
    }

    const normalizeDateRange = (from: string, to: string) => {
        if (from && to && new Date(to) < new Date(from)) {
            // Swap if out of order
            return { from: to, to: from };
        }
        return { from, to };
    };
    
    return <div>
        <Tile>
            <Alert type='info'>
                The following log is partially realtime. It will poll the current server for its latest log entries every 3 seconds.
            </Alert>
        </Tile>
        <Tile className={'stdout-controls'}>
            <div className={'group'}>
                <p>Log Level</p>
                <Input
                    type={'checkbox'}
                    defaultChecked={logLevelEnablement.includes('error')}
                    label={'Errors'}
                    onChange={(e) => toggleLogEnablement('error', (e.target as HTMLInputElement).checked)}
                />
                <Input
                    type={'checkbox'}
                    defaultChecked={logLevelEnablement.includes('ok')}
                    label={'Ok'}
                    onChange={(e) => toggleLogEnablement('ok', (e.target as HTMLInputElement).checked)}
                />

                <Input
                    type={'checkbox'}
                    defaultChecked={logLevelEnablement.includes('warn')}
                    label={'Warn'}
                    onChange={(e) => toggleLogEnablement('warn', (e.target as HTMLInputElement).checked)}
                />

                <Input
                    type={'checkbox'}
                    defaultChecked={logLevelEnablement.includes('info')}
                    label={'Info'}
                    onChange={(e) => toggleLogEnablement('info', (e.target as HTMLInputElement).checked)}
                />
            </div>
            <div className={'group'}>
                <p>ESLint</p>
                <Input
                    type={'checkbox'}
                    defaultChecked={disableReactWarnings}
                    label={'Hide react ESLint warnings'}
                    onChange={(e) => setDisableReactWarnings(e.target.checked)}
                />
            </div>
            <div className={'group'}>
                <p>Date Range</p>
                <Input
                    type="date"
                    value={fromDate}
                    label="From"
                    onChange={(e) => {
                        const newFrom = (e.target as HTMLInputElement).value;
                        const { from, to } = normalizeDateRange(newFrom, toDate);
                        setFromDate(from);
                        setToDate(to);
                    }}
                />

                <Input
                    type="date"
                    value={toDate}
                    label="To"
                    onChange={(e) => {
                        const newTo = (e.target as HTMLInputElement).value;
                        const { from, to } = normalizeDateRange(fromDate, newTo);
                        setFromDate(from);
                        setToDate(to);
                    }}
                />
            </div>
            <div className={'group'}>
                <p>.NET log tools</p>
                <Input
                    type={'checkbox'}
                    defaultChecked={exceptionsOnly}
                    label={'Exceptions only?'}
                    onChange={(e) => setExceptionsOnly(e.target.checked)}
                />
            </div>
            <div className={'group'}>
                <p>Filter by search</p>
                <Input 
                    type={'text'}
                    defaultValue={filterQuery}
                    onKeyUp={(ev) => {
                        setFilterQuery(ev.target.value);
                    }}
                />
            </div>
        </Tile>
        <div className="dashboards-stdout">
            {log.map((entry: StdEntry) => {
                
                if (!logLevelEnablement.includes(entry.type)) {
                    return;
                }
                
                if (disableReactWarnings && entry.messages.find(message => message.entry.toLowerCase().includes('react'))) {
                    return;
                }

                const entryTime = new Date(entry.createdUtc).getTime();
                const fromTime = fromDate ? new Date(fromDate).getTime() : null;
                const toTime = toDate ? new Date(toDate).getTime() : null;

                if ((fromTime && entryTime < fromTime) || (toTime && entryTime > (toTime + 86400000 - 1))) {
                    return;
                }
                
                if (filterQuery && !entry.messages.find(message => message.entry.toLowerCase().includes(filterQuery.toLowerCase()))) {
                    return;
                }
                
                if (exceptionsOnly && !entry.messages.find(message => message.entry.toLowerCase().includes('exception'))) {
                    return;
                }

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