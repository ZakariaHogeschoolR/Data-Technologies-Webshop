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
    const token = localStorage.getItem(`token`)

    const [quantity, setQuantity] = useState<number>(1)

    async function AddToWinkelwagen(){
        // console.log(quantity)
        // const { id } = useParams();
        if(!token){
            alert(`Log in eerst om producten toe te voegen`)
            return;
        }

        try{
            const response = await fetch(`http://localhost:5261/api/ShoppingCart/create`, { headers:{
                "Content-Type" : "application/json",
                "Accept" : "application/json",
                "Authorization": `Bearer ${token}`
            }, method: `POST`,
            body: JSON.stringify({
                // id: id,
                productId : id,
                quantity: quantity,
                // createdAt: new Date().toISOString(),
                // updatedAt: new Date().toISOString(),
            })
        })
        if(!response.ok){
            const errorData = await response.text();
            throw new Error(`Server fout: ${response.status} - ${errorData}`)
        }
        const json = await response.json();
        console.log(json);
        alert(`product toegevoegd aan winkelwagen`)
    }
    catch(e){
        console.log(`Something went wrong: ${e}`)
    }
}
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
            <div className="Addtowinkelwagenwindow">
                <input className="quantity-input" id="quantity" type="number" min={1} max={11} onChange={(e) => setQuantity(parseInt(e.target.value) > 11 ? 11 : parseInt(e.target.value) < 1 ? 1 : parseInt(e.target.value))}/>
                <button className="quantity-button" onClick={AddToWinkelwagen} value={`Submit`}>Submit</button>
            </div>
            <div className="product-container">
                <div className="product-content">
                    <img src={data.productImage} className="product-img-content"/> 
                </div>
                <div className="Costimizing-section">
                    <p className="Costimizing-Color">Description: </p>
                    <h1 className="name">{data.name}</h1>
                    <p className="Costimizing-Size">PRICE: </p>
                    <h5 className="price">€ {data.price}</h5>
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
                            <p className="products-Team-Price-p-tag">€ {prod.price}</p>
                        </div>
                    </Link>
                ))}
            </div>
            <section className="border-line-may-also-like-end"></section>
        </>
    );
}
export default ProductDetail;