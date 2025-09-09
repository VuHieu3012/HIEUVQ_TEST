import React, { useState } from 'react';
import { useAuth } from '../../contexts/AuthContext';
import { useAlert } from '../../hooks/useAlert';
import Button from '../common/Button';
import AlertContainer from '../common/AlertContainer';

interface LoginFormData {
  usernameOrEmail: string;
  password: string;
  rememberMe: boolean;
}

const LoginForm: React.FC = () => {
  const { login } = useAuth();
  const { alerts, showAlert, removeAlert } = useAlert();

  const [values, setValues] = useState<LoginFormData>({
    usernameOrEmail: '',
    password: '',
    rememberMe: false,
  });
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleChange = (field: keyof LoginFormData, value: any) => {
    setValues(prev => ({ ...prev, [field]: value }));
    // Clear error when user starts typing
    if (errors[field]) {
      setErrors(prev => {
        const newErrors = { ...prev };
        delete newErrors[field];
        return newErrors;
      });
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Basic validation
    const newErrors: Record<string, string> = {};
    if (!values.usernameOrEmail) {
      newErrors.usernameOrEmail = 'Username or email is required';
    }
    if (!values.password) {
      newErrors.password = 'Password is required';
    }
    
    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      return;
    }

    setIsSubmitting(true);
    setErrors({});

    try {
      const result = await login(values.usernameOrEmail, values.password, values.rememberMe);
      
      if (result.success) {
        showAlert('success', 'Login successful! Redirecting...');
        // Navigation is handled by AuthContext
      } else {
        showAlert('danger', result.message || 'Login failed');
      }
    } catch (error) {
      console.error('Login error:', error);
      showAlert('danger', 'An error occurred during login');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="container mt-5">
      <div className="row justify-content-center">
        <div className="col-md-6 col-lg-4">
          <div className="card shadow">
            <div className="card-body">
              <h3 className="card-title text-center mb-4">Login</h3>
              
              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label htmlFor="usernameOrEmail" className="form-label">
                    Username or Email
                  </label>
                  <input
                    type="text"
                    className={`form-control ${errors.usernameOrEmail ? 'is-invalid' : ''}`}
                    id="usernameOrEmail"
                    value={values.usernameOrEmail}
                    onChange={(e) => handleChange('usernameOrEmail', e.target.value)}
                    required
                  />
                  {errors.usernameOrEmail && (
                    <div className="invalid-feedback">{errors.usernameOrEmail}</div>
                  )}
                </div>
                
                <div className="mb-3">
                  <label htmlFor="password" className="form-label">Password</label>
                  <input
                    type="password"
                    className={`form-control ${errors.password ? 'is-invalid' : ''}`}
                    id="password"
                    value={values.password}
                    onChange={(e) => handleChange('password', e.target.value)}
                    required
                  />
                  {errors.password && (
                    <div className="invalid-feedback">{errors.password}</div>
                  )}
                </div>
                
                <div className="mb-3 form-check">
                  <input
                    type="checkbox"
                    className="form-check-input"
                    id="rememberMe"
                    checked={values.rememberMe}
                    onChange={(e) => handleChange('rememberMe', e.target.checked)}
                  />
                  <label className="form-check-label" htmlFor="rememberMe">
                    Remember me
                  </label>
                </div>
                
                <div className="d-grid">
                  <Button
                    type="submit"
                    isLoading={isSubmitting}
                    loadingText="Logging in..."
                  >
                    Login
                  </Button>
                </div>
              </form>
              
              <div className="text-center mt-3">
                <p className="mb-0">
                  Don't have an account? <a href="/register" className="btn btn-link p-0">Register here</a>
                </p>
              </div>
              
              <AlertContainer alerts={alerts} onRemoveAlert={removeAlert} />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default LoginForm;
