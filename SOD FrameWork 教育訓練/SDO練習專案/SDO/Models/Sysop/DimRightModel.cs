using System.ComponentModel.DataAnnotations;

namespace SDO.Models
{
    public class DimRightModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "DimRight_RIGHT_ID", ResourceType = typeof(i18N.Label))]
        public string RIGHT_ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "DimRight_RIGHT_NAME", ResourceType = typeof(i18N.Label))]
        public string RIGHT_NAME { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string RIGHT_DISPLAY
        {
            get
            {
                return string.Format("{0}-{1}", this.RIGHT_ID, this.RIGHT_NAME);
            }
            private set { }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DEL_FLG { get; set; }
    }
}