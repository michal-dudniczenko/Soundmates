using Microsoft.AspNetCore.Mvc;
using Soundmates.Domain.Entities;
using Soundmates.Domain.Interfaces.Repositories;
using System.IdentityModel.Tokens.Jwt;

namespace Soundmates.Api.Extensions;

public static class ControllerExtensions
{
    public static async Task<User?> GetAuthorizedUserAsync(
        this ControllerBase controller,
        IUserRepository userRepository,
        bool checkForFirstLogin = true)
    {
        var subClaim = controller.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (subClaim is null || !int.TryParse(subClaim, out var authorizedUserId))
        {
            return null;
        }

        var user = await userRepository.GetByIdAsync(authorizedUserId);
        if (user == null || user.IsLoggedOut || (checkForFirstLogin && user.IsFirstLogin))
        {
            return null;
        }

        return user;
    } 
}
