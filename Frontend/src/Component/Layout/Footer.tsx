import {type FormEvent, useState} from 'react';
import '../../Styles/Footer.css';

const shopLinks = ['New', 'Oxford', 'Linen', 'Overshirt', 'Sale', 'Gift cards'];
const companyLinks = ['Journal', 'About', 'Sustainability', 'Careers', 'Press'];
const supportLinks = ['Contact', 'Shipping', 'Returns', 'Size guide', 'FAQ'];

const Footer = () => {
    const [email, setEmail] = useState('');
    const [submitted, setSubmitted] = useState(false);

    const handleSubmit = (e: FormEvent) => {
        e.preventDefault();
        if (!email) return;
        setSubmitted(true);
    };

    return (<footer className="ft-root" aria-labelledby="footer-heading">
            <h2 id="footer-heading" className="sr-only">Atelier Verte footer</h2>

            <div className="ft-container">
                <div className="ft-top">
                    <div className="ft-brand-col">
                        <p className="ft-brand">Atelier Verte</p>
                        <p className="ft-tagline">
                            A small studio making considered shirting in Copenhagen and Porto since 2019.
                        </p>
                    </div>

                    <div className="ft-newsletter">
                        <p className="eyebrow ft-eyebrow">The letter</p>
                        <h3 className="ft-headline">New drops, once a week.</h3>
                        <form className="ft-form" onSubmit={handleSubmit}>
                            <label htmlFor="footer-email" className="sr-only">Email address</label>
                            <input
                                id="footer-email"
                                type="email"
                                required
                                placeholder="you@domain.com"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                className="ft-input"
                            />
                            <button type="submit" className="ft-submit">
                                {submitted ? 'Subscribed' : 'Subscribe'}
                            </button>
                        </form>
                    </div>
                </div>

                <nav className="ft-columns" aria-label="Footer navigation">
                    <div className="ft-col">
                        <h4 className="ft-col-title">Shop</h4>
                        <ul>
                            {shopLinks.map((l) => (<li key={l}><a href="#">{l}</a></li>))}
                        </ul>
                    </div>
                    <div className="ft-col">
                        <h4 className="ft-col-title">Support</h4>
                        <ul>
                            {supportLinks.map((l) => (<li key={l}><a href="#">{l}</a></li>))}
                        </ul>
                    </div>
                    <div className="ft-col">
                        <h4 className="ft-col-title">Studio</h4>
                        <ul>
                            {companyLinks.map((l) => (<li key={l}><a href="#">{l}</a></li>))}
                        </ul>
                    </div>
                    <div className="ft-col">
                        <h4 className="ft-col-title">Follow</h4>
                        <ul>
                            <li><a href="#">Instagram</a></li>
                            <li><a href="#">Pinterest</a></li>
                            <li><a href="#">TikTok</a></li>
                        </ul>
                    </div>
                </nav>

                <div className="ft-bottom">
                    <p className="ft-copy">
                        &copy; {new Date().getFullYear()} Atelier Verte. All rights reserved.
                    </p>
                    <ul className="ft-legal">
                        <li><a href="#">Privacy</a></li>
                        <li><a href="#">Terms</a></li>
                        <li><a href="#">Cookies</a></li>
                    </ul>
                </div>
            </div>
        </footer>);
};

export default Footer;
