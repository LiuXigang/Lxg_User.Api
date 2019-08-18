using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace User.Api.Model
{
    public class BPfile
    {
        [Key]
        //BP Id
        public int Id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public int AppUserId { get; set; }
        public string FileName { get; set; }
        /// <summary>
        /// 上传的源文件地址
        /// </summary>
        public string OriginFilePath { get; set; }

        /// <summary>
        /// 格式转化后的文件地址
        /// </summary>
        public string FromatFilePath { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(TypeName = "datetime")]
        public DateTime CreationTime { get; set; }


    }
}
