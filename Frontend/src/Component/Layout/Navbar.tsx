import {Link, useNavigate} from 'react-router-dom';
import '../../Styles/Navbar.css';
import winkelwagenIcon from '../../assets/shopping-cart.png'
import { useFetch } from '../../CustomHooks/GetFetchHook';
import { NavLink } from 'react-router-dom';
import {useState, useEffect} from "react";
import NotFound  from '../Pages/NotFound';

type categoryType =
{
    id: number;
    name: string;
}

const Navbar = () => {
    const { data, isLoading, error } = useFetch<categoryType[]>({ url: `http://localhost:5261/api/Category` });
    const [search, setSearch] = useState("");
    const [results, setResults] = useState<any[]>([]);
    const [username, setUsername] = useState<string | null>(localStorage.getItem('username'));
    const navigate = useNavigate();
    const [Categories, setCategories] = useState<categoryType[]>([]);
    const handleSearch = async (value: string) => {
        setSearch(value);

        if (value.length < 2) {
            setResults([]);
            return;
        }

        const res = await fetch(
            `http://localhost:5261/api/Product/search?name=${value}`
        );

        const data = await res.json();
        setResults(data);
    };

    const handleLogout = async () => {
        await fetch('http://localhost:5261/api/Auth/logout', {method: 'POST'});

        localStorage.removeItem('token');
        localStorage.removeItem('username');
        localStorage.removeItem('role');

        setUsername(null);
        navigate('/');
    };
    useEffect(() => {
        if(data )
        {
            setCategories(data);
        }
    }, [data]);
    if (isLoading) return <p>Loading...</p>;
    if (error) return <p>Error: {error}</p>;
    if (error || !data) {
        return <NotFound />;
    }

    return (<nav className="Navbar">
        <div className="Navbar-inner">

            <Link to="/" className="Navbar-brand">Webshop</Link>

            <div className="Container-Category">
                <p className="category">Categories</p>

                <div className="Category-dropdown">
                    {Categories.map(category => (
                        <NavLink to={`/category/${category.id}`} state={{ categoryName: category.name }}><p>{category.name}</p></NavLink>
                    ))}
                </div>
            </div>
            <div className="SearchBar-Container">
                <input
                    className='SearchBar'
                    type="text"
                    placeholder="Search..."
                    value={search}
                    onChange={(e) => handleSearch(e.target.value)}
                />
            </div>
            <div className="search-results">
                {results.map((product) => (
                    <NavLink to={`products/${product.id}`} key={product.id} className="Searched-Products" onClick={() => setResults([])}>
                        <img className='Searched-Product-Image' src={product.productImage} width={50} height={50}/>
                        <p className="Searched-Product-Name">{product.name}</p>
                    </NavLink>
                ))}
            </div>
            <div className="Navbar-actions">

                <Link to={`/winkelwagen/mine`} className="winkelwagen-link">
                    <img
                        src={winkelwagenIcon}
                        alt="Shopping Cart"
                        style={{width: '1.5rem', height: '1.5rem'}}
                    />
                </Link>

                <div className="Navbar-auth">
                    {username ? (<>
                        <span className="Navbar-username">{username}</span>
                        <button className="Navbar-logout" onClick={handleLogout}>
                            Logout
                        </button>
                    </>) : (<Link to="/auth" className="Navbar-login">
                        Login
                    </Link>)}
                </div>
            </div>
        </div>
    </nav>);
};
export default Navbar;