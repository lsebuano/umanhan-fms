namespace Umanhan.Reports.Api
{
    public interface ISchemaProvider
    {
        /// <summary>
        /// Returns the full schema description in a single string,
        /// e.g. "Table: financials …\nTable: farms …"
        /// </summary>
        Task<string> GetSchemaAsync(CancellationToken ct = default);
    }
}
