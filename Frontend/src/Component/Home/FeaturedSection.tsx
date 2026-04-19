type Highlight = {
    id: string; image: string; eyebrow: string; title: string; meta: string; badge?: string;
};

const highlights: Highlight[] = [{
    id: 'h1',
    image: '/images/highlight-1.jpg',
    eyebrow: 'Recent arrivals',
    title: 'The Linen Series',
    meta: '12 new pieces',
}, {
    id: 'h2',
    image: '/images/highlight-2.jpg',
    eyebrow: 'Summer sale',
    title: 'End-of-season edit',
    meta: 'Ends Sunday',
    badge: 'Up to 40% off',
},];

const FeaturedSection = () => {
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
                            <a href="#catalogue" className="hl-card-link">
                                <div className="hl-media">
                                    <img src={item.image} alt={item.title} loading="lazy"/>
                                    {item.badge ? (<span className="hl-badge">{item.badge}</span>) : null}
                                </div>
                                <div className="hl-meta">
                                    <div>
                                        <p className="eyebrow hl-meta-eyebrow">{item.eyebrow}</p>
                                        <h3 className="hl-card-title">{item.title}</h3>
                                    </div>
                                    <span className="hl-meta-side">{item.meta}</span>
                                </div>
                            </a>
                        </li>))}
                </ul>
            </div>
        </section>);
};

export default FeaturedSection;
