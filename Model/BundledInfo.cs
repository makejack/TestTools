using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class BundledInfo
    {
        public int Bid { get; set; }

        public int Cid { get; set; }
        /// <summary>
        /// 主卡编号
        /// </summary>
        public string HostCardNumber { get; set; }

        public int Vid { get; set; }
        /// <summary>
        /// 副卡编号
        /// </summary>
        public string ViceCardNumber { get; set; }
        
    }
}
