using DynamicQueryBuilder.Models;

namespace LibraryManagement.WebApi.Models
{
    public class DynamicQueryOutputDTO<T>
    {
        public DynamicQueryOutputDTO(DynamicQueryOptions options, IEnumerable<T> data, int? datasetCount = null)
        {
            int? count = datasetCount.HasValue ? datasetCount.Value : options.PaginationOption?.DataSetCount;

            Data = data;
            Count = count ?? data.Count();
        }

        public IEnumerable<T> Data { get; set; }

        public int Count { get; set; }
    }
}
