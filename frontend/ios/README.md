# Axity iOS Archetype
## Omicron

Proyecto base para desarrollo de aplicaciones iOS nativas.

## Pre-requisitos
  - macOS
  - [Xcode 11.5+](https://apps.apple.com/mx/app/xcode/id497799835?l=en&mt=12)
  - [CocoaPods](https://cocoapods.org)
  - [swiftlint](https://github.com/realm/SwiftLint)
  - [slather](https://github.com/SlatherOrg/slather)
  - [sonar-scanner](https://docs.sonarqube.org/latest/analysis/scan/sonarscanner/)

### Instalación

Para instalar los componentes de línea de comandos se recommienda usar [Homebrew](https://brew.sh):

```sh
$ /bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install.sh)
```

Una vez instalado ejecutar los siguientes comandos para instalar las dependencias:

```sh
$ brew install swiftlint
```
```sh
$ brew install sonar-scanner
```
```sh
$ sudo gem install slather
```

### Estructura

El patrón de diseño utilizado en el proyecto es MVVM (Model-View-ViewModel), con el fin de desacoplar toda la lógica de negocio, dejando así la vista como una simple capa de presentación, además de facilitar la generación de pruebas unitarias.

![MVVM](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/enterprise-application-patterns/mvvm-images/mvvm.png)

##### Carpetas

| Nombre | Uso |
| ------ | ------ |
| Assets | Archivos fijos (json, imágenes, et) |
| Common | Clases de uso común en toda la aplicación |
| Network | Clases de la capa de red (consumo de servicios) |
| Screens | Clases de pantallas y vistas de la aplicación |
| Utilities | Clases de utilidades de la aplicación |
| AxityStarterTests | Pruebas unitarias |
| AxityStarterUITests | Pruebas de interfaz de usuario |

### Calidad del código

#### Sonar

Con el fin de mantener la limpieza y calidad del código fuente, se incluyen scripts que permiten el análisis de los archivos del proyecto mediante la herramienta SonarQube.

Los parámetros del servidor al cual se enviará el análisis, así como la configuración del proyecto, se encuentra dentro del archivo `sonar-project.properties`

Para ejecutar el análisis se debe ejecutar el siguiente comando en una ventana de terminal:

```sh
$ sh run-sonar-swift.sh
```

## Contributors

Diego Cárcamo ->
[diego.carcamo@axity.com](mailto:diego.carcamo@axity.com)

## License

[MIT](https://opensource.org/licenses/MIT)