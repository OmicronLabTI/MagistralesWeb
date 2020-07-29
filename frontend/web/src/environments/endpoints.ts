import { environment } from './environment';
export const Endpoints = {
  security: {
    login: `${environment.baseUrl}/login?delay=2`
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
