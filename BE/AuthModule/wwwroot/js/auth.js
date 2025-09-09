// Authentication utility functions
class AuthManager {
    constructor() {
        this.tokenKey = 'authToken';
        this.refreshTokenKey = 'refreshToken';
        this.init();
    }

    init() {
        // Check token on page load
        this.checkAuthStatus();
        
        // Add event listeners for form submissions
        this.setupFormHandlers();
    }

    checkAuthStatus() {
        const token = this.getToken();
        const currentPath = window.location.pathname.toLowerCase();
        
        // Skip check for public pages
        const publicPages = [
            '/account/login',
            '/account/register',
            '/home/index',
            '/',
            '/favicon.ico'
        ];

        if (publicPages.some(page => currentPath.startsWith(page))) {
            return;
        }

        // If no token and not on public page, redirect to login
        if (!token) {
            this.redirectToLogin();
            return;
        }

        // Validate token with server
        this.validateToken(token);
    }

    getToken() {
        return localStorage.getItem(this.tokenKey);
    }

    getRefreshToken() {
        return localStorage.getItem(this.refreshTokenKey);
    }

    setToken(token) {
        localStorage.setItem(this.tokenKey, token);
    }

    setRefreshToken(refreshToken) {
        localStorage.setItem(this.refreshTokenKey, refreshToken);
    }

    setTokens(token, refreshToken) {
        this.setToken(token);
        this.setRefreshToken(refreshToken);
    }

    removeToken() {
        localStorage.removeItem(this.tokenKey);
    }

    removeRefreshToken() {
        localStorage.removeItem(this.refreshTokenKey);
    }

    removeTokens() {
        this.removeToken();
        this.removeRefreshToken();
    }

