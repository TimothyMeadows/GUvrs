using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GUvrs.Models
{
    public class PlayerRankModel
    {
        [JsonPropertyName("user_id")]
        public int ID { get; set; }

        [JsonPropertyName("game_mode")]
        public int? GameMode { get; set; }

        [JsonPropertyName("rating")]
        public double? Rating { get; set; }

        [JsonPropertyName("rank_level")]
        public double? Rank { get; set; }

        [JsonPropertyName("win_points")]
        public double? WinPoints { get; set; }

        [JsonPropertyName("loss_points")]
        public double? LossPoints { get; set; }

        [JsonPropertyName("safety_line")]
        public double? SafetyLine { get; set; }
    }
}
