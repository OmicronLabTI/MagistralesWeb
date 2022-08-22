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

### Ingress multiples dominios

```
helm repo add ingress-nginx https://kubernetes.github.io/ingress-nginx

helm install omicron-ingress-api ingress-nginx/ingress-nginx --namespace nginx-api --set controller.ingressClassResource.name=nginx-api --set controller.ingressClassResource.controllerValue=k8s.io/nginx-api

helm install omicron-ingress-log ingress-nginx/ingress-nginx --namespace nginx-log --set controller.ingressClassResource.name=nginx-log --set controller.ingressClassResource.controllerValue=k8s.io/nginx-log
```

_Como ejecutar el proyecto_

### Contributors

_Aquí van los colaboradores_

Javier Rodríguez  
[francisco.rodriguez@axity.com]
