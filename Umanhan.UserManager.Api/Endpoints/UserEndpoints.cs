using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Runtime.Internal;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update;
using System.Data;
using Umanhan.Models.Dtos;
using Umanhan.Models.Entities;
using Umanhan.Services;
using Umanhan.Services.Interfaces;

namespace Umanhan.UserManager.Api.Endpoints
{
    public class UserEndpoints
    {
        private readonly RoleService _roleService;
        private readonly ILogger<UserEndpoints> _logger;
        //private readonly ICacheService _cacheService;
        private readonly IEmailService _emailService;

        private const string MODULE_CACHE_KEY = "user";

        public UserEndpoints(RoleService roleService, 
            ILogger<UserEndpoints> logger,
            IEmailService emailService)
        {
            _roleService = roleService;
            _logger = logger;
            //_cacheService = cacheService;
            _emailService = emailService;
        }

        private string GenerateTemporaryPassword()
        {
            const int PasswordLength = 12; // Can be 8–99, depending on policy

            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$%^&*()-_=+[]{}|;:,.<>?";

            var random = new Random();
            var all = upper + lower + digits + special;

            // Ensure at least one character from each required group
            var password = new List<char>
            {
                upper[random.Next(upper.Length)],
                lower[random.Next(lower.Length)],
                digits[random.Next(digits.Length)],
                special[random.Next(special.Length)]
            };

            // Fill the rest of the password with random characters
            for (int i = password.Count; i < PasswordLength; i++)
            {
                password.Add(all[random.Next(all.Length)]);
            }

            // Shuffle the result to avoid predictable patterns
            return new string(password.OrderBy(x => random.Next()).ToArray());
        }

