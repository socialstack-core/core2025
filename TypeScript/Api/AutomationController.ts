/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson } from 'UI/Functions/WebRequest';

// TYPES

/**
* This type was generated to reflect {Automation} (Api.Automations.Automation)
**/
export type Automation = {
    lastTrigger?: Date | string | number;
    name?: string;
    description?: string;
    cronDescription?: string;
    cron?: string;
}

/**
* This type was generated to reflect {AutomationStructure} (Api.Automations.AutomationStructure)
**/
export type AutomationStructure = {
    results?: Automation[][];
}
// NON-ENTITY CONTROLLERS

export class AutomationController {

   private apiUrl: string = '/v1/automation';

    /**
     * Generated from a .NET type.
     * @see {Api.Automations.AutomationController}::{Get}
     * @url 'v1/automation/list'
     */
    get = (): Promise<AutomationStructure> => {
        return getJson<AutomationStructure>(this.apiUrl + '/list');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Automations.AutomationController}::{Execute}
     * @url 'v1/automation/' + name + '/run'
     */
    execute = (name: string): Promise<void> => {
        return getJson<void>(this.apiUrl + '/' + name + '/run');
    }

}

export default new AutomationController();
