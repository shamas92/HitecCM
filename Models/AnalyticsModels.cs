using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace HitecCM.Models
{
    [Table("tblVisitSession")]
    public partial class tblVisitSession
    {
        public tblVisitSession()
        {
            PageViewItems = new HashSet<tblPageView>();
        }

        [Key]
        public long VisitSessionId { get; set; }

        [Required]
        [StringLength(64)]
        public string SessionKey { get; set; }

        public int? UserId { get; set; }

        public DateTime FirstSeenUtc { get; set; }

        public DateTime LastSeenUtc { get; set; }

        public int PageViews { get; set; }

        [MaxLength(32)]
        public byte[] IpHash { get; set; }

        [StringLength(256)]
        public string UserAgent { get; set; }

        [StringLength(255)]
        public string ReferrerHost { get; set; }

        public bool IsBot { get; set; }

        // Navigation
        [InverseProperty("VisitSession")]
        public virtual ICollection<tblPageView> PageViewItems { get; set; }
    }

    [Table("tblPageView")]
    public partial class tblPageView
    {
        [Key]
        public long PageViewId { get; set; }

        [ForeignKey("VisitSession")]
        public long VisitSessionId { get; set; }

        public int? UserId { get; set; }

        [Required]
        [StringLength(300)]
        public string Path { get; set; }

        public DateTime ViewUtc { get; set; }

        public short StatusCode { get; set; }

        // Navigation
        public virtual tblVisitSession VisitSession { get; set; }
    }
}