import React, { useState } from 'react';
import { useAuth } from '../../contexts/AuthContext';
import { useAlert } from '../../hooks/useAlert';
import { USER_TYPES } from '../../constants/app';
import Button from '../common/Button';
import AlertContainer from '../common/AlertContainer';

interface RegisterFormData {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  userType: string;
  acceptTerms: boolean;
}

const RegisterForm: React.FC = () => {
  const { register } = useAuth();
  const { alerts, showAlert, removeAlert } = useAlert();
  const [showTermsModal, setShowTermsModal] = useState(false);

  const [values, setValues] = useState<RegisterFormData>({
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
    userType: '',
    acceptTerms: false,
  });
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleChange = (field: keyof RegisterFormData, value: any) => {
    setValues(prev => ({ ...prev, [field]: value }));
    
    // Clear error when user starts typing
    if (errors[field]) {
      setErrors(prev => {
        const newErrors = { ...prev };
        delete newErrors[field];
        return newErrors;
      });
    }
    
    // Validate confirmPassword when password changes
    if (field === 'password' && values.confirmPassword) {
      if (value !== values.confirmPassword) {
        setErrors(prev => ({ ...prev, confirmPassword: 'Passwords do not match' }));
      } else {
        setErrors(prev => {
          const newErrors = { ...prev };
          delete newErrors.confirmPassword;
          return newErrors;
        });
      }
    }
    
    // Validate confirmPassword when confirmPassword changes
    if (field === 'confirmPassword') {
      if (value !== values.password) {
        setErrors(prev => ({ ...prev, confirmPassword: 'Passwords do not match' }));
      } else {
        setErrors(prev => {
          const newErrors = { ...prev };
          delete newErrors.confirmPassword;
          return newErrors;
        });
      }
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Basic validation
    const newErrors: Record<string, string> = {};
    if (!values.username) {
      newErrors.username = 'Username is required';
    }
    if (!values.email) {
      newErrors.email = 'Email is required';
    } else if (!/\S+@\S+\.\S+/.test(values.email)) {
      newErrors.email = 'Email is invalid';
    }
    if (!values.password) {
      newErrors.password = 'Password is required';
    } else if (values.password.length < 6) {
      newErrors.password = 'Password must be at least 6 characters';
    }
    if (!values.confirmPassword) {
      newErrors.confirmPassword = 'Confirm password is required';
    } else if (values.password !== values.confirmPassword) {
      newErrors.confirmPassword = 'Passwords do not match';
    }
    if (!values.userType) {
      newErrors.userType = 'User type is required';
    }
    if (!values.acceptTerms) {
      newErrors.acceptTerms = 'You must accept the terms and conditions';
    }
    
    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      return;
    }

    setIsSubmitting(true);
    setErrors({});

    try {
      const result = await register(
        values.username,
        values.email,
        values.password,
        values.confirmPassword,
        values.userType,
        values.acceptTerms,
        false // rememberMe always false for registration
      );
      
      if (result.success) {
        showAlert('success', 'Registration successful! Redirecting to login...');
        setTimeout(() => {
          window.location.href = '/login';
        }, 1000);
      } else {
        showAlert('danger', result.message || 'Registration failed');
      }
    } catch (error) {
      console.error('Registration error:', error);
      showAlert('danger', 'An error occurred during registration');
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <>
      <div className="container mt-5">
        <div className="row justify-content-center">
          <div className="col-md-6 col-lg-5">
            <div className="card shadow">
              <div className="card-body">
                <h3 className="card-title text-center mb-4">Register</h3>
                
                <form onSubmit={handleSubmit}>
                  <div className="mb-3">
                    <label htmlFor="username" className="form-label">Username</label>
                    <input
                      type="text"
                      className={`form-control ${errors.username ? 'is-invalid' : ''}`}
                      id="username"
                      value={values.username}
                      onChange={(e) => handleChange('username', e.target.value)}
                      required
                    />
                    {errors.username && (
                      <div className="invalid-feedback">{errors.username}</div>
                    )}
                  </div>
                  
                  <div className="mb-3">
                    <label htmlFor="email" className="form-label">Email</label>
                    <input
                      type="email"
                      className={`form-control ${errors.email ? 'is-invalid' : ''}`}
                      id="email"
                      value={values.email}
                      onChange={(e) => handleChange('email', e.target.value)}
                      required
                    />
                    {errors.email && (
                      <div className="invalid-feedback">{errors.email}</div>
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
                  
                  <div className="mb-3">
                    <label htmlFor="confirmPassword" className="form-label">Confirm Password</label>
                    <input
                      type="password"
                      className={`form-control ${errors.confirmPassword ? 'is-invalid' : ''}`}
                      id="confirmPassword"
                      value={values.confirmPassword}
                      onChange={(e) => handleChange('confirmPassword', e.target.value)}
                      required
                    />
                    {errors.confirmPassword && (
                      <div className="invalid-feedback">{errors.confirmPassword}</div>
                    )}
                  </div>
                  
                  <div className="mb-3">
                    <label htmlFor="userType" className="form-label">User Type</label>
                    <select
                      className={`form-select ${errors.userType ? 'is-invalid' : ''}`}
                      id="userType"
                      value={values.userType}
                      onChange={(e) => handleChange('userType', e.target.value)}
                      required
                    >
                      <option value="">Select user type</option>
                      <option value={USER_TYPES.END_USER}>End User</option>
                      <option value={USER_TYPES.ADMIN}>Admin</option>
                      <option value={USER_TYPES.PARTNER}>Partner</option>
                    </select>
                    {errors.userType && (
                      <div className="invalid-feedback">{errors.userType}</div>
                    )}
                  </div>
                  
                  <div className="mb-3 form-check">
                    <input
                      type="checkbox"
                      className={`form-check-input ${errors.acceptTerms ? 'is-invalid' : ''}`}
                      id="acceptTerms"
                      checked={values.acceptTerms}
                      onChange={(e) => handleChange('acceptTerms', e.target.checked)}
                      required
                    />
                    <label className="form-check-label" htmlFor="acceptTerms">
                      I accept the{' '}
                      <button
                        type="button"
                        className="btn btn-link p-0"
                        onClick={() => setShowTermsModal(true)}
                      >
                        terms and conditions
                      </button>
                    </label>
                    {errors.acceptTerms && (
                      <div className="invalid-feedback">{errors.acceptTerms}</div>
                    )}
                  </div>
                  
                  <div className="d-grid">
                    <Button
                      type="submit"
                      isLoading={isSubmitting}
                      loadingText="Registering..."
                    >
                      Register
                    </Button>
                  </div>
                </form>
                
                <div className="text-center mt-3">
                  <p className="mb-0">
                    Already have an account? <a href="/login" className="btn btn-link p-0">Login here</a>
                  </p>
                </div>
                
                <AlertContainer alerts={alerts} onRemoveAlert={removeAlert} />
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Terms and Conditions Modal */}
      {showTermsModal && (
        <div className="modal show d-block" tabIndex={-1} style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
          <div className="modal-dialog">
            <div className="modal-content">
              <div className="modal-header">
                <h5 className="modal-title">Terms and Conditions</h5>
                <button
                  type="button"
                  className="btn-close"
                  onClick={() => setShowTermsModal(false)}
                />
              </div>
              <div className="modal-body">
                <p>By using this authentication module, you agree to:</p>
                <ul>
                  <li>Provide accurate information during registration</li>
                  <li>Keep your login credentials secure</li>
                  <li>Use the system responsibly</li>
                  <li>Comply with all applicable laws and regulations</li>
                </ul>
              </div>
              <div className="modal-footer">
                <Button
                  variant="secondary"
                  onClick={() => setShowTermsModal(false)}
                >
                  Close
                </Button>
              </div>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default RegisterForm;
