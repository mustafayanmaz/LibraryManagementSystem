import { Route, Routes } from "react-router";
import AdminRoute from "./components/AdminRoute";
import ProtectedRoute from "./components/ProtectedRoute";
import AdminLayout from "./layouts/AdminLayout";
import MainLayout from "./layouts/MainLayout";
import BookDetailPage from "./pages/BookDetailPage";
import BooksPage from "./pages/BooksPage";
import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage";
import MyReservationsPage from "./pages/MyReservationsPage";
import NotFoundPage from "./pages/NotFoundPage";
import RegisterPage from "./pages/RegisterPage";
import RoomsPage from "./pages/RoomsPage";
import AdminDashboardPage from "./pages/admin/AdminDashboardPage";
import ManageBooksPage from "./pages/admin/ManageBooksPage";
import ManageReservationsPage from "./pages/admin/ManageReservationsPage";
import ManageRoomsPage from "./pages/admin/ManageRoomsPage";

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

        <Route element={<ProtectedRoute />}>
          <Route
            path="my-reservations"
            element={<MyReservationsPage />}
          />
        </Route>

        <Route element={<AdminRoute />}>
          <Route path="admin" element={<AdminLayout />}>
            <Route
              index
              element={<AdminDashboardPage />}
            />

            <Route
              path="books"
              element={<ManageBooksPage />}
            />

            <Route
              path="rooms"
              element={<ManageRoomsPage />}
            />

            <Route
              path="reservations"
              element={<ManageReservationsPage />}
            />
          </Route>
        </Route>

        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  );
}