using SQLite;

namespace DefectRecording.Helpers
{
    [Table("Defects")]
    public class Defect
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        [MaxLength(500)]
        public string ImgName { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }
    }
}