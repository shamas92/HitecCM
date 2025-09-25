namespace HitecCM.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("tblPolicy")]
    public partial class tblPolicy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(255)]
        public string PageTitle { get; set; }

        [Column(TypeName = "nvarchar")]
        public string PageData { get; set; }

        public DateTime? UploadedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
