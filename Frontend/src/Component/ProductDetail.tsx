import { useParams } from "react-router-dom";
import { useFetch } from '../CustomHooks/GetFetchHook';
import { useState } from "react";
import { Link } from "react-router-dom";
import NotFound from '../Component/Pages/NotFound';
import '../Styles/ProductDetail.css';
import { AddRecentProducts } from "./storage/recentProducts";
import { useEffect } from "react";
import App from "../App";

const API = 'http://localhost:5261/api';

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
    const [recommendationResponse, setRecommendedProducts] = useState<product[]>([]);
    const [recommandedVisible, setRecommandedVisible] = useState<boolean>(true);
    const token = localStorage.getItem(`token`)
    
    const [quantity, setQuantity] = useState<number>(1)

    async function CreateWishlist(){
        if(!token){
            alert(`Log in eerst om producten toe te voegen`)
            return;
        }
        try{
            const query = `${API}/Wishlist/create`
            const response = await fetch(query, {
                headers:{
                    "Content-Type": `application/json`,
                    Authorization: `Bearer ${token}`
                }, method: `POST`,
                body: JSON.stringify({
                    name: `wishlist of productId: ${id}`,
                    productId: id
                })})

                if(response.ok) {
                    alert(`wishlist made`)
                    // const json = await response.json()
                    // console.log(json)
                }
                else {
                    const errorText = await response.text()
                    alert(`Fout bij aanmaken: ${response.status} ${errorText}`)
                }
        }
        catch(e){
            alert("Er is iets misgegaan met de verbinding.");
            console.error(e)
        }
    }
    async function AddToWinkelwagen(){
        // console.log(quantity)
        // const { id } = useParams();
        if(!token){
            alert(`Log in eerst om producten toe te voegen`)
            return;
        }

        try
        {
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

            const recommendationResponse = await fetch(
                `http://localhost:5261/api/User/recommended`,
                {
                    headers: {
                        "Authorization": `Bearer ${token}`
                    }
                }
            );

            if (!recommendationResponse.ok) {
                throw new Error("Recommendations ophalen mislukt");
            }

            const recommendations = await recommendationResponse.json();

            setRecommendedProducts(recommendations);
        }
        catch(e){
            console.log(`Something went wrong: ${e}`)
        }
        setRecommandedVisible(true);
    }

    async function AddToWinkelwagenRecommanded(prodId: number){
        // console.log(quantity)
        // const { id } = useParams();
        if(!token){
            alert(`Log in eerst om producten toe te voegen`)
            return;
        }

        try
        {
            const response = await fetch(`http://localhost:5261/api/ShoppingCart/create`, { headers:{
                "Content-Type" : "application/json",
                "Accept" : "application/json",
                "Authorization": `Bearer ${token}`
            }, method: `POST`,
            body: JSON.stringify({
                // id: id,
                productId : prodId,
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
                <p>add to wishlist?</p><button onClick={CreateWishlist}>+</button>
                <p>Add quantity to Shoppingcart:</p>
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
            { recommendationResponse.length > 0 && recommandedVisible &&(
                <div className="rec-container">
                    <button className="cross-button" onClick={() => setRecommandedVisible(false)} value={`Submit`}>X</button>
                    {recommendationResponse.map(prod => (
                        <div className="Product-Team-content-rec">
                            <p className="products-Team-Price-p-tag-rec">€ {prod.price}</p>
                            <img src={prod.productImage} className="products-Team-ProductImage-rec" style={{width:`300px`, height:`300px`, borderRadius: `6px`}}/>
                            <div className="Addtowinkelwagenwindow-rec">
                                <input className="quantity-input-rec" id="quantity" type="number" min={1} max={11} onChange={(e) => setQuantity(parseInt(e.target.value) > 11 ? 11 : parseInt(e.target.value) < 1 ? 1 : parseInt(e.target.value))}/>
                                <button className="quantity-button-rec" onClick={() => AddToWinkelwagenRecommanded(prod.id)} value={`Submit`}>Submit</button>
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </>
    );
}
export default ProductDetail;
