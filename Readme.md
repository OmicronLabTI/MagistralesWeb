# OMICRONLAB

## Proyecto App iOS Formulas

_Introducción al proyecto_

### Comenzando

_Preparación inicial y consideraciones_

### Pre-requisitos

_Para la instalación se necesita:_

- [Docker](https://docs.docker.com/install/)

### Instalación

_Como instalar el proyecto_

### Renovación de Certificado Producción

1. Descargar archivo PFX desde Key Vault Azure (el nombre del archivo es similar a `omicron-key-vault-omicron-certc5e66751-a4d4-4cf6-a0c5-59c1fc740169-20231003.pfx`)

2. En ambiente Linux (o WSL) ejecutar los siguientes comandos para extraer la información del archivo PFX

Transformar PFX a PEM

```
openssl pkcs12 -in <NOMBRE DEL ARCHIVO>.pfx -out <ARCHIVO DE SALIDA>.pem -nodes
```

Ejemplo:

```
openssl pkcs12 -in omicron-key-vault-omicron-certc5e66751-a4d4-4cf6-a0c5-59c1fc740169-20231003.pfx -out omicron2023.pem -nodes
```

Extraer Private Key de Certificado

```
openssl pkey -in <ARCHIVO PEM>.pem -out <ARCHIVO DE SALIDA KEY>.key
```

Ejemplo:

```
openssl pkey -in omicron2023.pem -out omicron2023.key
```

Extraer CRT

```
openssl crl2pkcs7 -nocrl -certfile <ARCHIVO PEM>.pem | openssl pkcs7 -print_certs -out <ARCHIVO SALIDA CRT>.crt
```

Ejemplo:

```
openssl crl2pkcs7 -nocrl -certfile omicron2023.pem | openssl pkcs7 -print_certs -out omicron2023.crt
```

Una vez exportado, modificar el archivo `.crt`, en un editor de texto, se encontrarán 3 secciones de certificados, identificadas con BEGIN CERTIFICATE Y END CERTIFICATE

```
subject=C = US, ST = Arizona, L = Scottsdale, O = "GoDaddy.com, Inc.", CN = Go Daddy Root Certificate Authority - G2
issuer=C = US, ST = Arizona, L = Scottsdale, O = "GoDaddy.com, Inc.", CN = Go Daddy Root Certificate Authority - G2
-----BEGIN CERTIFICATE-----
MIIDxTCCAq2gA...
-----END CERTIFICATE-----

subject=C = US, ST = Arizona, L = Scottsdale, O = "GoDaddy.com, Inc.", OU = http://certs.godaddy.com/repository/, CN = Go Daddy Secure Certificate Authority - G2
issuer=C = US, ST = Arizona, L = Scottsdale, O = "GoDaddy.com, Inc.", CN = Go Daddy Root Certificate Authority - G2
-----BEGIN CERTIFICATE-----
MIIE0DCCA7igAwIBAgIBBz...
-----END CERTIFICATE-----

subject=CN = *.omicronlab.com.mx
issuer=C = US, ST = Arizona, L = Scottsdale, O = "GoDaddy.com, Inc.", OU = http://certs.godaddy.com/repository/, CN = Go Daddy Secure Certificate Authority - G2
-----BEGIN CERTIFICATE-----
MIIGoDCCBYigAwIBAgIJALw....
-----END CERTIFICATE-----
```

3. Eliminar las primeras 2 secciones y solamente dejar la última, a partir del texto `subject=CN = *.omicronlab.com.mx` y guardar el archivo

\*\*\* En caso de no realizar este paso al tratar de crear el secret en el cluster de kubernetes (sig. paso) mostrará el sig. error:

```
error: tls: private key does not match public key
```

4. Crear secret dentro del cluster de Kubernetes

```
kubectl create secret tls <NOMBRE DE SECRET> --key <PATH>/<ARCHIVO KEY>.key --cert <PATH>/<ARCHIVO CRT>.crt
```

Ejemplo:

```
kubectl create secret tls secure-tls-prod-2023 --key omicron2023.key --cert omicron2023.crt
```

5. Modificar archivos Ingress para referenciar al nuevo secret generado

```
deploy/prod/ingress-api.yml
deploy/prod/ingress-log.yml
deploy/prod/ingress-oauth.yml
deploy/prod/ingress-services.yml
```

Ejemplo reemplazar el valor secretName:

```yaml
spec:
  ingressClassName: nginx-log
  tls:
    - secretName: NUEVO VALOR DE SECRET 👈
      hosts:
        - magistralesdashboard.omicronlab.com.mx
```

6. Realizar despliegue de la aplicación o aplicar archivos yaml modificados

### Ingress multiples dominios

```
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx
```

### AKS < 1.24

```
helm install omicron-ingress-api ingress-nginx/ingress-nginx --namespace nginx-api --set controller.ingressClassResource.name=nginx-api --set controller.ingressClassResource.controllerValue=k8s.io/nginx-api

helm install omicron-ingress-log ingress-nginx/ingress-nginx --namespace nginx-log --set controller.ingressClassResource.name=nginx-log --set controller.ingressClassResource.controllerValue=k8s.io/nginx-log
```

### AKS >= 1.24

```
helm install omicron-ingress-api ingress-nginx/ingress-nginx --namespace nginx-api --set controller.ingressClassResource.name=nginx-api --set controller.ingressClassResource.controllerValue=k8s.io/nginx-api --set controller.service.annotations."service\.beta\.kubernetes\.io/azure-load-balancer-health-probe-request-path"=/healthz

helm install omicron-ingress-log ingress-nginx/ingress-nginx --namespace nginx-log --set controller.ingressClassResource.name=nginx-log --set controller.ingressClassResource.controllerValue=k8s.io/nginx-log --set controller.service.annotations."service\.beta\.kubernetes\.io/azure-load-balancer-health-probe-request-path"=/healthz
```

_Como ejecutar el proyecto_

### Contributors

_Aquí van los colaboradores_

Javier Rodríguez  
[francisco.rodriguez@axity.com]
