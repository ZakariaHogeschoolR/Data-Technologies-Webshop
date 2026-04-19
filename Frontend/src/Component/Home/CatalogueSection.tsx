import {useEffect, useState} from 'react';

type Product = {
    id: number; productImage: string; name: string; description: string; price: number; teamId: number;
};

const CatalogueSection = () => {
    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchProducts = async () => {
            try {
                const response = await fetch('http://localhost:5261/api/Product');

                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }

                const data = await response.json();
                setProducts(data);
            } catch (err) {
                console.error("Failed to fetch products:", err);
                setError("Could not load products. Please try again later.");
            } finally {
                setLoading(false);
            }
        };

        fetchProducts();
    }, []);

    if (loading) {
        return (<section className="cat-section" id="catalogue">
            <div className="cat-container" style={{textAlign: 'center', padding: '4rem'}}>
                <p>Loading the catalogue...</p>
            </div>
        </section>);
    }

    if (error) {
        return (<section className="cat-section" id="catalogue">
            <div className="cat-container" style={{textAlign: 'center', padding: '4rem', color: 'red'}}>
                <p>{error}</p>
            </div>
        </section>);
    }

    return (<section className="cat-section" id="catalogue" aria-label="Catalogue">
        <div className="cat-container">
            <header className="cat-header">
                <div className="cat-header-text">
                    <p className="eyebrow">The Catalogue</p>
                    <h2 className="cat-heading">All shirts, all seasons.</h2>
                </div>
            </header>

            <ul className="cat-grid">
                {products.map((product) => (<li key={product.id} className="cat-card">
                    <a href={`#product/${product.id}`} className="cat-card-link">
                        <div className="cat-media">
                            <button
                                type="button"
                                className="cat-wish"
                                aria-label={`Save ${product.name} to wishlist`}
                                onClick={(e) => {
                                    e.preventDefault();
                                    console.log(`Saved ${product.id} to wishlist`);
                                }}
                            >
                                <svg width="16" height="16" viewBox="0 0 24 24" fill="none"
                                     stroke="currentColor" strokeWidth="1.6" strokeLinecap="round"
                                     strokeLinejoin="round" aria-hidden="true">
                                    <path
                                        d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
                                </svg>
                            </button>
                            <img src={product.productImage} alt={product.name} loading="lazy"/>
                        </div>
                        <div className="cat-body">
                            <div>
                                <h3 className="cat-name">{product.name}</h3>
                                <p className="cat-cat">{product.description}</p>
                            </div>
                            <div className="cat-price">
                                <span className="cat-price-now">€{product.price.toFixed(2)}</span>
                            </div>
                        </div>
                    </a>
                </li>))}
            </ul>

            {products.length === 0 && (<div style={{textAlign: 'center', padding: '2rem'}}>
                <p>No products found in the database.</p>
            </div>)}
        </div>
    </section>);
};

export default CatalogueSection;