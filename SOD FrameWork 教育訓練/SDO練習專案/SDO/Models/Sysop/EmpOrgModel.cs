using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SDO.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class EmpOrgModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "EmpOrg_ORG_ID", ResourceType = typeof(i18N.Label))]
        public string ORG_ID { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "EmpOrg_ORG_NAME", ResourceType = typeof(i18N.Label))]
        public string ORG_NAME { get; set; }

        public string ORG_DISPLAY
        {
            get
            {
                return string.Format("{0}-{1}", this.ORG_ID, this.ORG_NAME);
            }
            private set { }
        }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "EmpOrg_PARENT_ID", ResourceType = typeof(i18N.Label))]
        public string PARENT_ID { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public bool DEL_FLG { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool HASCHILDREN { get; set; }
    }
}
