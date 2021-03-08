package mx.com.Security.services.service.impl;

import mx.com.Security.commons.constants.ErrorMessages;
import mx.com.Security.commons.exceptions.UnAuthorizedException;
import mx.com.Security.model.SecurityDO;
import mx.com.Security.persistence.SecurityDAO;
import mx.com.Security.services.service.IAuthService;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.Base64;

@Service
public class AuthServiceImpl implements IAuthService {

    static final Logger LOG = LogManager.getLogger(AuthServiceImpl.class);

    @Autowired
    private SecurityDAO securityDAO;

    @Override
    public void validateCredentials(String usr, String password, String origin) {

        SecurityDO securityDO = securityDAO.findFirstByUsername(usr);

        if (securityDO == null){
            throw new UnAuthorizedException(ErrorMessages.USUARIO_NO_EXISTE);
        }

        if(!password.equals(securityDO.getPassword())){
            throw new UnAuthorizedException(ErrorMessages.INVALID_CREDENTIALS);
        }

        if (securityDO.getActivo() == 0){
            throw new UnAuthorizedException(ErrorMessages.USUARIO_INACTIVO);
        }

        if (origin == null){
            throw new UnAuthorizedException(ErrorMessages.PERFIL_INCORRECTO);
        }

        boolean isAbleToApp = securityDO.getRole() == 2;
        boolean isAbleToAppAlmacen = securityDO.getRole() == 5;
        boolean isAbleToWebDelivery = securityDO.getRole() == 6;
        boolean isAbleToWebMagistral = securityDO.getRole() == 1 || securityDO.getRole() == 3 || securityDO.getRole() == 4 || securityDO.getRole() == 5 || securityDO.getRole() == 7;

        boolean needsThrowError = isAbleToApp && !origin.toLowerCase().equals("app");
        needsThrowError = needsThrowError ? true : isAbleToWebMagistral && !origin.toLowerCase().equals("web");
        needsThrowError = needsThrowError ? true : isAbleToAppAlmacen && !origin.toLowerCase().equals("appalmacen");
        needsThrowError = needsThrowError ? true : isAbleToWebDelivery && !origin.toLowerCase().equals("webdelivery");

        if(needsThrowError){
            throw new UnAuthorizedException(ErrorMessages.PERFIL_INCORRECTO);
        }
    }

    @Override
    public void validateUserExist(String usr) {

    }
}
