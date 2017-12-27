namespace MicroElements.FileStorage.Tests
{
    public class DataTable
    {
        public string[] Columns { get; set; }
        public DataRow[] Rows { get; set; }
    }

    public class DataRow
    {
        public int[] Values { get; set; }
    }
}