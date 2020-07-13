FROM store/oracle/serverjre:1.8.0_241-b07
ENV JAR_FILE target/gateway-1.0-SNAPSHOT.jar
ADD ${JAR_FILE} app.jar
ENTRYPOINT ["java","-Djava.security.egd=file:/dev/./urandom","-jar","/app.jar"]