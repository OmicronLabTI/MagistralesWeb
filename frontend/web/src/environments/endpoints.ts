import { environment } from './environment';
export const Endpoints = {
  security: {
    login: `${environment.baseUrl}/api/oauth/oauthrs/authorize`
  },
  users:{
    createUser:`${environment.baseUrlUsers}/createUser`,
    roles:`${environment.baseUrlCatalogs}/getroles`,
    getUsers:`${environment.baseUrlUsers}/getUsers`,
    delete:`${environment.baseUrlUsers}/deactivateUser`
  }
}

export const TokenExcludedEndpoints = [
  'login'
]
