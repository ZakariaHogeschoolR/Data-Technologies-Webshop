import { useParams, useLocation, Link } from 'react-router-dom';
import { useFetch } from '../CustomHooks/GetFetchHook';
import { useState, useEffect } from 'react';
import NotFound from '../Component/Pages/NotFound';
import '../Styles/Product.css';

type product = {
    id: number;
    productImage: string;
    name: string;
    description: string;
    price: number;
    teamId: number;
};

const CategoryDetail = () => {
    const location = useLocation();
    const CTname = location.state?.categoryName || 'Category';
    const [getProducts, setProducts] = useState<product[]>([]);
    const { id } = useParams();
    const [firstId, setFirstId] = useState<number | null>(null);
    const [lastId, setLastId] = useState<number | null>(null);

    const { data, isLoading, error } = useFetch<product[]>({
        url: `http://localhost:5261/api/ProductCategory/${id}`,
    });

    useEffect(() => {
        if (data && data.length > 0) {
            setProducts(data);
            setFirstId(data[0].id);
            setLastId(data[data.length - 1].id);
        }
    }, [data]);

    const handleNext = async () => {
        if (!lastId) return;
        const res = await fetch(
            `http://localhost:5261/api/ProductCategory/next?categoryId=${id}&lastId=${lastId}`
        );
        const data = await res.json();
        if (data.length === 0) return;
        setProducts(data);
        setFirstId(data[0].id);
        setLastId(data[data.length - 1].id);
    };

    const handlePrev = async () => {
        if (!firstId) return;
        const res = await fetch(
            `http://localhost:5261/api/ProductCategory/prev?categoryId=${id}&firstId=${firstId}`
        );
        const data = await res.json();
        if (data.length === 0) return;
        setProducts(data);
        setFirstId(data[0].id);
        setLastId(data[data.length - 1].id);
    };

    if (isLoading) return <p style={{ padding: '2rem', color: 'var(--dark-green)', letterSpacing: '2px', fontSize: '13px' }}>Loading...</p>;
    if (error || !data) return <NotFound />;

    return (
        <>
            <p className="recent">{CTname.toUpperCase()}</p>
            <div className="recent-border-line" />

            <div className="Products-Container">
                {getProducts.map((prod) => (
                    <Link key={prod.id} to={`/products/${prod.id}`} className="link">
                        <div className="Product-content">
                            <img
                                src={prod.productImage}
                                className="products-ProductImage"
                                alt={prod.name}
                            />
                            <p className="products-Name">{prod.name}</p>
                        </div>
                    </Link>
                ))}
            </div>

            <div className="pagination-row">
                <button className="prev-button" onClick={handlePrev}>← Prev</button>
                <button className="next-button" onClick={handleNext}>Next →</button>
            </div>

            <div className="product-content-border-line" />
        </>
    );
};

export default CategoryDetail;
