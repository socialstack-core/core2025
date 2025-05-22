/**
 * This file was automatically generated. DO NOT EDIT.
 */

// TYPES
// INCLUDES
export class ApiIncludes {
    private text: string = '';

    constructor(existing: string = '', addition: string = ''){
        this.text = (existing.length != 0) ? existing : '';
        if (addition.length != 0) {
             if (this.text != ''){
                this.text += '.'
             }
             this.text += addition;
        }
    }

    toString(){ return this.text }

    get all(){ return new ApiIncludes(this.text, '*'); }
    get primaryurl() {
        return new ApiIncludes(this.toString(), 'primaryurl');
    }
    get emailaddress() {
        return new ApiIncludes(this.toString(), 'emailaddress');
    }
    get signedref128() {
        return new ApiIncludes(this.toString(), 'signedref128');
    }
    get signedref256() {
        return new ApiIncludes(this.toString(), 'signedref256');
    }
    get signedreforiginal() {
        return new ApiIncludes(this.toString(), 'signedreforiginal');
    }
    get rolepermits() {
        return new ApiIncludes(this.toString(), 'rolepermits');
    }
    get composition() {
        return new ApiIncludes(this.toString(), 'composition');
    }
    get tags() {
        return new ApiIncludes(this.toString(), 'tags');
    }
    get attributes() {
        return new ApiIncludes(this.toString(), 'attributes');
    }
    get productcategories() {
        return new ApiIncludes(this.toString(), 'productcategories');
    }
    get productquantities() {
        return new ApiIncludes(this.toString(), 'productquantities');
    }
    get tiers() {
        return new ApiIncludes(this.toString(), 'tiers');
    }
    get optionalextras() {
        return new ApiIncludes(this.toString(), 'optionalextras');
    }
    get accessories() {
        return new ApiIncludes(this.toString(), 'accessories');
    }
    get suggestions() {
        return new ApiIncludes(this.toString(), 'suggestions');
    }
    get subscriptions() {
        return new ApiIncludes(this.toString(), 'subscriptions');
    }
    get userpermits() {
        return new ApiIncludes(this.toString(), 'userpermits');
    }
    get customcontenttypefields() {
        return new ApiIncludes(this.toString(), 'customcontenttypefields');
    }
    get categories() {
        return new ApiIncludes(this.toString(), 'categories');
    }
    get productimages() {
        return new ApiIncludes(this.toString(), 'productimages');
    }
    get uploads() {
        return new ApiIncludes(this.toString(), 'uploads');
    }
}

export class TemplateIncludes extends ApiIncludes {
    get creatorUser() {
        return new TemplateIncludes(this.toString(), 'creatoruser');
    }
}

export class TagIncludes extends ApiIncludes {
    get creatorUser() {
        return new TagIncludes(this.toString(), 'creatoruser');
    }
}

export class PublishGroupIncludes extends ApiIncludes {
    get creatorUser() {
        return new PublishGroupIncludes(this.toString(), 'creatoruser');
    }
}

export class LocaleIncludes extends ApiIncludes {
    get creatorUser() {
        return new LocaleIncludes(this.toString(), 'creatoruser');
    }
}

export class TranslationIncludes extends ApiIncludes {
    get creatorUser() {
        return new TranslationIncludes(this.toString(), 'creatoruser');
    }
}

export class CouponIncludes extends ApiIncludes {
    get discountAmount() {
        return new CouponIncludes(this.toString(), 'discountamount');
    }
    get minSpendPrice() {
        return new CouponIncludes(this.toString(), 'minspendprice');
    }
    get creatorUser() {
        return new CouponIncludes(this.toString(), 'creatoruser');
    }
}

export class AddressIncludes extends ApiIncludes {
    get creatorUser() {
        return new AddressIncludes(this.toString(), 'creatoruser');
    }
}

export class DeliveryOptionIncludes extends ApiIncludes {
    get creatorUser() {
        return new DeliveryOptionIncludes(this.toString(), 'creatoruser');
    }
}

export class PaymentMethodIncludes extends ApiIncludes {
    get creatorUser() {
        return new PaymentMethodIncludes(this.toString(), 'creatoruser');
    }
}

export class PriceIncludes extends ApiIncludes {
    get creatorUser() {
        return new PriceIncludes(this.toString(), 'creatoruser');
    }
}

export class ProductAttributeIncludes extends ApiIncludes {
    get attributeGroup() {
        return new ProductAttributeIncludes(this.toString(), 'attributegroup');
    }
    get creatorUser() {
        return new ProductAttributeIncludes(this.toString(), 'creatoruser');
    }
}

export class ProductAttributeGroupIncludes extends ApiIncludes {
    get creatorUser() {
        return new ProductAttributeGroupIncludes(this.toString(), 'creatoruser');
    }
}

export class ProductAttributeValueIncludes extends ApiIncludes {
    get attribute() {
        return new ProductAttributeValueIncludes(this.toString(), 'attribute');
    }
    get creatorUser() {
        return new ProductAttributeValueIncludes(this.toString(), 'creatoruser');
    }
}

