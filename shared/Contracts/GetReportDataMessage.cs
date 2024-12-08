public class GetReportDataMessage
{
    public Guid CompanyId { get; set; }
    public Guid ReportRequestId { get; set; }
    public string ReportType { get; set; }
    public string DateSelect { get; set; }
}