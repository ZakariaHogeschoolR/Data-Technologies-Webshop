import {useState} from 'react';
import {Link, NavLink, useNavigate} from 'react-router-dom';
import '../../Styles/Navbar.css';

const navLinks = [{label: 'Shop', to: '/'}, {label: 'New', to: '/'}, {label: 'Sale', to: '/'}, {
    label: 'Journal',
    to: '/'
}, {label: 'About', to: '/about'},];

const Navbar = () => {
    const [username, setUsername] = useState<string | null>(localStorage.getItem('username'));
    const [menuOpen, setMenuOpen] = useState(false);
    const navigate = useNavigate();

    const handleLogout = async () => {
        try {
            await fetch('http://localhost:5261/api/Auth/logout', {method: 'POST'});
        } catch {
            // no-op for UI demo
        }
        localStorage.removeItem('token');
        localStorage.removeItem('username');
        localStorage.removeItem('role');
        setUsername(null);
        navigate('/');
    };

    return (<header className="nav-root">
            <div className="nav-bar">
                <button
                    type="button"
                    className="nav-burger"
                    aria-label="Open menu"
                    aria-expanded={menuOpen}
                    onClick={() => setMenuOpen((v) => !v)}
                >
                    <span/>
                    <span/>
                    <span/>
                </button>

                <nav className="nav-links" aria-label="Primary">
                    {navLinks.map((l) => (<NavLink
                            key={l.label}
                            to={l.to}
                            end={l.to === '/'}
                            className={({isActive}) => isActive ? 'nav-link is-active' : 'nav-link'}
                        >
                            {l.label}
                        </NavLink>))}
                </nav>

                <Link to="/" className="nav-brand" aria-label="Atelier Verte home">
                    Atelier Verte
                </Link>

                <div className="nav-actions">
                    {username ? (<button
                            type="button"
                            className="nav-text-btn"
                            onClick={handleLogout}
                            aria-label={`Logout ${username}`}
                        >
                            Logout
                        </button>) : (<Link to="/auth" className="nav-icon" aria-label="Account">
                            <svg viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="currentColor"
                                 strokeWidth="1.6" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
                                <circle cx="12" cy="8" r="4"/>
                                <path d="M4 21c0-4.418 3.582-8 8-8s8 3.582 8 8"/>
                            </svg>
                        </Link>)}

                    <Link to="/winkelwagen/mine" className="nav-icon nav-cart" aria-label="Cart, 2 items">
                        <svg viewBox="0 0 24 24" width="20" height="20" fill="none" stroke="currentColor"
                             strokeWidth="1.6" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
                            <path d="M5 7h14l-1.2 11.1a2 2 0 0 1-2 1.9H8.2a2 2 0 0 1-2-1.9L5 7Z"/>
                            <path d="M9 7a3 3 0 1 1 6 0"/>
                        </svg>
                        <span className="nav-cart-badge" aria-hidden="true">2</span>
                    </Link>
                </div>
            </div>

            {menuOpen ? (<div className="nav-mobile" role="dialog" aria-label="Mobile menu">
                    {navLinks.map((l) => (<Link
                            key={l.label}
                            to={l.to}
                            className="nav-mobile-link"
                            onClick={() => setMenuOpen(false)}
                        >
                            {l.label}
                        </Link>))}
                </div>) : null}
        </header>);
};

export default Navbar;
