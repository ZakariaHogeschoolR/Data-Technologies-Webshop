import { useCallback, useEffect, useMemo, useState } from 'react';
import Payment from '../Layout/Payment';
import bin from '../../../src/assets/bin.png';
import '../../Styles/Winkelwagen.css';

interface winkelwagen {
    id: number;
    productId: number;
    shoppingProducts: [];
    quantity: number;
    createdAt: string;
    updatedAt: string;
}

type product = {
    id: number;
    productImage: string;
    name: string;
    description: string;
    price: number;
};

export default function Winkelwagen() {
    const [winkelwagenItems, setWinkelwagenItems] = useState<winkelwagen[]>([]);
    const [products, setProducts] = useState<product[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [inError, setInError] = useState(false);

    const token = localStorage.getItem('token');

    const fetchData = useCallback(async () => {
        if (!token) {
            setLoading(false);
            return;
        }
        try {
            setLoading(true);
            const request = await fetch(`http://localhost:5261/api/ShoppingCart/mine`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            const json: winkelwagen[] = await request.json();
            setWinkelwagenItems(json);

            if (json.length > 0) {
                const productPromises = json.map((ww) =>
                    fetch(`http://localhost:5261/api/Product/${ww.productId}`).then((r) => r.json())
                );
                const results = await Promise.all(productPromises) as product[];
                setProducts(results);
            }
        } catch (err) {
            setInError(true);
            setError(`${err}`);
        } finally {
            setLoading(false);
        }
    }, [token]);

    useEffect(() => {
        fetchData();
    }, [fetchData]);

    const totalPrice = useMemo(() => {
        return winkelwagenItems.reduce((total, item) => {
            const product = products.find((p) => p.id === item.productId);
            return total + (product ? product.price * item.quantity : 0);
        }, 0);
    }, [winkelwagenItems, products]);

    const DeleteProductFromWinkelwagen = async (e: React.MouseEvent, productId: number) => {
        e.preventDefault();
        const url = `http://localhost:5261/api/ShoppingCart/delete/product`;
        try {
            const response = await fetch(url, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    Accept: 'application/json',
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({ id: 0, productId }),
            });
            if (response.status === 204) {
                await fetchData();
            }
        } catch (e) {
            console.log('netwerk error: ', e);
        }
    };

    if (loading) return (
        <div className="Winkelwagen_container">
            <p style={{ color: 'var(--dark-green)', letterSpacing: '2px', fontSize: '13px' }}>Loading...</p>
        </div>
    );

    if (inError) return (
        <div className="Winkelwagen_container">
            <p style={{ color: '#b00', fontSize: '13px' }}>{error}</p>
        </div>
    );

    if (!token) return (
        <div className="cart-empty-state">
            <p>Log in om je winkelwagen te zien.</p>
        </div>
    );

    if (winkelwagenItems.length === 0) return (
        <div className="cart-empty-state">
            <p>Je winkelwagen is leeg.</p>
        </div>
    );

    return (
        <div className="Winkelwagen_container">
            <h1>Winkelwagen</h1>

            <div className="cart-layout">
                {/* Items */}
                <div className="items-lijst">
                    <ul>
                        {winkelwagenItems.map((item) => {
                            const product = products.find((p) => p.id === item.productId);
                            return (
                                <li key={item.id}>
                                    <div className="winkelwagen-container-item">
                                        {product && (
                                            <img
                                                className="winkelwagen-item-img"
                                                src={product.productImage}
                                                alt={product.name}
                                            />
                                        )}

                                        <div className="winkelwagen-item-info">
                                            <p className="winkelwagen-name">
                                                {product ? product.name : 'Product niet gevonden'}
                                            </p>
                                            <p className="winkelwagen-qty-label">
                                                Qty: {item.quantity}
                                            </p>
                                        </div>

                                        <p className="winkelwagen-price">
                                            €{product
                                                ? (product.price * item.quantity).toFixed(2)
                                                : '0.00'}
                                        </p>

                                        <button
                                            className="bin-button"
                                            onClick={(e) => DeleteProductFromWinkelwagen(e, item.productId)}
                                            title="Verwijder"
                                        >
                                            <img className="bin-img" src={bin} alt="Verwijder" />
                                        </button>
                                    </div>
                                </li>
                            );
                        })}
                    </ul>
                </div>

                {/* Payment sidebar */}
                <Payment
                    winkelwagenItems={winkelwagenItems}
                    total={totalPrice}
                    currentWinkelwagenId={winkelwagenItems[0].id}
                />
            </div>
        </div>
    );
}
