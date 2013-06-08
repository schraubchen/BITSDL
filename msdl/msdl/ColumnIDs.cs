using System.Windows.Forms;

namespace msdl
{
    internal class ColumnIDs
    {
        public int FileName { get; set; }
        public int Progress { get; set; }
        public int Status { get; set; }
        public int ID { get; set; }
        public int Button { get; set; }

        public ColumnIDs(DataGridView grid)
        {
            FileName = grid.Columns["colFileName"].Index;
            Progress = grid.Columns["colProgress"].Index;
            Status = grid.Columns["colStatus"].Index;
            ID = grid.Columns["colID"].Index;
            Button = grid.Columns["colBtn"].Index;
        }
    }
}
