FROM    openjdk:17-jdk-alpine
WORKDIR /app
EXPOSE  8080
COPY    src /app/src
RUN     mkdir -p /app/data/cache
RUN     mkdir -p /app/data/graph
RUN     mkdir -p /app/data/struct
RUN     mkdir -p /app/tmp
COPY    build/libs/web-0.1.0.jar /app/app.jar
ENTRYPOINT ["java", "-jar", "app.jar"]

# app.jar runs in the WORKDIR 