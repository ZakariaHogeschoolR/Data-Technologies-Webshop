import { useEffect, useState } from 'react';
import type { Wishlist } from './Pages/ProfilePage';

interface AddToWishlistProp {
    productId: number;
}

const API = `http://localhost:5261/api/`;

export default function AddToWishlistButton({ productId }: AddToWishlistProp) {
    const [wishlists, setWishlists] = useState<Wishlist[]>([]);
    const [allEntries, setAllEntries] = useState<Wishlist[]>([]); // alle rijen, ook duplicaten
    const [showDropdown, setShowDropdown] = useState(false);
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState('');

    const [showNewInput, setShowNewInput] = useState(false);
    const [newWishlistName, setNewWishlistName] = useState('');
    const [creating, setCreating] = useState(false);

    useEffect(() => {
        if (!showDropdown) return;
        async function GetMyWishlists() {
            try {
                const token = localStorage.getItem('token');
                const response = await fetch(`${API}Wishlist/mine`, {
                    headers: { Authorization: `Bearer ${token}` },
                });
                if (!response.ok) throw new Error();
                const data: Wishlist[] = await response.json();

                // Alle rijen bewaren voor productcheck
                setAllEntries(data);

                // Unieke namen voor de lijst
                const uniqueData = data.filter((w, idx, self) =>
                    self.findIndex((item) => item.name === w.name) === idx
                );
                setWishlists(uniqueData);
            } catch {
                console.log('could not load wishlists');
            }
        }
        GetMyWishlists();
    }, [showDropdown]);

    // Check of product al in een wishlist zit
    function isAlreadyInWishlist(wishlistName: string): boolean {
        return allEntries.some(
            entry => entry.name === wishlistName &&
            (entry.productid === productId || (entry as any).productid === productId)
        );
    }

    async function HandleAdd(wishlistName: string) {
        if (isAlreadyInWishlist(wishlistName)) {
            setMessage(`Al in "${wishlistName}"`);
            setTimeout(() => setMessage(''), 2000);
            return;
        }
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
            // Update allEntries lokaal zodat de check direct werkt
            setAllEntries(prev => [...prev, { id: 0, name: wishlistName, userid: 0, productid: productId }]);
            setTimeout(() => {
                setMessage('');
                setShowDropdown(false);
            }, 2000);
        } catch {
            setMessage('Er ging iets mis.');
        } finally {
            setLoading(false);
        }
    }

    async function HandleCreateAndAdd() {
        if (!newWishlistName.trim()) return;
        setCreating(true);
        setMessage('');
        try {
            const token = localStorage.getItem('token');
            const response = await fetch(`${API}wishlist/create`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({ name: newWishlistName.trim(), productId }),
            });
            if (!response.ok) throw new Error();
            setMessage(`"${newWishlistName.trim()}" aangemaakt!`);
            setNewWishlistName('');
            setShowNewInput(false);
            setTimeout(() => {
                setMessage('');
                setShowDropdown(false);
            }, 2000);
        } catch {
            setMessage('Er ging iets mis.');
        } finally {
            setCreating(false);
        }
    }

    const dropdownStyle: React.CSSProperties = {
        position: 'absolute',
        top: 'calc(100% + 6px)',
        left: 0,
        minWidth: '220px',
        background: 'var(--beige)',
        border: '1.5px solid var(--mint)',
        borderRadius: '10px',
        boxShadow: '0 8px 24px rgba(15,61,46,0.12)',
        zIndex: 300,
        overflow: 'hidden',
    };

    return (
        <div style={{ position: 'relative', display: 'inline-block', marginBottom: '1rem' }}>
            <button
                onClick={() => { setShowDropdown(!showDropdown); setShowNewInput(false); setMessage(''); }}
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
                <div style={dropdownStyle}>
                    {wishlists.length === 0 ? (
                        <p style={{ fontFamily: '"Alfa Slab One", serif', fontSize: '12px', color: '#aaa', padding: '12px 18px', margin: 0 }}>
                            Nog geen wishlists.
                        </p>
                    ) : (
                        wishlists.map((list) => {
                            const alreadyAdded = isAlreadyInWishlist(list.name);
                            return (
                                <button
                                    key={list.id}
                                    onClick={() => HandleAdd(list.name)}
                                    disabled={loading || alreadyAdded}
                                    style={{
                                        display: 'block',
                                        width: '100%',
                                        textAlign: 'left',
                                        fontFamily: '"Alfa Slab One", serif',
                                        fontSize: '12px',
                                        letterSpacing: '0.5px',
                                        color: alreadyAdded ? '#aaa' : 'var(--dark-green)',
                                        background: alreadyAdded ? '#f5f5f5' : 'transparent',
                                        border: 'none',
                                        borderBottom: '1px solid rgba(102,205,170,0.2)',
                                        padding: '12px 18px',
                                        cursor: alreadyAdded ? 'not-allowed' : 'pointer',
                                        transition: 'background-color 0.15s',
                                    }}
                                    onMouseEnter={e => { if (!alreadyAdded) e.currentTarget.style.backgroundColor = '#eef7f3'; }}
                                    onMouseLeave={e => { if (!alreadyAdded) e.currentTarget.style.backgroundColor = 'transparent'; }}
                                >
                                    {alreadyAdded ? `✓ ${list.name}` : `♡ ${list.name}`}
                                </button>
                            );
                        })
                    )}

                    {!showNewInput ? (
                        <button
                            onClick={() => setShowNewInput(true)}
                            style={{
                                display: 'block',
                                width: '100%',
                                textAlign: 'left',
                                fontFamily: '"Alfa Slab One", serif',
                                fontSize: '11px',
                                letterSpacing: '1px',
                                color: 'var(--dark-green)',
                                background: '#eef7f3',
                                border: 'none',
                                borderTop: '1px solid rgba(102,205,170,0.2)',
                                padding: '12px 18px',
                                cursor: 'pointer',
                            }}
                            onMouseEnter={e => (e.currentTarget.style.backgroundColor = 'var(--mint)')}
                            onMouseLeave={e => (e.currentTarget.style.backgroundColor = '#eef7f3')}
                        >
                            ➕ Nieuwe wishlist
                        </button>
                    ) : (
                        <div style={{ padding: '12px 18px', display: 'flex', flexDirection: 'column', gap: '8px', borderTop: '1px solid rgba(102,205,170,0.2)' }}>
                            <input
                                autoFocus
                                type="text"
                                placeholder="Naam..."
                                value={newWishlistName}
                                onChange={e => setNewWishlistName(e.target.value)}
                                onKeyDown={e => { if (e.key === 'Enter') HandleCreateAndAdd(); }}
                                style={{
                                    fontFamily: '"Alfa Slab One", serif',
                                    fontSize: '12px',
                                    color: 'var(--dark-green)',
                                    background: 'var(--white)',
                                    border: '1.5px solid var(--mint)',
                                    borderRadius: '6px',
                                    padding: '8px 10px',
                                    outline: 'none',
                                    width: '100%',
                                    boxSizing: 'border-box',
                                }}
                            />
                            <div style={{ display: 'flex', gap: '6px' }}>
                                <button
                                    onClick={HandleCreateAndAdd}
                                    disabled={creating || !newWishlistName.trim()}
                                    style={{
                                        flex: 1,
                                        fontFamily: '"Alfa Slab One", serif',
                                        fontSize: '11px',
                                        letterSpacing: '1px',
                                        background: 'var(--dark-green)',
                                        color: 'var(--beige)',
                                        border: 'none',
                                        borderRadius: '6px',
                                        padding: '8px',
                                        cursor: creating ? 'not-allowed' : 'pointer',
                                        opacity: creating ? 0.6 : 1,
                                    }}
                                >
                                    {creating ? '...' : 'Aanmaken'}
                                </button>
                                <button
                                    onClick={() => { setShowNewInput(false); setNewWishlistName(''); }}
                                    style={{
                                        fontFamily: '"Alfa Slab One", serif',
                                        fontSize: '11px',
                                        background: 'transparent',
                                        color: '#aaa',
                                        border: '1.5px solid #ddd',
                                        borderRadius: '6px',
                                        padding: '8px 10px',
                                        cursor: 'pointer',
                                    }}
                                >
                                    ✕
                                </button>
                            </div>
                        </div>
                    )}

                    {message && (
                        <p style={{
                            fontFamily: '"Alfa Slab One", serif',
                            fontSize: '11px',
                            color: message.startsWith('Al in') ? '#b00' : 'var(--dark-green)',
                            background: message.startsWith('Al in') ? '#ffe0e0' : '#d0f0e0',
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
