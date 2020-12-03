using System.ComponentModel.DataAnnotations;

namespace SDO.Models
{
    public class SetFunctionModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "SetFunction_FUNCTION_ID", ResourceType = typeof(i18N.Label))]
        public string FUNCTION_ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "SetFunction_FUNCTION_NAME", ResourceType = typeof(i18N.Label))]
        public string FUNCTION_NAME { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string FUNCTION_DISPLAY
        {
            get
            {
                return string.Format("{0}-{1}", this.FUNCTION_ID, this.FUNCTION_NAME);
            }
            private set { }
        }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "SetFunction_FUNCTION_URL", ResourceType = typeof(i18N.Label))]
        public string FUNCTION_URL { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "SetFunction_PARENT_ID", ResourceType = typeof(i18N.Label))]
        public string PARENT_ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "SetFunction_SORT_ID", ResourceType = typeof(i18N.Label))]
        public int SORT_ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool DEL_FLG { get; set; }
    }
}