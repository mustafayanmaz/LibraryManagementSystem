import {
  useCallback,
  useEffect,
  useMemo,
  useState,
} from "react";
import authApi from "../api/authApi";
import authStorage from "../utils/authStorage";
import AuthContext from "./AuthContext";

export default function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [isInitializing, setIsInitializing] =
    useState(true);

  useEffect(() => {
    let isActive = true;

    async function loadCurrentUser() {
      const token = authStorage.getToken();

      if (!token) {
        if (isActive) {
          setIsInitializing(false);
        }

        return;
      }

      try {
        const profile = await authApi.getProfile();

        if (isActive) {
          setUser(profile);
        }
      } catch {
        authStorage.clearToken();

        if (isActive) {
          setUser(null);
        }
      } finally {
        if (isActive) {
          setIsInitializing(false);
        }
      }
    }

    loadCurrentUser();

    return () => {
      isActive = false;
    };
  }, []);

  const login = useCallback(async (credentials) => {
    const response = await authApi.login(credentials);

    authStorage.setToken(response.token);
    setUser(response.user);

    return response.user;
  }, []);

  const register = useCallback(async (userData) => {
    const response = await authApi.register(userData);

    authStorage.setToken(response.token);
    setUser(response.user);

    return response.user;
  }, []);

  const logout = useCallback(() => {
    authStorage.clearToken();
    setUser(null);
  }, []);

  const value = useMemo(
    () => ({
      user,
      isInitializing,
      isAuthenticated: Boolean(user),
      isAdmin: user?.roles?.includes("Admin") ?? false,
      login,
      register,
      logout,
    }),
    [user, isInitializing, login, register, logout],
  );

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}