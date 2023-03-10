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
