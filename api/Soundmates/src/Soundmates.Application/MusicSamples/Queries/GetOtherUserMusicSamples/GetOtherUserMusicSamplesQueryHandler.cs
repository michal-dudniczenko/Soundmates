using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.Messages.Common;
using Soundmates.Application.MusicSamples.Common;
using Soundmates.Application.MusicSamples.Queries.GetSelfMusicSamples;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.MusicSamples.Queries.GetOtherUserMusicSamples;

public class GetOtherUserMusicSamplesQueryHandler(
    IUserRepository _userRepository,
    IMusicSampleRepository _musicSampleRepository,
    IAuthService _authService
) : IRequestHandler<GetOtherUserMusicSamplesQuery, Result<List<OtherUserMusicSampleDto>>>
{
    private const string SamplesDirectoryPath = "mp3";
    private const int MaxLimit = 50;
    public async Task<Result<List<OtherUserMusicSampleDto>>> Handle(GetOtherUserMusicSamplesQuery request, CancellationToken cancellationToken)
    {
        var validationResult = PaginationValidator.ValidateLimitOffset<List<OtherUserMusicSampleDto>>(request.Limit, request.Offset, MaxLimit);
        if (!validationResult.IsSuccess)
            return validationResult;

        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result<List<OtherUserMusicSampleDto>>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var otherUserExists = await _userRepository.CheckIfExistsActiveAsync(request.OtherUserId);

        if (!otherUserExists)
        {
            return Result<List<OtherUserMusicSampleDto>>.Failure(
                errorType: ErrorType.NotFound,
                errorMessage: $"No user with ID: {request.OtherUserId}");
        }

        var musicSamples = await _musicSampleRepository.GetUserMusicSamplesAsync(
            userId: request.OtherUserId,
            limit: request.Limit,
            offset: request.Offset);

        var musicSamplesDtos = musicSamples.Select(ms => new OtherUserMusicSampleDto
        {
            FileUrl = SamplesDirectoryPath + "/" + ms.FileName,
            DisplayOrder = ms.DisplayOrder
        }).ToList();

        return Result<List<OtherUserMusicSampleDto>>.Success(musicSamplesDtos);
    }
}
