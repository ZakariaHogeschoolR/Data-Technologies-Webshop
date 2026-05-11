import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useFetch } from "../../CustomHooks/GetFetchHook";
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
    const navigate = useNavigate();

    useEffect(() => {
        const token = localStorage.getItem("token");
        const role = localStorage.getItem("role");
        if (!token || (role !== "admin" && role !== "hoofdadmin")) {
            navigate("/auth");
        }
    }, []);

    const [page, setPage] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [selectedCategories, setSelectedCategories] = useState<number[]>([]);

    const categories = [
        { id: 41, name: "Training" },
        { id: 1, name: "Shirt" },
        { id: 400, name: "Socks" },
        { id: 397, name: "Balls" },
        { id: 399, name: "Shorts" },
        { id: 398, name: "Gloves" },
    ];

    const toggleCategory = (id: number) => {
        setSelectedCategories(prev =>
            prev.includes(id) ? prev.filter(c => c !== id) : [...prev, id]
        );
        setPage(1);
    };

    const [searchQuery, setSearchQuery] = useState("");
    const [searchResults, setSearchResults] = useState<Product[] | null>(null);

    const { data: users, isLoading: usersLoading } = useFetch<User[]>({ url: "http://localhost:5261/api/Admin/users" });

    const productUrl = selectedCategories.length > 0
        ? `http://localhost:5261/api/Admin/products/filter?${selectedCategories.map(id => `categoryIds=${id}`).join("&")}&page=${page}&pageSize=${pageSize}`
        : `http://localhost:5261/api/Admin/products?page=${page}&pageSize=${pageSize}`;

    const { data: products, isLoading: productsLoading } = useFetch<Product[]>({ url: productUrl });
    const { data: stats } = useFetch<Stats>({ url: "http://localhost:5261/api/Admin/stats" });

    const [resetUserId, setResetUserId] = useState<number | null>(null);
    const [newPassword, setNewPassword] = useState("");
    const [resetMessage, setResetMessage] = useState("");
    const [userList, setUserList] = useState<User[] | null>(null);

    const [showAddProduct, setShowAddProduct] = useState(false);
    const [newProduct, setNewProduct] = useState({
        productImage: "",
        name: "",
        description: "",
        price: 0,
        teamId: 0,
        categoryId: 0
    });

    const [showEditPrice, setShowEditPrice] = useState(false);
    const [editProductId, setEditProductId] = useState<number | null>(null);
    const [editPrice, setEditPrice] = useState<number>(0);

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

    const handleDeleteUser = async (id: number) => {
        if (!confirm("Are you sure you want to delete this user?")) return;

        const token = localStorage.getItem("token");
        const res = await fetch(`http://localhost:5261/api/Admin/users/${id}`, {
            method: "DELETE",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (res.ok) {
            setResetMessage("User deleted successfully!");
            setUserList((prev) => (prev ?? users ?? []).filter(u => u.id !== id));
        } else {
            setResetMessage("Something went wrong.");
        }
    };

    const handleDeleteProduct = async (id: number) => {
        if (!confirm("Are you sure you want to delete this product?")) return;

        const token = localStorage.getItem("token");
        const res = await fetch(`http://localhost:5261/api/Admin/products/${id}`, {
            method: "DELETE",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (res.ok) {
            setResetMessage("Product deleted successfully!");
        } else {
            setResetMessage("Something went wrong.");
        }
    };

    const handleAddProduct = async () => {
        const token = localStorage.getItem("token");

        const res = await fetch(`http://localhost:5261/api/Admin/products/create`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify({
                productImage: newProduct.productImage,
                name: newProduct.name,
                description: newProduct.description,
                price: newProduct.price,
                teamId: newProduct.teamId
            })
        });

        if (res.ok) {
            const createdProduct = await res.json();

            if (newProduct.categoryId > 0) {
                await fetch(`http://localhost:5261/api/Admin/products/category`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": `Bearer ${token}`
                    },
                    body: JSON.stringify({
                        productId: createdProduct.id,
                        categoryId: newProduct.categoryId
                    })
                });
            }

            setResetMessage("Product added successfully!");
            setShowAddProduct(false);
            setNewProduct({ productImage: "", name: "", description: "", price: 0, teamId: 0, categoryId: 0 });
        } else {
            setResetMessage("Something went wrong.");
        }
    };

    const handleEditPrice = async () => {
        if (!editProductId) return;

        const token = localStorage.getItem("token");
        const res = await fetch(`http://localhost:5261/api/Admin/products/${editProductId}/price`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify({ price: editPrice })
        });

        if (res.ok) {
            setResetMessage("Price updated successfully!");
            setShowEditPrice(false);
            setEditProductId(null);
            setEditPrice(0);
        } else {
            setResetMessage("Something went wrong.");
        }
    };

    const handleSearch = async (value: string) => {
        setSearchQuery(value);

        if (value.length < 2) {
            setSearchResults(null);
            return;
        }

        const token = localStorage.getItem("token");
        const res = await fetch(`http://localhost:5261/api/Admin/products/search?name=${value}`, {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (res.ok) {
            const data = await res.json();
            setSearchResults(data);
        }
    };

    const handleUpdateRole = async (id: number, currentRole: string) => {
        const newRole = currentRole === "admin" ? "user" : "admin";
        if (!confirm(`Change role to "${newRole}"?`)) return;

        const token = localStorage.getItem("token");
        const res = await fetch(`http://localhost:5261/api/Admin/users/${id}/role`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify({ role: newRole })
        });

        if (res.ok) {
            setResetMessage(`Role updated to "${newRole}"!`);
            setUserList((prev) => (prev ?? users ?? []).map(u =>
                u.id === id ? { ...u, role: newRole } : u
            ));
        } else {
            setResetMessage("Something went wrong.");
        }
    };

    const displayedUsers = userList ?? users;

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
                {usersLoading ? (
                    <div className="loading-container">
                        <div className="spinner"></div>
                        <p className="loading-text">LOADING...</p>
                    </div>
                ) : (
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
                            {displayedUsers?.map(u => (
                                <tr key={u.id}>
                                    <td>{u.id}</td>
                                    <td>{u.firstName} {u.lastName}</td>
                                    <td>{u.username}</td>
                                    <td>{u.email}</td>
                                    <td>{u.address}</td>
                                    <td>{u.postCode}</td>
                                    <td>
                                        <span className={`admin-badge ${u.role === 'hoofdadmin' ? 'badge-hoofdadmin' : u.role === 'admin' ? 'badge-admin' : 'badge-user'}`}>
                                            {u.role}
                                        </span>
                                    </td>
                                    <td>
                                        {u.role === 'hoofdadmin' ? (
                                            <span style={{ fontSize: "12px", color: "var(--dark-green)", fontStyle: "italic" }}>Protected</span>
                                        ) : resetUserId === u.id ? (
                                            <div style={{ display: "flex", gap: "6px" }}>
                                                <input
                                                    type="password"
                                                    placeholder="New password"
                                                    value={newPassword}
                                                    onChange={e => setNewPassword(e.target.value)}
                                                    style={{ padding: "4px 8px", borderRadius: "6px", border: "1px solid #ccc", fontSize: "12px" }}
                                                />
                                                <button onClick={() => handleResetPassword(u.id)} className="admin-badge badge-admin" style={{ cursor: "pointer", border: "none" }}>Save</button>
                                                <button onClick={() => setResetUserId(null)} className="admin-badge badge-user" style={{ cursor: "pointer", border: "none" }}>Cancel</button>
                                            </div>
                                        ) : (
                                            <div style={{ display: "flex", gap: "6px", flexWrap: "wrap" }}>
                                                <button onClick={() => { setResetUserId(u.id); setResetMessage(""); }} className="admin-badge badge-user" style={{ cursor: "pointer", border: "none" }}>Reset Password</button>
                                                <button onClick={() => handleUpdateRole(u.id, u.role)} className="admin-badge badge-admin" style={{ cursor: "pointer", border: "none", backgroundColor: u.role === "admin" ? "#854F0B" : "#185FA5" }}>
                                                    {u.role === "admin" ? "Make User" : "Make Admin"}
                                                </button>
                                                {(localStorage.getItem("role") === "hoofdadmin" || u.role === "user") && (
                                                    <button onClick={() => handleDeleteUser(u.id)} className="admin-badge badge-admin" style={{ cursor: "pointer", border: "none", backgroundColor: "#c0392b" }}>Delete</button>
                                                )}
                                            </div>
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

                <div style={{ marginBottom: "1rem", display: "flex", gap: "8px" }}>
                    <button onClick={() => setShowAddProduct(!showAddProduct)} className="admin-badge badge-admin" style={{ cursor: "pointer", border: "none", fontSize: "13px", padding: "6px 14px" }}>
                        {showAddProduct ? "Cancel" : "+ Add Product"}
                    </button>
                    <button onClick={() => setShowEditPrice(!showEditPrice)} className="admin-badge badge-user" style={{ cursor: "pointer", border: "none", fontSize: "13px", padding: "6px 14px" }}>
                        {showEditPrice ? "Cancel" : "✏️ Edit Price"}
                    </button>
                </div>

                {showAddProduct && (
                    <div style={{ background: "var(--white)", borderRadius: "10px", padding: "1.5rem", marginBottom: "1.5rem", display: "flex", flexDirection: "column", gap: "10px", maxWidth: "500px" }}>
                        <input placeholder="Product Image URL" value={newProduct.productImage} onChange={e => setNewProduct({...newProduct, productImage: e.target.value})} style={{ padding: "8px 12px", borderRadius: "6px", border: "1px solid #ccc", fontSize: "14px" }} />
                        <input placeholder="Name" value={newProduct.name} onChange={e => setNewProduct({...newProduct, name: e.target.value})} style={{ padding: "8px 12px", borderRadius: "6px", border: "1px solid #ccc", fontSize: "14px" }} />
                        <input placeholder="Description" value={newProduct.description} onChange={e => setNewProduct({...newProduct, description: e.target.value})} style={{ padding: "8px 12px", borderRadius: "6px", border: "1px solid #ccc", fontSize: "14px" }} />
                        <input placeholder="Price" type="number" value={newProduct.price} onChange={e => setNewProduct({...newProduct, price: parseFloat(e.target.value)})} style={{ padding: "8px 12px", borderRadius: "6px", border: "1px solid #ccc", fontSize: "14px" }} />
                        <input placeholder="Team ID" type="number" value={newProduct.teamId} onChange={e => setNewProduct({...newProduct, teamId: parseInt(e.target.value)})} style={{ padding: "8px 12px", borderRadius: "6px", border: "1px solid #ccc", fontSize: "14px" }} />
                        <select value={newProduct.categoryId} onChange={e => setNewProduct({...newProduct, categoryId: parseInt(e.target.value)})} style={{ padding: "8px 12px", borderRadius: "6px", border: "1px solid #ccc", fontSize: "14px" }}>
                            <option value={0}>Select Category</option>
                            <option value={1}>Shirt</option>
                            <option value={41}>Training</option>
                            <option value={400}>Socks</option>
                            <option value={397}>Balls</option>
                            <option value={399}>Shorts</option>
                            <option value={398}>Gloves</option>
                        </select>
                        <button onClick={handleAddProduct} className="admin-badge badge-admin" style={{ cursor: "pointer", border: "none", fontSize: "13px", padding: "8px 14px" }}>Save Product</button>
                    </div>
                )}

                {showEditPrice && (
                    <div style={{ background: "var(--white)", borderRadius: "10px", padding: "1.5rem", marginBottom: "1.5rem", display: "flex", flexDirection: "column", gap: "10px", maxWidth: "500px" }}>
                        <input placeholder="Product ID" type="number" value={editProductId ?? ""} onChange={e => setEditProductId(parseInt(e.target.value))} style={{ padding: "8px 12px", borderRadius: "6px", border: "1px solid #ccc", fontSize: "14px" }} />
                        <input placeholder="New Price" type="number" value={editPrice} onChange={e => setEditPrice(parseFloat(e.target.value))} style={{ padding: "8px 12px", borderRadius: "6px", border: "1px solid #ccc", fontSize: "14px" }} />
                        <button onClick={handleEditPrice} className="admin-badge badge-admin" style={{ cursor: "pointer", border: "none", fontSize: "13px", padding: "8px 14px" }}>Update Price</button>
                    </div>
                )}

                <div style={{ marginBottom: "1rem" }}>
                    <input type="text" placeholder="Search by name or team..." value={searchQuery} onChange={e => handleSearch(e.target.value)} style={{ padding: "8px 12px", borderRadius: "6px", border: "1px solid #ccc", fontSize: "14px", width: "300px", color: "var(--dark-green)" }} />
                </div>

                <div style={{ display: "flex", gap: "8px", marginBottom: "1rem", flexWrap: "wrap" }}>
                    {categories.map(cat => (
                        <button key={cat.id} onClick={() => toggleCategory(cat.id)} className={`admin-badge ${selectedCategories.includes(cat.id) ? 'badge-admin' : 'badge-user'}`} style={{ cursor: "pointer", border: "none", fontSize: "13px", padding: "6px 14px" }}>
                            {cat.name}
                        </button>
                    ))}
                    {selectedCategories.length > 0 && (
                        <button onClick={() => { setSelectedCategories([]); setPage(1); }} className="admin-badge" style={{ cursor: "pointer", border: "none", backgroundColor: "#c0392b", color: "white", fontSize: "13px", padding: "6px 14px" }}>Clear</button>
                    )}
                </div>

                <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "1rem" }}>
                    <div style={{ display: "flex", gap: "8px" }}>
                        <button onClick={() => setPage(p => Math.max(1, p - 1))} className="admin-badge badge-user" style={{ cursor: "pointer", border: "none" }} disabled={page === 1}>Previous</button>
                        <span style={{ fontSize: "14px", color: "var(--dark-green)", alignSelf: "center" }}>Page {page}</span>
                        <button onClick={() => setPage(p => p + 1)} className="admin-badge badge-user" style={{ cursor: "pointer", border: "none" }} disabled={!products || products.length < pageSize}>Next</button>
                    </div>
                    <div style={{ display: "flex", alignItems: "center", gap: "8px" }}>
                        <label style={{ fontSize: "14px", color: "var(--dark-green)" }}>Items per page:</label>
                        <select value={pageSize} onChange={e => { setPageSize(Number(e.target.value)); setPage(1); }} style={{ padding: "6px 10px", borderRadius: "6px", border: "1px solid #ccc", fontSize: "14px", color: "var(--dark-green)" }}>
                            <option value={10}>10</option>
                            <option value={25}>25</option>
                            <option value={50}>50</option>
                        </select>
                    </div>
                </div>

                {productsLoading ? (
                    <div className="loading-container">
                        <div className="spinner"></div>
                        <p className="loading-text">LOADING...</p>
                    </div>
                ) : (
                    <table className="admin-table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>Name</th>
                                <th>Price</th>
                                <th>Team ID</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {(searchQuery.length >= 2 ? searchResults : products)?.map(p => (
                                <tr key={p.id}>
                                    <td>{p.id}</td>
                                    <td>{p.name}</td>
                                    <td>€{p.price}</td>
                                    <td>{p.teamId}</td>
                                    <td>
                                        <button onClick={() => handleDeleteProduct(p.id)} className="admin-badge badge-admin" style={{ cursor: "pointer", border: "none", backgroundColor: "#c0392b" }}>Delete</button>
                                    </td>
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