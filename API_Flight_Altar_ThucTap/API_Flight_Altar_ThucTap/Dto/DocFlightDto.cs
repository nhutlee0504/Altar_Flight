namespace API_Flight_Altar_ThucTap.Dto
{
    public class DocFlightDto
    {
        public int IdDocument { get; set; }
        public string DocumentName { get; set; }
        public byte[] FileContent { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string Status { get; set; }
    }
}
