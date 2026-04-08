import { useParams } from "react-router-dom";
import { useFetch } from '../CustomHooks/GetFetchHook';
import { useState } from "react";
import { Link } from "react-router-dom";
import NotFound from '../Component/Pages/NotFound';
import '../Styles/ProductDetail.css';
import { AddRecentProducts } from "./storage/recentProducts";
import { useEffect } from "react";

type product =
{
    id: number;
    productImage: string;
    name: string;
    description: string;
    price: number;
    teamId: number;
}

const ProductDetail = () => {
    const { id } = useParams();
    const { data, isLoading, error } = useFetch<product>({ url: `http://localhost:5261/api/Product/${id}` });
    const [productsByTeam, setProductsByTeam] = useState<product[]>([]);
    useEffect(() => {
        if(data )
        {
            AddRecentProducts(data);
            fetch(`http://localhost:5261/api/Product/team/${data.teamId}`)
            .then(res => res.json())
            .then(result => {
                console.log("teamId:", data.teamId);
                console.log("API result:", result);
                setProductsByTeam(result);
            })
            .catch(err => console.log("Fetch error:", err));
        }
    }, [data]);
    if (isLoading) return <p>Loading...</p>;
    if (error) return <p>Error: {error}</p>;
    if (error || !data) {
        return <NotFound />;
    }
    return (
        <>
            <p className="product-id-content">PRODUCT {id}</p>
            <section className="product-border-line"></section>
            <div className="product-container">
                <div className="product-content">
                    <img src={data.productImage} className="product-img-content"/> 
                </div>
                <div className="Costimizing-section">
                    <p className="Costimizing-Color">COLOR: </p>
                    <p className="Costimizing-Size">SIZE: </p>
                    <p className="Costomizing-Quantity">QUANTITY: </p>
                </div>
            </div>
            <p className="you-may-also-like-p-tag">You may also like</p>
            <section className="border-line-may-also-like"></section>
            <div className="Products-Team-Container">
                {productsByTeam.map(prod => (
                    <Link to={`/products/${prod.id}`} className="link">
                        <div className="Product-Team-content">
                            <img src={prod.productImage} className="products-Team-ProductImage"/>
                            <p className="products-Team-Name">{prod.name}</p>
                            {/* <p className="products-Team-Description">{prod.description}</p> */}
                            <p className="products-Team-Price-p-tag">{prod.price}</p>
                        </div>
                    </Link>
                ))}
            </div>
            <section className="border-line-may-also-like-end"></section>
        </>
    );
}
export default ProductDetail;