    async validateToken(token) {
        try {
            const response = await this.apiCall('/api/auth/validate', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(token)
            });

            const result = await response.json();
            
            if (!result.success) {
                this.removeTokens();
                this.redirectToLogin();
            }
        } catch (error) {
            console.error('Token validation error:', error);
            this.removeTokens();
            this.redirectToLogin();
        }
    }

    async refreshToken() {
        const refreshToken = this.getRefreshToken();
        if (!refreshToken) {
            return false;
        }

        try {
            const response = await fetch('/api/auth/refresh', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(refreshToken)
            });

            const result = await response.json();
            
            if (result.success) {
                this.setTokens(result.token, result.refreshToken);
                console.log('Token refreshed successfully');
                return true;
            } else {
                console.error('Token refresh failed:', result.message);
                this.removeTokens();
                return false;
            }
        } catch (error) {
            console.error('Token refresh error:', error);
            this.removeTokens();
            return false;
        }
    }

    redirectToLogin() {
        const currentPath = window.location.pathname;
        const loginUrl = `/Account/Login?returnUrl=${encodeURIComponent(currentPath)}`;
        window.location.href = loginUrl;
    }

    setupFormHandlers() {
        // Handle login form
        const loginForm = document.getElementById('loginForm');
        if (loginForm) {
            loginForm.addEventListener('submit', (e) => this.handleLogin(e));
        }

        // Handle register form
        const registerForm = document.getElementById('registerForm');
        if (registerForm) {
            registerForm.addEventListener('submit', (e) => this.handleRegister(e));
        }

        // Handle logout
        const logoutBtn = document.getElementById('logoutBtn');
        if (logoutBtn) {
            logoutBtn.addEventListener('click', (e) => this.handleLogout(e));
        }
    }

    async handleLogin(e) {
        e.preventDefault();
        
        const form = e.target;
        const loginBtn = document.getElementById('loginBtn');
        const spinner = loginBtn?.querySelector('.spinner-border');
        const alertContainer = document.getElementById('alertContainer');
        
        // Clear previous alerts
        if (alertContainer) {
            alertContainer.innerHTML = '';
        }
        
        // Show loading state
        if (loginBtn) {
            loginBtn.disabled = true;
            if (spinner) {
                spinner.classList.remove('d-none');
            }
        }
        
        try {
            const formData = new FormData(form);
            const data = {
                usernameOrEmail: formData.get('usernameOrEmail'),
                password: formData.get('password')
            };
            
            const response = await fetch('/api/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data)
            });
            
            const result = await response.json();
            
            if (result.success) {
                this.setTokens(result.token, result.refreshToken);
                this.showAlert('success', 'Login successful! Redirecting...', alertContainer);
                
                // Update navigation bar
                if (typeof updateNavigationBar === 'function') {
                    updateNavigationBar();
                }
                
                // Redirect to return URL or home
                const urlParams = new URLSearchParams(window.location.search);
                const returnUrl = urlParams.get('returnUrl') || '/Home';
                setTimeout(() => {
                    window.location.href = returnUrl;
                }, 1000);
            } else {
                this.showAlert('danger', result.message || 'Login failed', alertContainer);
            }
        } catch (error) {
            console.error('Login error:', error);
            this.showAlert('danger', 'An error occurred during login', alertContainer);
        } finally {
            // Hide loading state
            if (loginBtn) {
                loginBtn.disabled = false;
                if (spinner) {
                    spinner.classList.add('d-none');
                }
            }
        }
    }

    async handleRegister(e) {
        e.preventDefault();
        
        const form = e.target;
        const registerBtn = document.getElementById('registerBtn');
        const spinner = registerBtn?.querySelector('.spinner-border');
        const alertContainer = document.getElementById('alertContainer');
        
        // Clear previous alerts
        if (alertContainer) {
            alertContainer.innerHTML = '';
        }
        
        // Validate form
        if (!this.validateForm(form)) {
            return;
        }
        
        // Show loading state
        if (registerBtn) {
            registerBtn.disabled = true;
            if (spinner) {
                spinner.classList.remove('d-none');
            }
        }
        
        try {
            const formData = new FormData(form);
            const data = {
                username: formData.get('username'),
                email: formData.get('email'),
                password: formData.get('password'),
                confirmPassword: formData.get('confirmPassword'),
                userType: formData.get('userType'),
                acceptTerms: formData.get('acceptTerms') === 'on'
            };
            
            const response = await fetch('/api/auth/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data)
            });
            
            const result = await response.json();
            
            if (result.success) {
                this.showAlert('success', 'Registration successful! Redirecting to login...', alertContainer);
                
                setTimeout(() => {
                    window.location.href = '/Account/Login';
                }, 1000);
            } else {
                this.showAlert('danger', result.message || 'Registration failed', alertContainer);
            }
        } catch (error) {
            console.error('Registration error:', error);
            this.showAlert('danger', 'An error occurred during registration', alertContainer);
        } finally {
            // Hide loading state
            if (registerBtn) {
                registerBtn.disabled = false;
                if (spinner) {
                    spinner.classList.add('d-none');
                }
            }
        }
    }

    async handleLogout(e) {
        e.preventDefault();
        
        const token = this.getToken();
        if (token) {
            try {
                await fetch('/api/auth/logout', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(token)
                });
            } catch (error) {
                console.error('Logout error:', error);
            }
        }
        
        this.removeTokens();
        
        // Update navigation bar
        if (typeof updateNavigationBar === 'function') {
            updateNavigationBar();
        }
        
        window.location.href = '/Account/Login';
    }

    validateForm(form) {
        const password = form.querySelector('#password')?.value;
        const confirmPassword = form.querySelector('#confirmPassword')?.value;
        
        // Clear previous validation
        form.classList.remove('was-validated');
        
        // Check password match
        if (password !== confirmPassword) {
            const confirmPasswordField = form.querySelector('#confirmPassword');
            if (confirmPasswordField) {
                confirmPasswordField.setCustomValidity('Passwords do not match');
            }
            form.classList.add('was-validated');
            return false;
        } else {
            const confirmPasswordField = form.querySelector('#confirmPassword');
            if (confirmPasswordField) {
                confirmPasswordField.setCustomValidity('');
            }
        }
        
        // Check if form is valid
        if (!form.checkValidity()) {
            form.classList.add('was-validated');
            return false;
        }
        
        return true;
    }

    showAlert(type, message, container) {
        if (!container) return;
        
        container.innerHTML = `
            <div class="alert alert-${type} alert-dismissible fade show" role="alert">
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;
    }

    // API call with automatic token refresh
    async apiCall(url, options = {}) {
        const token = this.getToken();
        
        // Add authorization header if token exists
        if (token) {
            options.headers = {
                ...options.headers,
                'Authorization': `Bearer ${token}`
            };
        }

        try {
            const response = await fetch(url, options);
            
            // If unauthorized and we have a refresh token, try to refresh
            if (response.status === 401 && this.getRefreshToken()) {
                const refreshSuccess = await this.refreshToken();
                if (refreshSuccess) {
                    // Retry the request with new token
                    const newToken = this.getToken();
                    options.headers = {
                        ...options.headers,
                        'Authorization': `Bearer ${newToken}`
                    };
                    return await fetch(url, options);
                } else {
                    // Refresh failed, redirect to login
                    this.removeTokens();
                    this.redirectToLogin();
                    throw new Error('Authentication failed');
                }
            }
            
            return response;
        } catch (error) {
            console.error('API call error:', error);
            throw error;
        }
    }
}

// Global auth manager instance
let authManager;

// Initialize auth manager when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    authManager = new AuthManager();
    // Make it globally available
    window.authManager = authManager;
});

// Export for use in other scripts
if (typeof module !== 'undefined' && module.exports) {
    module.exports = AuthManager;
}