export class ProductCategoryIncludes extends ApiIncludes {
    get productCategory() {
        return new ProductCategoryIncludes(this.toString(), 'productcategory');
    }
    get creatorUser() {
        return new ProductCategoryIncludes(this.toString(), 'creatoruser');
    }
}

export class ProductIncludes extends ApiIncludes {
    get price() {
        return new ProductIncludes(this.toString(), 'price');
    }
    get creatorUser() {
        return new ProductIncludes(this.toString(), 'creatoruser');
    }
}

export class ProductQuantityIncludes extends ApiIncludes {
    get subscription() {
        return new ProductQuantityIncludes(this.toString(), 'subscription');
    }
    get purchase() {
        return new ProductQuantityIncludes(this.toString(), 'purchase');
    }
    get shoppingCart() {
        return new ProductQuantityIncludes(this.toString(), 'shoppingcart');
    }
    get product() {
        return new ProductQuantityIncludes(this.toString(), 'product');
    }
    get creatorUser() {
        return new ProductQuantityIncludes(this.toString(), 'creatoruser');
    }
}

export class ProductTemplateIncludes extends ApiIncludes {
    get creatorUser() {
        return new ProductTemplateIncludes(this.toString(), 'creatoruser');
    }
}

export class PurchaseIncludes extends ApiIncludes {
    get creatorUser() {
        return new PurchaseIncludes(this.toString(), 'creatoruser');
    }
}

export class ShoppingCartIncludes extends ApiIncludes {
    get creatorUser() {
        return new ShoppingCartIncludes(this.toString(), 'creatoruser');
    }
}

export class SubscriptionIncludes extends ApiIncludes {
    get creatorUser() {
        return new SubscriptionIncludes(this.toString(), 'creatoruser');
    }
}

export class SubscriptionUsageIncludes extends ApiIncludes {
    get creatorUser() {
        return new SubscriptionUsageIncludes(this.toString(), 'creatoruser');
    }
}

export class PasswordResetRequestIncludes extends ApiIncludes {
}

export class PageIncludes extends ApiIncludes {
    get creatorUser() {
        return new PageIncludes(this.toString(), 'creatoruser');
    }
}

export class PermalinkIncludes extends ApiIncludes {
    get creatorUser() {
        return new PermalinkIncludes(this.toString(), 'creatoruser');
    }
}

export class AdminNavMenuItemIncludes extends ApiIncludes {
}

export class NavMenuIncludes extends ApiIncludes {
    get creatorUser() {
        return new NavMenuIncludes(this.toString(), 'creatoruser');
    }
}

export class NavMenuItemIncludes extends ApiIncludes {
    get creatorUser() {
        return new NavMenuItemIncludes(this.toString(), 'creatoruser');
    }
}

export class EmailTemplateIncludes extends ApiIncludes {
    get creatorUser() {
        return new EmailTemplateIncludes(this.toString(), 'creatoruser');
    }
}

export class UserIncludes extends ApiIncludes {
    get userRole() {
        return new UserIncludes(this.toString(), 'userrole');
    }
    get creatorUser() {
        return new UserIncludes(this.toString(), 'creatoruser');
    }
}

export class CustomContentTypeIncludes extends ApiIncludes {
    get creatorUser() {
        return new CustomContentTypeIncludes(this.toString(), 'creatoruser');
    }
}

export class CustomContentTypeFieldIncludes extends ApiIncludes {
    get customContentType() {
        return new CustomContentTypeFieldIncludes(this.toString(), 'customcontenttype');
    }
    get creatorUser() {
        return new CustomContentTypeFieldIncludes(this.toString(), 'creatoruser');
    }
}

export class CustomContentTypeSelectOptionIncludes extends ApiIncludes {
    get customContentTypeField() {
        return new CustomContentTypeSelectOptionIncludes(this.toString(), 'customcontenttypefield');
    }
    get creatorUser() {
        return new CustomContentTypeSelectOptionIncludes(this.toString(), 'creatoruser');
    }
}

export class ConfigurationIncludes extends ApiIncludes {
    get creatorUser() {
        return new ConfigurationIncludes(this.toString(), 'creatoruser');
    }
}

export class ComponentGroupIncludes extends ApiIncludes {
    get role() {
        return new ComponentGroupIncludes(this.toString(), 'role');
    }
    get creatorUser() {
        return new ComponentGroupIncludes(this.toString(), 'creatoruser');
    }
}

export class DomainCertificateIncludes extends ApiIncludes {
    get creatorUser() {
        return new DomainCertificateIncludes(this.toString(), 'creatoruser');
    }
}

export class CategoryIncludes extends ApiIncludes {
    get creatorUser() {
        return new CategoryIncludes(this.toString(), 'creatoruser');
    }
}

export class UploadIncludes extends ApiIncludes {
    get creatorUser() {
        return new UploadIncludes(this.toString(), 'creatoruser');
    }
}

export class ContentFieldAccessRuleIncludes extends ApiIncludes {
    get creatorUser() {
        return new ContentFieldAccessRuleIncludes(this.toString(), 'creatoruser');
    }
}

export class RoleIncludes extends ApiIncludes {
    get creatorUser() {
        return new RoleIncludes(this.toString(), 'creatoruser');
    }
}
