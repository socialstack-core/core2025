/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
import {AutoApi, ApiIncludes} from 'Api/ApiEndpoints'
import {VersionedContent, UserCreatedContent, Content} from 'Api/Content'
import {ExchangeRateIncludes, LocaleIncludes, UserIncludes} from './Includes'
import {Locale} from 'Api/Locale'
import {User} from 'Api/User'
import {getJson, ApiList} from 'UI/Functions/WebRequest'

// Module
/*
  An ExchangeRate
*/
export type ExchangeRate = VersionedContent & {
    name?: string,
    fromLocaleId: uint,
    toLocaleId: uint,
    rate: double,
    fromLocale: Locale,
    toLocale: Locale,
    creatorUser: User,
}

/**
    Auto generated API for ExchangeRate
    Handles exchangeRate endpoints.
*/
export class ExchangeRateApi extends AutoApi<ExchangeRate, ExchangeRateIncludes>{
    /**
      This extends the AutoApi class, which provides CRUD functionality, any methods seen in are from custom endpoints in the controller

    */
    public constructor (){
        super('exchangeRate')
        this.includes = new ExchangeRateIncludes();
    }

}

export default new ExchangeRateApi();
