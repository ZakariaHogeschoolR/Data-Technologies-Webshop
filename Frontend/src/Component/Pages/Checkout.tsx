import {useLocation, useNavigate} from 'react-router-dom';
import {useState} from 'react';
import '../../Styles/Checkout.css';

type CheckoutItem = {
    productId: number; name: string; productImage: string; price: number; quantity: number; subTotal: number;
};

const Checkout = () => {
    const location = useLocation();
    const navigate = useNavigate();

    const {orderLines = [], total = 0} = location.state ?? {};
    const [finalOrderDate, setFinalOrderDate] = useState<string | null>(null);

    const [form, setForm] = useState({
        name: '', address: '', postCode: '', cardNumber: '', cvv: '',
    });

    const [submitted, setSubmitted] = useState(false);

    const formatCard = (value: string) => {
        const digits = value.replace(/\D/g, '').slice(0, 16);
        return digits.replace(/(.{4})/g, '$1 ').trim();
    };

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const {name, value} = e.target;

        if (name === 'cardNumber') {
            setForm(prev => ({...prev, cardNumber: formatCard(value)}));
        } else if (name === 'cvv') {
            setForm(prev => ({...prev, cvv: value.replace(/\D/g, '').slice(0, 4)}));
        } else if (name === 'postCode') {
            setForm(prev => ({...prev, postCode: value.toUpperCase().slice(0, 7)}));
        } else {
            setForm(prev => ({...prev, [name]: value}));
        }
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const token = localStorage.getItem('token');
        if (!token) {
            navigate('/auth');
            return;
        }

        try {
            const res = await fetch('http://localhost:5261/api/ShoppingCart/checkout', {
                method: 'POST', headers: {
                    'Content-Type': 'application/json', Authorization: `Bearer ${token}`,
                }, body: JSON.stringify({paymentMethod: 'card'}),
            });

            if (!res.ok) {
                const body = await res.json().catch(() => ({}));
                throw new Error(body.message ?? `Fout ${res.status}`);
            }

            const order = await res.json();

            setFinalOrderDate(order.orderedAt || new Date().toISOString());

            setSubmitted(true);

        } catch (error) {
            console.error("There was an error processing the checkout:", error);
            alert(error instanceof Error ? error.message : "Er ging iets mis bij het afrekenen.");
        }
    };

    if (submitted) {
        return (<div className="checkout-page">
            <div className="checkout-success">
                <div className="checkout-success-icon">✓</div>
                <h2>Order Confirmed!</h2>
                <p>Thank you, <strong>{form.name}</strong>. Your order is on its way.</p>

                {finalOrderDate && (<p className="checkout-success-date">
                    Ordered on {new Date(finalOrderDate).toLocaleDateString('nl-NL', {
                    year: 'numeric', month: 'long', day: 'numeric'
                })}
                </p>)}

                <p className="checkout-success-total">Total paid: <strong>€{Number(total).toFixed(2)}</strong></p>
                <button className="checkout-btn" onClick={() => navigate('/')}>
                    Continue Shopping
                </button>
            </div>
        </div>);
    }

    return (<div className="checkout-page">
        <div className="checkout-layout">

            <div className="checkout-summary">
                <h2 className="checkout-section-title">Order Summary</h2>
                <div className="checkout-items">
                    {orderLines.length === 0 ? (<p className="checkout-empty">No items in
                        order.</p>) : (orderLines.map((item: CheckoutItem) => (
                        <div key={item.productId} className="checkout-item">
                            <img
                                src={item.productImage}
                                alt={item.name}
                                className="checkout-item-img"
                            />
                            <div className="checkout-item-info">
                                <p className="checkout-item-name">{item.name}</p>
                                <p className="checkout-item-qty">Qty: {item.quantity}</p>
                            </div>
                            <p className="checkout-item-price">
                                €{Number(item.subTotal).toFixed(2)}
                            </p>
                        </div>)))}
                </div>
                <div className="checkout-total-row">
                    <span>Total</span>
                    <strong>€{Number(total).toFixed(2)}</strong>
                </div>
            </div>

            <form className="checkout-form" onSubmit={handleSubmit}>
                <h2 className="checkout-section-title">Payment Details</h2>

                <div className="checkout-field">
                    <label>Full Name</label>
                    <input
                        name="name"
                        type="text"
                        placeholder="John Doe"
                        value={form.name}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div className="checkout-field">
                    <label>Address</label>
                    <input
                        name="address"
                        type="text"
                        placeholder="123 Main Street"
                        value={form.address}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div className="checkout-field">
                    <label>Post Code</label>
                    <input
                        name="postCode"
                        type="text"
                        placeholder="1234AB"
                        value={form.postCode}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div className="checkout-divider"/>

                <div className="checkout-field">
                    <label>Card Number</label>
                    <input
                        name="cardNumber"
                        type="text"
                        placeholder="0000 0000 0000 0000"
                        value={form.cardNumber}
                        onChange={handleChange}
                        inputMode="numeric"
                        required
                    />
                </div>

                <div className="checkout-row">
                    <div className="checkout-field">
                        <label>CVV</label>
                        <input
                            name="cvv"
                            placeholder="•••"
                            value={form.cvv}
                            onChange={handleChange}
                            inputMode="numeric"
                            required
                        />
                    </div>
                    <div className="checkout-field checkout-field-wide">
                        <label>Total to pay</label>
                        <input
                            type="text"
                            value={`€${Number(total).toFixed(2)}`}
                            readOnly
                            className="checkout-readonly"
                        />
                    </div>
                </div>

                <button className="checkout-btn" type="submit">
                    Confirm &amp; Pay
                </button>
            </form>
        </div>
    </div>);
};

export default Checkout;
