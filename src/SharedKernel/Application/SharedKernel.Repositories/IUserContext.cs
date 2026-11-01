namespace TL.SharedKernel.Application.Repositories;

/// <summary>
/// User context.
/// Use to get data of the authenticated user
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// Get the user profile identifier
    /// </summary>
    /// <returns>User profile identifier</returns>
    Guid GetProfileId();
}
