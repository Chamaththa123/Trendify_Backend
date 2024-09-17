import { createBrowserRouter } from "react-router-dom";
import { MainLayout } from "../components/layouts/MainLayout";
import { GuestLayout } from "../components/layouts/GuestLayout";
import { Dashboard } from "../pages/Dashboard/Dashboard";
import { Products } from "../pages/products/Products";
import SignIn from "../pages/users/SignIn";

const router = createBrowserRouter([
  {
    path: "/",
    element: <MainLayout />,
    children: [
      { path: "/", element: <Dashboard /> },
      { path: "/products", element: <Products /> },      

      //   {
      //     path: "/estates",
      //     element: <Estates />,
      //   },
    ],
  },
  { path: "/login", element: <SignIn /> },
  // {
  //   path: "/",
  //   element: <GuestLayout />,
  //   children: [
  //     {
  //       path: "/login",
  //       element: <Dashboard />
  //     },
  //     {
  //       path: "/signup",
  //       // element: <SignUp />,
  //     },
  //   ],
  // },
]);

export default router;
