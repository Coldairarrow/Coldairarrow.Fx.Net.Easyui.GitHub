using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.DevManage
{
    /// <summary>
    /// 项目类型表
    /// </summary>
    [Table("Dev_ProjectType")]
    public class Dev_ProjectType
    {

        /// <summary>
        /// 自然主键
        /// </summary>
        [Key]
        public String Id { get; set; }

        /// <summary>
        /// 项目类型Id
        /// </summary>
        public String ProjectTypeId { get; set; }

        /// <summary>
        /// 项目类型名
        /// </summary>
        public String ProjectTypeName { get; set; }

    }
}