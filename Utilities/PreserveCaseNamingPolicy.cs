using System.Text.Json;

namespace LicentaBackend.Utilities
{
    public class PreserveCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            
            return name;
        }
    }
}
