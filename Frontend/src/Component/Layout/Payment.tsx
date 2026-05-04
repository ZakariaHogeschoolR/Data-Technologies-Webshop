import './../../Styles/Payment.css';
import { useNavigate } from "react-router-dom";
import { useEffect, useState } from 'react';
import { useFetch } from '../../CustomHooks/GetFetchHook';
import NotFound from '../Pages/NotFound';
type props = {
    total: number;
    currentWinkelwagenId:number;
}

type Orders = {
    id: number;
    winkelwagenUsersId: number;
    total: number;
    paymentStatus: boolean;
    createdAt: Date;
}



const Payout = ({total, currentWinkelwagenId}:props) => {
    const navigate = useNavigate();

    const handleCheckout = async () => {
        console.log(currentWinkelwagenId);
        try 
        {
            await fetch(
            `http://localhost:5261/api/Order/winkelwagen/delete/${currentWinkelwagenId}`,
            { method: "DELETE" }
            );
            console.log(total);
            await fetch("http://localhost:5261/api/Order/create", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    "winkelwagenUsersId": currentWinkelwagenId,
                    "total": total,
                    "paymentStatus": true,
                    "createdAt": new Date().toISOString()
                })
            });
            navigate("/checkout");
        } catch (err) 
        {
            console.error(err);
        }
    };
    return(
        <>
            <div className="Payment-container">
                <p className="sub-total-price-payment">subTotal: <p className="sub-total">{total}</p></p>
                <section className="Borderline"></section>
                <p className="total-price">Total: <p className="total">{total}</p></p>
                <p className="tax">inc tax</p>
                <button type="button" className="button-checkout" onClick={handleCheckout}>Checkout</button>
            </div>
        </>
    );
}
export default Payout;