package mx.com.Security.services.facade.impl;

import mx.com.Security.commons.to.*;
import mx.com.Security.services.facade.IOauthFacade;
import mx.com.Security.services.service.IAuthService;
import mx.com.Security.services.service.ITokenService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import java.util.Map;

@Component
public class OauthFacade implements IOauthFacade {

    @Autowired
    private IAuthService authService;

    @Autowired
    private ITokenService tokenService;

    @Value("${security.values.ttl}")
    private int ttl;

    @Value("${security.values.ttl-refresh}")
    private int ttlRefresh;

    @Override
    public TokenResponseTO authorize(LoginRequestTO loginRequest) {

        authService.validateCredentials(loginRequest.getUser(), loginRequest.getPassword(), loginRequest.getOrigin());

        String access_token = tokenService.generateToken(loginRequest.getClientId(), loginRequest.getUser(), "admin", ttl);

        String refresh_token = tokenService.generateToken(loginRequest.getClientId(), loginRequest.getUser(), "admin", ttlRefresh);

        TokenResponseTO tokenResponse = new TokenResponseTO();
        tokenResponse.setAccess_token(access_token);
        tokenResponse.setExpires_in(ttl);
        tokenResponse.setToken_type("Bearer");
        tokenResponse.setRefresh_token(refresh_token);

        return tokenResponse;
    }

    @Override
    public void validate(ValidTokenRequestTO validTokenRequest) {

        tokenService.validateToken(validTokenRequest.getToken());
    }

    @Override
    public TokenResponseTO renew(TokenRenewRequestTO tokenRenewRequest) {

        Map<String, String> claims = tokenService.decodeToken(tokenRenewRequest.getRefresh_token());

        authService.validateUserExist(claims.get("user"));

        String access_token = tokenService.generateToken(claims.get("clientId"), claims.get("user"), claims.get("profile"), ttl);

        String refresh_token = tokenService.generateToken(claims.get("clientId"), claims.get("user"), claims.get("profile"), ttlRefresh);

        TokenResponseTO tokenResponse = new TokenResponseTO();
        tokenResponse.setAccess_token(access_token);
        tokenResponse.setExpires_in(ttl);
        tokenResponse.setToken_type("Bearer");
        tokenResponse.setRefresh_token(refresh_token);

        return tokenResponse;
    }
}
