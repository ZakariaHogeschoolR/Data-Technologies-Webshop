import {Link, useNavigate} from 'react-router-dom';
import '../../Styles/Navbar.css';
import winkelwagenIcon from '../../assets/shopping-cart.png'
import {useState} from "react";

const Navbar = () => {
    const [username, setUsername] = useState<string | null>(localStorage.getItem('username'));
    const navigate = useNavigate();

    const testParam = 1;

    const handleLogout = async () => {
        await fetch('http://localhost:5261/api/Auth/logout', {method: 'POST'});

        localStorage.removeItem('jwtToken');
        localStorage.removeItem('username');
        localStorage.removeItem('role');

        setUsername(null);
        navigate('/');
    };

    return (<nav className="Navbar">
        <div className="Navbar-inner">

            <Link to="/" className="Navbar-brand">Webshop</Link>

            <div className="Navbar-actions">

                <Link to={`/winkelwagen/${testParam}`} className="winkelwagen-link">
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