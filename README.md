# AltGraph

An **alternative graph implementation** built on the **Azure CosmosDB SQL API** and 
**Java** with the **Spring Boot** and **Spring Data** frameworks.

This working reference application is a **refactored version 2.0** of this original repository:</br>
https://github.com/Azure-Samples/azure-cosmos-db-graph-npm-bom-sample

It uses the **Node.js NPM** ecosystem of libraries as a public-domain dataset and an example of a
**Bill-of-Materials (BOM)** business application.

---

<p align="center">
    <img src="docs/img/UI-Tedious-1-No-Cache.png" width="100%">
</p>

## Chris Joakim, Microsoft, CosmosDB Global Black Belt (GBB)

---

## Directory Structure of this Repository

```
├── altgraph_data_app       A Java Spring CommandLineRunner (console) application 
└── altgraph_web_app        A Java Spring Web application
├── CosmosDB-AltGraph.pdf   Presentation PDF

```

## Links

- [Presentation PDF](CosmosDB-AltGraph.pdf)
- [Links](docs/links.md)
- [Setup](docs/setup.md)
- [AltGraph GitHub Repo](https://github.com/cjoakim/azure-cosmosdb-altgraph)
- [CosmosDB Live TV Episode #59](https://www.youtube.com/watch?v=SGih_Kj_1yk)
- https://jnbridge.com/

### JGraphT

The **v2** AltGraph implementation uses an in-memory graph using the JGraphT library.
The graph is maintained in the memory of the JVM (Java Virtual Machine), it is
mutable, and offers excellent built-in algorithms (PageRank, DijkstraShortestPath, Centrality, etc.)
so that you don't have to create this logic yourself.

- https://jgrapht.org/guide/UserOverview
- https://jgrapht.org/javadoc/
- https://jgrapht.org/javadoc/org.jgrapht.core/org/jgrapht/alg/scoring/PageRank.html
- https://jgrapht.org/javadoc/org.jgrapht.core/org/jgrapht/alg/shortestpath/JohnsonShortestPaths.html

### Guava Graph 

I evaluated the Google Guava library as a possible in-memory graph implementation.
I compared it to the above JGraphT library.  Though it is more recently maintained,
I chose not to use Google Guava for the graph because its' graph traversal logic
was is lacking and the docs were very limited on this.  However, I leave this comment
here for your reference in case you wish to evaluate Google Guava

- https://github.com/google/guava/wiki/GraphsExplained
- https://javadoc.io/doc/com.google.guava/guava/latest/index.html
- https://dzone.com/articles/an-overview-of-google-guava-and-introduction-to-it
- https://github.com/google/guava/blob/master/guava-tests/test/com/google/common/graph/TraverserTest.java
- https://www.demo2s.com/java/google-guava-graphbuilder-tutorial-with-examples.html
- https://github.com/google/guava/tree/master/guava/src/com/google/common/graph
