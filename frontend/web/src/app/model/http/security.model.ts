export interface ILoginReq {
  user: string;
  password: string;
  redirectUri: string;
  clientId2: string;
  origin: string;
}

export interface ILoginRes {
  access_token: string;
  refresh_token: string;
  token_type: string;
  expires_in: number;
  scope: string;
}

export interface IRefreshTokenReq {
  grant_type: string;
  refresh_token: string;
  scope: string;
}
