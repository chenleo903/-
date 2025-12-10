import { Navigate, useLocation } from 'react-router-dom';
import { isAuthenticated } from '../api/auth';

interface ProtectedRouteProps {
  children: React.ReactNode;
  requireAuth?: boolean;
}

/**
 * ProtectedRoute component that handles authentication-based routing.
 * 
 * When requireAuth is true (default), it checks if the user is authenticated.
 * If not authenticated, redirects to the login page with the current location
 * stored in state so we can redirect back after login.
 * 
 * When requireAuth is false, the route is accessible without authentication.
 */
export default function ProtectedRoute({ children, requireAuth = true }: ProtectedRouteProps) {
  const location = useLocation();

  // If authentication is not required, render children directly
  if (!requireAuth) {
    return <>{children}</>;
  }

  // Check if user is authenticated
  if (!isAuthenticated()) {
    // Redirect to login page, saving the current location
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <>{children}</>;
}
