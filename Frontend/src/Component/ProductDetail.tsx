import { useParams, Link } from 'react-router-dom';
import { useFetch } from '../CustomHooks/GetFetchHook';
import { useState, useEffect } from 'react';
import NotFound from '../Component/Pages/NotFound';
import '../Styles/ProductDetail.css';
import { AddRecentProducts } from './storage/recentProducts';
import AddToWishlistButton from './AddToWishlist';

const API = 'http://localhost:5261/api';

type product = {
    id: number;
    productImage: string;
    name: string;
    description: string;
    price: number;
    teamId: number;
};

const ProductDetail = () => {
    const { id } = useParams();
    const { data, isLoading, error } = useFetch<product>({ url: `${API}/Product/${id}` });
    const [productsByTeam, setProductsByTeam] = useState<product[]>([]);
    const [recommendationResponse, setRecommendedProducts] = useState<product[]>([]);
    const [recommandedVisible, setRecommandedVisible] = useState<boolean>(true);
    const [quantity, setQuantity] = useState<number>(1);
    const token = localStorage.getItem('token');

    async function AddToWinkelwagen() {
        if (!token) {
            alert('Log in eerst om producten toe te voegen');
            return;
        }
        try {
            const response = await fetch(`${API}/ShoppingCart/create`, {
                headers: {
                    'Content-Type': 'application/json',
                    Accept: 'application/json',
                    Authorization: `Bearer ${token}`,
                },
                method: 'POST',
                body: JSON.stringify({ productId: id, quantity }),
            });
            if (!response.ok) {
                const errorData = await response.text();
                throw new Error(`Server fout: ${response.status} - ${errorData}`);
            }
            alert('Product toegevoegd aan winkelwagen');

            const recResponse = await fetch(`${API}/User/recommended`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (recResponse.ok) {
                const recommendations = await recResponse.json();
                setRecommendedProducts(recommendations);
                setRecommandedVisible(true);
            }
        } catch (e) {
            console.log(`Something went wrong: ${e}`);
        }
    }

    async function AddToWinkelwagenRecommended(prodId: number) {
        if (!token) {
            alert('Log in eerst om producten toe te voegen');
            return;
        }
        try {
            const response = await fetch(`${API}/ShoppingCart/create`, {
                headers: {
                    'Content-Type': 'application/json',
                    Accept: 'application/json',
                    Authorization: `Bearer ${token}`,
                },
                method: 'POST',
                body: JSON.stringify({ productId: prodId, quantity }),
            });
            if (!response.ok) {
                const errorData = await response.text();
                throw new Error(`Server fout: ${response.status} - ${errorData}`);
            }
            alert('Product toegevoegd aan winkelwagen');
        } catch (e) {
            console.log(`Something went wrong: ${e}`);
        }
    }

    useEffect(() => {
        if (data) {
            AddRecentProducts(data);
            fetch(`${API}/Product/team/${data.teamId}`)
                .then((res) => res.json())
                .then((result) => setProductsByTeam(result))
                .catch((err) => console.log('Fetch error:', err));
        }
    }, [data]);

    if (isLoading) return <p style={{ padding: '2rem', color: 'var(--dark-green)' }}>Loading...</p>;
    if (error || !data) return <NotFound />;

    return (
        <div className="product-page">
            <p className="product-id-content">Product {id}</p>
            <div className="product-border-line" />

            {/* Main product card */}
            <div className="product-container">
                {/* Image */}
                <div className="product-content">
                    <img src={data.productImage} className="product-img-content" alt={data.name} />
                </div>

                {/* Info + Add to cart */}
                <div className="Costimizing-section">
                    <p className="product-label">Naam</p>
                    <h1 className="name">{data.name}</h1>

                    <p className="product-label">Prijs</p>
                    <p className="price">€ {data.price}</p>

                    <p className="product-label">Aantal</p>
                    <div className="Addtowinkelwagenwindow">
                        <input
                            className="quantity-input"
                            id="quantity"
                            type="number"
                            min={1}
                            max={11}
                            value={quantity}
                            onChange={(e) => {
                                const v = parseInt(e.target.value);
                                setQuantity(v > 11 ? 11 : v < 1 ? 1 : v);
                            }}
                        />
                        <button className="quantity-button" onClick={AddToWinkelwagen}>
                            Toevoegen
                        </button>
                    </div>

                    {/* Wishlist button — nu onder de toevoegen knop */}
                    {token && (
                        <div style={{ marginTop: '1rem' }}>
                            <AddToWishlistButton productId={Number.parseInt(id!)} />
                        </div>
                    )}
                </div>
            </div>

            {/* You may also like */}
            {productsByTeam.length > 0 && (
                <>
                    <p className="you-may-also-like-p-tag">You may also like</p>
                    <div className="border-line-may-also-like" />
                    <div className="Products-Team-Container">
                        {productsByTeam.map((prod) => (
                            <Link to={`/products/${prod.id}`} key={prod.id} className="link">
                                <div className="Product-Team-content">
                                    <img
                                        src={prod.productImage}
                                        className="products-Team-ProductImage"
                                        alt={prod.name}
                                    />
                                    <p className="products-Team-Name">{prod.name}</p>
                                    <p className="products-Team-Price-p-tag">€ {prod.price}</p>
                                </div>
                            </Link>
                        ))}
                    </div>
                    <div className="border-line-may-also-like-end" />
                </>
            )}

            {/* Recommendations popup */}
            {recommendationResponse.length > 0 && recommandedVisible && (
                <div className="rec-container">
                    <div className="rec-header">
                        <p className="rec-title">Aanbevolen</p>
                        <button className="cross-button" onClick={() => setRecommandedVisible(false)}>
                            ✕
                        </button>
                    </div>
                    {recommendationResponse.map((prod) => (
                        <div key={prod.id} className="Product-Team-content-rec">
                            <img
                                src={prod.productImage}
                                className="products-Team-ProductImage-rec"
                                alt={prod.name}
                            />
                            <div className="rec-item-info">
                                <p className="products-Team-Name-rec">{prod.name}</p>
                                <p className="products-Team-Price-p-tag-rec">€ {prod.price}</p>
                            </div>
                            <div className="Addtowinkelwagenwindow-rec">
                                <input
                                    className="quantity-input-rec"
                                    type="number"
                                    min={1}
                                    max={11}
                                    defaultValue={1}
                                    onChange={(e) => {
                                        const v = parseInt(e.target.value);
                                        setQuantity(v > 11 ? 11 : v < 1 ? 1 : v);
                                    }}
                                />
                                <button
                                    className="quantity-button-rec"
                                    onClick={() => AddToWinkelwagenRecommended(prod.id)}
                                >
                                    +
                                </button>
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
};

export default ProductDetail;
