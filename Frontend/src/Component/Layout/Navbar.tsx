import { Link } from 'react-router-dom';
import '../../Styles/Navbar.css';
import winkelwagenIcon from '../../assets/shopping-cart.png'

const Navbar = () => 
{
    const testParam = 2
    return(
        <>
            <nav className="Navbar">
                <Link to={`/winkelwagen/${testParam}`} className={`winkelwagen-link`}><img src={winkelwagenIcon} style={{ width: `1rem`, height: `1rem`}}/></Link>
            </nav>
        </>
    );
}
export default Navbar;