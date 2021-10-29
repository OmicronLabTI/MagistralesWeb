FROM maven:3.8.1-jdk-8-slim AS build-env
WORKDIR /app
COPY . ./
RUN mvn clean package

FROM store/oracle/serverjre:1.8.0_241-b07
WORKDIR /app
COPY --from=build-env /app/target/gateway-1.0-SNAPSHOT.jar .
ENTRYPOINT ["java","-Djava.security.egd=file:/dev/./urandom","-jar","/app/gateway-1.0-SNAPSHOT.jar"]