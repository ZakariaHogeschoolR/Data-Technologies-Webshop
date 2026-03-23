import { useParams } from 'react-router-dom';
import '../../Styles/Wishlist.css';
import { useEffect, useState } from 'react';

export default function Wishlist(){
    const {id} = useParams()
    const [wishlistItems, setWishlistitems] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(``)

    useEffect(()=>{
        async function getData(){
            try{
                const response = await fetch(`http://localhost:5261/WishlistController/${id}`, {
                    headers:{
                        "Content-Type":"application/json",
                        "Accept":"application/json"
                    }
                })
                const json = await response.json()
                setWishlistitems(json)
            }
            catch(e){
                setError(`Error: ${error}`)
            }
            finally{
                setLoading(false)
            }
        }
        getData()
    }, [])
    if(loading) return <>Loading...</>
    if(error != null) return <>{error}</>
    return(
        <>
        <div className="wishlist-container">
            <h1 className="wishlist-name">Name</h1>
            <div className="products-list">
                {wishlistItems}
                {/* <ul>
                    <li>Poduct1 name, price: 11.99, quantity: 2</li>
                    <li>Poduct2 name, price: 5.99, quantity: 1</li>
                    <li>Poduct3 name, price: 0.99, quantity: 4</li>
                    <li>Poduct4 name, price: 59.99, quantity: 2</li>
                </ul> */}
            </div>
        </div>
        </>
    );
}