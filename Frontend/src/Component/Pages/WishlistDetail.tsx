import { useParams, useLocation, useNavigate } from 'react-router-dom';
import '../../Styles/Wishlist.css';
import React, { useEffect, useState } from 'react';
import type { Wishlist } from './ProfilePage';
import type { ProductInfo } from './ProfilePage';
import App from '../../App';

// interface WishlistDetailProps{
//     wishlists: Wishlist[];
// }

const API = `http://localhost:5261/api/`


export default function WishlistDetail(/*{ wishlists }: WishlistDetailProps*/){
    const { id } = useParams<{ id: string }>()
    const location = useLocation()


    const allWishlists = location.state?.allWishlist as Wishlist[] | undefined
    const currentName = location.state?.currentName as string | undefined
    const [wishlistName, setWishlistName] = useState<string>(currentName || ``)
    const [products, setProducts] = useState<ProductInfo[]>([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(``)

    const navigate = useNavigate()
    const [isDeleting, setIsDeleting] = useState(false)

    async function HandleDeleteWishlist() {
        const confirmDelete = window.confirm(`are you sure you want to delete, ${wishlistName}?`)
        if(!confirmDelete) return

        try{
            setIsDeleting(true)
            setError(``)
            const token = localStorage.getItem(`token`)
            const response = await fetch(`${API}Wishlist/delete/${id}`, {
                method: `DELETE`,
                headers: {
                    "Content-Type" : `application/json`,
                    "Authorization" : `Bearer ${token}`
                }
            })
            if(!response.ok){
                throw new Error(`wishlist could not be deleted`)
            }
            alert(`wislist deleted succesfully`)
            navigate(`/profile`)
        }
        catch(e: any){
            setError(e.message || `deleting failed`)
        }
        finally{
            setIsDeleting(false)
        }
    }

    useEffect(()=>{
        async function getData(){
            console.log(`id: ${id}`)
            setError(``)
            try{
                setLoading(true)
                let items : Wishlist[] = []
                if(allWishlists && currentName){
                    items = allWishlists.filter(w => w.name === currentName)
                }
                else if(id){
                    const wishlistResponse = await fetch(`${API}Wishlist/${id}`)
                    if(!wishlistResponse.ok) throw new Error(`wishlist could not be retrieved`)
                    const wishlistData: Wishlist[] = await wishlistResponse.json()
                    if(Array.isArray(wishlistData) && wishlistData.length > 0){
                        setWishlistName(wishlistData[0].name)
                        items = wishlistData
                    }
                    else if(!Array.isArray(wishlistData)){
                        throw new Error(`not behaving as expected`)
                        // setWishlistName(wishlistData.name || wishlistData.Name)
                        // items = [wishlistData]
                    }
                    // const singleWishlist: Wishlist = await wishlistResponse.json()
                    // setWislistName(singleWishlist.name)
                    // items = [singleWishlist]
                }
                if(items.length === 0){
                    setLoading(false)
                    return;
                }
                const validItems = items.filter(item => {
                    const PID =  item.productid ?? (item as any).productid
                    return PID !== null && PID !== undefined && PID !== 0
                })
                // item.productid !== null && item.productid !== 0)
                const promises = validItems.map(item => {
                    const PID = item.productid  ?? (item as any).productid
                    return fetch(`${API}Product/${PID}`).then(res =>{
                        if(!res.ok) throw new Error(`product with id not found`)
                        return res.json() as Promise<ProductInfo>

                    })
                    // fetch(`${API}Product/${item.productid}`).then(res => {
                    // if(!res.ok) throw new Error(`product with id not found`)
                    // return res.json() as Promise<ProductInfo>
                })
                // const productData = await Promise.all(promises)
                // setProducts(productData)
                // const promises = wishlists.map(item => fetch(`${API}ProductController/${item.productid}`))
                // const responses = await Promise.all(promises)
                // const jsons = responses.map(res =>{
                //     if(!res.ok) throw new Error(`failed to fetch a product`)
                //     return res.json() as Promise<ProductInfo>
                // })
                const productData = await Promise.all(promises)
                setProducts(productData)
            }
            catch(e){
                setError(`Error: ${error}` || `could not fetch data`)
            }
            finally{
                setLoading(false)
            }
        }
        getData()
    }, [id, allWishlists, currentName])

    if(loading) return <p>Loading...</p>
    if(error) return <p>error: {error}</p>
    return(<div className={`wishlistWrapper`}>
        <div className={`wishlistContainer`}>
            <h1 className={`wishlistName`}>{wishlistName}</h1>
        </div>
        <div className={`productsList`}>
            {products.length === 0 ? (
                <p className={`noProducts`}>no products in wishlist yet</p>
            ) : (
                products.map((product) => (
                <div key={product.id} className={`wishlistProductItem`}>
                    <button onClick={HandleDeleteWishlist} disabled ={isDeleting}>
                        {isDeleting ? `Deleting...`:`delete wishlist`}
                    </button>
                    <h3>Name: {product.name}</h3>
                    <p>Price: {product.price}</p>
                    <img src={product.productImage}/>
                </div>
                ))
            )}
        </div>
    </div>)
    // return(<div className={`wishlistWrapper`}>
    //     {wishlists.map((item) => {
    //         const Product = products.find(p => p.id === item.productid)
    //         return(
    //         <div className={`wishlistContainer`}>
    //             <h1 className={`wishlistName`}>{item.name}</h1>
    //             <div className={`productsList`}>
    //                 {Product ? (
    //                     <div>
    //                         <h3>Name: {Product.name}</h3>
    //                         <p>Price: {Product.price}</p>
    //                     </div>
    //                     ) : (<p>Product details unavailable</p>)}
    //             </div>
    //         </div>
    //     )
    //     })}
    // </div>);
}
