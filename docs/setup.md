# AltGraph - How to Setup This App in your Environment

## Required Development Desktop Software

- git source control system
- Java JDK, Microsoft OpenJDK 17 is recommended
  - https://docs.microsoft.com/en-us/java/openjdk/download
- Gradle Build tool 
  - https://gradle.org/releases/
- A Java IDE is recommended, such as JetBrains IntelliJ or Visual Studio Code

## Azure PaaS Services

- **Azure CosmosDB SQL API Account**
  - Database named **dev**
  - Container named **altgraph**
    - Partition key: **/pk**
  - Indexing Policy:
    - See file default indexing file **altgraph_data_app/indexing/default.json** in this repo
    - See recommended file **altgraph_data_app/indexing/altgraph_indexing.json** in this repo

- **Azure Redis Cache Account**

## Environment Variables

- See file **altgraph_web_app/src/main/resources/application.properties**
- Set environment variables for all of the **${AZURE_XXX}** names
- See your Azure Portal for the values of these environment variables

## Getting Started

In a Windows, Linux, macOS Terminal:

```
$ git clone https://github.com/cjoakim/azure-cosmosdb-altgraph.git

$ cd azure-cosmosdb-altgraph

$ cd altgraph_data_app

$ gradle clean build            Compile the Java code
$ gradle loadCosmos             Load CosmosDB with the already-created data files

$ cd ..
$ cd altgraph_web_app

$ gradle clean build             Compile the Java code
$ gradle bootRun                 Run the Web Application locally
                                 Then visit http://localhost:8080 with your browser
```