        public async Task<IResult> GetAllUsersAsync(IAmazonCognitoIdentityProvider cognitoClient, string userPoolId)
        {
            var result = new List<UserType>();
            string token = null;

            do
            {
                try
                {
                    var response = await cognitoClient.ListUsersAsync(new ListUsersRequest
                    {
                        UserPoolId = userPoolId,
                        Limit = 60,
                        PaginationToken = token
                    }).ConfigureAwait(false);

                    result.AddRange(response.Users);
                    token = response.PaginationToken;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error fetching users from Cognito User Pool {UserPoolId}", userPoolId);
                    break;
                }
            } while (!string.IsNullOrEmpty(token));

            try
            {
                var localGroups = await _roleService.GetAllRolesAsync(false).ConfigureAwait(false);
                var userDtos = await System.Threading.Tasks.Task.WhenAll(
                    result.Select(async cognitoUser =>
                    {
                        // Build base DTO
                        var dto = new UserDto
                        {
                            Username = cognitoUser.Username,
                            Status = cognitoUser.UserStatus?.Value,
                            IsActive = cognitoUser.Enabled,
                            Email = cognitoUser.Attributes
                                                 .FirstOrDefault(a => a.Name == "email")?.Value,
                            PhoneNumber = cognitoUser.Attributes
                                                 .FirstOrDefault(a => a.Name == "phone_number")?.Value,
                            EmailVerified = cognitoUser.Attributes
                                                 .FirstOrDefault(a => a.Name == "email_verified")?.Value,
                            FirstName = cognitoUser.Attributes
                                                 .FirstOrDefault(a => a.Name == "given_name")?.Value,
                            LastName = cognitoUser.Attributes
                                                 .FirstOrDefault(a => a.Name == "family_name")?.Value
                        };

                        try
                        {
                            // Fetch their Cognito groups
                            var grpResp = await cognitoClient.AdminListGroupsForUserAsync(
                                new AdminListGroupsForUserRequest
                                {
                                    Username = dto.Username,
                                    UserPoolId = userPoolId
                                }).ConfigureAwait(false);

                            // Map each Cognito group to your local RoleDto
                            dto.Roles = grpResp.Groups
                                .Select(g =>
                                {
                                    // Find matching local role (or null)
                                    var local = localGroups.FirstOrDefault(r => r.GroupName == g.GroupName);

                                    return new RoleDto
                                    {
                                        RoleId = local?.RoleId ?? Guid.Empty,
                                        GroupName = g.GroupName
                                    };
                                })
                                .ToList();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error fetching groups for {dto.Username}: {ex.Message}");
                            dto.Roles = new List<RoleDto>(); // fallback empty
                        }

                        return dto;
                    })
                ).ConfigureAwait(false);

                return Results.Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing users from Cognito User Pool {UserPoolId}", userPoolId);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> GetUserByEmailAsync(IAmazonCognitoIdentityProvider cognitoClient, string email, string userPoolId)
        {
            var request = new ListUsersRequest
            {
                UserPoolId = userPoolId,
                Filter = $"email = \"{email}\"",
                Limit = 1
            };

            try
            {
                var response = await cognitoClient.ListUsersAsync(request).ConfigureAwait(false);
                var user = response.Users.FirstOrDefault();
                return Results.Ok(new UserDto
                {
                    Username = user?.Username,
                    Status = user?.UserStatus?.Value,
                    IsActive = user?.Enabled ?? false,
                    Email = user?.Attributes.FirstOrDefault(a => a.Name == "email")?.Value,
                    PhoneNumber = user?.Attributes.FirstOrDefault(a => a.Name == "phone_number")?.Value,
                    EmailVerified = user?.Attributes.FirstOrDefault(a => a.Name == "email_verified")?.Value,
                    FirstName = user?.Attributes.FirstOrDefault(a => a.Name == "given_name")?.Value,
                    LastName = user?.Attributes.FirstOrDefault(a => a.Name == "family_name")?.Value,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by email {Email}", email);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> CreateCognitoUserAsync(IAmazonCognitoIdentityProvider cognitoClient, UserDto user, string userPoolId)
        {
            string temporaryPassword = GenerateTemporaryPassword();
            var request = new AdminCreateUserRequest
            {
                UserPoolId = userPoolId,
                Username = user.Username,
                TemporaryPassword = temporaryPassword,
                UserAttributes = new List<AttributeType>
                    {
                        new() {
                            Name = "email",
                            Value = user.Email
                        },
                        new() {
                            Name = "given_name",
                            Value = user.FirstName
                        },
                        new() {
                            Name = "family_name",
                            Value = user.LastName
                        },
                    },
                //MessageAction = MessageActionType.SUPPRESS,
                DesiredDeliveryMediums = ["EMAIL"],
            };

            try
            {
                var response = await cognitoClient.AdminCreateUserAsync(request).ConfigureAwait(false);

                // send email
                _ = _emailService.SendWelcomeMessageEmailAsync(user.Email, user.FirstName);

                return Results.Ok(new { Message = "User created successfully", UserStatus = response.User?.UserStatus });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Cognito user {Username}", user.Username);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> AddCognitoUserToGroupsAsync(string username, IAmazonCognitoIdentityProvider cognitoClient, IEnumerable<string> groups, string userPoolId)
        {
            if (!groups.Any())
                return Results.BadRequest("No groups provided to add user to.");

            var addedToGroups = new List<string>();
            var failedGroups = new List<string>();
            var removedFromGroups = new List<string>();
            var groupsToRemoveFrom = new List<string>();
            var failedGroupsToRemoveFrom = new List<string>();

            try
            {
                // get all groups first
                var roles = new List<GroupType>();
                string nextToken = null;

                do
                {
                    var request = new ListGroupsRequest
                    {
                        UserPoolId = userPoolId,
                        Limit = 60,
                        NextToken = nextToken
                    };

                    var response = await cognitoClient.ListGroupsAsync(request).ConfigureAwait(false);
                    roles.AddRange(response.Groups);
                    nextToken = response.NextToken;
                } while (!string.IsNullOrEmpty(nextToken));

                // validate if the groups from the client exist
                var validGroupNames = new HashSet<string>(roles.Select(r => r.GroupName), StringComparer.OrdinalIgnoreCase);
                var invalidGroups = groups.Where(g => !validGroupNames.Contains(g)).ToList();

                // get the groups where the user id about to be removed from
                var targetGroups = new HashSet<string>(groups, StringComparer.OrdinalIgnoreCase);
                groupsToRemoveFrom = validGroupNames.Where(g => !targetGroups.Contains(g))
                                                    .Select(g => g)
                                                    .ToList();

                // remove user from groups
                foreach (var groupName in groupsToRemoveFrom)
                {
                    try
                    {
                        var response = await cognitoClient.AdminRemoveUserFromGroupAsync(new AdminRemoveUserFromGroupRequest
                        {
                            UserPoolId = userPoolId,
                            Username = username,
                            GroupName = groupName
                        });

                        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            removedFromGroups.Add(groupName);
                        }
                    }
                    catch (Exception ex)
                    {
                        failedGroupsToRemoveFrom.Add(groupName);
                    }
                }

                // get the groups the user is currently in
                var currentGroups = await cognitoClient.AdminListGroupsForUserAsync(new AdminListGroupsForUserRequest
                {
                    Username = username,
                    UserPoolId = userPoolId
                }).ConfigureAwait(false);

                // remove current groups from the groups to add to avoid readding them
                var currentGroupNames = new HashSet<string>(currentGroups.Groups.Select(g => g.GroupName), StringComparer.OrdinalIgnoreCase);
                var groupsToAdd = groups.Except(currentGroupNames, StringComparer.OrdinalIgnoreCase).ToList();

                // then add to reselected or new groups
                foreach (var groupName in groupsToAdd)
                {
                    var request = new AdminAddUserToGroupRequest
                    {
                        UserPoolId = userPoolId,
                        Username = username,
                        GroupName = groupName
                    };

                    try
                    {
                        await cognitoClient.AdminAddUserToGroupAsync(request).ConfigureAwait(false);
                        addedToGroups.Add(groupName);
                    }
                    catch (Exception ex)
                    {
                        failedGroups.Add(groupName);
                    }
                }

                // then force logout user
                if (removedFromGroups.Any() || addedToGroups.Any())
                {
                    await cognitoClient.AdminUserGlobalSignOutAsync(new AdminUserGlobalSignOutRequest
                    {
                        UserPoolId = userPoolId,
                        Username = username
                    });
                }

                return Results.Ok(new
                {
                    Message = $"Processed add to groups.",
                    Added = addedToGroups,
                    FailedAdd = failedGroups,
                    Removed = removedFromGroups,
                    FailedRemove = failedGroupsToRemoveFrom
                });
            }
            catch (Exception)
            {
                _logger.LogError("An error occurred while updating groups for user {Username}", username);
                string msg = $"An error occurred while updating groups for user '{username}'.";

                return Results.Problem(msg);
            }
        }

        public async Task<IResult> UpdateCognitoUserDetailsAsync(string username, IAmazonCognitoIdentityProvider cognitoClient, UserDto user, string userPoolId)
        {
            var request = new AdminUpdateUserAttributesRequest
            {
                UserPoolId = userPoolId,
                Username = username,
                UserAttributes = new List<AttributeType>()
                {
                    //new AttributeType { Name = "phone_number", Value = user.PhoneNumber },
                    new() { Name = "given_name", Value = user.FirstName },
                    new() { Name = "family_name", Value = user.LastName },
                    // Other attributes as needed
                }
            };

            try
            {
                await cognitoClient.AdminUpdateUserAttributesAsync(request).ConfigureAwait(false);
                return Results.Ok(new { Message = $"User '{username}' attributes updated" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Cognito user {Username}", username);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> DisableCognitoUserAsync(string username, IAmazonCognitoIdentityProvider cognitoClient, string userPoolId)
        {
            var request = new AdminDisableUserRequest
            {
                UserPoolId = userPoolId,
                Username = username
            };

            try
            {
                var response = await cognitoClient.AdminDisableUserAsync(request).ConfigureAwait(false);
                return Results.Ok(new { Message = $"User '{username}' has been disabled" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling Cognito user {Username}", username);
                return Results.Problem(ex.Message);
            }
        }

        public async Task<IResult> LogoutCognitoUsersAsync(string groupName, IAmazonCognitoIdentityProvider cognitoClient, string userPoolId)
        {
            try
            {
                var userResponse = await cognitoClient.ListUsersInGroupAsync(new ListUsersInGroupRequest
                {
                    GroupName = groupName,
                    UserPoolId = userPoolId
                });

                var signedOutUsers = new List<string>();
                var failedUsers = new List<string>();

                foreach (var user in userResponse.Users)
                {
                    try
                    {
                        await cognitoClient.AdminUserGlobalSignOutAsync(new AdminUserGlobalSignOutRequest
                        {
                            UserPoolId = userPoolId,
                            Username = user.Username
                        });
                        signedOutUsers.Add(user.Username);
                    }
                    catch (Exception ex)
                    {
                        failedUsers.Add(user.Username);
                    }
                }

                return Results.Ok(new
                {
                    Message = $"Processed logout for group '{groupName}'.",
                    SignedOut = signedOutUsers,
                    Failed = failedUsers
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing group logout for {GroupName}", groupName);
                string msg = "An error occurred while processing the group logout.";
                return Results.Problem(msg);
            }
        }

        public async Task<IResult> LogoutCognitoUserAsync(string username, IAmazonCognitoIdentityProvider cognitoClient, string userPoolId)
        {
            try
            {
                try
                {
                    var response = await cognitoClient.AdminUserGlobalSignOutAsync(new AdminUserGlobalSignOutRequest
                    {
                        UserPoolId = userPoolId,
                        Username = username
                    });

                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        // force logout user
                        await LogoutCognitoUserAsync(username, cognitoClient, userPoolId).ConfigureAwait(false);

                        return Results.Ok($"Processed logout for user '{username}'.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while logging out user {Username}", username);
                    return Results.Problem($"Unable to process request.");
                }
                return Results.Problem($"Failed logging out user '{username}'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing user logout for {Username}", username);
                string msg = "An error occurred while processing the user logout request.";

                return Results.Problem(msg);
            }
        }

        public async Task<IResult> RemoveCognitoUserFromGroupAsync(string username, string groupName, IAmazonCognitoIdentityProvider cognitoClient, string userPoolId)
        {
            try
            {
                try
                {
                    var response = await cognitoClient.AdminRemoveUserFromGroupAsync(new AdminRemoveUserFromGroupRequest
                    {
                        UserPoolId = userPoolId,
                        Username = username,
                        GroupName = groupName
                    });

                    if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return Results.Ok($"Processed removal from group for user '{username}'.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while removing user {Username} from group {GroupName}", username, groupName);
                    return Results.Problem($"Unable to process request.");
                }
                return Results.Problem($"Failed removing user '{username}' from group.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing removal of user {Username} from group {GroupName}", username, groupName);
                string msg = "An error occurred while processing the removal of user from the group.";

                return Results.Problem(msg);
            }
        }
    }
}
