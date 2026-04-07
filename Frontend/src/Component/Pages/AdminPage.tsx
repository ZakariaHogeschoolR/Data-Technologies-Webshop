import { useFetch } from "../../CustomHooks/GetFetchHook";

type User = {
    id: number;
    firstName: string;
    lastName: string;
    username: string;
    email: string;
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
        <div style={{ padding: "2rem" }}>
            <h1>Admin Panel</h1>

            <h2>Users</h2>
            {usersLoading ? <p>Loading...</p> : (
                <table border={1} cellPadding={8}>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Name</th>
                            <th>Email</th>
                            <th>Role</th>
                        </tr>
                    </thead>
                    <tbody>
                        {users?.map(u => (
                            <tr key={u.id}>
                                <td>{u.id}</td>
                                <td>{u.firstName} {u.lastName}</td>
                                <td>{u.email}</td>
                                <td>{u.role}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}

            <h2 style={{ marginTop: "2rem" }}>Products</h2>
            {productsLoading ? <p>Loading...</p> : (
                <table border={1} cellPadding={8}>
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
        </div>
    );
};

export default AdminPage;