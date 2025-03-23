namespace TL.Socials.Identity.Startups.Api.Models;

public sealed record LoginResultDto(Guid UserId, string RefreshToken);