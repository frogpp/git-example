namespace SDO.Models
{
    public class ParticalModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string NAME { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public dynamic VALUE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public dynamic CONFIG { get; set; }
    }

    public class ConfigDateTimeModel
    {
        /// <summary>
        /// 
        /// </summary>
        public bool DISPLAYTIME { get; set; }
    }

    public class ConfigOrgSelectorModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string PARENT_NODEID { get; set; }

        public bool MULTISELECT { get; set; }
    }
}