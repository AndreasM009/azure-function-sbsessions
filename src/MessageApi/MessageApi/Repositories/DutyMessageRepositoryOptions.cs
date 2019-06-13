using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageApi.Repositories
{
    public class DutyMessageRepositoryOptions
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
    }
}
