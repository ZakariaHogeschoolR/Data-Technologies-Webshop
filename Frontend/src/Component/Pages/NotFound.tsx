import { Link } from 'react-router-dom';
import '../../Styles/NotFound.css';

const NotFound = () => {
    return (
        <div className="NotFound-container">
            <p className="NotFound-code">404</p>
            <p className="NotFound-message">Pagina niet gevonden</p>
            <Link to="/" className="NotFound-link">← Terug naar home</Link>
        </div>
    );
};

export default NotFound;
