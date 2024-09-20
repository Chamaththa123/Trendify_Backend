import { createBrowserRouter } from "react-router-dom";
import { MainLayout } from "../components/layouts/MainLayout";
import { GuestLayout } from "../components/layouts/GuestLayout";
import { Dashboard } from "../pages/Dashboard/Dashboard";
import { Products } from "../pages/products/Products";
import SignIn from "../pages/users/SignIn";
import ProtectedRoute from "./ProtectedRoute";

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
