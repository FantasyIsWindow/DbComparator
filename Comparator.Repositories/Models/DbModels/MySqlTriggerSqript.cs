
namespace Comparator.Repositories.Models.DbModels
{
    public class MySqlTriggerSqript
    {
        public string TRIGGER_NAME { get; set; }

        public string ACTION_TIMING { get; set; }

        public string EVENT_MANIPULATION { get; set; }

        public string EVENT_OBJECT_TABLE { get; set; }

        public string ACTION_ORIENTATION { get; set; }

        public string ACTION_STATEMENT { get; set; }
    }
}
