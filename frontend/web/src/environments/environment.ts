// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  baseUrl: 'https://reqres.in/api',
  baseUrlUsers:'http://localhost:5101',
  baseUrlCatalogs:'http://localhost:9010',
  apiCatalogs:'/api/catalogos',
  apiUsers:'/api/usuarios',
  baseUrlPedidos:'http://localhost:5102',
};

export const MODAL_NAMES =  {
  addUserModal: 'addModal',
  editUser: 'editModal'
};

export const CONST_USER_DIALOG = {
  defaultDefault: 'QFB',
  patternPassWord: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/
}

export const CONST_NUMBER = {
  zero:0,
  one:1,
  two:2,
  three:3

}
/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.