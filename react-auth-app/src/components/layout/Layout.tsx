import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { APP_CONFIG } from '../../constants/app';
import Button from '../common/Button';

interface LayoutProps {
  children: React.ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  const { isAuthenticated, isAdmin, logout } = useAuth();
  const location = useLocation();

  const handleLogout = async () => {
    await logout();
  };

  // Hide header on login and register pages
  const isAuthPage = location.pathname === APP_CONFIG.ROUTES.LOGIN || location.pathname === APP_CONFIG.ROUTES.REGISTER;

  return (
    <div>
      {!isAuthPage && (
        <header>
        <nav className="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3">
          <div className="container-fluid">
            <Link className="navbar-brand" to={APP_CONFIG.ROUTES.HOME}>
              {APP_CONFIG.NAME}
            </Link>
            <button
              className="navbar-toggler"
              type="button"
              data-bs-toggle="collapse"
              data-bs-target=".navbar-collapse"
              aria-controls="navbarSupportedContent"
              aria-expanded="false"
              aria-label="Toggle navigation"
            >
              <span className="navbar-toggler-icon"></span>
            </button>
            <div className="navbar-collapse collapse d-sm-inline-flex justify-content-between">
              <ul className="navbar-nav flex-grow-1">
                <li className="nav-item">
                  <Link
                    className={`nav-link ${location.pathname === APP_CONFIG.ROUTES.HOME ? 'text-primary' : 'text-dark'}`}
                    to={APP_CONFIG.ROUTES.HOME}
                  >
                    Home
                  </Link>
                </li>
                {isAuthenticated && isAdmin && (
                  <li className="nav-item">
                    <Link
                      className={`nav-link ${location.pathname === APP_CONFIG.ROUTES.DATA ? 'text-primary' : 'text-dark'}`}
                      to={APP_CONFIG.ROUTES.DATA}
                    >
                      Data
                    </Link>
                  </li>
                )}
              </ul>
              <ul className="navbar-nav">
                {isAuthenticated ? (
                  <li className="nav-item">
                    <Button variant="outline-danger" onClick={handleLogout}>
                      Logout
                    </Button>
                  </li>
                ) : (
                  <>
                    <li className="nav-item me-2">
                      <Link to={APP_CONFIG.ROUTES.LOGIN}>
                        <Button variant="outline-primary">Login</Button>
                      </Link>
                    </li>
                    <li className="nav-item">
                      <Link to={APP_CONFIG.ROUTES.REGISTER}>
                        <Button>Register</Button>
                      </Link>
                    </li>
                  </>
                )}
              </ul>
            </div>
          </div>
        </nav>
      </header>
      )}
      <div className="container">
        <main role="main" className="pb-3">
          {children}
        </main>
      </div>
    </div>
  );
};

export default Layout;
