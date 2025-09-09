import React from 'react';
import Alert from './Alert';
import { Alert as AlertType } from '../../hooks/useAlert';

interface AlertContainerProps {
  alerts: AlertType[];
  onRemoveAlert: (id: string) => void;
}

const AlertContainer: React.FC<AlertContainerProps> = ({ alerts, onRemoveAlert }) => {
  if (alerts.length === 0) return null;

  return (
    <div className="alert-container">
      {alerts.map((alert) => (
        <Alert
          key={alert.id}
          type={alert.type}
          message={alert.message}
          id={alert.id}
          onClose={() => alert.id && onRemoveAlert(alert.id)}
        />
      ))}
    </div>
  );
};

export default AlertContainer;
