# AltGraph - Implementation Notes

## Java: JGraphT

The **v2** AltGraph implementation uses an in-memory graph using the **JGraphT library**.
The graph is maintained in the memory of the JVM (Java Virtual Machine), it is
mutable, and offers excellent built-in algorithms (PageRank, DijkstraShortestPath, Centrality, etc.)
so that you don't have to create this logic yourself.

- https://jgrapht.org/guide/UserOverview
- https://jgrapht.org/javadoc/
- https://jgrapht.org/javadoc/org.jgrapht.core/org/jgrapht/alg/scoring/PageRank.html
- https://jgrapht.org/javadoc/org.jgrapht.core/org/jgrapht/alg/shortestpath/JohnsonShortestPaths.html
- https://search.maven.org/search?q=a:jgrapht

## Java: Guava Graph 

I evaluated the Google Guava Graph library as a possible in-memory graph implementation.
I compared it to the above JGraphT library.  Though it is more recently maintained,
I chose not to use Google Guava for the graph because its' graph traversal logic
is lacking and the docs were very sparse on this.  However, I leave this comment
here for your reference in case you wish to evaluate the Google Guava Graph solution.

- https://github.com/google/guava/wiki/GraphsExplained
- https://javadoc.io/doc/com.google.guava/guava/latest/index.html
- https://dzone.com/articles/an-overview-of-google-guava-and-introduction-to-it
- https://github.com/google/guava/blob/master/guava-tests/test/com/google/common/graph/TraverserTest.java
- https://www.demo2s.com/java/google-guava-graphbuilder-tutorial-with-examples.html
- https://github.com/google/guava/tree/master/guava/src/com/google/common/graph

## DotNet, Python, and other Programming Languages

At the current time AltGraph is only implemented in Java  Based on customer demand,
it may be implemented in other programming languages in the future.

### DotNet

For DotNet, the JNBridgePro software is one possible implementation. 

- https://jnbridge.com/

### Python 

There is a language binding for Python to JGraphT.

- https://github.com/d-michail/python-jgrapht
