import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { APP_CONFIG } from '../constants/app';
import ProtectedRoute from '../components/layout/ProtectedRoute';
import AdminRoute from '../components/layout/AdminRoute';
import HomePage from '../components/pages/HomePage';
import LoginForm from '../components/auth/LoginForm';
import RegisterForm from '../components/auth/RegisterForm';
import DataPage from '../components/pages/DataPage';

const AppRouter: React.FC = () => {
  return (
    <Routes>
      {/* Public Routes */}
      <Route path={APP_CONFIG.ROUTES.HOME} element={<HomePage />} />
      <Route path={APP_CONFIG.ROUTES.LOGIN} element={<LoginForm />} />
      <Route path={APP_CONFIG.ROUTES.REGISTER} element={<RegisterForm />} />
      
      {/* Admin Only Routes */}
      <Route
        path={APP_CONFIG.ROUTES.DATA}
        element={
          <AdminRoute>
            <DataPage />
          </AdminRoute>
        }
      />
      
      {/* Default redirect to Login */}
      <Route path="/" element={<Navigate to={APP_CONFIG.ROUTES.LOGIN} replace />} />
      <Route path="*" element={<Navigate to={APP_CONFIG.ROUTES.LOGIN} replace />} />
    </Routes>
  );
};

export default AppRouter;
