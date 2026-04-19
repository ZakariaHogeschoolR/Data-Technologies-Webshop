import '../../Styles/Home.css';
import SearchBar from '../Home/SearchBar';
import FeaturedSection from '../Home/FeaturedSection';
import CatalogueSection from '../Home/CatalogueSection';

const Home = () => {
    return (<div className="home">
        <section className="hero" aria-label="Hero">
            <div className="hero-inner">
                <p className="eyebrow">Find your fit</p>
                <h1 className="hero-title text-balance">
                    Considered shirting,<br/>
                    made to last.
                </h1>
                <SearchBar/>
            </div>
        </section>

        <FeaturedSection/>
        <CatalogueSection/>
    </div>);
};

export default Home;
