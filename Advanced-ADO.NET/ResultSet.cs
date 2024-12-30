using System.Collections.Generic;

namespace AdvancedADO
{
    public class ResultSet
    {
        public int ResultIndex { get; set; } = 0;
        public List<dynamic> ResultData = new List<dynamic>();
    }
}
