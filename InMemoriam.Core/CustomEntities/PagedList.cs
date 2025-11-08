using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoriam.Core.CustomEntities
{
    public class PagedList<T>
    {
        public IEnumerable<T> Items { get; }
        public int TotalCount { get; }
        public int PageSize { get; }
        public int PageNumber { get; }

        public PagedList(IEnumerable<T> items, int total, int pageSize, int pageNumber)
        { Items = items; TotalCount = total; PageSize = pageSize; PageNumber = pageNumber; }
    }
}
