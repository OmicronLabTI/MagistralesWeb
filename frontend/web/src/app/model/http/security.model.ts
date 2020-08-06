export interface ILoginReq {
  user: string;
  password: string;
  redirectUri: string;
  clientId2: string;
}

export interface ILoginRes {
  access_token: string;
  token_type: string;
  expires_in: number;
  scope: string;
}
