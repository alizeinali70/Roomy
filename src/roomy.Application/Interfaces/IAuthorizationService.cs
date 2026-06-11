namespace roomy.Domain.Interfaces
{
    public interface IAuthorizationService
    {
        Guid GetCurrentUserId();
        bool IsAdmin();
        bool IsOwnerOf(Guid resourceOwnerId);
    }
}
