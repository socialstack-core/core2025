/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports
// eslint-disable-next-line no-restricted-imports
import {getOne, getList, getJson, getText} from 'UI/Functions/WebRequest'

// Module
/*
*/
export type StripeIntentResponse = {
    clientSecret?: string,
}

/*
*/
export type PublicMessage = {
    message?: string,
    code?: string,
}

/**
*/
export class StripeApi{
    public apiUrl: string = 'stripe-gateway';

    /**
      Create a setup intent.

    */
    public setupIntent = (): Promise<StripeIntentResponse>  => {
        return getText(this.apiUrl + '/setup')
    }

    /**
      Updates a purchase based on a webhook event from a stripe payment.

    */
    public webhook = (): Promise<PublicMessage>  => {
        return getText(this.apiUrl + '/webhook')
    }

}

export default new StripeApi();
