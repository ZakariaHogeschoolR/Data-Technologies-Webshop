import { Link } from 'react-router-dom';
import '../../Styles/About.css';

const About = () => {
    return (
        <div className="about-page">

            {/* Hero */}
            <section className="about-hero">
                <h1 className="about-hero-title">About Us</h1>
                <p className="about-hero-sub">Your home for authentic football jerseys from around the world.</p>
            </section>

            {/* Mission */}
            <section className="about-section">
                <div className="about-card">
                    <span className="about-icon">⚽</span>
                    <h2 className="about-card-title">Our Mission</h2>
                    <p className="about-card-text">
                        We believe every football fan deserves to wear the colours of their club with pride.
                        Our mission is to bring authentic, high-quality football jerseys from clubs and national
                        teams across the globe — straight to your door.
                    </p>
                </div>

                <div className="about-card">
                    <span className="about-icon">🏆</span>
                    <h2 className="about-card-title">Quality First</h2>
                    <p className="about-card-text">
                        Every jersey in our collection is carefully selected for quality and authenticity.
                        From classic retro shirts to the latest season kits, we stock only the best
                        from top clubs and national sides worldwide.
                    </p>
                </div>

                <div className="about-card">
                    <span className="about-icon">🌍</span>
                    <h2 className="about-card-title">Global Selection</h2>
                    <p className="about-card-text">
                        Whether you support a Premier League giant, a La Liga legend, or your national team,
                        we have something for every fan. Our collection spans hundreds of clubs across
                        all major leagues and tournaments.
                    </p>
                </div>
            </section>

            {/* Stats */}
            <section className="about-stats">
                <div className="about-stat">
                    <p className="about-stat-number">500+</p>
                    <p className="about-stat-label">Jerseys</p>
                </div>
                <div className="about-stat">
                    <p className="about-stat-number">50+</p>
                    <p className="about-stat-label">Clubs</p>
                </div>
                <div className="about-stat">
                    <p className="about-stat-number">30+</p>
                    <p className="about-stat-label">Countries</p>
                </div>
                <div className="about-stat">
                    <p className="about-stat-number">100%</p>
                    <p className="about-stat-label">Authentic</p>
                </div>
            </section>

            {/* CTA */}
            <section className="about-cta">
                <h2 className="about-cta-title">Ready to find your jersey?</h2>
                <Link to="/" className="about-cta-btn">Shop Now</Link>
            </section>

        </div>
    );
};

export default About;
