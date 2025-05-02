/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
// eslint-disable-next-line no-restricted-imports
import {getOne, getList, getJson, getText} from 'UI/Functions/WebRequest'

// Module
/**
*/
export class EcmaApi{
    public apiUrl: string = 'ecma';

    /**
      A test endpoint

    */
    public testResponse = (): Promise<String>  => {
        return getJson<string>(this.apiUrl + '/test')
    }

}

export default new EcmaApi();
