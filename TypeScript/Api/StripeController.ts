/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson, getText } from 'UI/Functions/WebRequest';

// IMPORTS

// TYPES

/**
* This type was generated to reflect {StripeIntentResponse} (Api.Payments.StripeController+StripeIntentResponse)
**/
export type StripeIntentResponse = {
    clientSecret?: string;
}

/**
* This type was generated to reflect {PublicMessage} (Api.Startup.PublicMessage)
**/
export type PublicMessage = {
    message?: string;
    code?: string;
}
// NON-ENTITY CONTROLLERS

export class StripeController {

   private apiUrl: string = '/v1/stripe-gateway';

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.StripeController}::{SetupIntent}
     * @url '/setup'
     */
    setupIntent = (): Promise<StripeIntentResponse> => {
        return getJson<StripeIntentResponse>(this.apiUrl + '/setup')
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.StripeController}::{Webhook}
     * @url '/webhook'
     */
    webhook = (): Promise<PublicMessage | undefined> => {
        return getJson<PublicMessage | undefined>(this.apiUrl + '/webhook')
    }

}

export default new StripeController();
