using altgraph_web_app.Models;
using altgraph_web_app.Services.Graph;

namespace altgraph_web_app.Services.IO
{
  public class FileUtil
  {
    public string ReadUnicode(string filename) { throw new NotImplementedException(); }
    public List<string> ReadLines(string infile) { throw new NotImplementedException(); }
    public Dictionary<string, object> ReadJsonMap(string infile) { throw new NotImplementedException(); }

    public TripleQueryStruct ReadTripleQueryStruct(string infile) { throw new NotImplementedException(); }

    public Library ReadLibrary(string infile) { throw new NotImplementedException(); }

    public Author ReadAuthor(string infile) { throw new NotImplementedException(); }

    public altgraph_web_app.Services.Graph.Graph ReadGraph(string infile) { throw new NotImplementedException(); }
    public void WriteJson(object obj, string outfile, bool pretty, bool verbose) { throw new NotImplementedException(); }
    public void WriteTextFile(string outfile, string text, bool verbose) { throw new NotImplementedException(); }
  }
}