import './../../Styles/Payment.css';
import { useNavigate } from 'react-router-dom';
import { useState } from 'react';

interface WinkelwagenItem {
    id: number;
    productId: number;
    shoppingProducts: any;
    quantity: number;
    createdAt: string;
    updatedAt: string;
}

type Props = {
    winkelwagenItems: WinkelwagenItem[];
    total: number;
    currentWinkelwagenId: number;
};

const Payment = ({ winkelwagenItems, total }: Props) => {
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
                const product = Array.isArray(item.shoppingProducts)
                    ? item.shoppingProducts[0]
                    : item.shoppingProducts;

                return {
                    productId: item.productId,
                    name: product?.name || 'Product Name',
                    productImage: product?.productImage || product?.imageUrl || '',
                    price: product?.price || 0,
                    quantity: item.quantity,
                    subTotal: (product?.price || 0) * item.quantity,
                };
            });

            navigate('/checkout', {
                state: { orderLines, total },
            });
        } catch (err) {
            setError('Er ging iets mis bij het voorbereiden van de checkout.');
            setLoading(false);
        }
    };

    return (
        <div className="Payment-container">
            <div className="payment-row">
                <span>Subtotaal</span>
                <span>€{total.toFixed(2)}</span>
            </div>

            <section className="Borderline" />

            <div className="payment-row total">
                <span>Totaal</span>
                <span>€{total.toFixed(2)}</span>
            </div>

            <p className="tax">incl. btw</p>

            {error && (
                <p style={{ color: '#c0392b', fontSize: '11px', marginBottom: '0.75rem' }}>
                    {error}
                </p>
            )}

            <button
                type="button"
                className="button-checkout"
                onClick={handleCheckout}
                disabled={loading}
            >
                {loading ? 'Bezig...' : 'Checkout'}
            </button>
        </div>
    );
};

export default Payment;
