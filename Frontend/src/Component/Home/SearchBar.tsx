import {type FormEvent, useState} from "react";

const popularTags = ["Barcelona", "Real Madrid", "Ajax", "Feyenoord"];

const SearchBar = () => {
    const [query, setQuery] = useState('');

    const onSubmit = (e: FormEvent) => {
        e.preventDefault();
    };

    return (<div className="search-wrap">
            <form className="search-form" onSubmit={onSubmit} role="search" aria-label="Site search">
                <span className="search-icon" aria-hidden="true">
                    <svg viewBox="0 0 24 24" width="18" height="18" fill="none" stroke="currentColor" strokeWidth="1.6"
                         strokeLinecap="round" strokeLinejoin="round">
                        <circle cx="11" cy="11" r="7"/>
                        <path d="m20 20-3.5-3.5"/>
                    </svg>
                </span>
                <input
                    type="search"
                    className="search-input"
                    placeholder="Search shirts, fabrics, or collections..."
                    value={query}
                    onChange={(e) => setQuery(e.target.value)}
                    aria-label="Search"
                />
                <button type="submit" className="search-submit">
                    <span>Search</span>
                    <svg viewBox="0 0 24 24" width="14" height="14" fill="none" stroke="currentColor" strokeWidth="2"
                         strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
                        <path d="M5 12h14"/>
                        <path d="m12 5 7 7-7 7"/>
                    </svg>
                </button>
            </form>

            <div className="search-popular">
                <span className="eyebrow search-popular-label">Popular:</span>
                <ul className="search-tags">
                    {popularTags.map((tag) => (<li key={tag}>
                            <button type="button" className="search-tag">{tag}</button>
                        </li>))}
                </ul>
            </div>
        </div>);
};
export default SearchBar;