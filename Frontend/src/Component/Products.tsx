import { useEffect, useState } from 'react';
import { useLocation, Link } from 'react-router-dom';
import { useFetch } from '../CustomHooks/GetFetchHook';
import { GetRecentProducts } from './storage/recentProducts';
import '../../src/Styles/Product.css';

type product = {
    id: number;
    productImage: string;
    name: string;
    description: string;
    price: number;
};

const Products = () => {
    const [getProducts, setProducts] = useState<product[]>([]);
    const { data, isLoading, error } = useFetch<product[]>({ url: 'http://localhost:5261/api/Product' });
    const [recent, setRecent] = useState<product[]>([]);
    const location = useLocation();
    const [firstId, setFirstId] = useState<number | null>(null);
    const [lastId, setLastId] = useState<number | null>(null);

    useEffect(() => {
        if (data && data.length > 0) {
            setProducts(data);
            setFirstId(data[0].id);
            setLastId(data[data.length - 1].id);
        }
    }, [data]);

    useEffect(() => {
        setRecent(GetRecentProducts());
    }, [location.pathname]);

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

    if (isLoading) return <p style={{ padding: '2rem', color: 'var(--dark-green)', letterSpacing: '2px', fontSize: '13px' }}>Loading...</p>;
    if (error) return <p style={{ padding: '2rem', color: '#b00' }}>Error: {error}</p>;

    return (
        <>
            {/* Recent */}
            {recent.length > 0 && (
                <>
                    <p className="recent">Recent bekeken</p>
                    <div className="recent-border-line" />
                    <div className="Products-Container-recent">
                        {recent.map((prod) => (
                            <Link to={`products/${prod.id}`} className="link" key={prod.id}>
                                <div className="Product-content-recent">
                                    <img
                                        src={prod.productImage}
                                        className="recent-ProductImage"
                                        alt={prod.name}
                                    />
                                </div>
                            </Link>
                        ))}
                    </div>
                </>
            )}

            {/* All products grid */}
            <div className="Products-Container">
                {getProducts.map((prod) => (
                    <Link to={`products/${prod.id}`} className="link" key={prod.id}>
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

            {/* Pagination */}
            <div className="pagination-row">
                <button className="prev-button" onClick={handlePrev}>← Prev</button>
                <button className="next-button" onClick={handleNext}>Next →</button>
            </div>

            <div className="product-content-border-line" />

            {/* Trending teams placeholder */}
            <p className="trending-teams">Trending Teams</p>
            <div className="trending-teams-border-line" />
            <div className="trending-teams-content-border-line" />
        </>
    );
};

export default Products;
