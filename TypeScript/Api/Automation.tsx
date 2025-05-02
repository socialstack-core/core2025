/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
// eslint-disable-next-line no-restricted-imports
import {getOne, getList, getJson, getText} from 'UI/Functions/WebRequest'

// Module
/*
*/
export type AutomationStructure = {
    results?: Automation[],
}

/**
*/
export class AutomationApi{
    public apiUrl: string = 'automation';

    /**
      GET /v1/automation/list
            Returns meta about automations available from this API. Includes endpoints and content types.

    */
    public get = (): Promise<AutomationStructure>  => {
        return getJson<AutomationStructure>(this.apiUrl + '/list')
    }

    /**
      GET /v1/automation/{name}/run
            Runs the named automation and waits for it to complete.

    */
    public execute = (name: string): Promise<string>  => {
        return getText(this.apiUrl + '/' + name + '/run')
    }

}

/*
*/
export type Automation = {
    name?: string,
    description?: string,
    cronDescription?: string,
    cron?: string,
    lastTrigger?: Date,
}

export default new AutomationApi();
