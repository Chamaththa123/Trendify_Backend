import { Navigate } from "react-router-dom";
import { useStateContext } from "../contexts/NavigationContext";

// Higher-order component to protect routes based on user role
const ProtectedRoute = ({ element, allowedRoles }) => {
  const { user } = useStateContext();

  if (!user || !allowedRoles.includes(user.role)) {
    // Redirect to login page or show an unauthorized message
    return <Navigate to="/login" replace />;
  }

  return element;
};

export default ProtectedRoute;
