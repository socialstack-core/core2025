import shoppingCartApi, { ShoppingCart, CartItemChange } from 'Api/ShoppingCart';
import { createContext, useContext, useState, useEffect } from 'react';
// import { useCart } from 'UI/Payments/CartSession';
// var { addToCart, emptyCart, shoppingCart } = useCart();
// addToCart({product: ProductIdOrObject, quantity: PositiveOrNegativeNumber});
// addToCart({product: ProductIdOrObject, isSubscribing: true}); (adds a quantity of 1)
// To remove either, just addToCart with a negative quantity.
import store from 'UI/Functions/Store';
import {ApiIncludes} from "Api/Includes";

interface CartContext {
}

const CartSession = createContext<CartContext>({
    
});

export const Provider: React.FC<React.PropsWithChildren> = (props) => {
    const [shoppingCart, setShoppingCart] = useState<ShoppingCart | null>(null);
    const [loading, setLoading] = useState<boolean>(true);

    const includeSet: ApiIncludes[] = [
        shoppingCartApi.includes.productquantities,
        new ApiIncludes('', 'productquantities.product'),
        new ApiIncludes('', 'productquantities.product.price')
    ];

    useEffect(() => {
        var cartId = store.get('shopping_cart_id') as string;
        if (cartId) {
            shoppingCartApi.load(parseInt(cartId) as int, includeSet)
                .then(response => {
                    setShoppingCart(response);
                    setLoading(false);
                }).catch(e => {
                    console.error(e);
                    setLoading(false);
                })
        } else {
            setLoading(false);
        }
    }, []);

    // return quantity of given product within cart (or all products if productId == null)
    let getCartQuantity = (productId: uint) => {
        var qty = 0;

        shoppingCart?.productQuantities?.forEach(product => {
            if (productId == product?.productId) {
                qty += product?.quantity || 0;
            }
        });

        return qty;
    }

    let hasSubscriptions = () => {
        var result = false;

        shoppingCart?.productQuantities?.forEach(productQty => {
            var product = productQty?.product;
            if (product?.billingFrequency != 0 || product?.isBilledByUsage) {
                result = true;
            }
        });

        return result;
    }

    let cartIsEmpty = () => {
        return !shoppingCart?.productQuantities?.length;
    };

    let addToCart = (productId: uint, quantity: int, isDelta: boolean = false) => {

        var items : CartItemChange[] = [
            {
                productId,
                quantity: isDelta ? undefined : quantity,
                deltaQuantity: isDelta ? quantity : undefined
            }
        ];

        return shoppingCartApi.changeItems({
            shoppingCartId: shoppingCart?.id || (0 as uint),
            items
        }, includeSet).then(cart => {
            store.set('shopping_cart_id', cart.id.toString());
            setShoppingCart(cart);
            return cart;
        }).catch((e: PublicError) => {
            if (e?.type && e.type == 'cart/not_found') {
                console.log("Removing old cart reference - try adding again.");
                store.remove('shopping_cart_id');
                setShoppingCart(null);
            } else {
                // rethrow
                throw e;
            }
        });
    }
	
    let emptyCart = () => {
        store.remove('shopping_cart_id');
        setShoppingCart(null);
        setLoading(false);
    };

    return (
        <CartSession.Provider
            value={{
                shoppingCart,
                addToCart,
                loading,
                emptyCart,
                cartIsEmpty,
                getCartQuantity,
                hasSubscriptions
            }}
        >
            {props.children}
        </CartSession.Provider>
    );
};

export { CartSession };

export function useCart() {
    return useContext(CartSession);
}
