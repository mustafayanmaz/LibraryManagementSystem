import { Route, Routes } from "react-router";
import MainLayout from "./layouts/MainLayout";
import BookDetailPage from "./pages/BookDetailPage";
import BooksPage from "./pages/BooksPage";
import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage";
import NotFoundPage from "./pages/NotFoundPage";
import RegisterPage from "./pages/RegisterPage";
import RoomsPage from "./pages/RoomsPage";

export default function App() {
  return (
    <Routes>
      <Route element={<MainLayout />}>
        <Route index element={<HomePage />} />

        <Route path="login" element={<LoginPage />} />

        <Route
          path="register"
          element={<RegisterPage />}
        />

        <Route path="books" element={<BooksPage />} />

        <Route
          path="books/:id"
          element={<BookDetailPage />}
        />

        <Route path="rooms" element={<RoomsPage />} />

        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
}