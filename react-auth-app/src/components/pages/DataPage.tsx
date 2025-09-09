import React, { useState, useEffect } from 'react';
import { authService } from '../../services/authService';
import { User } from '../../types';
import { useAlert } from '../../hooks/useAlert';
import LoadingSpinner from '../common/LoadingSpinner';
import AlertContainer from '../common/AlertContainer';

const DataPage: React.FC = () => {
  const [users, setUsers] = useState<User[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const { alerts, showAlert, removeAlert } = useAlert();

  useEffect(() => {
    loadUsers();
  }, []);


  const loadUsers = async () => {
    setIsLoading(true);
    try {
      const result = await authService.getUsers();
      if (result.success && result.data) {
        setUsers(result.data);
      } else {
        showAlert('danger', 'Failed to load users: ' + (result.error || 'Unknown error'));
      }
    } catch (error) {
      showAlert('danger', 'An error occurred while loading users');
    } finally {
      setIsLoading(false);
    }
  };



  return (
    <div className="container mt-5">
      <div className="row">
        <div className="col-12">
          <div className="card">
            <div className="card-header">
              <h2 className="card-title mb-0">Database Data Viewer</h2>
            </div>
            <div className="card-body">

              {/* Users Table */}
              <div className="table-responsive">
                <table className="table table-striped table-hover">
                  <thead className="table-dark">
                    <tr>
                      <th>ID</th>
                      <th>Username</th>
                      <th>Email</th>
                      <th>User Type</th>
                      <th>Created At</th>
                      <th>Updated At</th>
                    </tr>
                  </thead>
                  <tbody>
                    {isLoading ? (
                      <tr>
                        <td colSpan={6} className="text-center">
                          <LoadingSpinner />
                        </td>
                      </tr>
                    ) : users.length > 0 ? (
                      users.map((user) => (
                        <tr key={user.id}>
                          <td>{user.id}</td>
                          <td>{user.username}</td>
                          <td>{user.email}</td>
                          <td>
                            <span className={`badge ${user.userType === 'Admin' ? 'bg-danger' : 'bg-primary'}`}>
                              {user.userType}
                            </span>
                          </td>
                          <td>{new Date(user.createdAt).toLocaleString()}</td>
                          <td>{new Date(user.updatedAt).toLocaleString()}</td>
                        </tr>
                      ))
                    ) : (
                      <tr>
                        <td colSpan={6} className="text-center text-muted">No users found</td>
                      </tr>
                    )}
                  </tbody>
                </table>
              </div>

              <AlertContainer alerts={alerts} onRemoveAlert={removeAlert} />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default DataPage;
