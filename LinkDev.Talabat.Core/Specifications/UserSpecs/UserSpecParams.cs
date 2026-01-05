namespace LinkDev.Talabat.Core.Specifications.UserSpecs
{
    public class UserSpecParams
    {
        private const int MaxPageSize = 10;
        private int pageSize = 5;
        private string? role;
        private string? displayName;

        public string? Role
        {
            get => role;
            set => role = value?.ToLower();
        }
        public string? DisplayName
        {
            get => displayName;
            set => displayName = value?.ToLower();
        }
        public string? Sort { get; set; }
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }
        public int PageIndex { get; set; } = 1;
    }
}
