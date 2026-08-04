import apiClient from "./axiosInstance";

const adminApi = {
  async getBooks() {
    const response = await apiClient.get("/books");
    return response.data;
  },

  async getBookById(id) {
    const response = await apiClient.get(`/books/${id}`);
    return response.data;
  },

  async createBook(bookData) {
    const response = await apiClient.post(
      "/books",
      bookData,
    );

    return response.data;
  },

  async updateBook(id, bookData) {
    await apiClient.put(`/books/${id}`, bookData);
  },

  async deleteBook(id) {
    await apiClient.delete(`/books/${id}`);
  },

  async getAuthors() {
    const response = await apiClient.get("/authors");
    return response.data;
  },

  async getCategories() {
    const response = await apiClient.get("/categories");
    return response.data;
  },

  async getShelves() {
    const response = await apiClient.get("/shelves");
    return response.data;
  },

  async getRooms() {
    const response = await apiClient.get("/study-rooms");
    return response.data;
  },

  async createRoom(roomData) {
    const response = await apiClient.post(
      "/study-rooms",
      roomData,
    );

    return response.data;
  },

  async updateRoom(id, roomData) {
    await apiClient.put(`/study-rooms/${id}`, roomData);
  },

  async deleteRoom(id) {
    await apiClient.delete(`/study-rooms/${id}`);
  },

  async getBookReservations() {
    const response = await apiClient.get(
      "/book-reservations",
    );

    return response.data;
  },

  async updateBookReservationStatus(id, status) {
    const response = await apiClient.put(
      `/book-reservations/${id}/status`,
      { status },
    );

    return response.data;
  },

  async getRoomReservations() {
    const response = await apiClient.get(
      "/room-reservations",
    );

    return response.data;
  },

  async updateRoomReservationStatus(id, status) {
    const response = await apiClient.put(
      `/room-reservations/${id}/status`,
      { status },
    );

    return response.data;
  },
};

export default adminApi;