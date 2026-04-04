import { useEffect, useState } from 'react';
import '../../Styles/Winkelwagen.css';
// import { useParams } from 'react-router-dom';

interface winkelwagen{
    id: number,
    productId:number,
    shoppingProducts:[],
    quantity:number,
    createdAt:string,
    updatedAt:string
}
export default function Winkelwagen(){
    // const {id} = useParams()
    const [winkelwagenItems, setWinkelwagenItems] = useState<winkelwagen[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(``)
    const [inError, setInError] = useState(false)

    useEffect(() =>
        {
            async function getData(){
                try{
                    setLoading(true)
                    const request = await fetch(`http://localhost:5261/api/ShoppingCart/2`, {
                        headers:{
                            "Content-Type": "application/json",
                            "Accept": "application/json"
                        }
                    })
                    // console.log(request)
                    const json = await request.json()
                    console.log(json)
                    setWinkelwagenItems([json]) 
                }
                catch(e){
                    setInError(true)
                    setError(`${e}`)
                    console.log(`error: `+ e)
                }
                finally{
                    setLoading(false)
                }
            }
            getData()
        }, [])
    if(loading) return <>Loading...</>
    // if (winkelwagenItems.length == 0) return <>No winkelwagens</>
    if(inError) return <>{error}</>
    return(
        <>
        <div className="Winkelwagen_container">
            {/* <p>{winkelwagenItems}</p> */}
            <div className="items-lijst">
                <ul>
                    {winkelwagenItems.map((winkelwagen) =>(
                        <li key={winkelwagen.id}>{winkelwagen.id}<br/>
                            Name:  {winkelwagen.productId} x Quan:{winkelwagen.quantity}<br/>
                            {winkelwagen.createdAt}<br/>
                            {winkelwagen.updatedAt}
                        </li>
                    ))}
                </ul>
            </div>
            <div className="total-price"></div>
        </div>
        </>
    );
}