using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helpers
{
    public static class TreeHelper
    {
        public static void AddWithChildren<T>(
            T parent,
            List<T> all,
            List<T> result,
            HashSet<Guid> visited,
            Func<T, Guid> getId,
            Func<T, Guid?> getParentId,
            Func<T, int> getSortOrder)
        {
            if (!visited.Add(getId(parent))) return;

            result.Add(parent);

            var children = all
                .Where(x => getParentId(x).HasValue && getParentId(x) == getId(parent))
                .OrderBy(getSortOrder)
                .ToList();

            foreach (var child in children)
            {
                AddWithChildren(child, all, result, visited, getId, getParentId, getSortOrder);
            }
        }
    }

}
