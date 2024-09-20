import { Navigate } from "react-router-dom";
import { useStateContext } from "../contexts/NavigationContext";

const ProtectedRoute = ({ children, roles }) => {
  const { user } = useStateContext();

  if (!user) {
    // If the user is not logged in, redirect to the login page
    return <Navigate to="/login" />;
  }

  if (roles && !roles.includes(user.role)) {
    // If the user role is not authorized, redirect to the dashboard or an unauthorized page
    return <Navigate to="/" />;
  }

  return children;
};

export default ProtectedRoute;
