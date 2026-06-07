import { Link } from 'react-router-dom';
import '../../Styles/Footer.css';

const Footer = () => {
    return (
        <footer className="Footer">
            <div className="Footer-inner">
                <span className="Footer-brand">Webshop</span>
                <div className="Footer-links">
                    <Link to="/about" className="Footer-link">About</Link>
                    <Link to="/auth" className="Footer-link">Login</Link>
                    <Link to="/profile" className="Footer-link">Profile</Link>
                </div>
                <p className="Footer-copy">© {new Date().getFullYear()} Webshop</p>
            </div>
        </footer>
    );
};

export default Footer;
