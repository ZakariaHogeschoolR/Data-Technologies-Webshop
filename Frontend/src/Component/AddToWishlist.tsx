import { useEffect, useState } from "react"
import type { Wishlist } from "./Pages/ProfilePage"

interface AddToWishlistProp{
    productId: number
}

const API = `http://localhost:5261/api/`

export default function AddToWishlistButton({productId} : AddToWishlistProp){
    const [wishlists, setWishlists] = useState<Wishlist[]>([])
    const [showDropdown, setShowDropdown] = useState(false)
    const [loading, setLoading] = useState(false)
    const [message, setMessage] = useState(``)

    useEffect(()=>{
        if(!showDropdown) return
        async function GetMyWishlists()
        {
            try{
                const token = localStorage.getItem(`token`)
                const response = await fetch(`${API}Wishlist/mine`, {
                    headers: {
                        "Authorization": `Bearer ${token}`
                    }
                })
                if(!response.ok) throw new Error()
                const data = await response.json()

                const uniqueData = data.filter((w: any, idx: number, self: any[]) =>
                self.findIndex(item => item.name === w.name) === idx)
                setWishlists(uniqueData)
            }
            catch(e:any){
                console.log(`could not load wishlists`)
            }
        }
        GetMyWishlists()
    }, [showDropdown])
    async function HandleAdd(wishlistName: string) {
        setLoading(true)
        setMessage(``)
        try{
            const token = localStorage.getItem(`token`)
            const response = await fetch(`${API}wishlist/create`,{
                method: `POST`,
                headers:{
                    "Content-Type": `application/json`,
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify({
                    name: wishlistName,
                    productId: productId
                })
            })
            if(!response.ok) throw new Error()
            setMessage(`Added to ${wishlistName}`)
            setTimeout(() => {
                setMessage(``)
                setShowDropdown(false)
            }, 2000);
        }
        catch(err){
            setMessage(`${err}`)
        }
        finally{
            setLoading(false)
        }
    }
    return(
        <div>
            <button onClick={() => setShowDropdown(!showDropdown)} className={`addToWishlistBtn`}>Add To Wishlist</button>
            {showDropdown && (
                <div>
                    {wishlists.length === 0 ? (
                        <p>make a wishlist in profile to see it here!</p>
                    ) : (
                        wishlists.map(list => (
                            <button key={list.id}
                            onClick={()=>HandleAdd(list.name)}
                            disabled={loading}>{list.name}</button>
                        ))
                    )}
                    {message && (
                        <div>{message}</div>
                    )}
                </div>
            )}
        </div>
    )
}
