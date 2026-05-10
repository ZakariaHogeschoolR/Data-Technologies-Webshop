import { useParams, useLocation} from "react-router-dom";
import { useFetch } from '../CustomHooks/GetFetchHook';
import { useState } from "react";
import { Link } from "react-router-dom";
import NotFound from '../Component/Pages/NotFound';
import '../Styles/ProductDetail.css';
import { useEffect } from "react";

type product =
{
    id: number;
    productImage: string;
    name: string;
    description: string;
    price: number;
    teamId: number;
}


const CategoryDetail = () => {
    const location = useLocation();

    const CTname = location.state?.categoryName || "Category";
    const [getProducts, setProducts] = useState<product[]>([]);
    const { id } = useParams();
    const [firstId, setFirstId] = useState<number | null>(null);
    const [lastId, setLastId] = useState<number | null>(null);
    console.log(id);
    const { data, isLoading, error } = useFetch<product[]>({
        url: `http://localhost:5261/api/ProductCategory/${id}`
    });

    useEffect(() => {
        if (data && data.length > 0) {
            setProducts(data);
            setFirstId(data[0].id);
            setLastId(data[data.length - 1].id);
        }
    }, [data]);

    if (isLoading) return <p>Loading...</p>;
    if (error || !data) return <NotFound />;
    const handleNext = async () => {
        if (!lastId) return;

        const res = await fetch(`http://localhost:5261/api/ProductCategory/next?categoryId=${id}&lastId=${lastId}`);
        const data = await res.json();

        if (data.length === 0) return; // geen volgende pagina

        setProducts(data);
        setFirstId(data[0].id);
        setLastId(data[data.length - 1].id);
    };

    const handlePrev = async () => {
        if (!firstId) return;

        const res = await fetch(`http://localhost:5261/api/ProductCategory/prev?categoryId=${id}&firstId=${firstId}`);
        const data = await res.json();

        if (data.length === 0) return; // geen vorige pagina

        setProducts(data);
        setFirstId(data[0].id);
        setLastId(data[data.length - 1].id);
    };
    return (
        <>
            <p className="recent">{CTname.toUpperCase()}</p>
            <section className="recent-border-line"></section>
            <div className="Products-Container">
                {getProducts.map(prod => (
                    <Link key={prod.id} to={`/products/${prod.id}`} className="link">
                        <div className="Product-content">
                            <img src={prod.productImage} className="products-ProductImage"/>
                            <p className="products-Name">{prod.name}</p>
                            {/* <p className="products-Description">{prod.description}</p> */}
                        </div>
                    </Link>
                ))}
            </div>
            <button className="prev-button" onClick={handlePrev}>Prev</button>
            <button className="next-button" onClick={handleNext}>Next</button>
            <section className="product-content-border-line"></section>
        </>
    );
} 
export default CategoryDetail;
