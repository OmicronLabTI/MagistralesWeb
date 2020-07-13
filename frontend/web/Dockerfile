FROM nginx
#RUN rm -rf /usr/share/nginx/html/*
COPY nginx.conf /etc/nginx/nginx.conf
COPY dist/my-app /usr/share/nginx/html
#COPY 3a72e7eae40624a.crt /etc/ssl/3a72e7eae40624a.crt
#COPY server.key /etc/ssl/server.key
#EXPOSE 443
CMD ["nginx", "-g", "daemon off;"]