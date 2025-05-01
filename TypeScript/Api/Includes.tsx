/* * * * * * * | Auto Generated Script, do not edit | * * * * * * * */
// Imports

// Module
/**
*/
export class ApiIncludes{
    public text: string;

    /**

    */
    public constructor (chain: string, includeName: string){
        if(chain && includeName){
        this.text = chain + '.' + includeName;
        }else{
        this.text = chain || includeName;
        }
    }

    /**

    */
    public toString = () => {
        return this.text
    }

    /*
    */
    get primaryurl(): ApiIncludes {
        return new ApiIncludes(this.text, 'primaryurl');
    }

    /*
    */
    get emailaddress(): ApiIncludes {
        return new ApiIncludes(this.text, 'emailaddress');
    }

    /*
    */
    get signedref128(): ApiIncludes {
        return new ApiIncludes(this.text, 'signedref128');
    }

    /*
    */
    get signedref256(): ApiIncludes {
        return new ApiIncludes(this.text, 'signedref256');
    }

    /*
    */
    get signedreforiginal(): ApiIncludes {
        return new ApiIncludes(this.text, 'signedreforiginal');
    }

    /*
    */
    get rolepermits(): ApiIncludes {
        return new ApiIncludes(this.text, 'rolepermits');
    }

    /*
    */
    get composition(): ApiIncludes {
        return new ApiIncludes(this.text, 'composition');
    }

    /*
    */
    get tags(): ApiIncludes {
        return new ApiIncludes(this.text, 'tags');
    }

    /*
    */
    get productquantities(): ApiIncludes {
        return new ApiIncludes(this.text, 'productquantities');
    }

    /*
    */
    get tiers(): ApiIncludes {
        return new ApiIncludes(this.text, 'tiers');
    }

    /*
    */
    get subscriptions(): ApiIncludes {
        return new ApiIncludes(this.text, 'subscriptions');
    }

    /*
    */
    get userpermits(): ApiIncludes {
        return new ApiIncludes(this.text, 'userpermits');
    }

    /*
    */
    get customcontenttypefields(): ApiIncludes {
        return new ApiIncludes(this.text, 'customcontenttypefields');
    }

    /*
    */
    get categories(): ApiIncludes {
        return new ApiIncludes(this.text, 'categories');
    }

    /*
    */
    get productimages(): ApiIncludes {
        return new ApiIncludes(this.text, 'productimages');
    }

    /*
    */
    get uploads(): ApiIncludes {
        return new ApiIncludes(this.text, 'uploads');
    }

    /*
    */
    get all(): ApiIncludes {
        return new ApiIncludes(this.text, '*')
    }

}

/**
*/
export class TemplateIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class SearchEntityIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class TagIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class PublishGroupIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class LocaleIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class TranslationIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class CouponIncludes extends ApiIncludes{
    /*
    */
    get discountAmount(): PriceIncludes {
        return new PriceIncludes(this.text, 'discountAmount');
    }

    /*
    */
    get minSpendPrice(): PriceIncludes {
        return new PriceIncludes(this.text, 'minSpendPrice');
    }

    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class PaymentMethodIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class PriceIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class ProductAttributeIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class ProductIncludes extends ApiIncludes{
    /*
    */
    get price(): PriceIncludes {
        return new PriceIncludes(this.text, 'price');
    }

    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class ProductQuantityIncludes extends ApiIncludes{
    /*
    */
    get subscription(): SubscriptionIncludes {
        return new SubscriptionIncludes(this.text, 'subscription');
    }

    /*
    */
    get purchase(): PurchaseIncludes {
        return new PurchaseIncludes(this.text, 'purchase');
    }

    /*
    */
    get shoppingCart(): ShoppingCartIncludes {
        return new ShoppingCartIncludes(this.text, 'shoppingCart');
    }

    /*
    */
    get product(): ProductIncludes {
        return new ProductIncludes(this.text, 'product');
    }

    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class PurchaseIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class ShoppingCartIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class SubscriptionIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class SubscriptionUsageIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class PasswordResetRequestIncludes extends ApiIncludes{
}

/**
*/
export class PageIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class PermalinkIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class AdminNavMenuItemIncludes extends ApiIncludes{
}

/**
*/
export class NavMenuIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class NavMenuItemIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class EmailTemplateIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class SiteDomainIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class UserIncludes extends ApiIncludes{
    /*
    */
    get userRole(): RoleIncludes {
        return new RoleIncludes(this.text, 'userRole');
    }

    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class CustomContentTypeIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class CustomContentTypeFieldIncludes extends ApiIncludes{
    /*
    */
    get customContentType(): CustomContentTypeIncludes {
        return new CustomContentTypeIncludes(this.text, 'customContentType');
    }

    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class CustomContentTypeSelectOptionIncludes extends ApiIncludes{
    /*
    */
    get customContentTypeField(): CustomContentTypeFieldIncludes {
        return new CustomContentTypeFieldIncludes(this.text, 'customContentTypeField');
    }

    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class ConfigurationIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class ComponentGroupIncludes extends ApiIncludes{
    /*
    */
    get role(): RoleIncludes {
        return new RoleIncludes(this.text, 'role');
    }

    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class RedirectIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class DomainCertificateIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class CategoryIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class UploadIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}

/**
*/
export class RoleIncludes extends ApiIncludes{
    /*
    */
    get creatorUser(): UserIncludes {
        return new UserIncludes(this.text, 'creatorUser');
    }

}


