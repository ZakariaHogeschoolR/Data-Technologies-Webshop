import { useEffect, useState } from 'react';
import type { Wishlist } from './Pages/ProfilePage';

interface AddToWishlistProp {
    productId: number;
}

const API = `http://localhost:5261/api/`;

export default function AddToWishlistButton({ productId }: AddToWishlistProp) {
    const [wishlists, setWishlists] = useState<Wishlist[]>([]);
    const [showDropdown, setShowDropdown] = useState(false);
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState('');

    useEffect(() => {
        if (!showDropdown) return;
        async function GetMyWishlists() {
            try {
                const token = localStorage.getItem('token');
                const response = await fetch(`${API}Wishlist/mine`, {
                    headers: { Authorization: `Bearer ${token}` },
                });
                if (!response.ok) throw new Error();
                const data = await response.json();
                const uniqueData = data.filter((w: any, idx: number, self: any[]) =>
                    self.findIndex((item) => item.name === w.name) === idx
                );
                setWishlists(uniqueData);
            } catch {
                console.log('could not load wishlists');
            }
        }
        GetMyWishlists();
    }, [showDropdown]);

    async function HandleAdd(wishlistName: string) {
        setLoading(true);
        setMessage('');
        try {
            const token = localStorage.getItem('token');
            const response = await fetch(`${API}wishlist/create`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({ name: wishlistName, productId }),
            });
            if (!response.ok) throw new Error();
            setMessage(`Toegevoegd aan ${wishlistName}`);
            setTimeout(() => {
                setMessage('');
                setShowDropdown(false);
            }, 2000);
        } catch (err) {
            setMessage('Er ging iets mis.');
        } finally {
            setLoading(false);
        }
    }

    return (
        <div style={{ position: 'relative', display: 'inline-block', marginBottom: '1rem' }}>
            <button
                onClick={() => setShowDropdown(!showDropdown)}
                style={{
                    fontFamily: '"Alfa Slab One", serif',
                    fontSize: '11px',
                    letterSpacing: '2px',
                    textTransform: 'uppercase',
                    color: 'var(--dark-green)',
                    background: 'transparent',
                    border: '2px solid var(--mint)',
                    borderRadius: '8px',
                    padding: '8px 16px',
                    cursor: 'pointer',
                    transition: 'background-color 0.2s',
                }}
                onMouseEnter={e => (e.currentTarget.style.backgroundColor = 'var(--mint)')}
                onMouseLeave={e => (e.currentTarget.style.backgroundColor = 'transparent')}
            >
                ♡ Wishlist
            </button>

            {showDropdown && (
                <div style={{
                    position: 'absolute',
                    top: 'calc(100% + 6px)',
                    left: 0,
                    minWidth: '200px',
                    background: 'var(--beige)',
                    border: '1.5px solid var(--mint)',
                    borderRadius: '10px',
                    boxShadow: '0 8px 24px rgba(15,61,46,0.12)',
                    zIndex: 300,
                    overflow: 'hidden',
                }}>
                    {wishlists.length === 0 ? (
                        <p style={{
                            fontFamily: '"Alfa Slab One", serif',
                            fontSize: '12px',
                            color: 'var(--dark-green)',
                            padding: '14px 18px',
                        }}>
                            Maak eerst een wishlist aan in je profiel.
                        </p>
                    ) : (
                        wishlists.map((list) => (
                            <button
                                key={list.id}
                                onClick={() => HandleAdd(list.name)}
                                disabled={loading}
                                style={{
                                    display: 'block',
                                    width: '100%',
                                    textAlign: 'left',
                                    fontFamily: '"Alfa Slab One", serif',
                                    fontSize: '12px',
                                    letterSpacing: '0.5px',
                                    color: 'var(--dark-green)',
                                    background: 'transparent',
                                    border: 'none',
                                    borderBottom: '1px solid rgba(102,205,170,0.2)',
                                    padding: '12px 18px',
                                    cursor: loading ? 'not-allowed' : 'pointer',
                                    transition: 'background-color 0.15s',
                                }}
                                onMouseEnter={e => (e.currentTarget.style.backgroundColor = '#eef7f3')}
                                onMouseLeave={e => (e.currentTarget.style.backgroundColor = 'transparent')}
                            >
                                {list.name}
                            </button>
                        ))
                    )}
                    {message && (
                        <p style={{
                            fontFamily: '"Alfa Slab One", serif',
                            fontSize: '11px',
                            color: 'var(--dark-green)',
                            background: '#d0f0e0',
                            padding: '10px 18px',
                            margin: 0,
                        }}>
                            {message}
                        </p>
                    )}
                </div>
            )}
        </div>
    );
}
