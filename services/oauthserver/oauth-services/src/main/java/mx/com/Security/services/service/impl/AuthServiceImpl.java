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

        if(origin != null && (securityDO.getRole() == 1 || securityDO.getRole() == 3 ) && origin.toLowerCase().equals("app")){
            throw new UnAuthorizedException(ErrorMessages.PERFIL_INCORRECTO);
        }

        if(origin != null && securityDO.getRole() == 2 && origin.toLowerCase().equals("web")){
            throw new UnAuthorizedException(ErrorMessages.PERFIL_INCORRECTO);
        }
    }

    @Override
    public void validateUserExist(String usr) {

    }
}
