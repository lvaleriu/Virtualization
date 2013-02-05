#region

using System.Windows.Controls;
using System.Windows.Data;

#endregion

namespace DVFilterSort
{
    public static class DataGridHelper
    {
        public static string GetSortMemberPath(DataGridColumn column)
        {
            string sortPropertyName = column.SortMemberPath;
            if (string.IsNullOrEmpty(sortPropertyName))
            {
                var boundColumn = column as DataGridBoundColumn;
                if (boundColumn != null)
                {
                    var binding = boundColumn.Binding as Binding;
                    if (binding != null)
                    {
                        if (!string.IsNullOrEmpty(binding.XPath))
                        {
                            sortPropertyName = binding.XPath;
                        }
                        else if (binding.Path != null)
                        {
                            sortPropertyName = binding.Path.Path;
                        }
                    }
                }
            }

            return sortPropertyName;
        }
    }
}