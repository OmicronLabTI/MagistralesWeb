package mx.com.Security.services.facade;

import mx.com.Security.commons.exceptions.UnAuthorizedException;
import mx.com.Security.commons.to.*;
import mx.com.Security.services.BaseTest;
import mx.com.Security.services.facade.impl.OauthFacade;
import org.junit.Assert;
import org.junit.Test;
import org.springframework.beans.factory.annotation.Value;

import java.util.Map;

public class OauthFacadeTest extends BaseTest {

    @Value("${security.values.ttl}")
    private int ttl;

    @Test
    public void emptyTest() {
        Assert.assertTrue(true);
    }

    @Test
    public void shouldGenerateToken() {

        LoginRequestTO loginRequestTO = new LoginRequestTO();
        loginRequestTO.setClientId("1");
        loginRequestTO.setPassword("QUJD");
        loginRequestTO.setUser("guz");
        loginRequestTO.setOrigin("web");

        TokenResponseTO tokenResponseTO = oauthFacade.authorize(loginRequestTO);

        Assert.assertNotNull(tokenResponseTO.getAccess_token());
        Assert.assertTrue(tokenResponseTO.getAccess_token().length() > 30);
        Assert.assertEquals(ttl, tokenResponseTO.getExpires_in());
        Assert.assertEquals("Bearer", tokenResponseTO.getToken_type());
        Assert.assertNotNull("", tokenResponseTO.getRefresh_token());
        Assert.assertTrue(tokenResponseTO.getRefresh_token().length() > 30);
    }

    @Test(expected = UnAuthorizedException.class)
    public void shouldReturnUnAuthorizedExceptionInvalidCredentials() {

        LoginRequestTO loginRequestTO = new LoginRequestTO();
        loginRequestTO.setClientId("1");
        loginRequestTO.setPassword("BC");
        loginRequestTO.setUser("guz");

        oauthFacade.authorize(loginRequestTO);
    }

    @Test(expected = UnAuthorizedException.class)
    public void shouldReturnUnAuthorizedExceptionUserNotExist() {

        LoginRequestTO loginRequestTO = new LoginRequestTO();
        loginRequestTO.setClientId("1");
        loginRequestTO.setPassword("BC");
        loginRequestTO.setUser("Beto");

        oauthFacade.authorize(loginRequestTO);
    }

    @Test(expected = UnAuthorizedException.class)
    public void shouldReturnUnAuthorizedExceptionWrongRoleAdmin() {

        LoginRequestTO loginRequestTO = new LoginRequestTO();
        loginRequestTO.setClientId("1");
        loginRequestTO.setPassword("BC");
        loginRequestTO.setUser("guz");
        loginRequestTO.setOrigin("app");

        oauthFacade.authorize(loginRequestTO);
    }

    @Test(expected = UnAuthorizedException.class)
    public void shouldReturnUnAuthorizedExceptionUserInactive() {

        LoginRequestTO loginRequestTO = new LoginRequestTO();
        loginRequestTO.setClientId("1");
        loginRequestTO.setPassword("BC");
        loginRequestTO.setUser("test3");
        loginRequestTO.setOrigin("app");

        oauthFacade.authorize(loginRequestTO);
    }

    @Test(expected = UnAuthorizedException.class)
    public void shouldReturnUnAuthorizedExceptionWrongRoleQfb() {

        LoginRequestTO loginRequestTO = new LoginRequestTO();
        loginRequestTO.setClientId("1");
        loginRequestTO.setPassword("BC");
        loginRequestTO.setUser("test2");
        loginRequestTO.setOrigin("web");

        oauthFacade.authorize(loginRequestTO);
    }

    @Test
    public void shouldValidateToken() {

        LoginRequestTO loginRequestTO = new LoginRequestTO();
        loginRequestTO.setClientId("1");
        loginRequestTO.setPassword("QUJD");
        loginRequestTO.setUser("guz");
        loginRequestTO.setOrigin("web");

        TokenResponseTO tokenResponseTO = oauthFacade.authorize(loginRequestTO);

        ValidTokenRequestTO validTokenRequestTO = new ValidTokenRequestTO();
        validTokenRequestTO.setToken(tokenResponseTO.getAccess_token());
        validTokenRequestTO.setUser("guz");

        oauthFacade.validate(validTokenRequestTO);

        Assert.assertTrue(true);
    }

    @Test(expected = UnAuthorizedException.class)
    public void shouldReturnUnAuthorizedExceptionInvalidToken() {

        LoginRequestTO loginRequestTO = new LoginRequestTO();
        loginRequestTO.setClientId("1");
        loginRequestTO.setPassword("QUJD");
        loginRequestTO.setUser("guz");
        loginRequestTO.setOrigin("");

        TokenResponseTO tokenResponseTO = oauthFacade.authorize(loginRequestTO);

        ValidTokenRequestTO validTokenRequestTO = new ValidTokenRequestTO();
        validTokenRequestTO.setToken(tokenResponseTO.getAccess_token() + "1");
        validTokenRequestTO.setUser("qwe");

        oauthFacade.validate(validTokenRequestTO);
    }

    @Test
    public void shouldRenewToken() {

        LoginRequestTO loginRequestTO = new LoginRequestTO();
        loginRequestTO.setClientId("1");
        loginRequestTO.setPassword("QUJD");
        loginRequestTO.setUser("guz");
        loginRequestTO.setOrigin(("web"));

        TokenResponseTO tokenResponseTO = oauthFacade.authorize(loginRequestTO);

        TokenRenewRequestTO tokenRenewRequestTO = new TokenRenewRequestTO();
        tokenRenewRequestTO.setGrant_type("");
        tokenRenewRequestTO.setRefresh_token(tokenResponseTO.getRefresh_token());
        tokenRenewRequestTO.setScope("");

        TokenResponseTO tokenRenewedResponseTO = oauthFacade.renew(tokenRenewRequestTO);

        Assert.assertNotNull(tokenRenewedResponseTO.getAccess_token());
        Assert.assertTrue(tokenRenewedResponseTO.getAccess_token().length() > 30);
        Assert.assertEquals(ttl, tokenRenewedResponseTO.getExpires_in());
        Assert.assertEquals("Bearer", tokenRenewedResponseTO.getToken_type());
        Assert.assertNotNull("", tokenRenewedResponseTO.getRefresh_token());
        Assert.assertTrue(tokenRenewedResponseTO.getRefresh_token().length() > 30);
    }

    @Test(expected = UnAuthorizedException.class)
    public void shouldReturnUnAuthorizedExceptionRenewToken() {

        LoginRequestTO loginRequestTO = new LoginRequestTO();
        loginRequestTO.setClientId("1");
        loginRequestTO.setPassword("BC");
        loginRequestTO.setUser("guz");

        TokenResponseTO tokenResponseTO = oauthFacade.authorize(loginRequestTO);

        TokenRenewRequestTO tokenRenewRequestTO = new TokenRenewRequestTO();
        tokenRenewRequestTO.setGrant_type("");
        tokenRenewRequestTO.setRefresh_token(tokenResponseTO.getAccess_token() + "1");
        tokenRenewRequestTO.setScope("");

        oauthFacade.renew(tokenRenewRequestTO);
    }

}
