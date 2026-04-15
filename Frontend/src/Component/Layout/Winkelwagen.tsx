import { useEffect, useState } from 'react';
import '../../Styles/Winkelwagen.css';
import { useParams } from 'react-router-dom';
// import { useParams } from 'react-router-dom';

interface winkelwagen{
    id: number,
    productId:number,
    shoppingProducts:[],
    quantity:number,
    createdAt:string,
    updatedAt:string
}
type product =
{
    id: number;
    productImage: string;
    name: string;
    description: string;
    price: number;
}
export default function Winkelwagen(){
    // const {id} = useParams()
    const [winkelwagenItems, setWinkelwagenItems] = useState<winkelwagen[]>([])
    const [products, setProducts] = useState<product[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(``)
    const [inError, setInError] = useState(false)
    const token = localStorage.getItem(`token`)

    useEffect(() =>
        {
            async function getWinkelwagenData(){
                try{
                    setLoading(true)
                    const request = await fetch(`http://localhost:5261/api/ShoppingCart/mine`, {
                        headers:{
                            "Content-Type": "application/json",
                            "Accept": "application/json",
                            "Authorization":`Bearer ${token}`
                        }
                    })
                    // console.log(request)
                    const json = await request.json()
                    // console.log(json)
                    setWinkelwagenItems(json) 
                }
                catch(err){
                    setInError(true)
                    setError(`${err}`)
                    console.log(`error: `+ err)
                }
                finally{
                    setLoading(false)
                }
            }
            getWinkelwagenData()
        }, [])
    useEffect(()=>{
        async function getProductnamesFromWinkelwagen() {
            if (winkelwagenItems.length === 0) return;
            try{
                // const results: product[] = [];
                // setLoading(true)
                // winkelwagenItems.forEach(async winkelwagen => {
                //     const request = await fetch(`http://localhost:5261/Product/${winkelwagen.productId}`,{
                //         headers:{
                //             "Content-Type":"Application/json",
                //             "Accept":"Application/json"
                //         }
                //     })
                //     console.log({request});
                //     const json :product = await request.json();
                //     console.log(json);
                //     results.push(json)
                // });
                // setProducts(results)
                const productPromises = winkelwagenItems.map(async (ww) =>{
                    const request = await fetch(`http://localhost:5261/api/Product/${ww.productId}`)
                    // const json = await request.json();
                    // console.log(json)
                    // return await json as product
                    return await request.json() as product
                })
                const results = await Promise.all(productPromises)
                setProducts(results)
            }
            catch(err){
                setInError(true)
                setError(`${err}`)
                console.log(`error: `+ err)
            }
            finally{
                setLoading(false)
            }
        }
        getProductnamesFromWinkelwagen()
    }, [winkelwagenItems])
    if(loading) return <>Loading...</>
    // if (winkelwagenItems.length == 0) return <>No winkelwagens</>
    else if(inError) return <>{error}</>
    else if (winkelwagenItems.length == 0) return (<p>Your shoppingcart is empty. Add Products to see them here.</p>)
    return(
        <>
        <div className="Winkelwagen_container">
            <div className="items-lijst">
                <ul>
                    {winkelwagenItems.map((winkelwagen) => {
                        const product = products.find(p => p.id == winkelwagen.productId)
                        return(<li key={winkelwagen.id}>
                            {product&& (
                                <img src={`${product.productImage}`}
                                alt={`${product.name}`}
                                style={{width:`2rem`, height:`2rem`}}/>)}
                                Name: {product ? product.name : `Error...`}; Quantity: {winkelwagen.quantity}; price: {product ? product.price: 0.00};
                                <p>Sum Price:{product ? (product.price * winkelwagen.quantity).toFixed(2) : '0.00'}</p> 
                                </li>);
                            })}
                </ul>
                
            </div>
        </div>
        </>
    );
}