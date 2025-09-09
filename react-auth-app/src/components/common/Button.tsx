import React from 'react';
import LoadingSpinner from './LoadingSpinner';

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: 'primary' | 'secondary' | 'success' | 'danger' | 'warning' | 'info' | 'light' | 'dark' | 'outline-primary' | 'outline-secondary' | 'outline-success' | 'outline-danger' | 'outline-warning' | 'outline-info' | 'outline-light' | 'outline-dark';
  size?: 'sm' | 'md' | 'lg';
  isLoading?: boolean;
  loadingText?: string;
  children: React.ReactNode;
}

const Button: React.FC<ButtonProps> = ({
  variant = 'primary',
  size = 'md',
  isLoading = false,
  loadingText,
  children,
  className = '',
  disabled,
  ...props
}) => {
  const sizeClass = {
    sm: 'btn-sm',
    md: '',
    lg: 'btn-lg',
  }[size];

  const isDisabled = disabled || isLoading;

  return (
    <button
      className={`btn btn-${variant} ${sizeClass} ${className}`}
      disabled={isDisabled}
      {...props}
    >
      {isLoading && (
        <LoadingSpinner size="sm" className="me-2" />
      )}
      {isLoading && loadingText ? loadingText : children}
    </button>
  );
};

export default Button;
