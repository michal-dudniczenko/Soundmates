using MediatR;
using Soundmates.Application.Common;
using Soundmates.Application.MusicSamples.Common;
using Soundmates.Domain.Interfaces.Repositories;
using Soundmates.Domain.Interfaces.Services.Auth;

namespace Soundmates.Application.MusicSamples.Queries.GetSelfMusicSamples;

public class GetSelfMusicSamplesQueryHandler(
    IMusicSampleRepository _musicSampleRepository,
    IAuthService _authService
) : IRequestHandler<GetSelfMusicSamplesQuery, Result<List<SelfMusicSampleDto>>>
{
    private const string SamplesDirectoryPath = "mp3";
    private const int MaxLimit = 50;
    public async Task<Result<List<SelfMusicSampleDto>>> Handle(GetSelfMusicSamplesQuery request, CancellationToken cancellationToken)
    {
        var validationResult = PaginationValidator.ValidateLimitOffset<List<SelfMusicSampleDto>>(request.Limit, request.Offset, MaxLimit);
        if (!validationResult.IsSuccess)
            return validationResult;

        var authorizedUser = await _authService.GetAuthorizedUserAsync(subClaim: request.SubClaim, checkForFirstLogin: true);

        if (authorizedUser is null)
        {
            return Result<List<SelfMusicSampleDto>>.Failure(
                errorType: ErrorType.Unauthorized,
                errorMessage: "Invalid access token.");
        }

        var musicSamples = await _musicSampleRepository.GetUserMusicSamplesAsync(
            userId: authorizedUser.Id, 
            limit: request.Limit, 
            offset: request.Offset);

        var musicSamplesDtos = musicSamples.Select(ms => new SelfMusicSampleDto
        {
            Id = ms.Id,
            FileUrl = SamplesDirectoryPath + "/" + ms.FileName,
            DisplayOrder = ms.DisplayOrder
        }).ToList();

        return Result<List<SelfMusicSampleDto>>.Success(musicSamplesDtos);
    }
}
