using Cassandra;
using System.Linq;
using System.Collections.Generic;

namespace URL_Shortcut.Utils
{
    public static class Helper
    {
        public static Row GetFirstRow(RowSet rowSet)
        {
            // Conver RowSet into List
            List<Row> rowList = rowSet.GetRows().ToList<Row>();

            // Return null if list is empty
            if (rowList.Count == 0)
            {
                return null;
            }

            // Get the first row
            Row row = rowList[0];

            return row;
        }
    }
}
