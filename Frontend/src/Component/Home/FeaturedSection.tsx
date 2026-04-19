import {useEffect, useState} from 'react';

type Product = {
    id: number; productImage: string; name: string; description: string; price: number; teamId: number;
};

const FeaturedSection = () => {
    const [highlights, setHighlights] = useState<Product[]>([]);
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchHighlights = async () => {
            try {
                const response = await fetch('http://localhost:5261/api/Product');

                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }

                const data: Product[] = await response.json();

                setHighlights(data.slice(0, 2));
            } catch (err) {
                console.error("Failed to fetch featured items:", err);
                setError("Could not load highlights.");
            } finally {
                setLoading(false);
            }
        };

        fetchHighlights();
    }, []);

    if (loading) {
        return (<section className="hl-section" aria-label="Weekly highlights">
                <div className="hl-container" style={{textAlign: 'center', padding: '4rem'}}>
                    <p>Loading highlights...</p>
                </div>
            </section>);
    }

    if (error) {
        return (<section className="hl-section" aria-label="Weekly highlights">
                <div className="hl-container" style={{textAlign: 'center', padding: '4rem', color: 'red'}}>
                    <p>{error}</p>
                </div>
            </section>);
    }

    return (<section className="hl-section" aria-label="Weekly highlights">
            <div className="hl-container">
                <header className="hl-header">
                    <div className="hl-header-text">
                        <p className="eyebrow">Highlights</p>
                        <h2 className="hl-heading">This week at the atelier</h2>
                    </div>
                    <a href="#catalogue" className="hl-view-all">
                        <span>View all</span>
                        <svg viewBox="0 0 24 24" width="16" height="16" fill="none" stroke="currentColor"
                             strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
                            <path d="M5 12h14"/>
                            <path d="m12 5 7 7-7 7"/>
                        </svg>
                    </a>
                </header>

                <ul className="hl-grid">
                    {highlights.map((item) => (<li key={item.id} className="hl-card">
                            <a href={`#product/${item.id}`} className="hl-card-link">
                                <div className="hl-media">
                                    <img src={item.productImage} alt={item.name} loading="lazy"/>
                                </div>
                                <div className="hl-meta">
                                    <div>
                                        <p className="eyebrow hl-meta-eyebrow">{item.description}</p>
                                        <h3 className="hl-card-title">{item.name}</h3>
                                    </div>
                                    <span className="hl-meta-side">€{item.price.toFixed(2)}</span>
                                </div>
                            </a>
                        </li>))}
                </ul>
            </div>
        </section>);
};

export default FeaturedSection;