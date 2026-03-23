import { useParams } from "react-router-dom";
import { useFetch } from '../CustomHooks/GetFetchHook';
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
}

const ProductDetail = () => {
    const { id } = useParams();
    const { data, isLoading, error } = useFetch<product>({ url: `http://localhost:5261/api/Product/${id}` });
    useEffect(() => {
        if(data)
        {
            AddRecentProducts(data);
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
            <section className="border-line-may-also-like-end"></section>
        </>
    );
}
export default ProductDetail;