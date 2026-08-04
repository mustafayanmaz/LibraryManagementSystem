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

  async getAvailableRooms(date, startTime, endTime) {
    const response = await apiClient.get(
      "/study-rooms/available",
      {
        params: {
          date,
          startTime,
          endTime,
        },
      },
    );

    return response.data;
  },
};

export default roomApi;