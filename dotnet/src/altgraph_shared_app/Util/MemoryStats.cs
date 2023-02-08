using System.Text;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace altgraph_shared_app.Util
{
  public class MemoryStats : IMemoryStats
  {
    // Class variables:
    //private static List<MemoryStats> history = new List<MemoryStats>();
    //private static bool shouldRecordHistory = false;

    // Instance variables
    public double Epoch { get; set; }
    // public string Note { get; set; } = string.Empty;
    public double TotalMb { get; set; }
    public double FreeMb { get; set; }
    public double MaxMb { get; set; }
    public double PctFree { get; set; }
    public double PctMax { get; set; }
    public double TotalPerDoc { get; set; }
    private ILogger<MemoryStats> _logger;

    // public static void SetShouldRecordHistory(bool b)
    // {
    //   //shouldRecordHistory = b;
    // }

    public static void DisplayHistory(string desc)
    {
      // _logger.LogDebug("MemoryStats displayHistory:");

      // for (int i = 0; i < history.size(); i++)
      // {
      //   _logger.LogDebug(history.get(i).asJson(false));
      // }
    }

    public static void WriteHistory(string desc)
    {
      // string path = Constants.MEMORY_STATS_BASE + desc + ".csv";
      // _logger.LogDebug("write memory stats to: " + path);

      // try (FileOutputStream out = new FileOutputStream(path);
      // OutputStreamWriter writer = new OutputStreamWriter(out)) {

      //   Iterator<MemoryStats> it = history.iterator();
      //   int count = 0;
      //   while (it.hasNext())
      //   {
      //     count++;
      //     MemoryStats ms = it.next();
      //     if (count == 1)
      //     {
      //       writer.write(ms.asDelimitedHeaderLine(","));
      //       writer.write(System.lineSeparator());
      //     }
      //     writer.write(ms.asDelimitedDataLine(","));
      //     writer.write(System.lineSeparator());
      //   }
      // }
    }

    public MemoryStats(ILogger<MemoryStats> logger)
    {
      _logger = logger;
      // if (note == null)
      // {
      //   note = "";
      // }
      // else
      // {
      //   note = note.Replace(',', ' ');
      // }
      Epoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
      using (Process proc = Process.GetCurrentProcess())
      {
        TotalMb = proc.WorkingSet64 / Constants.MB;
        FreeMb = (proc.WorkingSet64 - GC.GetTotalMemory(false)) / Constants.MB;
        MaxMb = proc.PeakWorkingSet64 / Constants.MB;
      }
      PctFree = FreeMb / TotalMb;
      PctMax = TotalMb / MaxMb;

      // if (shouldRecordHistory)
      // {
      //   history.add(this);
      // }
    }

    public void Display()
    {
      // NumberFormat df = NumberFormat.getInstance();
      // df.setMaximumFractionDigits(0);

      // NumberFormat pf = NumberFormat.getInstance();
      // df.setMaximumFractionDigits(12);

      _logger.LogDebug("MemoryStats:");
      _logger.LogDebug($"  epoch:     {Epoch}");
      _logger.LogDebug($"  totalMb:   {TotalMb}");
      _logger.LogDebug($"  freeMb:    {FreeMb}");
      _logger.LogDebug($"  maxMb:     {MaxMb}");
      _logger.LogDebug($"  pctFree:   {PctFree}");
      _logger.LogDebug($"  pctMax:    {PctMax}");
      // _logger.LogDebug($"  note:      {Note}");
    }

    public string AsDelimitedHeaderLine(string delim)
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("creationEpoch");
      sb.Append(delim);
      sb.Append("totalMb");
      sb.Append(delim);
      sb.Append("freeMb");
      sb.Append(delim);
      sb.Append("maxMb");
      sb.Append(delim);
      sb.Append("pctFree");
      sb.Append(delim);
      sb.Append("pctMax");
      // sb.Append(delim);
      // sb.Append("note");
      return sb.ToString();
    }

    public string AsDelimitedDataLine(string delim)
    {
      // NumberFormat nf = NumberFormat.getInstance();
      // nf.setMaximumFractionDigits(0);
      // nf.setGroupingUsed(false);

      // NumberFormat df = NumberFormat.getInstance();
      // df.setMaximumFractionDigits(12);
      // df.setGroupingUsed(false);

      StringBuilder sb = new StringBuilder();
      sb.Append(Epoch);
      sb.Append(delim);
      sb.Append(TotalMb);
      sb.Append(delim);
      sb.Append(FreeMb);
      sb.Append(delim);
      sb.Append(MaxMb);
      sb.Append(delim);
      sb.Append(PctFree);
      sb.Append(delim);
      sb.Append(PctMax);
      // sb.Append(delim);
      // sb.Append(Note);
      return sb.ToString();
    }
  }
}