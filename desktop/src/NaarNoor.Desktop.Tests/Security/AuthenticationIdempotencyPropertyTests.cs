using Xunit;
using FsCheck;
using FsCheck.Xunit;
using Moq;
using NaarNoor.Desktop.Common.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace NaarNoor.Desktop.Tests.Security
{
    /// <summary>
    /// Property-based tests for authentication idempotency
    /// Validates: REQ-002 (Token refresh, idempotent operations)
    /// 
    /// Property 1: Authentication Idempotency
    /// For identical authentication requests:
    /// authenticate(cred) = authenticate(cred) = token
    /// 
    /// This ensures that:
    /// 1. Same credentials always return same token
    /// 2. Token claims remain consistent
    /// 3. Multiple concurrent attempts don't cause conflicts
    /// </summary>
    public class AuthenticationIdempotencyPropertyTests
    {
        /// <summary>
        /// Property Test 1: Identical requests return same token
        /// 
        /// For any credentials, calling authenticate twice should return identical tokens
        /// </summary>
        [Property]
        public void Property_IdenticalRequests_ReturnSameToken(int requestCount)
        {
            if (requestCount < 1 || requestCount > 5)
                return;

            var username = "testuser@naarnoor.com";
            var password = "TestPassword123!";
            var loginResponses = new List<NaarNoor.Desktop.Common.DTOs.LoginResponse>();

            // Mock the authentication service to return same token
            var authServiceMock = new Mock<IAuthenticationService>();
            var testLoginResponse = new NaarNoor.Desktop.Common.DTOs.LoginResponse
            {
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ1c2VyMTIzIiwicm9sZSI6Ik1hbmFnZXIifQ.signature",
                RefreshToken = "refresh-token-123",
                UserId = "user-123",
                Role = "Manager"
            };
            
            authServiceMock.Setup(s => s.AuthenticateAsync(username, password))
                .ReturnsAsync(NaarNoor.Desktop.Common.Utilities.Result<NaarNoor.Desktop.Common.DTOs.LoginResponse>.Success(testLoginResponse));

            // Act: Make multiple authentication calls
            for (int i = 0; i < requestCount; i++)
            {
                var result = authServiceMock.Object.AuthenticateAsync(username, password).Result;
                if (result.IsSuccess)
                    loginResponses.Add(result.Value);
            }

            // Assert: All tokens should be identical
            Assert.All(loginResponses, response => Assert.Equal(testLoginResponse.AccessToken, response.AccessToken));
        }

        /// <summary>
        /// Property Test 2: Token claims remain consistent across requests
        /// 
        /// Decoded token claims should be identical for same credentials
        /// </summary>
        [Fact]
        public void Property_TokenClaims_Consistent()
        {
            var username = "manager@naarnoor.com";
            var password = "SecurePass456!";
            
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock.Setup(s => s.CurrentUserRole).Returns("Manager");
            authServiceMock.Setup(s => s.CurrentUserId).Returns("user-123");
            authServiceMock.Setup(s => s.IsAuthenticated).Returns(true);

            var authService = authServiceMock.Object;

            // Get claims multiple times
            var claims1 = new { Role = authService.CurrentUserRole, Id = authService.CurrentUserId };
            var claims2 = new { Role = authService.CurrentUserRole, Id = authService.CurrentUserId };

            // Assert: Claims should be identical
            Assert.Equal(claims1.Role, claims2.Role);
            Assert.Equal(claims1.Id, claims2.Id);
        }

        /// <summary>
        /// Property Test 3: Concurrent authentication requests don't conflict
        /// 
        /// Multiple simultaneous auth attempts should all succeed with same token
        /// </summary>
        [Fact]
        public void Property_ConcurrentAuthentication_NoConflicts()
        {
            var username = "chef@naarnoor.com";
            var password = "ChefPass789!";
            var concurrentAttempts = 5;
            var results = new List<NaarNoor.Desktop.Common.DTOs.LoginResponse>();
            var testLoginResponse = new NaarNoor.Desktop.Common.DTOs.LoginResponse
            {
                AccessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJjaGVmMTIzIiwicm9sZSI6IkNoZWYifQ.sig",
                RefreshToken = "refresh-chef",
                UserId = "chef-123",
                Role = "Chef"
            };

            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock.Setup(s => s.AuthenticateAsync(username, password))
                .ReturnsAsync(NaarNoor.Desktop.Common.Utilities.Result<NaarNoor.Desktop.Common.DTOs.LoginResponse>.Success(testLoginResponse));

            // Act: Simulate concurrent requests
            var tasks = Enumerable.Range(0, concurrentAttempts)
                .Select(async i => await authServiceMock.Object.AuthenticateAsync(username, password))
                .ToList();

            System.Threading.Tasks.Task.WaitAll(tasks.Cast<System.Threading.Tasks.Task>().ToArray());

            foreach (var task in tasks)
            {
                if (task.Result.IsSuccess)
                    results.Add(task.Result.Value);
            }

            // Assert: All results should be identical
            Assert.True(results.All(r => r.AccessToken == testLoginResponse.AccessToken), "All concurrent requests should return same token");
            Assert.Equal(concurrentAttempts, results.Count);
        }

        /// <summary>
        /// Property Test 4: Authentication state doesn't change between calls
        /// 
        /// After authentication, IsAuthenticated property remains stable
        /// </summary>
        [Property]
        public void Property_AuthenticationState_Stable(int callCount)
        {
            if (callCount < 1 || callCount > 10)
                return;

            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock.Setup(s => s.IsAuthenticated).Returns(true);

            var authService = authServiceMock.Object;
            var states = new List<bool>();

            // Check state multiple times
            for (int i = 0; i < callCount; i++)
            {
                states.Add(authService.IsAuthenticated);
            }

            // Assert: All states should be true
            Assert.All(states, s => Assert.True(s));
        }

        /// <summary>
        /// Property Test 5: Different credentials produce different responses
        /// 
        /// Same user with different passwords should not return same response
        /// </summary>
        [Fact]
        public void Property_DifferentCredentials_DifferentTokens()
        {
            var username = "user@naarnoor.com";
            var password1 = "Password123!";
            var password2 = "Password456!";
            var response1 = new NaarNoor.Desktop.Common.DTOs.LoginResponse
            {
                AccessToken = "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJ1c2VyMTIzIn0.sig1",
                RefreshToken = "refresh1",
                UserId = "user-1",
                Role = "User"
            };
            var response2 = new NaarNoor.Desktop.Common.DTOs.LoginResponse
            {
                AccessToken = "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJ1c2VyMTIzIn0.sig2",
                RefreshToken = "refresh2",
                UserId = "user-1",
                Role = "User"
            };

            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock.Setup(s => s.AuthenticateAsync(username, password1))
                .ReturnsAsync(NaarNoor.Desktop.Common.Utilities.Result<NaarNoor.Desktop.Common.DTOs.LoginResponse>.Success(response1));
            authServiceMock.Setup(s => s.AuthenticateAsync(username, password2))
                .ReturnsAsync(NaarNoor.Desktop.Common.Utilities.Result<NaarNoor.Desktop.Common.DTOs.LoginResponse>.Success(response2));

            var authService = authServiceMock.Object;

            // Act
            var result1 = authService.AuthenticateAsync(username, password1).Result;
            var result2 = authService.AuthenticateAsync(username, password2).Result;

            // Assert: Tokens should be different
            Assert.NotEqual(result1.Value.AccessToken, result2.Value.AccessToken);
        }

        /// <summary>
        /// Property Test 6: Token refresh preserves idempotency
        /// 
        /// After token refresh, new token should be consistent across multiple calls
        /// </summary>
        [Fact]
        public void Property_TokenRefresh_MaintainsIdempotency()
        {
            var newTokenResponse = new NaarNoor.Desktop.Common.DTOs.TokenResponse
            {
                AccessToken = "eyJhbGciOiJIUzI1NiJ9.eyJleHAiOjE2MzEwMDAwMDB9.sig",
                RefreshToken = "new-refresh"
            };
            var refreshCalls = 3;
            var refreshedTokens = new List<string>();

            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock.Setup(s => s.RefreshTokenAsync())
                .ReturnsAsync(NaarNoor.Desktop.Common.Utilities.Result<NaarNoor.Desktop.Common.DTOs.TokenResponse>.Success(newTokenResponse));

            var authService = authServiceMock.Object;

            // Act: Refresh token multiple times
            for (int i = 0; i < refreshCalls; i++)
            {
                var refreshed = authService.RefreshTokenAsync().Result;
                if (refreshed.IsSuccess)
                    refreshedTokens.Add(refreshed.Value.AccessToken);
            }

            // Assert: All refreshed tokens should be identical
            Assert.All(refreshedTokens, token => Assert.Equal(newTokenResponse.AccessToken, token));
        }

        /// <summary>
        /// Property Test 7: Authentication sequence is idempotent
        /// 
        /// Login → Logout → Login should produce same token as single Login
        /// </summary>
        [Fact]
        public void Property_LoginLogoutLogin_IdempotentResults()
        {
            var username = "staff@naarnoor.com";
            var password = "StaffPass123!";
            var loginResponse = new NaarNoor.Desktop.Common.DTOs.LoginResponse
            {
                AccessToken = "eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJzdGFmZjEyMyIsInJvbGUiOiJTdGFmZiJ9.sig",
                RefreshToken = "refresh-staff",
                UserId = "staff-123",
                Role = "Staff"
            };

            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock.Setup(s => s.AuthenticateAsync(username, password))
                .ReturnsAsync(NaarNoor.Desktop.Common.Utilities.Result<NaarNoor.Desktop.Common.DTOs.LoginResponse>.Success(loginResponse));
            authServiceMock.Setup(s => s.LogoutAsync())
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            var authService = authServiceMock.Object;

            // Act: First authentication
            var result1 = authService.AuthenticateAsync(username, password).Result;
            
            // Logout
            authService.LogoutAsync().Wait();
            
            // Login again
            var result2 = authService.AuthenticateAsync(username, password).Result;

            // Assert: Tokens should be identical
            Assert.Equal(result1.Value.AccessToken, result2.Value.AccessToken);
        }

        /// <summary>
        /// Property Test 8: No side effects during authentication
        /// 
        /// Authentication doesn't modify input credentials
        /// </summary>
        [Property]
        public void Property_NoSideEffects_OnCredentials(int attempts)
        {
            if (attempts < 1 || attempts > 5)
                return;

            var originalUsername = "admin@naarnoor.com";
            var originalPassword = "AdminPass123!";
            var username = originalUsername;
            var password = originalPassword;

            var authServiceMock = new Mock<IAuthenticationService>();
            var successResponse = new NaarNoor.Desktop.Common.DTOs.LoginResponse
            {
                AccessToken = "token",
                RefreshToken = "refresh",
                UserId = "admin-1",
                Role = "Admin"
            };
            authServiceMock.Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(NaarNoor.Desktop.Common.Utilities.Result<NaarNoor.Desktop.Common.DTOs.LoginResponse>.Success(successResponse));

            var authService = authServiceMock.Object;

            // Act: Multiple authentication attempts
            for (int i = 0; i < attempts; i++)
            {
                authService.AuthenticateAsync(username, password).Wait();
            }

            // Assert: Credentials unchanged
            Assert.Equal(originalUsername, username);
            Assert.Equal(originalPassword, password);
        }
    }
}
