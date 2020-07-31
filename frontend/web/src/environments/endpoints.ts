import { environment } from './environment';
export const Endpoints = {
  security: {
    login: `${environment.baseUrl}/api/oauth/oauthrs/authorize`
  },
  users:{
    createUser:`${environment.baseUrl}/api/usuarios/createuser`,
    roles:`${environment.baseUrl}/api/catalogos/getroles`,
    getUsers:`${environment.baseUrl}/api/usuarios/getUsers?offset=0&limit=10`,
    delete:`${environment.baseUrl}/api/usuarios/deactivateUser`,
    update:`${environment.baseUrl}/api/usuarios/updateUser`,
  },
  pedidos:{
    getPedidos: `${environment.baseUrlPedidos}/orders`,
    getDetallePedido: `${environment.baseUrlPedidos}/detail/`,
  }
}

export const TokenExcludedEndpoints = [
  'login'
]
