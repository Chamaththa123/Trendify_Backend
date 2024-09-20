import { Navigate, Outlet, useLocation } from "react-router-dom";
import { useStateContext } from "../../contexts/NavigationContext";
// import logo from "../../assets/images/login/logo.jpg";
// import signup from "../../assets/images/login/signup.jpg";

export const GuestLayout = () => {
    const { token } = useStateContext();
    const location = useLocation();
    if (token) {
      return <Navigate to="/" />;
    }
  return (
    <div>
      <Outlet />
  </div>
  )
}