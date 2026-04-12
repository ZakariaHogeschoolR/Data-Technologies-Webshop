import { useFetch } from "../../CustomHooks/GetFetchHook";
import { useState } from "react";
import '../../Styles/AdminPage.css';

type User = {
    id: number;
    firstName: string;
    lastName: string;
    username: string;
    email: string;
    address: string;
    postCode: string;
    role: string;
};

type Product = {
    id: number;
    name: string;
    price: number;
    teamId: number;
};

type Stats = {
    totalUsers: number;
    totalProducts: number;
};

const AdminPage = () => {
    const { data: users, isLoading: usersLoading } = useFetch<User[]>({ url: "http://localhost:5261/api/Admin/users" });
    const { data: products, isLoading: productsLoading } = useFetch<Product[]>({ url: "http://localhost:5261/api/Admin/products" });
    const { data: stats } = useFetch<Stats>({ url: "http://localhost:5261/api/Admin/stats" });

    const [resetUserId, setResetUserId] = useState<number | null>(null);
    const [newPassword, setNewPassword] = useState("");
    const [resetMessage, setResetMessage] = useState("");

    const handleResetPassword = async (id: number) => {
        const token = localStorage.getItem("token");
        const res = await fetch(`http://localhost:5261/api/Admin/users/${id}/reset-password`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify({ newPassword })
        });

        if (res.ok) {
            setResetMessage("Password reset successful!");
            setResetUserId(null);
            setNewPassword("");
        } else {
            setResetMessage("Something went wrong.");
        }
    };

    return (
        <div className="admin-container">
            <h1 className="admin-title">Admin Panel</h1>

            <div className="admin-stats">
                <div className="stat-card">
                    <p className="stat-number">{stats?.totalUsers ?? 0}</p>
                    <p className="stat-label">Total Users</p>
                </div>
                <div className="stat-card">
                    <p className="stat-number">{stats?.totalProducts ?? 0}</p>
                    <p className="stat-label">Total Products</p>
                </div>
            </div>

            <section className="admin-section">
                <h2 className="admin-section-title">Users</h2>
                {resetMessage && <p style={{ color: "var(--dark-green)", marginBottom: "1rem" }}>{resetMessage}</p>}
                {usersLoading ? <p>Loading...</p> : (
                    <table className="admin-table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Name</th>
                                <th>Username</th>
                                <th>Email</th>
                                <th>Address</th>
                                <th>Postcode</th>
                                <th>Role</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {users?.map(u => (
                                <tr key={u.id}>
                                    <td>{u.id}</td>
                                    <td>{u.firstName} {u.lastName}</td>
                                    <td>{u.username}</td>
                                    <td>{u.email}</td>
                                    <td>{u.address}</td>
                                    <td>{u.postCode}</td>
                                    <td>
                                        <span className={`admin-badge ${u.role === 'admin' ? 'badge-admin' : 'badge-user'}`}>
                                            {u.role}
                                        </span>
                                    </td>
                                    <td>
                                        {resetUserId === u.id ? (
                                            <div style={{ display: "flex", gap: "6px" }}>
                                                <input
                                                    type="password"
                                                    placeholder="New password"
                                                    value={newPassword}
                                                    onChange={e => setNewPassword(e.target.value)}
                                                    style={{ padding: "4px 8px", borderRadius: "6px", border: "1px solid #ccc", fontSize: "12px" }}
                                                />
                                                <button
                                                    onClick={() => handleResetPassword(u.id)}
                                                    className="admin-badge badge-admin"
                                                    style={{ cursor: "pointer", border: "none" }}
                                                >
                                                    Save
                                                </button>
                                                <button
                                                    onClick={() => setResetUserId(null)}
                                                    className="admin-badge badge-user"
                                                    style={{ cursor: "pointer", border: "none" }}
                                                >
                                                    Cancel
                                                </button>
                                            </div>
                                        ) : (
                                            <button
                                                onClick={() => { setResetUserId(u.id); setResetMessage(""); }}
                                                className="admin-badge badge-user"
                                                style={{ cursor: "pointer", border: "none" }}
                                            >
                                                Reset Password
                                            </button>
                                        )}
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                )}
            </section>

            <section className="admin-section">
                <h2 className="admin-section-title">Products</h2>
                {productsLoading ? <p>Loading...</p> : (
                    <table className="admin-table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Name</th>
                                <th>Price</th>
                                <th>Team ID</th>
                            </tr>
                        </thead>
                        <tbody>
                            {products?.map(p => (
                                <tr key={p.id}>
                                    <td>{p.id}</td>
                                    <td>{p.name}</td>
                                    <td>€{p.price}</td>
                                    <td>{p.teamId}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                )}
            </section>
        </div>
    );
};

export default AdminPage;