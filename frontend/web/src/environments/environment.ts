// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
    production: false,
    // baseUrl: 'http://localhost:8000', // prod
    baseUrl: 'https://5b09ea7ad2d9.ngrok.io', // dev
    baseUrlLogin: 'http://192.168.0.101:8090', // dev
    baseUrlUsers: 'http://192.168.0.101:5101', // dev
    baseUrlCatalogs: 'http://192.168.0.101:9010', // dev
    apiLogin: '/api/oauth/oauthrs',
    apiCatalogs: '/api/catalogos',
    apiUsers: '/api/usuarios',
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
