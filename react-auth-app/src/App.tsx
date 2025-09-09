import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import Layout from './components/layout/Layout';
import AppRouter from './router/AppRouter';
import 'bootstrap/dist/css/bootstrap.min.css';

const App: React.FC = () => {
  return (
    <Router>
      <AuthProvider>
        <Layout>
          <AppRouter />
        </Layout>
      </AuthProvider>
    </Router>
  );
};

export default App;
