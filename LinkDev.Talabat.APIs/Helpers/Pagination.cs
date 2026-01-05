using LinkDev.Talabat.APIs.DTOs;

namespace LinkDev.Talabat.APIs.Helpers
{
    public class Pagination<T>
    {
        public Pagination(int pageIndex, int pageSize, int count, IReadOnlyList<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = count;
            Data = data;
        }

        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int Count { get; set; }
        public IReadOnlyList<T>? Data { get; set; }
    }
}
