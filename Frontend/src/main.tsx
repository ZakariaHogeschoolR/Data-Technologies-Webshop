import React from 'react';
import ReactDOM from 'react-dom/client';
import '../src/index.css';
import { createHashRouter, RouterProvider } from 'react-router-dom';
import Layout from "../src/Component/Layout/Layout";
import Home from "../src/Component/Pages/Home";
import About from "../src/Component/Pages/About";
import NotFound from "../src/Component/Pages/NotFound";
import ProductDetail from './Component/ProductDetail';
import CategoryDetail from './Component/CategoryDetail.tsx'
import Winkelwagen from './Component/Layout/Winkelwagen';
import AdminPage from "../src/Component/Pages/AdminPage";
import Authentication from "./Component/Pages/Authentication.tsx";
import ProfilePage from "./Component/Pages/ProfilePage.tsx";
import CheckoutPage from "./Component/Pages/Checkout.tsx";
const router = createHashRouter([
  {
    path: '/',
    element: <Layout />,
    children: [
        { index: true, element: <Home /> },
        { path: 'about', element: <About /> },
        { path: 'products/:id', element: <ProductDetail />},
        { path: 'category/:id', element: <CategoryDetail />},
        { path: `winkelwagen/mine`, element: <Winkelwagen />},
        { path: 'admin', element: <AdminPage /> },
        { path: 'profile', element: <ProfilePage/> },
        { path: '*', element: <NotFound /> },
        { path: 'auth', element: <Authentication />},
        { path: "checkout", element: <CheckoutPage /> }
    ],
  }
]);

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>
)
