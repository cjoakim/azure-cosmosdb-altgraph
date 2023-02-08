using System.ComponentModel.DataAnnotations;

namespace altgraph_web_app.Areas.ImdbGraph
{
  public enum FormFunctionEnum
  {
    [Display(Name = "Graph Stats")]
    GraphStats,
    [Display(Name = "PageRank")]
    PageRank,
    [Display(Name = "Network")]
    Network,
    [Display(Name = "Shortest Path")]
    ShortestPath
  }
}