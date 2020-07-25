package mx.com.Security.persistence;

import mx.com.Security.model.SecurityDO;
import org.springframework.data.repository.CrudRepository;
import java.util.List;

public interface SecurityDAO extends CrudRepository<SecurityDO, String> {

    SecurityDO findFirstByUsername(String userName);
}
