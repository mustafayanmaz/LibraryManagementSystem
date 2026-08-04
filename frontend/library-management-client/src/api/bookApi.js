import apiClient from "./axiosInstance";

const bookApi = {
  async getBooks(params = {}) {
    const response = await apiClient.get("/books", {
      params,
    });

    return response.data;
  },

  async getBookById(id) {
    const response = await apiClient.get(`/books/${id}`);

    return response.data;
  },

  async getCategories() {
    const response = await apiClient.get("/categories");

    return response.data;
  },
};

export default bookApi;