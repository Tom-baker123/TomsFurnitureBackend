namespace TomsFurnitureBackend.Common.Contansts
{
    public static class Number
    {
        public struct Pagination
        {
            public const int DefaultPageSize = 150;
            public const int DefaultPageNumber = 1;
            public static readonly int[] DefaultRecordLimit = [10, 25, 50, 100, 150, 200];
        }
    }
}
