import {
  Navigate,
  Outlet,
  useLocation,
} from "react-router";
import useAuth from "../hooks/useAuth";
import LoadingSpinner from "./LoadingSpinner";

export default function AdminRoute() {
  const location = useLocation();

  const {
    isAuthenticated,
    isAdmin,
    isInitializing,
  } = useAuth();

  if (isInitializing) {
    return (
      <LoadingSpinner message="Yetki kontrol ediliyor..." />
    );
  }

  if (!isAuthenticated) {
    return (
      <Navigate
        to="/login"
        state={{ from: location }}
        replace
      />
    );
  }

  if (!isAdmin) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
}