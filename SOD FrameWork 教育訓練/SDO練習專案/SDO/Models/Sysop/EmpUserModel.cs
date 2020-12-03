using SDO.Dacs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SDO.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class EmpUserModel : ICloneable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "帳號")]
        public string USER_ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name = "姓名")]
        public string USER_NAME { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string USER_DISPLAY
        {
            get
            {
                return string.Format("{0}-{1}", this.USER_ID, this.USER_NAME);
            }
            private set { }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SEND_USER_ID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.AGENT_ID))
                {
                    return this.USER_ID;
                }
                else
                {
                    return string.Format("{0}*{1}", this.AGENT_ID, this.USER_ID);
                }
            }
            private set { }
        }


        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "電子郵件")]
        [EmailAddress]
        public string USER_EMAIL { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "密碼")]
        public string USER_PWD { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Display(Name = "確認密碼")]
        [System.ComponentModel.DataAnnotations.Compare("USER_PWD")]
        public string CONFIRM_USER_PWD { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string USER_IP { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AGENT_ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool DEL_FLG { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public EmpOrgModel GetEmpOrg()
        {
            using (EmpOrgDac Dac = new EmpOrgDac((EmpUserModel)this.Clone()))
            {
                return Dac.ReadByUser(this.USER_ID);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SelectList GetEmpAgents()
        {
            using (EmpUserDac Dac = new EmpUserDac((EmpUserModel)this.Clone()))
            {
                return new SelectList(Dac.ReadByAgent(this.USER_ID), "USER_ID", "USER_DISPLAY");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<DimRoleModel> GetEmpRoles()
        {
            using (DimRoleDac Dac = new DimRoleDac((EmpUserModel)this.Clone()))
            {
                return Dac.ReadByUser(this.USER_ID);
            }
        }
    }
}