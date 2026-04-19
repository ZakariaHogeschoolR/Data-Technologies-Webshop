import '../../Styles/Home.css';
import SearchBar from '../Home/SearchBar';
import FeaturedSection from '../Home/FeaturedSection';
import CatalogueSection from '../Home/CatalogueSection';

const Home = () => {
    return (<div className="home">
        <section className="hero" aria-label="Hero">
            <div className="hero-inner">
                <p className="eyebrow">Find your Team</p>
                <h1 className="hero-title text-balance">
                    Find your Team
                </h1>
                <SearchBar/>
            </div>
        </section>

        <FeaturedSection/>
        <CatalogueSection/>
    </div>);
};

export default Home;
