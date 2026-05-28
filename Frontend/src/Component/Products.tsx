import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import { useFetch } from "../CustomHooks/GetFetchHook";
import { useFetchSecond } from "../CustomHooks/GetFetchSecond";
import { GetRecentProducts } from "./storage/recentProducts";
import '../../src/Styles/Product.css';
import { Link } from "react-router-dom";

type product =
{
    id: number;
    productImage: string;
    name: string;
    description: string;
    price: number;
    teamId: number;
}

type teams =
{
    id: number;
    name: string;
    type: string;
}

const Products = () => {
    const [getProducts, setProducts] = useState<product[]>([]);
    const [getTeams, setTeams] = useState<teams[]>([]);
    const { data, isLoading, error } = useFetch<product[]>({ url: "http://localhost:5261/api/Product" });
    const { data2, isLoading2, error2 } = useFetchSecond<teams[]>({ url: "http://localhost:5261/api/Team" });
    const [recent, setRecent] = useState<product[]>([]);
    const location = useLocation();
    const [firstId, setFirstId] = useState<number | null>(null);
    const [lastId, setLastId] = useState<number | null>(null);

    useEffect(() => {
        if (data && data.length > 0 && data2) {
            setProducts(data);
            setFirstId(data[0].id);
            setLastId(data[data.length - 1].id);
            setTeams(data2);
        }
    }, [data, data2]);

    useEffect(() => {
        setRecent(GetRecentProducts());
    }, [location.pathname]);

    if (isLoading) return <p>Loading...</p>;
    if (error) return <p>Error: {error}</p>;
    if (isLoading2) return <p>Loading...</p>;
    if (error2) return <p>Error: {error2}</p>;

    const handleNext = async () => {
        if (!lastId) return;

        const res = await fetch(`http://localhost:5261/api/Product/next?lastId=${lastId}`);
        const data = await res.json();

        if (data.length === 0) return;

        setProducts(data);
        setFirstId(data[0].id);
        setLastId(data[data.length - 1].id);
    };

    const handlePrev = async () => {
        if (!firstId) return;

        const res = await fetch(`http://localhost:5261/api/Product/prev?firstId=${firstId}`);
        const data = await res.json();

        if (data.length === 0) return;

        setProducts(data);
        setFirstId(data[0].id);
        setLastId(data[data.length - 1].id);
    };

    // ✅ FIX: random products + team mapping correct gekoppeld
    const randomProducts = [...getProducts]
        .sort(() => Math.random() - 0.5)
        .slice(0, 3);

    const productsWithTeam = randomProducts.map(prod => {
        const team = getTeams.find(t => t.id === prod.TeamId);

        return {
            product: prod,
            teamName: team?.name ?? "Unknown"
        };
    });

    return (
        <>
            <p className="recent">RECENT</p>
            <section className="recent-border-line"></section>

            <div className="Products-Container-recent">
                {recent.map(prod => (
                    <Link key={prod.id} to={`products/${prod.id}`} className="link">
                        <div className="Product-content-recent">
                            <img src={prod.productImage} className="recent-ProductImage" />
                        </div>
                    </Link>
                ))}
            </div>

            <div className="Products-Container">
                {getProducts.map(prod => (
                    <Link key={prod.id} to={`products/${prod.id}`} className="link">
                        <div className="Product-content">
                            <img src={prod.productImage} className="products-ProductImage" />
                            <p className="products-Name">{prod.name}</p>
                        </div>
                    </Link>
                ))}
            </div>

            <button className="prev-button" onClick={handlePrev}>Prev</button>
            <button className="next-button" onClick={handleNext}>Next</button>

            <section className="product-content-border-line"></section>

            <p className="trending-teams">TRENDING TEAMS</p>
            <section className="trending-teams-border-line"></section>

            <div className="trending-products-container">

                {/* ✅ FIX: gebruik productsWithTeam i.p.v. randomProducts */}
                {productsWithTeam.map(item => (
                    <Link
                        to={`products/${item.product.id}`}
                        className="link"
                        key={item.product.id}
                    >
                        <div className="Product-content">
                            <img
                                src={item.product.productImage}
                                className="products-ProductImage"
                            />

                            <p className="products-Name">
                                {item.product.name}
                            </p>

                            <p className="team-name">
                                {item.teamName}
                            </p>
                        </div>
                    </Link>
                ))}
            </div>

            <section className="trending-teams-content-border-line"></section>
        </>
    );
}

export default Products;
