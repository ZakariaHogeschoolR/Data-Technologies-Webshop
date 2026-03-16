import { Outlet } from 'react-router-dom';
import Navbar from "../Layout/Navbar";
import Footer from "../Layout/Footer";
import '../../Styles/Layout.css';

const Layout = () => 
{
    return(
        <>
            <Navbar/>
            <main>
                <Outlet/>
            </main>
            <Footer/>
        </>
    );
}
export default Layout;