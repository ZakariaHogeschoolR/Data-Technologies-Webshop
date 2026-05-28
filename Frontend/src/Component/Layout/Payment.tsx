import './../../Styles/Payment.css';
import {useNavigate} from "react-router-dom";
import {useState} from 'react';

interface WinkelwagenItem {
    id: number;
    productId: number;
    shoppingProducts: any;
    quantity: number;
    createdAt: string;
    updatedAt: string;
}

type Props = {
    winkelwagenItems: WinkelwagenItem[]; total: number; currentWinkelwagenId: number;
}

const Payment = ({winkelwagenItems, total}: Props) => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const handleCheckout = () => {
        const token = localStorage.getItem('token');
        if (!token) {
            navigate('/auth');
            return;
        }

        setLoading(true);

        try {
            const orderLines = winkelwagenItems.map((item: any) => {
                const product = Array.isArray(item.shoppingProducts) ? item.shoppingProducts[0] : item.shoppingProducts;

                return {
                    productId: item.productId,
                    name: product?.name || 'Product Name',
                    productImage: product?.productImage || product?.imageUrl || '',
                    price: product?.price || 0,
                    quantity: item.quantity,
                    subTotal: (product?.price || 0) * item.quantity
                };
            });

            navigate('/checkout', {
                state: {
                    orderLines: orderLines, total: total
                }
            });
        } catch (err) {
            setError('Er ging iets mis bij het voorbereiden van de checkout.');
            setLoading(false);
        }
    };

    return (<div className="Payment-container">
            <p className="sub-total-price-payment">
                Subtotal: <span className="sub-total">€{total.toFixed(2)}</span>
            </p>
            <section className="Borderline"/>
            <p className="total-price">
                Total: <span className="total">€{total.toFixed(2)}</span>
            </p>
            <p className="tax">inc. tax</p>

            {error && (<p style={{color: '#c0392b', fontSize: '11px', padding: '0 20px', textAlign: 'center'}}>
                    {error}
                </p>)}

            <button
                type="button"
                className="button-checkout"
                onClick={handleCheckout}
                disabled={loading}
            >
                {loading ? 'Bezig...' : 'Checkout'}
            </button>
        </div>);
};

export default Payment;
