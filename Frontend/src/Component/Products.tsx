import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import { useFetch } from "../CustomHooks/GetFetchHook";
import { GetRecentProducts } from "./storage/recentProducts";
import '../../src/Styles/Product.css';
import { Link } from "react-router-dom";


type product =
{
    id: number;
    productImage: string;
    name: string;
    description: string;
    price: number;
}

const Products = () => {
    const [getProducts, setProducts] = useState<product[]>([]);
    const { data, isLoading, error } = useFetch<product[]>({ url: "http://localhost:5261/api/Product" });
    const [recent, setRecent ] = useState<product[]>([]);
    const location = useLocation();
    useEffect(() => {
        if (data) {
            setProducts(data);
        }
    }, [data]);
    useEffect(() => {
        setRecent(GetRecentProducts());
    }, [location.pathname]);
    if (isLoading) return <p>Loading...</p>;
    if (error) return <p>Error: {error}</p>;
    return (
        <>
            <p className="recent">RECENT</p>
            <section className="recent-border-line"></section>
            <div className="Products-Container-recent">
                {recent.map(prod => (
                    <Link to={`products/${prod.id}`} className="link">
                        <div className="Product-content-recent">
                            <img src={prod.productImage} className="recent-ProductImage"/>
                        </div>
                    </Link>
                ))}
            </div>
            <div className="Products-Container">
                {getProducts.map(prod => (
                    <Link to={`products/${prod.id}`} className="link">
                        <div className="Product-content">
                            <img src={prod.productImage} className="products-ProductImage"/>
                            <p className="products-Name">{prod.name}</p>
                            {/* <p className="products-Description">{prod.description}</p> */}
                            <p className="products-Price-p-tag">{prod.price}</p>
                        </div>
                    </Link>
                ))}
            </div>
            <section className="product-content-border-line"></section>
            <p className="trending-teams">TRENDING TEAMS</p>
            <section className="trending-teams-border-line"></section>
            <section className="trending-teams-content-border-line"></section>

        </>
    );
} 
export default Products;