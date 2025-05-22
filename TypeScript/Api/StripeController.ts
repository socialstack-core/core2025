/**
 * This file was automatically generated. DO NOT EDIT.
 */

import { ApiList, getJson } from 'UI/Functions/WebRequest';

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
     * @url 'v1/stripe-gateway/setup'
     */
    setupIntent = (): Promise<StripeIntentResponse> => {
        return getJson<StripeIntentResponse>(this.apiUrl + '/setup');
    }

    /**
     * Generated from a .NET type.
     * @see {Api.Payments.StripeController}::{Webhook}
     * @url 'v1/stripe-gateway/webhook'
     */
    webhook = (): Promise<PublicMessage> => {
        return getJson<PublicMessage>(this.apiUrl + '/webhook');
    }

}

export default new StripeController();
