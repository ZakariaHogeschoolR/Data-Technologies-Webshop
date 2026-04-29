import { useEffect, useMemo, useState } from 'react';
import '../../Styles/Winkelwagen.css';

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
    const DeleteProductFromWinkelwagen = async(productId:number) => {
        const url = `http://localhost:5261/api/ShoppingCart/delete/product`
        try{
            const response = await fetch(url, {
                method: `DELETE`,
                headers: {
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({productId: productId})
            })
            if(response.status == 204){
                setWinkelwagenItems(previtems => previtems.filter(item => item.productId !== productId))
            }
            else{
                console.error(`verwijderen mislukt` + response.statusText)
            }
        }
        catch(e){
            console.log(`netwerk error: `, e)
        }
    }
    // gebruik useMemo() voor het caching van een resultaat
    // van een calculatie tussen rerenders(pagina herladen). cool!
    // https://react.dev/reference/react/useMemo
    const totalPrice = useMemo(()=>{
        return winkelwagenItems.reduce((total, item) => {
            const product = products.find(p => p.id == item.productId)
            return total + (product ? product.price * item.quantity : 0)
        }, 0)
    }, [winkelwagenItems, products])
    useEffect(() =>
        {
            if(!token){
                setLoading(false)
                return;
            }
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
        }, [token])
    useEffect(()=>{
        async function getProductnamesFromWinkelwagen() {
            if (winkelwagenItems.length === 0) return;
            try{
                const productPromises = winkelwagenItems.map(async (ww) =>{
                    const request = await fetch(`http://localhost:5261/api/Product/${ww.productId}`)

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
    else if(!token){
        return(<div className='Winkelwagen_container'>
            <p>Je moet een account hebben en ingelogd zijn om te kunnen shoppen.</p>
        </div>)
    }
    else if (winkelwagenItems.length == 0) return (<p>Your shoppingcart is empty. Add Products to see them here.</p>)
    return(
        <>
        <div className="Winkelwagen_container">
            <div className="items-lijst">
                <ul>
                    {winkelwagenItems.map((winkelwagen) => {
                        const product = products.find(p => p.id == winkelwagen.productId)
                        return(
                        <li key={winkelwagen.id}>
                            {product&& (
                                <img src={`${product.productImage}`}
                                alt={`${product.name}`}
                                style={{width:`5rem`, height:`5rem`}}/>
                                )}
                                <p>Name: {product ? product.name : `Error...`}; Quantity: {winkelwagen.quantity}; price: {product ? product.price: 0.00};</p>
                                <p>Sum Price:{product ? (product.price * winkelwagen.quantity).toFixed(2) : '0.00'}</p>
                                <button onClick={() => DeleteProductFromWinkelwagen(winkelwagen.productId)}>Delete</button>
                                </li>
                                );
                            }
                        )
                    }
                </ul>
                <div className={`totale-prijs`}><p>Total Price: {totalPrice}</p></div>
            </div>
        </div>
        </>
    );
}
