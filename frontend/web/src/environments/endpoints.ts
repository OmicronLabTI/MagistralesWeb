import { environment } from './environment';
export const Endpoints = {
  security: {
    //login: `${environment.baseUrl}/api/oauth/oauthrs/authorize`
    login: `https://febf9d417342.ngrok.io/api/oauth/oauthrs/authorize`
  },
  users:{
    createUser:`https://febf9d417342.ngrok.io/api/usuarios/createuser`,
    //createUser:`http://localhost:5101/createUser`,
    roles:`https://febf9d417342.ngrok.io/api/catalogos/getroles`,
    //roles:`${environment.baseUrlCatalogs}/getroles`,
    getUsers:`https://febf9d417342.ngrok.io/api/usuarios/getUsers?offset=0&limit=10`,
    //getUsers:`${environment.baseUrlUsers}/getUsers?offset=0&limit=10`,
    delete:`https://febf9d417342.ngrok.io/api/usuarios/deactivateUser`,
    //delete:`${environment.baseUrlUsers}/deactivateUser`,
    update:'https://febf9d417342.ngrok.io/api/usuarios/updateUser'
  }
}

export const TokenExcludedEndpoints = [
  'login'
]
