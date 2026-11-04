namespace vangoplus_server.Application.Auth
{
    public interface IJwtTokenService
    {
        /// <summary>
        /// Builds a signed token that identifies this user. The token carries
        /// the user id and role, so a later request does not have to be
        /// trusted when it says who it is.
        /// </summary>
        string CreateToken(int userId, string name, string email, string? role);
    }
}
