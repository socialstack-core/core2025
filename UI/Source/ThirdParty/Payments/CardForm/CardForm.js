import React from 'react';
import Input from 'UI/Input';
import Row from 'UI/Row';
import Col from 'UI/Column';
import Payment from './helpers.js';
import {isoConvert} from 'UI/Functions/DateTools';

/*
* A custom variant of the react-credit-card module so 
* we can reuse the card images on the payment method list.
*/

var paymentGateways = global.paymentGateways = global.paymentGateways || {};

const prefix = 'react-credit-card';
const validate = Payment.fns;

export default CardForm = (props) => {
	
	if(props.readonly){
		// If expiry is a number, map to mm/yy
		var expr  = props.expiry;
		
		if(typeof expr === 'number'){
			var date = isoConvert(expr);
			var month = date.getUTCMonth() + 1;
			
			if(month < 10){
				month = '0' + month;
			}
			
			var year = date.getUTCFullYear().toString();
			expr = month + '/' + year.substring(year.length - 2);
		}
		
		return <>
			<ReactCreditCardIntl name='' expiry={expr} number={props.last4} issuer={props.issuer} prefixNumber focused={'name'}/>
			<input type='hidden' name={props.fieldName} ref={ir=>{
			if(ir){
				ir.onGetValue=(val, ele)=>{
					if(ele == ir){
						// Numeric
						return props.paymentMethodId;
					}
				}
			}}}/>
		</>;
	}
	
	var [number, setNumber] = React.useState('');
	var [cvc, setCvc] = React.useState('');
	var [name, setName] = React.useState('');
	var [focus, setFocus] = React.useState('');
	var [expiry, setExpiry] = React.useState('');
    var [isAmexOrDinersClub, setIsAmexOrDinersClub] = React.useState(false);

	return <>
		<div className="my-3">
			<ReactCreditCardIntl expiry={expiry} number={number} cvc={cvc} name={name} focused={focus}/>
		</div>
		{/* Do not specify a name on any of the following inputs. This prevents them from being submitted with surrounding forms. */}
        <Input label={`Card number`} placeholder='Long number on front of card' type='text' validate={['Required']}
            onFocus={e => setFocus('number')}
            onKeyUp={e => setNumber(e.target.value)}
            onChange={e => setNumber(e.target.value)}
            onInput={e => {
                var target = e.target;
                var value = target.value;
                var cursor = target.selectionStart;

                const filterSpace = value.replace(/\s+/g, '');
                const filtered = filterSpace.replace(/[^0-9]/g, '');
                const cardNum = filtered.substr(0, 16);

                // most cards map to 4-4-4-4 (NB: American Express / Diners Club use 4-6-5)
                var isAmexOrDinersClub =
                    // amex
                    cardNum.startsWith('34') || cardNum.startsWith('37') ||
                    // diners club
                    cardNum.startsWith('36') || cardNum.startsWith('38');
                setIsAmexOrDinersClub(isAmexOrDinersClub);
                const partitions = isAmexOrDinersClub ? [4, 6, 5] : [4, 4, 4, 4];
                const cardNumUpdated = [];
                let position = 0;

                partitions.forEach(expandCard => {
                    const segment = cardNum.substr(position, expandCard);
                    if (segment) cardNumUpdated.push(segment);
                    position += expandCard;
                });

                const cardNumFormatted = cardNumUpdated.join(' ');

                // handle cursor position if user edits the number later
                if (cursor < cardNumFormatted.length - 1) {
                    // determine if the new value entered was valid, and set cursor progression
                    cursor = filterSpace !== filtered ? cursor - 1 : cursor;

                    setTimeout(() => {
                        target.setSelectionRange(cursor, cursor, 'none');
                    });

                }

                target.value = cardNumFormatted;
            }} />
		
        <Input label={`Name shown on card`} placeholder={`Name on card`} type='text'
            onFocus={e => setFocus('name')}
            onKeyUp={e => setName(e.target.value)}
            onChange={e => setName(e.target.value)} />

        <Row>
            <Col sizeXs={12} sizeSm={7}>
                <Input label={`Expiry date`} placeholder='MM/YY' type='text' validate={['Required']}
                    onFocus={e => setFocus('expiry')}
                    onKeyUp={e => setExpiry(e.target.value)}
                    onChange={e => setExpiry(e.target.value)}
                    onInput={e => {
                        var target = e.target;
                        var value = target.value;
                        var cursor = target.selectionStart;

                        const filterSlash = value.replace(/\//g, '');
                        const filtered = filterSlash.replace(/[^0-9]/g, '');
                        const cardExpiry = filtered.substr(0, 4);
                        const partitions = [2, 2];
                        const cardExpiryUpdated = [];
                        let position = 0;

                        partitions.forEach(expandCard => {
                            const segment = cardExpiry.substr(position, expandCard);
                            if (segment) cardExpiryUpdated.push(segment);
                            position += expandCard;
                        });

                        const cardExpiryFormatted = cardExpiryUpdated.join('/');

                        // handle cursor position if user edits the number later
                        if (cursor < cardExpiryFormatted.length - 1) {
                            // determine if the new value entered was valid, and set cursor progression
                            cursor = filterSlash !== filtered ? cursor - 1 : cursor;

                            setTimeout(() => {
                                target.setSelectionRange(cursor, cursor, 'none');
                            });

                        }

                        target.value = cardExpiryFormatted;
                }} />
            </Col>
            <Col sizeXs={12} sizeSm={5}>
                {/* typically 3 digits, but American Express uses 4 */}
                {/* text with a pattern instead of type=number otherwise maxlength ignored */}
                <Input label={`Card verification code`} placeholder={isAmexOrDinersClub ? `4 digits on front of card` : `Last 3 digits on reverse`}
                    type='text' inputmode='numeric' pattern='\d*' maxlength={isAmexOrDinersClub ? 4 : 3} validate={['Required']} required
                    onFocus={e => setFocus('cvc')}
                    onKeyUp={e => setCvc(e.target.value)}
                    onChange={e => setCvc(e.target.value)}
                    onInput={e => {
                        var target = e.target;
                        var value = target.value;
                        target.value = value.replace(/[^0-9]/g, '');
                    }} />
            </Col>
        </Row>
		
		<input type='hidden' name={props.fieldName} ref={ir=>{
			if(ir){
				ir.onGetValue=(val, ele)=>{
					if(ele == ir){
						
						var expr = expiry.trim();
						var exp_parts = expr.split('/');
						var year;
						var month;
						
						// Gateway internally will validate
						if(exp_parts.length != 2){
							year = 0;
							month = 0;
						}else{
							year = exp_parts[1];
							month = exp_parts[0];
						}
						
						var num = number.trim();
						
						var card = {
							number: num,
							cvc: cvc.trim(),
							name: name.trim(),
							expiry: expiry.trim(),
							exp_year: parseInt(year),
							exp_month: parseInt(month),
							issuer: Payment.fns.cardType(num) || 'unknown'
						};
						
						// Indicate to gateway component (Payments/Stripe.js for example):
						var res = paymentGateways.onSubmittedCard && paymentGateways.onSubmittedCard(card);
						
						if(res.then){
							
							return res.then(success => {
								
								// Output the value as something which is safe to be sent to our server.
								return {
									name: success.last4,
									expiry: success.expiry,
									issuer: success.issuer,
									gatewayToken: success.gatewayToken,
									gatewayId: success.gatewayId
								};
							});
							
						}else{
							Promise.reject({message: `No payment gateways available`});
						}
					}
				};
			}
		}} />
	</>;
	
};

class ReactCreditCardIntl extends React.Component {
  constructor(props) {
    super(props);

    this.setCards();
  }

  static defaultProps = {
    acceptedCards: [],
    locale: {
      valid: 'valid thru',
    },
    placeholders: {
      name: 'YOUR NAME HERE',
    },
    preview: false,
  };

  componentDidUpdate(prevProps) {
    const { acceptedCards, callback, number } = this.props;

    if (prevProps.number !== number) {
      /* istanbul ignore else */
      if (typeof callback === 'function') {
        callback(this.options, Payment.fns.validateCardNumber(number));
      }
    }

    if (prevProps.acceptedCards.toString() !== acceptedCards.toString()) {
      this.setCards();
    }
  }

  get issuer() {
    const { issuer, preview } = this.props;

    return issuer ? issuer.toLowerCase() : this.options.issuer;
  }

  get number() {
    const { number, preview } = this.props;

    let maxLength = preview ? 19 : this.options.maxLength;
    let nextNumber = typeof number === 'number' ? number.toString() : number.replace(/[A-Za-z]| /g, '');

    if (isNaN(parseInt(nextNumber, 10)) && !preview) {
      nextNumber = '';
    }

    if (maxLength > 16) {
      maxLength = nextNumber.length <= 16 ? 16 : maxLength;
    }

    if (nextNumber.length > maxLength) {
      nextNumber = nextNumber.slice(0, maxLength);
    }
	
    while (nextNumber.length < maxLength) {
      if(this.props.prefixNumber){
		  nextNumber = '•' + nextNumber;
	  }else{
		  nextNumber += '•';
	  }
    }

    if (['amex', 'dinersclub'].includes(this.issuer)) {
      const format = [0, 4, 10];
      const limit = [4, 6, 5];
      nextNumber = `${nextNumber.substr(format[0], limit[0])} ${nextNumber.substr(format[1], limit[1])} ${nextNumber.substr(format[2], limit[2])}`;
    }
    else if (nextNumber.length > 16) {
      const format = [0, 4, 8, 12];
      const limit = [4, 7];
      nextNumber = `${nextNumber.substr(format[0], limit[0])} ${nextNumber.substr(format[1], limit[0])} ${nextNumber.substr(format[2], limit[0])} ${nextNumber.substr(format[3], limit[1])}`;
    }
    else {
      for (let i = 1; i < (maxLength / 4); i++) {
        const space_index = (i * 4) + (i - 1);
        nextNumber = `${nextNumber.slice(0, space_index)} ${nextNumber.slice(space_index)}`;
      }
    }

    return nextNumber;
  }

  get expiry() {
    const { expiry = '' } = this.props;
    const date = typeof expiry === 'number' ? expiry.toString() : expiry;
    let month = '';
    let year = '';

    if (date.includes('/')) {
      [month, year] = date.split('/');
    }
    else if (date.length) {
      month = date.substr(0, 2);
      year = date.substr(2, 6);
    }

    while (month.length < 2) {
      month += '•';
    }

    if (year.length > 2) {
      year = year.substr(2, 4);
    }

    while (year.length < 2) {
      year += '•';
    }

    return `${month}/${year}`;
  }

  get options() {
    const { number } = this.props;
    const issuer = Payment.fns.cardType(number) || 'unknown';

    let maxLength = 16;

    if (issuer === 'amex') {
      maxLength = 15;
    }
    else if (issuer === 'dinersclub') {
      maxLength = 14;
    }
    else if (['hipercard', 'mastercard', 'visa'].includes(issuer)) {
      maxLength = 19;
    }

    return {
      issuer,
      maxLength,
    };
  }

  setCards() {
    const { acceptedCards } = this.props;
    let newCardArray = [];

    if (acceptedCards.length) {
      Payment.getCardArray()
        .forEach(d => {
          if (acceptedCards.includes(d.type)) {
            newCardArray.push(d);
          }
        });
    }
    else {
      newCardArray = newCardArray.concat(Payment.getCardArray());
    }

    Payment.setCardArray(newCardArray);
  }

  render() {
    const { cvc, focused, locale, name, placeholders } = this.props;
    const { number, expiry } = this;

    return (
      <div key="Cards" className="rccs">
        <div
          className={[
            'rccs__card',
            `rccs__card--${this.issuer}`,
            focused === 'cvc' && this.issuer !== 'amex' ? 'rccs__card--flipped' : '',
          ].join(' ').trim()}
        >
          <div className="rccs__card--front">
            <div className="rccs__card__background" />
            <div className="rccs__issuer" />
            <div
              className={[
                'rccs__cvc__front',
                focused === 'cvc' ? 'rccs--focused' : '',
              ].join(' ').trim()}
            >
              {cvc}
            </div>
            <div
              className={[
                'rccs__number',
                number.replace(/ /g, '').length > 16 ? 'rccs__number--large' : '',
                focused === 'number' ? 'rccs--focused' : '',
                number.substr(0, 1) !== '•' ? 'rccs--filled' : '',
              ].join(' ').trim()}
            >
              {number}
            </div>
            <div
              className={[
                'rccs__name',
                focused === 'name' ? 'rccs--focused' : '',
                name ? 'rccs--filled' : '',
              ].join(' ').trim()}
            >
              {name}
            </div>
            <div
              className={[
                'rccs__expiry',
                focused === 'expiry' ? 'rccs--focused' : '',
                expiry.substr(0, 1) !== '•' ? 'rccs--filled' : '',
              ].join(' ').trim()}
            >
              <div className="rccs__expiry__valid">{locale.valid}</div>
              <div className="rccs__expiry__value">{expiry}</div>
            </div>
            <div className="rccs__chip" />
          </div>
          <div className="rccs__card--back">
            <div className="rccs__card__background" />
            <div className="rccs__stripe" />
            <div className="rccs__signature" />
            <div
              className={[
                'rccs__cvc',
                focused === 'cvc' ? 'rccs--focused' : '',
              ].join(' ').trim()}
            >
              {cvc}
            </div>
            <div className="rccs__issuer" />
          </div>
        </div>
      </div>
    );
  }
}

CardForm.defaultProps = {
  number: null,
  cvc: null,
  name: '',
  expiry: '',
  focused: null,
  namePlaceholder: 'FULL NAME',
  expiryBefore: 'month/year',
  expiryAfter: 'valid thru',
  shinyAfterBack: '',
  type: null
};