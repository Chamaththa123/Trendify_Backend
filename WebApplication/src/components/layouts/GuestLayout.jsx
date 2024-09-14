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
    <div className="flex flex-col md:flex-row h-screen ">
    <div className="flex-1 p-4 border-b-2 md:border-b-0 md:border-r-2 ">
      {/* <img className="md:w-[130px] w-[20%] md:mb-0" src={logo} /> */}
      <br />
      {/* <img src={signup} alt="" /> */}signin
    </div>
    <div className="flex-1 p-4 bg-[#263679]">
      <Outlet />
    </div>
  </div>
  )
}