using SDO.Dacs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SDO.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class EmpAgentModel
    {
        /// <summary>
        /// 
        /// </summary>
        public Int64 SID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string USER_ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "EmpAgent_AGENT_ID", ResourceType = typeof(i18N.Label))]
        public string AGENT_ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AGENT_NAME { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "EmpAgent_AGENT_FROM", ResourceType = typeof(i18N.Label))]
        public DateTime AGENT_FROM { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AGENT_FROM_TWDATE
        {
            get
            {
                return string.Format("{0}/{1}", this.AGENT_FROM.Year - 1911, this.AGENT_FROM.ToString("MM/dd"));
            }
            set { }
        }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "EmpAgent_AGENT_TO", ResourceType = typeof(i18N.Label))]
        public DateTime AGENT_TO { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AGENT_TO_TWDATE
        {
            get
            {
                return string.Format("{0}/{1}", this.AGENT_TO.Year - 1911, this.AGENT_TO.ToString("MM/dd"));
            }
            set { }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DEL_FLG { get; set; }
    }
}