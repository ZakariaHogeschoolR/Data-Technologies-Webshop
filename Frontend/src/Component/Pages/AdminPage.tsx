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

const AdminPage = () => {
    const { data: users, isLoading: usersLoading } = useFetch<User[]>({ url: "http://localhost:5261/api/Admin/users" });
    const { data: products, isLoading: productsLoading } = useFetch<Product[]>({ url: "http://localhost:5261/api/Admin/products" });

    return (
        <div className="admin-container">
            <h1 className="admin-title">Admin Panel</h1>

            <section className="admin-section">
                <h2 className="admin-section-title">Users</h2>
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