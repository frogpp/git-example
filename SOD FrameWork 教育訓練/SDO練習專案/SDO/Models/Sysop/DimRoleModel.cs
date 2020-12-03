using System.ComponentModel.DataAnnotations;

namespace SDO.Models
{
    public class DimRoleModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "DimRole_ROLE_ID", ResourceType = typeof(i18N.Label))]
        public string ROLE_ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "DimRole_ROLE_NAME", ResourceType = typeof(i18N.Label))]
        public string ROLE_NAME { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ROLE_DISPLAY
        {
            get
            {
                return string.Format("{0}-{1}", this.ROLE_ID, this.ROLE_NAME);
            }
            private set { }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DEL_FLG { get; set; }
    }
}