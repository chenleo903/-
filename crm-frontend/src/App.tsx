import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ConfigProvider } from 'antd';
import zhCN from 'antd/locale/zh_CN';
import {
  CustomerListPage,
  CustomerDetailPage,
  CustomerFormPage,
  LoginPage,
  NotFoundPage,
} from './pages';
import { ProtectedRoute } from './components';
import './App.css';

// Create a client
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000, // 5 minutes
      retry: 1,
    },
  },
});

// Check if authentication is enabled via environment variable
// Default to false if not set (matching backend behavior)
const isAuthEnabled = import.meta.env.VITE_ENABLE_AUTH === 'true';

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ConfigProvider locale={zhCN}>
        <BrowserRouter>
          <Routes>
            {/* Public route - Login page */}
            <Route path="/login" element={<LoginPage />} />

            {/* Home redirects to customers list */}
            <Route path="/" element={<Navigate to="/customers" replace />} />

            {/* Protected routes - Customer management */}
            <Route
              path="/customers"
              element={
                <ProtectedRoute requireAuth={isAuthEnabled}>
                  <CustomerListPage />
                </ProtectedRoute>
              }
            />
            <Route
              path="/customers/new"
              element={
                <ProtectedRoute requireAuth={isAuthEnabled}>
                  <CustomerFormPage />
                </ProtectedRoute>
              }
            />
            <Route
              path="/customers/:id"
              element={
                <ProtectedRoute requireAuth={isAuthEnabled}>
                  <CustomerDetailPage />
                </ProtectedRoute>
              }
            />
            <Route
              path="/customers/:id/edit"
              element={
                <ProtectedRoute requireAuth={isAuthEnabled}>
                  <CustomerFormPage />
                </ProtectedRoute>
              }
            />

            {/* 404 - Not Found */}
            <Route path="*" element={<NotFoundPage />} />
          </Routes>
        </BrowserRouter>
      </ConfigProvider>
    </QueryClientProvider>
  );
}

export default App;
