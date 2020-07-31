package mx.com.Security.services.facade;

import mx.com.Security.commons.to.*;

public interface IOauthFacade {

    TokenResponseTO authorize(LoginRequestTO loginRequest);

    void validate(ValidTokenRequestTO validTokenRequest);

    TokenResponseTO renew(TokenRenewRequestTO tokenRenewRequest);

}
