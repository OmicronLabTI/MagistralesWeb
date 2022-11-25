FROM node:12-slim AS build
WORKDIR /usr/src/app
COPY package*.json ./
RUN npm ci --no-optional
COPY . .
RUN npm run lint
ARG CONFIGURATION=""
ARG BASE_HREF="/"
RUN npm run build --configuration=$CONFIGURATION --basehref=$BASE_HREF


FROM nginx AS release
COPY nginx.conf /etc/nginx/nginx.conf
WORKDIR /usr/share/nginx/html
COPY --from=build /usr/src/app/dist ./
CMD ["nginx", "-g", "daemon off;"]
