import { useState, useCallback } from 'react';

export type AlertType = 'success' | 'danger' | 'warning' | 'info';

export interface Alert {
  type: AlertType;
  message: string;
  id?: string;
}

export const useAlert = () => {
  const [alerts, setAlerts] = useState<Alert[]>([]);

  const showAlert = useCallback((type: AlertType, message: string) => {
    const id = Date.now().toString();
    const newAlert: Alert = { type, message, id };
    
    setAlerts(prev => [...prev, newAlert]);
    
    // Auto remove alert after 5 seconds
    setTimeout(() => {
      removeAlert(id);
    }, 5000);
  }, []);

  const removeAlert = useCallback((id: string) => {
    setAlerts(prev => prev.filter(alert => alert.id !== id));
  }, []);

  const clearAlerts = useCallback(() => {
    setAlerts([]);
  }, []);

  return {
    alerts,
    showAlert,
    removeAlert,
    clearAlerts,
  };
};
