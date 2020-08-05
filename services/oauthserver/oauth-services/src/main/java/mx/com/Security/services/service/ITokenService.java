package mx.com.Security.services.service;

import java.util.Map;

public interface ITokenService{

    String generateToken(String clientId, String user, String profile, int ttl);

    void validateToken(String token);

    void revokeToken();

    public Map<String, String> decodeToken(String token);
}
