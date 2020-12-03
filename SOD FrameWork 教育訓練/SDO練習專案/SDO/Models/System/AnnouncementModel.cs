using System;
using System.ComponentModel.DataAnnotations;

namespace SDO.Models
{
    public class AnnouncementModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string SID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "Announcement_TITLE", ResourceType = typeof(i18N.Label))]
        public string TITLE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "Announcement_COMMENT", ResourceType = typeof(i18N.Label))]
        public string COMMENT { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "Announcement_DATE", ResourceType = typeof(i18N.Label))]
        public DateTime EFFECTIVE_DATE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EFFECTIVE_TWDATE
        {
            get
            {
                return string.Format("{0}/{1}", this.EFFECTIVE_DATE.Year - 1911, this.EFFECTIVE_DATE.ToString("MM/dd"));
            }
            set { }
        }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "Announcement_DATE", ResourceType = typeof(i18N.Label))]
        public DateTime EXPIRE_DATE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EXPIRE_TWDATE
        {
            get
            {
                return string.Format("{0}/{1}", this.EXPIRE_DATE.Year - 1911, this.EXPIRE_DATE.ToString("MM/dd"));
            }
            set { }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "Announcement_ATTACHMENT", ResourceType = typeof(i18N.Label))]
        public string ATTACH_NAME { get; set; }
    }
}