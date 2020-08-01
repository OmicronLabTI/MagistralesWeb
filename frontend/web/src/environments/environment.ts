// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,

  baseUrl: 'http://localhost:8000', // prod
  // baseUrl: 'https://febf9d417342.ngrok.io', //dev
  baseUrlLogin: 'http://localhost:8090', // dev
  baseUrlUsers: 'http://localhost:5101', // dev
  baseUrlCatalogs: 'http://localhost:9010', // dev
  apiLogin: '/api/oauth/oauthrs',
  apiCatalogs: '/api/catalogos',
  apiUsers: '/api/usuarios',
};

export const MODAL_NAMES =  {
  addUserModal: 'addModal',
  editUser: 'editModal'
};
export const CONST_USER_DIALOG = {
  defaultDefault: 'QFB',
  patternPassWord: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/
};

export const CONST_NUMBER = {
  zero: 0,
  one: 1,
  two: 2,
  three: 3

};

export const CONST_STRING = {
  empty: ''
};
/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
