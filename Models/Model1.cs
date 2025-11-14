using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace HitecCM.Models
{
    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        // Core entities still present
        public virtual DbSet<AcademicCalendar> AcademicCalendars { get; set; }
        public virtual DbSet<AdverQsement> AdverQsements { get; set; }
        public virtual DbSet<tblDownloadsNew> tblDownloadsNews { get; set; }
        public virtual DbSet<tblFeeStructure> tblFeeStructures { get; set; }
        public virtual DbSet<tblAccomodation> tblAccomodations { get; set; }
        public virtual DbSet<tblAdmTransport> tblAdmTransports { get; set; }

        // Site structure/content entities (tbl*)
        public virtual DbSet<tblAbout> tblAbouts { get; set; }
        public virtual DbSet<tblBMEOBECurriculum> tblBMEOBECurriculums { get; set; }
        public virtual DbSet<tblCsOBECurriculum> tblCsOBECurriculums { get; set; }
        public virtual DbSet<tblDepartment> tblDepartments { get; set; }
        public virtual DbSet<tblDepartmentChairperson> tblDepartmentChairpersons { get; set; }
        public virtual DbSet<tblDepartmentContact> tblDepartmentContacts { get; set; }
        public virtual DbSet<tblDepartmentFacility> tblDepartmentFacilities { get; set; }
        public virtual DbSet<tblDepartmentFacultyAbout> tblDepartmentFacultyAbouts { get; set; }
        public virtual DbSet<tblDepartmentFAQ> tblDepartmentFAQs { get; set; }
        public virtual DbSet<tblDepartmentPastEvent> tblDepartmentPastEvents { get; set; }
        public virtual DbSet<tblDepartmentProgram> tblDepartmentPrograms { get; set; }
        public virtual DbSet<tblDepartmentProgramStudy> tblDepartmentProgramStudies { get; set; }
        public virtual DbSet<tblDepartmentSchemeOfStudy> tblDepartmentSchemeOfStudies { get; set; }
        public virtual DbSet<tblDepartmentSuccessStory> tblDepartmentSuccessStories { get; set; }
        public virtual DbSet<tblDepartmentTeachingFaculty> tblDepartmentTeachingFaculties { get; set; }
        public virtual DbSet<tblDepartmentUpcomingEvent> tblDepartmentUpcomingEvents { get; set; }
        public virtual DbSet<tblDesignation> tblDesignations { get; set; }
        public virtual DbSet<tblDownload> tblDownloads { get; set; }
        public virtual DbSet<tblEvent> tblEvents { get; set; }
        public virtual DbSet<tblEventHead> tblEventHeads { get; set; }
        public virtual DbSet<tblEventPhoto> tblEventPhotos { get; set; }
        public virtual DbSet<tblEventVideo> tblEventVideos { get; set; }
        public virtual DbSet<tblFacilite> tblFacilites { get; set; }
        public virtual DbSet<tblFacilityPhoto> tblFacilityPhotos { get; set; }
        public virtual DbSet<tblFacultiesET> tblFacultiesETs { get; set; }
        public virtual DbSet<tblFaculty> tblFaculties { get; set; }
        public virtual DbSet<tblFAQ> tblFAQs { get; set; }
        public virtual DbSet<tblFinancialAssistance> tblFinancialAssistances { get; set; }
        public virtual DbSet<tblLatePaymentPenalty> tblLatePaymentPenalties { get; set; }
        public virtual DbSet<tblMenus> tblMenus { get; set; }
        public virtual DbSet<tblMenuType> tblMenuTypes { get; set; }
        public virtual DbSet<tblMiscellaneousCharge> tblMiscellaneousCharges { get; set; }
        public virtual DbSet<tblNew> tblNews { get; set; }
        public virtual DbSet<tblNewsHead> tblNewsHeads { get; set; }
        public virtual DbSet<tblNewsPhoto> tblNewsPhotos { get; set; }
        public virtual DbSet<tblNoticeBoard> tblNoticeBoards { get; set; }
        public virtual DbSet<tblPolicy> tblPolicys { get; set; }
        public virtual DbSet<tblPolicyCategory> tblPolicyCategories { get; set; }
        public virtual DbSet<tblPolicyItem> tblPolicyItems { get; set; }
        public virtual DbSet<tblQAC> tblQACs { get; set; }
        public virtual DbSet<tblQACAccreditation> tblQACAccreditations { get; set; }
        public virtual DbSet<tblQACAnnualActivityCalender> tblQACAnnualActivityCalenders { get; set; }
        public virtual DbSet<tblQACContact> tblQACContacts { get; set; }
        public virtual DbSet<tblQACIPE> tblQACIPEs { get; set; }
        public virtual DbSet<tblQACOrganogram> tblQACOrganograms { get; set; }
        public virtual DbSet<tblQACPGPR> tblQACPGPRs { get; set; }
        public virtual DbSet<tblQACRIPE> tblQACRIPEs { get; set; }
        public virtual DbSet<tblQACSAR> tblQACSARs { get; set; }
        public virtual DbSet<tblQACSurvay> tblQACSurvays { get; set; }
        public virtual DbSet<tblQAPolicy> tblQAPolicies { get; set; }
        public virtual DbSet<tblSchemeOfStudy> tblSchemeOfStudies { get; set; }
        public virtual DbSet<tblSeOBECurriculum> tblSeOBECurriculums { get; set; }
        public virtual DbSet<tblSuccessStory> tblSuccessStories { get; set; }
        public virtual DbSet<tblTransport> tblTransports { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<tblVisitSession> tblVisitSessions { get; set; }
        public virtual DbSet<tblPageView> tblPageViews { get; set; }

        // Newly added academic pages
        public virtual DbSet<tblAcademicDeficiency> tblAcademicDeficiencies { get; set; }
        public virtual DbSet<tblAcademicRegulation> tblAcademicRegulations { get; set; }
        public virtual DbSet<tblRulesRegulationsMS> tblRulesRegulationsMS { get; set; }
        public virtual DbSet<tblRulesRegulationsPhd> tblRulesRegulationsPhds { get; set; }
        public virtual DbSet<tblRefundPolicy> tblRefundPolicies { get; set; }

        // New facility and service pages
        public virtual DbSet<tblUniversityFacilities> tblUniversityFacilities { get; set; }
        public virtual DbSet<tblSports> tblSports { get; set; }
        public virtual DbSet<tblHostel> tblHostels { get; set; }
        public virtual DbSet<tblCafeteria> tblCafeterias { get; set; }

        // New HR management pages
        public virtual DbSet<tblHrClassificationServices> tblHrClassificationServices { get; set; }
        public virtual DbSet<tblHrESTR> tblHrESTRs { get; set; }
        public virtual DbSet<tblHrPerks> tblHrPerks { get; set; }
        public virtual DbSet<tblHrLeaves> tblHrLeaves { get; set; }
        public virtual DbSet<tblHrContributoryFund> tblHrContributoryFunds { get; set; }
        public virtual DbSet<tblHrDiscipline> tblHrDisciplines { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Explicitly map tblUser to tblUsers table
            modelBuilder.Entity<tblUser>().ToTable("tblUsers");

            // Relationships
            modelBuilder.Entity<tblDepartment>()
                .HasMany(e => e.tblFaculties)
                .WithRequired(e => e.tblDepartment)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblDepartmentSchemeOfStudy>()
                .HasMany(e => e.tblDepartmentProgramStudies)
                .WithOptional(e => e.tblDepartmentSchemeOfStudy)
                .HasForeignKey(e => e.ProgramStudyId);

            modelBuilder.Entity<tblEvent>()
                .HasMany(e => e.tblEventPhotos)
                .WithOptional(e => e.tblEvent)
                .WillCascadeOnDelete();

            modelBuilder.Entity<tblEvent>()
                .HasMany(e => e.tblEventVideos)
                .WithOptional(e => e.tblEvent)
                .WillCascadeOnDelete();

            // Simple text mappings
            modelBuilder.Entity<tblBMEOBECurriculum>().Property(e => e.CurriculumTitle).IsUnicode(false);
            modelBuilder.Entity<tblCsOBECurriculum>().Property(e => e.CurriculumTitle).IsUnicode(false);
            modelBuilder.Entity<tblDepartmentContact>().Property(e => e.ContactTitle).IsUnicode(false);
            modelBuilder.Entity<tblDepartmentContact>().Property(e => e.ContactInfo).IsUnicode(false);
            modelBuilder.Entity<tblDepartmentFacility>().Property(e => e.FacilityTitle).IsUnicode(false);
            modelBuilder.Entity<tblDepartmentFacility>().Property(e => e.FacilityInfo).IsUnicode(false);
            modelBuilder.Entity<tblDepartmentPastEvent>().Property(e => e.EventTitle).IsUnicode(false);
            modelBuilder.Entity<tblDepartmentPastEvent>().Property(e => e.EventDetails).IsUnicode(false);
            modelBuilder.Entity<tblDepartmentProgram>().Property(e => e.ProgramsInfo).IsUnicode(false);
            modelBuilder.Entity<tblDepartmentProgramStudy>().Property(e => e.CourseCode).IsUnicode(false);
            modelBuilder.Entity<tblDepartmentProgramStudy>().Property(e => e.CourseTitle).IsUnicode(false);
            modelBuilder.Entity<tblDepartmentProgramStudy>().Property(e => e.CourseCreditHours).IsUnicode(false);
            modelBuilder.Entity<tblDepartmentSchemeOfStudy>().Property(e => e.ProgramTitle).IsUnicode(false);
            modelBuilder.Entity<tblDepartmentSchemeOfStudy>().Property(e => e.ProgramStudyTitle).IsUnicode(false);
            modelBuilder.Entity<tblDepartmentSuccessStory>().Property(e => e.PersonName).IsUnicode(false);
            modelBuilder.Entity<tblDepartmentSuccessStory>().Property(e => e.SuccessDetails).IsUnicode(false);
            modelBuilder.Entity<tblDepartmentTeachingFaculty>().Property(e => e.TeacherInfo).IsUnicode(false);
            modelBuilder.Entity<tblDepartmentUpcomingEvent>().Property(e => e.EventTitle).IsUnicode(false);
            modelBuilder.Entity<tblDepartmentUpcomingEvent>().Property(e => e.EventDetails).IsUnicode(false);
            modelBuilder.Entity<tblFacultiesET>().Property(e => e.VisionText).IsUnicode(false);
            modelBuilder.Entity<tblFacultiesET>().Property(e => e.MissionText).IsUnicode(false);
            modelBuilder.Entity<tblPolicyCategory>().Property(e => e.CategoryName).IsUnicode(false);
            modelBuilder.Entity<tblPolicyCategory>().Property(e => e.CategoryDescription).IsUnicode(false);
            modelBuilder.Entity<tblPolicyItem>().Property(e => e.ItemTitle).IsUnicode(false);
            modelBuilder.Entity<tblPolicyItem>().Property(e => e.ItemDescription).IsUnicode(false);
            modelBuilder.Entity<tblPolicyItem>().Property(e => e.FileName).IsUnicode(false);
            modelBuilder.Entity<tblQAC>().Property(e => e.PageTitle).IsUnicode(false);
            modelBuilder.Entity<tblQAC>().Property(e => e.PageText).IsUnicode(false);
            modelBuilder.Entity<tblQACAccreditation>().Property(e => e.PageTitle).IsUnicode(false);
            modelBuilder.Entity<tblQACAccreditation>().Property(e => e.PageText).IsUnicode(false);
            modelBuilder.Entity<tblQACContact>().Property(e => e.PageTitle).IsUnicode(false);
            modelBuilder.Entity<tblQACContact>().Property(e => e.PageText).IsUnicode(false);
            modelBuilder.Entity<tblQACIPE>().Property(e => e.PageTitle).IsUnicode(false);
            modelBuilder.Entity<tblQACIPE>().Property(e => e.PageText).IsUnicode(false);
            modelBuilder.Entity<tblQACRIPE>().Property(e => e.PageTitle).IsUnicode(false);
            modelBuilder.Entity<tblQACRIPE>().Property(e => e.PageText).IsUnicode(false);
            modelBuilder.Entity<tblQACOrganogram>().Property(e => e.PageTitle).IsUnicode(false);
            modelBuilder.Entity<tblQACOrganogram>().Property(e => e.PageText).IsUnicode(false);
            modelBuilder.Entity<tblQACPGPR>().Property(e => e.PageTitle).IsUnicode(false);
            modelBuilder.Entity<tblQACPGPR>().Property(e => e.PageText).IsUnicode(false);
            modelBuilder.Entity<tblQACSAR>().Property(e => e.PageTitle).IsUnicode(false);
            modelBuilder.Entity<tblQACSAR>().Property(e => e.PageText).IsUnicode(false);
            modelBuilder.Entity<tblQACSurvay>().Property(e => e.PageTitle).IsUnicode(false);
            modelBuilder.Entity<tblQACSurvay>().Property(e => e.PageText).IsUnicode(false);
            modelBuilder.Entity<tblSeOBECurriculum>().Property(e => e.CurriculumTitle).IsUnicode(false);

            // Downloads/Policy simple mapping
            modelBuilder.Entity<tblDownloadsNew>().Property(e => e.PageTitle).IsUnicode(false);
            modelBuilder.Entity<tblDownloadsNew>().Property(e => e.PageData).IsUnicode(false);
            modelBuilder.Entity<tblPolicy>().Property(e => e.PageTitle).IsUnicode(false);
            modelBuilder.Entity<tblPolicy>().Property(e => e.PageData).IsUnicode(false);

            // Admissions text pages
            modelBuilder.Entity<tblAcademicDeficiency>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblAcademicDeficiency>().Property(e => e.PageData).IsUnicode(false);
            modelBuilder.Entity<tblAcademicRegulation>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblAcademicRegulation>().Property(e => e.PageData).IsUnicode(false);
            modelBuilder.Entity<tblRulesRegulationsMS>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblRulesRegulationsMS>().Property(e => e.PageData).IsUnicode(false);
            modelBuilder.Entity<tblRulesRegulationsPhd>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblRulesRegulationsPhd>().Property(e => e.PageData).IsUnicode(false);
            modelBuilder.Entity<tblFeeStructure>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblFeeStructure>().Property(e => e.PageData).IsUnicode(false);
            modelBuilder.Entity<tblAccomodation>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblAccomodation>().Property(e => e.PageData).IsUnicode(false);
            modelBuilder.Entity<tblAdmTransport>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblAdmTransport>().Property(e => e.PageData).IsUnicode(false);
            modelBuilder.Entity<tblRefundPolicy>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblRefundPolicy>().Property(e => e.PageData).IsUnicode(false);

            // New facility and service pages mapping
            modelBuilder.Entity<tblUniversityFacilities>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblUniversityFacilities>().Property(e => e.PageData).IsUnicode(false);
            modelBuilder.Entity<tblSports>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblSports>().Property(e => e.PageData).IsUnicode(false);
            modelBuilder.Entity<tblHostel>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblHostel>().Property(e => e.PageData).IsUnicode(false);
            modelBuilder.Entity<tblCafeteria>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblCafeteria>().Property(e => e.PageData).IsUnicode(false);
            
            // New tblTransport mapping (Facilities section)
            modelBuilder.Entity<tblTransport>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblTransport>().Property(e => e.PageData).IsUnicode(false);

            // New HR pages mapping
            modelBuilder.Entity<tblHrClassificationServices>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblHrClassificationServices>().Property(e => e.PageData).IsUnicode(false);
            modelBuilder.Entity<tblHrESTR>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblHrESTR>().Property(e => e.PageData).IsUnicode(false);
            modelBuilder.Entity<tblHrPerks>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblHrPerks>().Property(e => e.PageData).IsUnicode(false);
            modelBuilder.Entity<tblHrLeaves>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblHrLeaves>().Property(e => e.PageData).IsUnicode(false);
            modelBuilder.Entity<tblHrContributoryFund>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblHrContributoryFund>().Property(e => e.PageData).IsUnicode(false);
            modelBuilder.Entity<tblHrDiscipline>().Property(e => e.Title).IsUnicode(false);
            modelBuilder.Entity<tblHrDiscipline>().Property(e => e.PageData).IsUnicode(false);
        }
    }
}
