import { Outlet } from 'react-router-dom';
import Navbar from "../Layout/Navbar";
import Footer from "../Layout/Footer";

const Layout = () => 
{
    return(
        <>
            <Navbar/>
            <Outlet/>
            <Footer/>
        </>
    );
}
export default Layout;