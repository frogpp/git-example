using System.Web;

namespace SDO.Models
{
    public class RtnResultModel
    {
        /// <summary>
        /// execute success?
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private string _message;
        /// <summary>
        /// return message
        /// </summary>
        public string message
        {
            get
            {
                return HttpUtility.HtmlEncode(_message);
            }
            set
            {
                this._message = value;
            }
        }

        /// <summary>
        /// return object
        /// </summary>
        public dynamic result { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        /// <param name="message"></param>
        /// <param name="result"></param>
        public RtnResultModel(bool success, string message = "", dynamic result = null)
        {
            this.success = success;
            this.message = message;
            this.result = result;
        }
    }
}