import { environment } from './environment';

export const Endpoints = {
  security: {
    login: `${environment.baseUrl}/login?delay=2`
  }
}

export const TokenExcludedEndpoints = [
  'login'
]
