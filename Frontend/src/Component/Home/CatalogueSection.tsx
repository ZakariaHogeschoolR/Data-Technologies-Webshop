import {useState} from 'react';

type Product = {
    id: string;
    name: string;
    category: string;
    price: string;
    oldPrice?: string;
    image: string;
    tag?: 'New' | 'Sale' | 'Bestseller';
};

const products: Product[] = [{
    id: 'p1',
    name: 'Harbour Oxford',
    category: 'Oxford',
    price: '$128',
    image: '/images/product-1.jpg',
    tag: 'Bestseller'
}, {
    id: 'p2',
    name: 'Atlas Striped Linen',
    category: 'Linen',
    price: '$98',
    oldPrice: '$145',
    image: '/images/product-2.jpg',
    tag: 'Sale'
}, {
    id: 'p3',
    name: 'Field Overshirt',
    category: 'Overshirt',
    price: '$168',
    image: '/images/product-3.jpg',
    tag: 'New'
}, {id: 'p4', name: 'Meadow Linen', category: 'Linen', price: '$115', image: '/images/product-4.jpg'}, {
    id: 'p5',
    name: 'Ivory Heavyweight',
    category: 'Cotton',
    price: '$145',
    image: '/images/product-5.jpg'
}, {
    id: 'p6',
    name: 'Onyx Button-Up',
    category: 'Cotton',
    price: '$129',
    oldPrice: '$179',
    image: '/images/product-6.jpg',
    tag: 'Sale'
}, {
    id: 'p7',
    name: 'Sand Chambray',
    category: 'Chambray',
    price: '$132',
    image: '/images/product-7.jpg',
    tag: 'New'
}, {id: 'p8', name: 'Pine Stripe Linen', category: 'Linen', price: '$138', image: '/images/product-8.jpg'},];

const filters = ['All', 'Oxford', 'Linen', 'Cotton', 'Overshirt', 'Chambray'];

const CatalogueSection = () => {
    const [filter, setFilter] = useState('All');
    const visible = filter === 'All' ? products : products.filter((p) => p.category === filter);

    return (<section className="cat-section" id="catalogue" aria-label="Catalogue">
            <div className="cat-container">
                <header className="cat-header">
                    <div className="cat-header-text">
                        <p className="eyebrow">The Catalogue</p>
                        <h2 className="cat-heading">All shirts, all seasons.</h2>
                    </div>

                    <div className="cat-filters" role="tablist" aria-label="Filter by fabric">
                        {filters.map((f) => (<button
                                key={f}
                                role="tab"
                                aria-selected={filter === f}
                                className={`cat-filter ${filter === f ? 'is-active' : ''}`}
                                onClick={() => setFilter(f)}
                            >
                                {f}
                            </button>))}
                    </div>
                </header>

                <ul className="cat-grid">
                    {visible.map((product) => (<li key={product.id} className="cat-card">
                            <a href="#product" className="cat-card-link">
                                <div className="cat-media">
                                    {product.tag ? (<span className={`cat-tag cat-tag--${product.tag.toLowerCase()}`}>
                                            {product.tag}
                                        </span>) : null}
                                    <button
                                        type="button"
                                        className="cat-wish"
                                        aria-label={`Save ${product.name} to wishlist`}
                                        onClick={(e) => {
                                            e.preventDefault();
                                        }}
                                    >
                                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none"
                                             stroke="currentColor" strokeWidth="1.6" strokeLinecap="round"
                                             strokeLinejoin="round" aria-hidden="true">
                                            <path
                                                d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
                                        </svg>
                                    </button>
                                    <img src={product.image} alt={product.name} loading="lazy"/>
                                </div>
                                <div className="cat-body">
                                    <div>
                                        <h3 className="cat-name">{product.name}</h3>
                                        <p className="cat-cat">{product.category}</p>
                                    </div>
                                    <div className="cat-price">
                                        <span className="cat-price-now">{product.price}</span>
                                        {product.oldPrice ? (
                                            <span className="cat-price-old">{product.oldPrice}</span>) : null}
                                    </div>
                                </div>
                            </a>
                        </li>))}
                </ul>

                <div className="cat-footer">
                    <button type="button" className="cat-load">
                        <span>Load more</span>
                        <svg viewBox="0 0 24 24" width="14" height="14" fill="none" stroke="currentColor"
                             strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
                            <path d="M5 12h14"/>
                            <path d="m12 5 7 7-7 7"/>
                        </svg>
                    </button>
                </div>
            </div>
        </section>);
};

export default CatalogueSection;
