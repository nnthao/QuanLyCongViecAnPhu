using System.Collections.Generic;

namespace Dashboard.Models.ViewModel
{
    public class DataTableResult<T>
    {
        public DataTableResult()
        {
            rows = new List<T>();
        }
        public List<T> rows { get; set; }
        public int total { get; set; }
        public int totalNotFiltered
        {
            get
            {
                return total;
            }
        }
    }
}