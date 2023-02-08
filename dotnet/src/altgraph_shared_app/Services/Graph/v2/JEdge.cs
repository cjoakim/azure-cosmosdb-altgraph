using System.Diagnostics.CodeAnalysis;
using altgraph_shared_app.Services.Graph.v2.Structs;
using QuikGraph;

namespace altgraph_shared_app.Services.Graph.v2
{
  public class JEdge : IEdge<VertexValueStruct>
  {
    public VertexValueStruct Source { get; set; } = new VertexValueStruct();

    public VertexValueStruct Target { get; set; } = new VertexValueStruct();

    public string? S()
    {
      return Source.ToString();
    }

    public String? T()
    {
      return Target.ToString();
    }
  }
}