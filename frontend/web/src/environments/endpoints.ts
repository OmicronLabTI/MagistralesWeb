import { environment } from './environment';

export const Endpoints = {
  security: {
    login: `${environment.baseUrl}/login?delay=2`
  },
  users:{
    createUser:`${environment.baseUrlServices}/createUser`,
    roles:`${environment.baseUrlServices}/api/catalogos/getroles`,
    getUsers:`${environment.baseUrlServices}/api/usuarios/getUsers`,
    delete:`${environment.baseUrlServices}/api/usuario/`
  }
}

export const TokenExcludedEndpoints = [
  'login'
]
