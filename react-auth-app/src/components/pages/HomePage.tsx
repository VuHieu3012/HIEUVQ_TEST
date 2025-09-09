import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { useAlert } from '../../hooks/useAlert';
import { APP_CONFIG } from '../../constants/app';
import AlertContainer from '../common/AlertContainer';

const HomePage: React.FC = () => {
  const { isAuthenticated, isLoading } = useAuth();
  const { alerts, removeAlert } = useAlert();

  // Show loading while checking authentication
  if (isLoading) {
    return (
      <div className="d-flex justify-content-center align-items-center" style={{ height: '50vh' }}>
        <div className="spinner-border text-primary" role="status">
          <span className="visually-hidden">Loading...</span>
        </div>
      </div>
    );
  }

  // Always redirect to login if not authenticated
  if (!isAuthenticated) {
    return <Navigate to={APP_CONFIG.ROUTES.LOGIN} replace />;
  }

  return (
    <div className="container mt-5">
      <div className="row">
        <div className="col-12">
          <div className="card">
            <div className="card-header">
              <h2 className="card-title mb-0">Welcome to Homepage</h2>
            </div>
            <div className="card-body">
              <div>
                <p className="card-text">You have successfully logged in!</p>
                <p className="card-text">This is a protected page that requires authentication.</p>
              </div>
              
              <AlertContainer alerts={alerts} onRemoveAlert={removeAlert} />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default HomePage;
