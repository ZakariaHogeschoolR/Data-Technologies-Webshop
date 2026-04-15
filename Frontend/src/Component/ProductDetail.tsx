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

    const [quantity, setQuantity] = useState(``)
    async function AddToWinkelwagen(){
        // console.log(quantity)
        // const { id } = useParams();
        try{
            const response = await fetch(`http://localhost:5261/api/ShoppingCart/create`, { headers:{
                "Content-Type" : "application/json",
                "Accept" : "application/json"
            }, method: `POST`,
            body: JSON.stringify({
                userId: 1,
                id: id,
                productId : id,
                quantity: quantity,
                // createdAt: new Date().toISOString(),
                // updatedAt: new Date().toISOString(),
            })
        })
        const json = await response.json();
        console.log(json);
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
            <div className="Addtowinkelwagenwindow">
                <p>Add quantity to Shoppingcart:</p>
                <input id="quantity" type="number" min={1} max={100} onChange={(e) => setQuantity(e.target.value)}/>
                <input type={"button"} onClick={AddToWinkelwagen} value={`Submit`}/>
            </div>
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
                <div><button>Add to Shoppingcart</button></div>
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