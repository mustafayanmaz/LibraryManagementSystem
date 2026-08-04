import apiClient from "./axiosInstance";

const reservationApi = {
  async reserveBook(bookId) {
    const response = await apiClient.post(
      "/book-reservations",
      { bookId },
    );

    return response.data;
  },

  async getMyBookReservations(status = "") {
    const response = await apiClient.get(
      "/book-reservations/my",
      {
        params: status ? { status } : {},
      },
    );

    return response.data;
  },

  async cancelBookReservation(id) {
    const response = await apiClient.put(
      `/book-reservations/${id}/cancel`,
    );

    return response.data;
  },

  async reserveRoom(reservationData) {
    const response = await apiClient.post(
      "/room-reservations",
      reservationData,
    );

    return response.data;
  },

  async getMyRoomReservations(status = "") {
    const response = await apiClient.get(
      "/room-reservations/my",
      {
        params: status ? { status } : {},
      },
    );

    return response.data;
  },

  async cancelRoomReservation(id) {
    const response = await apiClient.put(
      `/room-reservations/${id}/cancel`,
    );

    return response.data;
  },
};

export default reservationApi;