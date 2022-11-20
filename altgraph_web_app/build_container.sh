#!/bin/bash

# https://spring.io/guides/gs/spring-boot-docker/
# https://betulsahinn.medium.com/dockerizing-a-spring-boot-application-and-using-the-jib-maven-plugin-95c329866f34
# https://discuss.gradle.org/t/gradle-fat-jar-with-module-dependency-fails-to-build-module-jar/43380/2
# https://github.com/spring-projects/spring-framework/blob/main/spring-context/src/main/java/org/springframework/context/annotation/ClassPathScanningCandidateComponentProvider.java

echo "clean..."
gradle clean

echo "build..."
gradle build

find . | grep jar

echo "docker build..."
docker build -t cjoakim/azure-cosmosdb-altgraph-v2 .

echo "docker image list..."
docker image list | grep altgraph
