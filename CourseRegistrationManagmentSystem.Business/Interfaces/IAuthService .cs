
public interface IAuthService
{
    Task Login(string userName, string password);

    Task Register(string userName, string password);


}
