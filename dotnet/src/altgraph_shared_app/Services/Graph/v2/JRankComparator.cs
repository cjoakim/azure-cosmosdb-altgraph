namespace altgraph_shared_app.Services.Graph.v2
{
  public class JRankComparator : IComparer<JRank>
  {
    public int Compare(JRank? x, JRank? y)
    {
      if (x == null)
      {
        return -1;
      }

      if (y == null)
      {
        return 1;
      }

      return x.CompareTo(y);
    }
  }
}
