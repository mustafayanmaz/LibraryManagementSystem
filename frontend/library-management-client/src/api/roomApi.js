import apiClient from "./axiosInstance";

const roomApi = {
  async getRooms(activeOnly = true) {
    const response = await apiClient.get("/study-rooms", {
      params: {
        activeOnly,
      },
    });

    return response.data;
  },
};

export default roomApi;