import {useState} from 'react';
import '../../Styles/Auth.css';

const API = 'http://localhost:5261/api/Auth';

type Tab = 'login' | 'register';

const Auth = () => {
    const [tab, setTab] = useState<Tab>('login');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    const [loggedIn, setLoggedIn] = useState<string | null>(localStorage.getItem('username'));

    const [loginEmail, setLoginEmail] = useState('');
    const [loginPassword, setLoginPassword] = useState('');

    const [reg, setReg] = useState({
        firstName: '', lastName: '', username: '', email: '', password: '', address: '', postCode: '',
    });

    const reset = () => {
        setError('');
        setSuccess('');
    };

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();
        reset();
        setLoading(true);
        try {
            const res = await fetch(`${API}/login`, {
                method: 'POST',
                headers: {'Content-Type': 'application/json'},
                body: JSON.stringify({email: loginEmail, password: loginPassword}),
            });

            if (!res.ok) {
                const data = await res.json().catch(() => ({}));
                throw new Error(data.message ?? 'Invalid email or password');
            }

            const {token, username, role} = await res.json();

            localStorage.setItem('token', token);
            localStorage.setItem('username', username);
            localStorage.setItem('role', role);

            setSuccess('Logged in!');
            setLoggedIn(username);

            window.location.href = '/';
        } catch (err: unknown) {
            setError(err instanceof Error ? err.message : 'Something went wrong');
        } finally {
            setLoading(false);
        }
    };


    const handleRegister = async (e: React.FormEvent) => {
        e.preventDefault();
        reset();
        setLoading(true);
        try {
            const res = await fetch(`${API}/register`, {
                method: 'POST', headers: {'Content-Type': 'application/json'}, body: JSON.stringify(reg),
            });

            if (!res.ok) {
                const data = await res.json().catch(() => ({}));
                throw new Error(data.message ?? 'Registration failed');
            }

            const data = await res.json();
            setSuccess((data.message ?? 'Registered!') + ' You can now log in.');
            setTab('login');
        } catch (err: unknown) {
            setError(err instanceof Error ? err.message : 'Something went wrong');
        } finally {
            setLoading(false);
        }
    };


    const handleLogout = async () => {
        reset();
        await fetch(`${API}/logout`, {method: 'POST'});
        localStorage.removeItem('token');
        localStorage.removeItem('username');
        localStorage.removeItem('role');
        setLoggedIn(null);
    };

    return (<div className="auth-page">
        <div className="auth-card">
            {loggedIn ? (<div className="auth-logged-in">
                <h2>Welcome back!</h2>
                <p>{loggedIn}</p>
                {error && <p className="auth-error">{error}</p>}
                {success && <p className="auth-success">{success}</p>}
                <button className="auth-logout" onClick={handleLogout} disabled={loading}>
                    {loading ? 'Logging out...' : 'Log Out'}
                </button>
            </div>) : (<>
                <div className="auth-tabs">
                    <button
                        className={`auth-tab ${tab === 'login' ? 'active' : ''}`}
                        onClick={() => {
                            setTab('login');
                            reset();
                        }}
                    >
                        Login
                    </button>
                    <button
                        className={`auth-tab ${tab === 'register' ? 'active' : ''}`}
                        onClick={() => {
                            setTab('register');
                            reset();
                        }}
                    >
                        Register
                    </button>
                </div>

                {error && <p className="auth-error" style={{marginBottom: 16}}>{error}</p>}
                {success && <p className="auth-success" style={{marginBottom: 16}}>{success}</p>}

                {tab === 'login' && (<form className="auth-form" onSubmit={handleLogin}>
                    <div className="auth-field">
                        <label>Email</label>
                        <input type="email" placeholder="you@example.com" value={loginEmail}
                               onChange={e => setLoginEmail(e.target.value)} required/>
                    </div>
                    <div className="auth-field">
                        <label>Password</label>
                        <input type="password" placeholder="••••••••" value={loginPassword}
                               onChange={e => setLoginPassword(e.target.value)} required/>
                    </div>
                    <button className="auth-submit" type="submit" disabled={loading}>
                        {loading ? 'Logging in...' : 'Log In'}
                    </button>
                </form>)}

                {tab === 'register' && (<form className="auth-form" onSubmit={handleRegister}>
                    <div className="auth-row">
                        <div className="auth-field">
                            <label>First Name</label>
                            <input placeholder="John" value={reg.firstName}
                                   onChange={e => setReg({...reg, firstName: e.target.value})} required/>
                        </div>
                        <div className="auth-field">
                            <label>Last Name</label>
                            <input placeholder="Doe" value={reg.lastName}
                                   onChange={e => setReg({...reg, lastName: e.target.value})} required/>
                        </div>
                    </div>
                    <div className="auth-field">
                        <label>Username</label>
                        <input placeholder="johndoe" value={reg.username}
                               onChange={e => setReg({...reg, username: e.target.value})} required/>
                    </div>
                    <div className="auth-field">
                        <label>Email</label>
                        <input type="email" placeholder="you@example.com" value={reg.email}
                               onChange={e => setReg({...reg, email: e.target.value})} required/>
                    </div>
                    <div className="auth-field">
                        <label>Password</label>
                        <input type="password" placeholder="••••••••" value={reg.password}
                               onChange={e => setReg({...reg, password: e.target.value})} required/>
                    </div>
                    <div className="auth-field">
                        <label>Address</label>
                        <input placeholder="123 Main Street" value={reg.address}
                               onChange={e => setReg({...reg, address: e.target.value})} required/>
                    </div>
                    <div className="auth-field">
                        <label>Post Code</label>
                        <input placeholder="1234AB" value={reg.postCode}
                               onChange={e => setReg({...reg, postCode: e.target.value})} required/>
                    </div>
                    <button className="auth-submit" type="submit" disabled={loading}>
                        {loading ? 'Creating account...' : 'Create Account'}
                    </button>
                </form>)}
            </>)}
        </div>
    </div>);
};

export default Auth;