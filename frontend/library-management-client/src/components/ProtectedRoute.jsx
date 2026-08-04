import {
  Navigate,
  Outlet,
  useLocation,
} from "react-router";
import useAuth from "../hooks/useAuth";
import LoadingSpinner from "./LoadingSpinner";

export default function ProtectedRoute() {
  const location = useLocation();

  const {
    isAuthenticated,
    isInitializing,
  } = useAuth();

  if (isInitializing) {
    return (
      <LoadingSpinner message="Oturum kontrol ediliyor..." />
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

  return <Outlet />;
}