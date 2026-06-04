import { Link, useNavigate, NavLink } from 'react-router-dom';
import '../../Styles/Navbar.css';
import winkelwagenIcon from '../../assets/shopping-cart.png';
import { useFetch } from '../../CustomHooks/GetFetchHook';
import { useState, useEffect, useRef } from 'react';
import NotFound from '../Pages/NotFound';

type categoryType = {
    id: number;
    name: string;
};

const Navbar = () => {
    const { data, isLoading, error } = useFetch<categoryType[]>({ url: `http://localhost:5261/api/Category` });
    const [search, setSearch] = useState('');
    const [results, setResults] = useState<any[]>([]);
    const [username, setUsername] = useState<string | null>(localStorage.getItem('username'));
    const [categories, setCategories] = useState<categoryType[]>([]);
    const navigate = useNavigate();
    const searchRef = useRef<HTMLDivElement>(null);

    const handleSearch = async (value: string) => {
        setSearch(value);
        if (value.length < 2) {
            setResults([]);
            return;
        }
        const res = await fetch(`http://localhost:5261/api/Product/search?name=${value}`);
        const data = await res.json();
        setResults(data);
    };

    const handleLogout = async () => {
        await fetch('http://localhost:5261/api/Auth/logout', { method: 'POST' });
        localStorage.removeItem('token');
        localStorage.removeItem('username');
        localStorage.removeItem('role');
        setUsername(null);
        navigate('/');
    };

    // Close search results when clicking outside
    useEffect(() => {
        const handleClickOutside = (e: MouseEvent) => {
            if (searchRef.current && !searchRef.current.contains(e.target as Node)) {
                setResults([]);
            }
        };
        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);

    useEffect(() => {
        if (data) setCategories(data);
    }, [data]);

    if (isLoading) return <nav className="Navbar"><div className="Navbar-inner"><span className="Navbar-brand">Webshop</span></div></nav>;
    if (error || !data) return <NotFound />;

    return (
        <nav className="Navbar">
            <div className="Navbar-inner">

                {/* Brand */}
                <Link to="/" className="Navbar-brand">Webshop</Link>

                {/* Categories */}
                <div className="Container-Category">
                    <p className="category">Categories</p>
                    <div className="Category-dropdown">
                        {categories.map(category => (
                            <NavLink
                                key={category.id}
                                to={`/category/${category.id}`}
                                state={{ categoryName: category.name }}
                            >
                                <p>{category.name}</p>
                            </NavLink>
                        ))}
                    </div>
                </div>

                {/* Search */}
                <div className="SearchBar-Container" ref={searchRef}>
                    <input
                        className="SearchBar"
                        type="text"
                        placeholder="Search products..."
                        value={search}
                        onChange={(e) => handleSearch(e.target.value)}
                    />
                    {results.length > 0 && (
                        <div className="search-results">
                            {results.map((product) => (
                                <NavLink
                                    to={`products/${product.id}`}
                                    key={product.id}
                                    className="Searched-Products"
                                    onClick={() => { setResults([]); setSearch(''); }}
                                >
                                    <img
                                        className="Searched-Product-Image"
                                        src={product.productImage}
                                        alt={product.name}
                                    />
                                    <p className="Searched-Product-Name">{product.name}</p>
                                </NavLink>
                            ))}
                        </div>
                    )}
                </div>

                {/* Right side actions */}
                <div className="Navbar-actions">
                    <Link to="/winkelwagen/mine" className="winkelwagen-link">
                        <img src={winkelwagenIcon} alt="Shopping Cart" />
                    </Link>

                    <div className="Navbar-auth">
                        {username ? (
                            <>
                                {(localStorage.getItem('role') === 'admin' || localStorage.getItem('role') === 'hoofdadmin') && (
                                    <Link
                                        to="/admin"
                                        className="Navbar-login"
                                        style={{ backgroundColor: 'var(--dark-green)' }}
                                    >
                                        Admin
                                    </Link>
                                )}
                                <Link to="/profile" className="Navbar-username" title="My Account">
                                    {username}
                                </Link>
                                <button className="Navbar-logout" onClick={handleLogout}>
                                    Logout
                                </button>
                            </>
                        ) : (
                            <Link to="/auth" className="Navbar-login">
                                Login
                            </Link>
                        )}
                    </div>
                </div>

            </div>
        </nav>
    );
};

export default Navbar;
