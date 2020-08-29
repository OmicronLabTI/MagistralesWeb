package mx.com.Security.services.service;

public interface IAuthService {

    void validateCredentials(String usr, String password, String origin);

    void validateUserExist(String usr);

}
