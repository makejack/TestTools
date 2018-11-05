using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bll
{
    /// <summary>
    /// 数据校验
    /// </summary>
    public class DataValidation
    {
        public List<DealHeadEnd> DealHeadEnds;

        private List<byte> m_Bytes;

        public DataValidation() : this(null)
        {
        }

        public DataValidation(DealHeadEnd deal)
        {
            m_Bytes = new List<byte>();
            DealHeadEnds = new List<DealHeadEnd>
            {
                new DealHeadEnd(2, 3)
            };
            if (deal != null)
            {
                DealHeadEnds.Add(deal);
            }
        }

        public byte[] Combination(byte[] by)
        {
            m_Bytes.AddRange(by);

            int startIndex = 0;
            int endIndex = 0;

            by = null;
            foreach (DealHeadEnd item in DealHeadEnds)
            {
                startIndex = m_Bytes.IndexOf((byte)item.Head);

                if (startIndex < 0)
                {
                    continue;
                }
                else
                {
                    endIndex = m_Bytes.IndexOf((byte)item.End);
                    if (endIndex > -1)
                    {
                        if (endIndex > startIndex)
                        {
                            int count = (endIndex - startIndex) + 1;
                            by = new byte[count];
                            m_Bytes.CopyTo(startIndex, by, 0, count);
                            m_Bytes.RemoveRange(0, endIndex + 1);

                            bool ret = Validation(by);
                            if (ret)
                            {
                                break;
                            }
                        }
                        else
                        {
                            m_Bytes.RemoveRange(0, startIndex);
                        }
                    }
                }
            }
            return by;
        }

        private bool Validation(byte[] by)
        {
            int end = by.Length - 3;
            int xor = Utility.Xor(by, 0, end);
            return Contrast(by, xor, end);
        }

        private bool Contrast(byte[] by, int xor, int start)
        {
            byte[] xorBy = Utility.IntToAscii(xor);
            for (int i = 0; i < xorBy.Length; i++)
            {
                if (by[start + i] != xorBy[i])
                {
                    return false;
                }
            }
            return true;
        }

        public void Dispose()
        {
            if (m_Bytes.Count > 0)
            {
                m_Bytes.Clear();
            }
        }
    }

    public class DealHeadEnd
    {
        public DealHeadEnd(int head, int end)
        {
            this.Head = head;
            this.End = end;
        }

        public int Head { get; set; }

        public int End { get; set; }
    }
}
