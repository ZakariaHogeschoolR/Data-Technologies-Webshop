import { useEffect, useState } from 'react';
import '../../Styles/Winkelwagen.css';
import { useParams } from 'react-router-dom';

export default function Winkelwagen(){
    const {id} = useParams()
    const [winkelwagenItems, setWinkelwagenItems] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(``)

    useEffect(() =>
        {
            async function getData(){
                setLoading(true)
                try{
                    const reponse = await fetch(`http://localhost:5261/api/ShoppingCart/${id}`, {
                        headers:{
                            "Content-Type": "application/json",
                            "Accept": "application/json"
                        }
                    })
                    const json = await reponse.json()
                    setWinkelwagenItems(json) 
                }
                catch(e){
                    setError(`fetch winkelwagen failed`)
                }
                finally{
                    setLoading(false)
                }
            }
            getData()
        }, [])
    if(loading) return <>Loading...</>
    if(error != null) return <>Error: {error}</>
    return(
        <>
        <div className="Winkelwagen_container">
            <div className="items-lijst">
                {winkelwagenItems}
                {/* <ul>
                    <li>Poduct1 name, price: 11.99, quantity: 2</li>
                    <li>Poduct2 name, price: 5.99, quantity: 1</li>
                    <li>Poduct3 name, price: 0.99, quantity: 4</li>
                    <li>Poduct4 name, price: 59.99, quantity: 2</li>
                </ul> */}
            </div>
            <div className="total-price">153.91</div>
        </div>
        </>
    );
}