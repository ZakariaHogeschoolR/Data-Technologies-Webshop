import '../../Styles/Winkelwagen.css';

export default function Winkelwagen(){
    return(
        <>
        <div className="Winkelwagen_container">
            <div className="items-lijst">
                <ul>
                    <li>Poduct1 name, price: 11.99, quantity: 2</li>
                    <li>Poduct2 name, price: 5.99, quantity: 1</li>
                    <li>Poduct3 name, price: 0.99, quantity: 4</li>
                    <li>Poduct4 name, price: 59.99, quantity: 2</li>
                </ul>
            </div>
            <div className="total-price">78.96</div>
        </div>
        </>
    );
}