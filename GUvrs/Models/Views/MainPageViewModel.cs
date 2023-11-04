using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUvrs.Models.Views
{
    public class MainPageViewModel
    {
        public string GUVRS_GAME_ID { get; set; } = "-1";
        public string GUVRS_PLAYER_NAME { get; set; } = "...";
        public string GUVRS_PLAYER_GUID { get; set; } = "-1";
        public string GUVRS_PLAYER_RATING { get; set; } = "-1";
        public string GUVRS_PLAYER_WINPOINTS { get; set; } = "-1";
        public string GUVRS_PLAYER_LOSSPOINTS { get; set; } = "-1";
        public string GUVRS_PLAYER_SAFELINE { get; set; } = "-1";

        public string GUVRS_OPPONENT_NAME { get; set; } = "...";
        public string GUVRS_OPPONENT_GUID { get; set; } = "-1";
        public string GUVRS_OPPONENT_RATING { get; set; } = "-1";
        public string GUVRS_OPPONENT_WINPOINTS { get; set; } = "-1";
        public string GUVRS_OPPONENT_LOSSPOINTS { get; set; } = "-1";
        public string GUVRS_OPPONENT_SAFELINE { get; set; } = "-1";
    }
}
