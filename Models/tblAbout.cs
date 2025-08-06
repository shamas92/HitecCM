namespace HitecCM.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    [Table("tblAbout")]
    public partial class tblAbout
    {
        public int id { get; set; }

        [AllowHtml]
        [Required]
        public string chairman_message { get; set; }

        [AllowHtml]
        [Required]
        public string vc_message { get; set; }

        [AllowHtml]
        [Required]
        public string motto { get; set; }

        [AllowHtml]
        [Required]
        public string vision { get; set; }

        [AllowHtml]
        [Required]
        public string mission { get; set; }

        [AllowHtml]
        [Required]
        public string emblem { get; set; }

        public string chairman_image { get; set; }
        public string vc_image { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
