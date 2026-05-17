import { useState } from "react";
import { useNavigate } from "react-router-dom";
import '../../Styles/Auth.css';

const ResetPassword = () => {
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState("");
    const navigate = useNavigate();

    const token = new URLSearchParams(window.location.hash.split("?")[1]).get("token");

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (password !== confirmPassword) {
            setMessage("Passwords do not match.");
            return;
        }
        setLoading(true);
        try {
            const res = await fetch("http://localhost:5261/api/Auth/reset-password", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ token, password })
            });

            if (res.ok) {
                setMessage("Password reset successful! Redirecting to login...");
                setTimeout(() => navigate("/auth"), 2000);
            } else {
                setMessage("Invalid or expired token.");
            }
        } catch {
            setMessage("Something went wrong.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="auth-page">
            <div className="auth-card">
                <h2 style={{ color: "var(--dark-green)", marginBottom: "1.5rem" }}>Reset Password</h2>
                {message ? (
                    <p className={message.includes("successful") ? "auth-success" : "auth-error"}>{message}</p>
                ) : null}
                <form className="auth-form" onSubmit={handleSubmit}>
                    <div className="auth-field">
                        <label>New Password</label>
                        <input
                            type="password"
                            placeholder="••••••••"
                            value={password}
                            onChange={e => setPassword(e.target.value)}
                            required
                        />
                    </div>
                    <div className="auth-field">
                        <label>Confirm Password</label>
                        <input
                            type="password"
                            placeholder="••••••••"
                            value={confirmPassword}
                            onChange={e => setConfirmPassword(e.target.value)}
                            required
                        />
                    </div>
                    <button className="auth-submit" type="submit" disabled={loading}>
                        {loading ? "Resetting..." : "Reset Password"}
                    </button>
                </form>
            </div>
        </div>
    );
};

export default ResetPassword;