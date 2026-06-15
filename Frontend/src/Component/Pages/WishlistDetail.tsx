import { useParams, useLocation, useNavigate } from 'react-router-dom';
import '../../Styles/Wishlist.css';
import { useEffect, useState } from 'react';
import type { Wishlist } from './ProfilePage';
import type { ProductInfo } from './ProfilePage';

const API = `http://localhost:5261/api/`;

export default function WishlistDetail() {
    const { id } = useParams<{ id: string }>();
    const location = useLocation();
    const navigate = useNavigate();

    const allWishlists = location.state?.allWishlist as Wishlist[] | undefined;
    const currentName = location.state?.currentName as string | undefined;

    const [wishlistName, setWishlistName] = useState<string>(currentName || '');
    const [products, setProducts] = useState<ProductInfo[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [isDeleting, setIsDeleting] = useState(false);
    const [deletingProductId, setDeletingProductId] = useState<number | null>(null);

    async function HandleDeleteWishlist() {
        if (!window.confirm(`Weet je zeker dat je "${wishlistName}" wilt verwijderen?`)) return;
        try {
            setIsDeleting(true);
            const token = localStorage.getItem('token');
            const response = await fetch(`${API}Wishlist/delete/${id}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`,
                },
            });
            if (!response.ok) throw new Error('Wishlist kon niet worden verwijderd');
            navigate('/profile');
        } catch (e: any) {
            setError(e.message || 'Verwijderen mislukt');
        } finally {
            setIsDeleting(false);
        }
    }

    async function HandleDeleteProduct(productId: number) {
        if (!window.confirm('Weet je zeker dat je dit product wilt verwijderen?')) return;
        try {
            setDeletingProductId(productId);
            const token = localStorage.getItem('token');
            const response = await fetch(`${API}Wishlist/delete/product`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({ id: productId, wishlistName }),
            });
            if (!response.ok) throw new Error('Product kon niet worden verwijderd');
            // Remove product from local state
            setProducts(prev => prev.filter(p => p.id !== productId));
        } catch (e: any) {
            setError(e.message || 'Verwijderen mislukt');
        } finally {
            setDeletingProductId(null);
        }
    }

    useEffect(() => {
        async function getData() {
            setError('');
            try {
                setLoading(true);
                let items: Wishlist[] = [];

                if (allWishlists && currentName) {
                    items = allWishlists.filter(w => w.name === currentName);
                } else if (id) {
                    const res = await fetch(`${API}Wishlist/${id}`);
                    if (!res.ok) throw new Error('Wishlist kon niet worden opgehaald');
                    const data: Wishlist[] = await res.json();
                    if (Array.isArray(data) && data.length > 0) {
                        setWishlistName(data[0].name);
                        items = data;
                    }
                }

                if (items.length === 0) {
                    setLoading(false);
                    return;
                }

                const validItems = items.filter(item => {
                    const pid = item.productid ?? (item as any).productid;
                    return pid !== null && pid !== undefined && pid !== 0;
                });

                const promises = validItems.map(item => {
                    const pid = item.productid ?? (item as any).productid;
                    return fetch(`${API}Product/${pid}`).then(res => {
                        if (!res.ok) throw new Error('Product niet gevonden');
                        return res.json() as Promise<ProductInfo>;
                    });
                });

                const productData = await Promise.all(promises);
                setProducts(productData);
            } catch (e) {
                setError('Kon de wishlist niet laden.');
            } finally {
                setLoading(false);
            }
        }
        getData();
    }, [id, allWishlists, currentName]);

    if (loading) return <p style={{ padding: '2rem', color: 'var(--dark-green)', letterSpacing: '2px', fontSize: '13px' }}>Loading...</p>;
    if (error) return <p style={{ padding: '2rem', color: '#b00' }}>{error}</p>;

    return (
        <div className="wishlistWrapper">
            <div className="wishlist-header">
                <h1 className="wishlistName">{wishlistName}</h1>
                <button
                    className="wishlist-delete-btn"
                    onClick={HandleDeleteWishlist}
                    disabled={isDeleting}
                >
                    {isDeleting ? 'Verwijderen...' : '🗑 Delete wishlist'}
                </button>
            </div>

            <div className="productsList">
                {products.length === 0 ? (
                    <p className="noProducts">Nog geen producten in deze wishlist.</p>
                ) : (
                    products.map((product) => (
                        <div key={product.id} className="wishlistProductItem">
                            <img
                                src={product.productImage}
                                alt={product.name}
                                className="wishlist-product-img"
                            />
                            <div className="wishlist-product-info">
                                <p className="wishlist-product-name">{product.name}</p>
                                <p className="wishlist-product-price">€ {product.price}</p>
                                <button
                                    className="wishlist-remove-product-btn"
                                    onClick={() => HandleDeleteProduct(product.id)}
                                    disabled={deletingProductId === product.id}
                                >
                                    {deletingProductId === product.id ? 'Verwijderen...' : '✕ Remove'}
                                </button>
                            </div>
                        </div>
                    ))
                )}
            </div>
        </div>
    );
}
