import apiClient from "./axiosInstance";

const authApi = {
  async login(credentials) {
    const response = await apiClient.post(
      "/auth/login",
      credentials,
    );

    return response.data;
  },

  async register(userData) {
    const response = await apiClient.post(
      "/auth/register",
      userData,
    );

    return response.data;
  },

  async getProfile() {
    const response = await apiClient.get("/auth/profile");

    return response.data;
  },
};

export default authApi;