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
        element: <ProtectedRoute element={<Dashboard />} allowedRoles={["0", "1"]} />,
      },
      {
        path: "/products",
        element: <ProtectedRoute element={<Products />} allowedRoles={["0","1"]} />,
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
