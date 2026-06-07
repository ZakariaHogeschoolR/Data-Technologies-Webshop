import {useEffect, useState} from 'react';
import {Link, useNavigate} from 'react-router-dom';
import '../../Styles/Profile.css';
import WishlistDetail from './WishlistDetail';

const API = 'http://localhost:5261/api';

type Profile = {
    id: number;
    firstName: string;
    lastName: string;
    username: string;
    email: string;
    address: string;
    postCode: string;
    role: string;
};

type OrderItem = {
    productId: number; quantity: number;
};

type Order = {
    orderId: number; date: string; items: OrderItem[];
};

export type ProductInfo = {
    id: number; name: string; productImage: string; price: number;
};

export type Wishlist = {
    id: number,
    name: string;
    userid: number;
    productid: number;
}

type Tab = 'profile' | 'password' | 'orders' | `wishlists`;

export default function ProfilePage() {
    const navigate = useNavigate();
    const token = localStorage.getItem('token');

    const [tab, setTab] = useState<Tab>('profile');
    const [profile, setProfile] = useState<Profile | null>(null);
    const [editing, setEditing] = useState(false);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [msg, setMsg] = useState<{ type: 'success' | 'error'; text: string } | null>(null);

    const [form, setForm] = useState({
        firstName: '', lastName: '', username: '', email: '', address: '', postCode: '',
    });

    const [pwForm, setPwForm] = useState({
        currentPassword: '', newPassword: '', confirmPassword: '',
    });

    const [orders, setOrders] = useState<Order[]>([]);
    const [productMap, setProductMap] = useState<Record<number, ProductInfo>>({});
    const [ordersLoading, setOrdersLoading] = useState(false);

    const [wishlists, setWislists] = useState<Wishlist[]>([]);
    const [newWishlistName, setNewWishlistName] = useState(``)
    const [isSubmitting, setIsSubmitting] = useState(false)
    const [formError, setFormError] = useState(``)

    async function handleCreateEmptyWishlist(e: React.FormEvent) {
        e.preventDefault()
        setFormError(``)

        if(!newWishlistName.trim()){
            setFormError(`naam is verplicht`)
            return;
        }
        try{
            setIsSubmitting(true)

            const token = localStorage.getItem(`token`)

            const response = await fetch(`${API}/Wishlist/create`, {
                method: `POST`,
                headers: {
                    "Content-Type": "application/json",
                    "Authorization" : `Bearer ${token}`
                },
                body: JSON.stringify({
                    name: newWishlistName.trim(),
                    productId: null
                })
            })
            if(!response.ok){
                if(response.status == 401) throw new Error(`je ben niet ingelogd`)
                throw new Error(`wishlist creation failure`)
            }
            const CreatedWishlist = await response.json()

            setWislists([...wishlists, CreatedWishlist])
            setNewWishlistName(``)
        }
        catch(e: any){
            setFormError(e.message || `err ging iets mis`)
        }
        finally{
            setIsSubmitting(false)
        }
}

    useEffect(() => {
        if(!token) return;

        async function GetWishLists(){
            try {
                const query = API + `/Wishlist/mine`
                const reponse = await fetch(query, {headers: {Authorization: `Bearer ${token}`}})
                if (!reponse.ok) throw new Error(`failed to load wishlists`)
                const data : Wishlist[] = await reponse.json()
                console.log(data)
                setWislists(data)
            }
            catch{
                setMsg({type: `error`, text: `could not load wishlists`})
            }
        }
        GetWishLists()
    }, [token])


    useEffect(() => {
        if (!token) navigate('/auth');
    }, [token, navigate]);

    useEffect(() => {
        if (!token) return;

        async function load() {
            setLoading(true);
            try {
                const res = await fetch(`${API}/User/me`, {
                    headers: {Authorization: `Bearer ${token}`},
                });
                if (!res.ok) throw new Error('Failed to load profile');
                const data: Profile = await res.json();
                setProfile(data);
                setForm({
                    firstName: data.firstName,
                    lastName: data.lastName,
                    username: data.username,
                    email: data.email,
                    address: data.address,
                    postCode: data.postCode,
                });
            } catch {
                setMsg({type: 'error', text: 'Could not load profile.'});
            } finally {
                setLoading(false);
            }
        }

        load();
    }, [token]);

    useEffect(() => {
        if (tab !== 'orders' || !token) return;

        async function loadOrders() {
            setOrdersLoading(true);
            try {
                const res = await fetch(`${API}/ShoppingCart/history`, {
                    headers: {Authorization: `Bearer ${token}`},
                });
                if (!res.ok) throw new Error();
                const data: Order[] = await res.json();
                setOrders(data);

                const allIds = Array.from(new Set(data.flatMap(o => o.items.map(i => i.productId))));
                const entries = await Promise.all(allIds.map(id => fetch(`${API}/Product/${id}`)
                    .then(r => r.json() as Promise<ProductInfo>)
                    .then(p => [p.id, p] as [number, ProductInfo])));
                setProductMap(Object.fromEntries(entries));
            } catch {
            } finally {
                setOrdersLoading(false);
            }
        }

        loadOrders();
    }, [tab, token]);

    const clearMsg = () => setMsg(null);

    const handleSaveProfile = async () => {
        clearMsg();
        setSaving(true);
        try {
            const res = await fetch(`${API}/User/me/update`, {
                method: 'PUT', headers: {
                    'Content-Type': 'application/json', Authorization: `Bearer ${token}`,
                }, body: JSON.stringify(form),
            });
            if (!res.ok) {
                const d = await res.json().catch(() => ({}));
                throw new Error(d.message ?? 'Update failed');
            }
            setProfile({...profile!, ...form});
            setEditing(false);
            setMsg({type: 'success', text: 'Profile updated successfully!'});
        } catch (err) {
            setMsg({type: 'error', text: err instanceof Error ? err.message : 'Something went wrong'});
        } finally {
            setSaving(false);
        }
    };

    const handleChangePassword = async () => {
        clearMsg();
        if (pwForm.newPassword !== pwForm.confirmPassword) {
            setMsg({type: 'error', text: 'New passwords do not match.'});
            return;
        }
        if (pwForm.newPassword.length < 4) {
            setMsg({type: 'error', text: 'New password must be at least 4 characters.'});
            return;
        }
        setSaving(true);
        try {
            const res = await fetch(`${API}/User/me/update/password`, {
                method: 'PUT', headers: {
                    'Content-Type': 'application/json', Authorization: `Bearer ${token}`,
                }, body: JSON.stringify({
                    currentPassword: pwForm.currentPassword, newPassword: pwForm.newPassword,
                }),
            });
            if (!res.ok) {
                const d = await res.json().catch(() => ({}));
                throw new Error(d.message ?? 'Password change failed');
            }
            setPwForm({currentPassword: '', newPassword: '', confirmPassword: ''});
            setMsg({type: 'success', text: 'Password changed successfully!'});
        } catch (err) {
            setMsg({type: 'error', text: err instanceof Error ? err.message : 'Something went wrong'});
        } finally {
            setSaving(false);
        }
    };

    const handleTabSwitch = (t: Tab) => {
        setTab(t);
        clearMsg();
        setEditing(false);
    };

    if (loading) return <p style={{padding: '2rem'}}>Loading...</p>;

    return (<div className="profile-page">
        <h1 className="profile-heading">My Account</h1>

        <div className="profile-tabs">
            {(['profile', 'password', 'orders', `wishlists`] as Tab[]).map(t => (<button
                key={t}
                className={`profile-tab ${tab === t ? 'active' : ''}`}
                onClick={() => handleTabSwitch(t)}
            >
                {t === 'profile' ? 'Profile' : t === 'password' ? 'Password' : t === `wishlists` ? `Wishlists`: 'Order History'}
            </button>))}
        </div>

        {tab === 'profile' && (<div className="profile-card">
            {msg && <p className={msg.type === 'success' ? 'profile-success' : 'profile-error'}>{msg.text}</p>}

            {!editing ? (<>
                <div className="profile-info-grid">
                    <div className="profile-info-item">
                        <label>First Name</label>
                        <p>{profile?.firstName}</p>
                    </div>
                    <div className="profile-info-item">
                        <label>Last Name</label>
                        <p>{profile?.lastName}</p>
                    </div>
                    <div className="profile-info-item">
                        <label>Username</label>
                        <p>{profile?.username}</p>
                    </div>
                    <div className="profile-info-item">
                        <label>Email</label>
                        <p>{profile?.email}</p>
                    </div>
                    <div className="profile-info-item">
                        <label>Address</label>
                        <p>{profile?.address}</p>
                    </div>
                    <div className="profile-info-item">
                        <label>Post Code</label>
                        <p>{profile?.postCode}</p>
                    </div>
                    <div className="profile-info-item">
                        <label>Role</label>
                        <p style={{textTransform: 'capitalize'}}>{profile?.role}</p>
                    </div>
                </div>
                <button className="profile-edit-btn" onClick={() => {
                    clearMsg();
                    setEditing(true);
                }}>
                    Edit Profile
                </button>
            </>) : (<div className="profile-form">
                <div className="profile-form-row">
                    <div className="profile-field">
                        <label>First Name</label>
                        <input
                            value={form.firstName}
                            onChange={e => setForm({...form, firstName: e.target.value})}
                            placeholder="John"
                        />
                    </div>
                    <div className="profile-field">
                        <label>Last Name</label>
                        <input
                            value={form.lastName}
                            onChange={e => setForm({...form, lastName: e.target.value})}
                            placeholder="Doe"
                        />
                    </div>
                </div>
                <div className="profile-field">
                    <label>Username</label>
                    <input
                        value={form.username}
                        onChange={e => setForm({...form, username: e.target.value})}
                        placeholder="johndoe"
                    />
                </div>
                <div className="profile-field">
                    <label>Email</label>
                    <input
                        type="email"
                        value={form.email}
                        onChange={e => setForm({...form, email: e.target.value})}
                        placeholder="you@example.com"
                    />
                </div>
                <div className="profile-field">
                    <label>Address</label>
                    <input
                        value={form.address}
                        onChange={e => setForm({...form, address: e.target.value})}
                        placeholder="123 Main Street"
                    />
                </div>
                <div className="profile-field">
                    <label>Post Code</label>
                    <input
                        value={form.postCode}
                        onChange={e => setForm({...form, postCode: e.target.value})}
                        placeholder="1234AB"
                    />
                </div>
                <div className="profile-form-actions">
                    <button className="profile-save-btn" onClick={handleSaveProfile} disabled={saving}>
                        {saving ? 'Saving...' : 'Save Changes'}
                    </button>
                    <button className="profile-cancel-btn" onClick={() => {
                        setEditing(false);
                        clearMsg();
                        if (profile) setForm({
                            firstName: profile.firstName,
                            lastName: profile.lastName,
                            username: profile.username,
                            email: profile.email,
                            address: profile.address,
                            postCode: profile.postCode,
                        });
                    }}>
                        Cancel
                    </button>
                </div>
            </div>)}
        </div>)}

        {tab === 'password' && (<div className="profile-card">
            <p className="profile-section-title">Change Password</p>
            {msg && <p className={msg.type === 'success' ? 'profile-success' : 'profile-error'}>{msg.text}</p>}
            <div className="profile-form">
                <div className="profile-field">
                    <label>Current Password</label>
                    <input
                        type="password"
                        placeholder="••••••••"
                        value={pwForm.currentPassword}
                        onChange={e => setPwForm({...pwForm, currentPassword: e.target.value})}
                    />
                </div>
                <div className="profile-field">
                    <label>New Password</label>
                    <input
                        type="password"
                        placeholder="••••••••"
                        value={pwForm.newPassword}
                        onChange={e => setPwForm({...pwForm, newPassword: e.target.value})}
                    />
                </div>
                <div className="profile-field">
                    <label>Confirm New Password</label>
                    <input
                        type="password"
                        placeholder="••••••••"
                        value={pwForm.confirmPassword}
                        onChange={e => setPwForm({...pwForm, confirmPassword: e.target.value})}
                    />
                </div>
                <div className="profile-form-actions">
                    <button className="profile-save-btn" onClick={handleChangePassword} disabled={saving}>
                        {saving ? 'Updating...' : 'Update Password'}
                    </button>
                </div>
            </div>
        </div>)}

        {tab === 'orders' && (<div className="profile-card">
            <p className="profile-section-title">Order History</p>
            {ordersLoading ? (
                <p style={{color: '#aaa', fontSize: '14px'}}>Loading orders...</p>) : orders.length === 0 ? (
                <p className="order-empty">No orders yet. Start shopping!</p>) : (<div className="order-list">
                {orders.map(order => {
                    const total = order.items.reduce((sum, item) => {
                        const p = productMap[item.productId];
                        return sum + (p ? p.price * item.quantity : 0);
                    }, 0);

                    return (<div key={order.orderId} className="order-card">
                        <div className="order-card-header">
                            <span>Order #{order.orderId}</span>
                            <span className="order-date">{order.date}</span>
                        </div>
                        <ul className="order-items-list">
                            {order.items.map(item => {
                                const p = productMap[item.productId];
                                return (<li key={item.productId} className="order-item-row">
                                    {p ? (<img src={p.productImage} alt={p.name}/>) : (<div style={{
                                        width: 44, height: 44, background: '#eee', borderRadius: 6
                                    }}/>)}
                                    <div className="order-item-info">
                                                            <span
                                                                className="item-name">{p ? p.name : `Product #${item.productId}`}</span>
                                        <span className="item-qty">Qty: {item.quantity}</span>
                                    </div>
                                    {p && (<span className="order-item-price">
                                                                €{(p.price * item.quantity).toFixed(2)}
                                                            </span>)}
                                </li>);
                            })}
                        </ul>
                        <div className="order-total-row">
                            <span>Total:</span>
                            <strong>€{total.toFixed(2)}</strong>
                        </div>
                    </div>);
                })}
            </div>)}
        </div>)}
        {tab === 'wishlists' && (
            <div className="profile-card">
                <p className="profile-section-title">My Wishlists</p>

                <form onSubmit={handleCreateEmptyWishlist} className="wishlist-create-form">
                    <div className="wishlist-create-row">
                        <input
                            type="text"
                            placeholder="New wishlist name..."
                            value={newWishlistName}
                            onChange={(e) => setNewWishlistName(e.target.value)}
                            disabled={isSubmitting}
                            className="wishlist-input"
                        />
                        <button
                            type="submit"
                            disabled={isSubmitting}
                            className="wishlist-create-btn"
                        >
                            {isSubmitting ? 'Creating...' : '+ Create'}
                        </button>
                    </div>
                    {formError && <p className="profile-error" style={{ marginTop: '0.5rem' }}>{formError}</p>}
                </form>

                {wishlists.length === 0 ? (
                    <p className="order-empty">No wishlists yet. Create one above!</p>
                ) : (
                    <div className="wishlist-list">
                        {wishlists
                            .filter((w, index, self) => self.findIndex(x => x.name === w.name) === index)
                            .map((wishlist) => (
                                <Link
                                    key={wishlist.id}
                                    to={`/wishlist/${wishlist.id}`}
                                    state={{ allWishlist: wishlists, currentName: wishlist.name }}
                                    className="wishlist-card"
                                >
                                    <span className="wishlist-card-icon">♡</span>
                                    <span className="wishlist-card-name">{wishlist.name}</span>
                                    <span className="wishlist-card-arrow">→</span>
                                </Link>
                            ))}
                    </div>
                )}
            </div>
        )}
    </div>);
}
