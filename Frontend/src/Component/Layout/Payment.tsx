import './../../Styles/Payment.css';
import { useNavigate } from "react-router-dom";
import { useState } from 'react';

interface WinkelwagenItem {
    id: number;
    productId: number;
    shoppingProducts: [];
    quantity: number;
    createdAt: string;
    updatedAt: string;
}

type Props = {
    winkelwagenItems: WinkelwagenItem[];
    total: number;
    currentWinkelwagenId: number;
}

const Payment = ({ total }: Props) => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const handleCheckout = async () => {
        const token = localStorage.getItem('token');
        if (!token) {
            navigate('/auth');
            return;
        }

        setLoading(true);
        setError('');

        try {
            const res = await fetch('http://localhost:5261/api/ShoppingCart/checkout', {
                method: 'POST',
                headers: { Authorization: `Bearer ${token}` }
            });

            if (!res.ok) {
                const body = await res.json().catch(() => ({}));
                throw new Error(body.message ?? `Fout ${res.status}`);
            }

            const order = await res.json();

            navigate('/checkout', {
                state: {
                    orderLines: order.items,
                    total: order.total,
                    orderedAt: order.orderedAt
                }
            });
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Er ging iets mis.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="Payment-container">
            <p className="sub-total-price-payment">
                Subtotal: <span className="sub-total">€{total.toFixed(2)}</span>
            </p>
            <section className="Borderline" />
            <p className="total-price">
                Total: <span className="total">€{total.toFixed(2)}</span>
            </p>
            <p className="tax">inc. tax</p>

            {error && (
                <p style={{ color: '#c0392b', fontSize: '11px', padding: '0 20px', textAlign: 'center' }}>
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
