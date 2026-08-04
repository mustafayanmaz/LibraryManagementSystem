import axios from "axios";
import authStorage from "../utils/authStorage";

const apiClient = axios.create({
  baseURL:
    import.meta.env.VITE_API_URL ??
    "http://localhost:5299/api",
  timeout: 10000,
  headers: {
    "Content-Type": "application/json",
  },
});

apiClient.interceptors.request.use(
  (config) => {
    const token = authStorage.getToken();

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => Promise.reject(error),
);

export default apiClient;