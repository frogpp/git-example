using System.ComponentModel.DataAnnotations;

namespace SDO.Models
{
    public class SetParamItemModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "SetParamItem_SET_ITEM", ResourceType = typeof(i18N.Label))]
        public string SET_ITEM { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "SetParamItem_SET_ITEM_NAME", ResourceType = typeof(i18N.Label))]
        public string SET_ITEM_NAME { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SET_ITEM_DISPLAY
        {
            get
            {
                return string.Format("{0}-{1}", this.SET_ITEM, this.SET_ITEM_NAME);
            }
            private set { }
        }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "SetParamItem_MEMO", ResourceType = typeof(i18N.Label))]
        [MaxLength(500)]
        public string MEMO { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool EDITABLE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool DEL_FLG { get; set; }
    }

    public class SetParamModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "SetParam_SET_ITEM", ResourceType = typeof(i18N.Label))]
        public string SET_ITEM { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "SetParam_SET_TYPE", ResourceType = typeof(i18N.Label))]
        public string SET_TYPE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "SetParam_SET_VALUE", ResourceType = typeof(i18N.Label))]
        public string SET_VALUE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "SetParam_MEMO", ResourceType = typeof(i18N.Label))]
        [MaxLength(500)]
        public string MEMO { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool DEL_FLG { get; set; }
    }
}