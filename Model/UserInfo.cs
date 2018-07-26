using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class UserInfo
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public int UserNumber { get; set; }

        public string Description { get; set; }

        private DateTime _RecordTime;

        public DateTime RecordTime
        {
            get
            {
                if (_RecordTime <= DateTime.MinValue)
                {
                    _RecordTime = DateTime.Now;
                }
                return _RecordTime;
            }
            set
            {
                if (value <= DateTime.MinValue)
                {
                    value = DateTime.Now;
                }
                _RecordTime = value;
            }
        }

    }
}
