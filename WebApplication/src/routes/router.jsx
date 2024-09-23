import { createBrowserRouter } from "react-router-dom";
import { MainLayout } from "../components/layouts/MainLayout";
import { GuestLayout } from "../components/layouts/GuestLayout";
import { Dashboard } from "../pages/Dashboard/Dashboard";
import { Products } from "../pages/products/Products";
import SignIn from "../pages/users/SignIn";
import ProtectedRoute from "./ProtectedRoute";
import { Vendors } from "../pages/users/Vendors";
import { Csr } from "../pages/users/Csr";
import { ViewProduct } from "../pages/products/ViewProduct";
import { UserProfile } from "../pages/users/UserProfile";

const router = createBrowserRouter([
  {
    path: "/",
    element: <MainLayout />,
    children: [
      {
        path: "/",
        element: (
          <ProtectedRoute roles={['1', '3']}>
            <Dashboard />
          </ProtectedRoute>
        ),
      },
      {
        path: "/products",
        element: (
          <ProtectedRoute roles={['1', '3']}>
            <Products />
          </ProtectedRoute>
        ),
      },
      {
        path: "/products/view/:id",
        element: (
          <ProtectedRoute roles={['1', '3']}>
            <ViewProduct />
          </ProtectedRoute>
        ),
      },
      {
        path: "/users/vendors",
        element: (
          <ProtectedRoute roles={['1', '3']}>
            <Vendors />
          </ProtectedRoute>
        ),
      },
      {
        path: "/users/csr",
        element: (
          <ProtectedRoute roles={['1', '3']}>
            <Csr />
          </ProtectedRoute>
        ),
      },
      {
        path: "/profile",
        element: (
          <ProtectedRoute roles={['1', '3']}>
            <UserProfile />
          </ProtectedRoute>
        ),
      },
    ],
  },
  {
    path: "/",
    element: <GuestLayout />,
    children: [
      {
        path: "/login",
        element: <SignIn />
      },
    ],
  },
]);

export default router;
