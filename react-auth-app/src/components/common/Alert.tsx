import React from 'react';
import { AlertType } from '../../hooks/useAlert';

interface AlertProps {
  type: AlertType;
  message: string;
  onClose?: () => void;
  id?: string;
}

const Alert: React.FC<AlertProps> = ({ type, message, onClose, id }) => {
  return (
    <div className={`alert alert-${type} alert-dismissible fade show`} role="alert" data-alert-id={id}>
      {message}
      {onClose && (
        <button
          type="button"
          className="btn-close"
          onClick={onClose}
          aria-label="Close"
        />
      )}
    </div>
  );
};

export default Alert;
