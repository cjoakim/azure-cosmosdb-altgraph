FROM    openjdk:17-jdk-alpine
WORKDIR /app
EXPOSE  8080
COPY    src /app/src
RUN     mkdir -p /app/data/cache
RUN     mkdir -p /app/data/graph
RUN     mkdir -p /app/data/struct
RUN     mkdir -p /app/tmp
COPY    build/libs/altgraph_web_app-2.0.0.jar /app/app.jar
ENV JAVA_OPTS="-Xms1024m -Xmx4096m"
ENTRYPOINT ["java", "-jar", "app.jar"]

# app.jar runs in the WORKDIR
