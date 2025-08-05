namespace MyDotNet9Api.DTOs;

public class PaginationDTO
{
    public int Page { get; set; } = 1;
    private int recordsPerPage = 10;
    private int maxAmountOfRecordsPerPage = 50;

    public int RecordsPerPage
    {
        get
        {
            return recordsPerPage;
        }
        set
        {
            recordsPerPage = (value> maxAmountOfRecordsPerPage) ? maxAmountOfRecordsPerPage : value;
        }
    }
}