import React from 'react';

interface LoadingSpinnerProps {
  size?: 'sm' | 'md' | 'lg';
  className?: string;
}

const LoadingSpinner: React.FC<LoadingSpinnerProps> = ({ size = 'md', className = '' }) => {
  const sizeClass = {
    sm: 'spinner-border-sm',
    md: '',
    lg: 'spinner-border-lg',
  }[size];

  return (
    <div className={`spinner-border ${sizeClass} ${className}`} role="status">
      <span className="visually-hidden">Loading...</span>
    </div>
  );
};

export default LoadingSpinner;
