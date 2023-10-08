using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUvrs.Models.Views
{
    public class DefaultMainPageViewModel : MainPageViewModel
    {
        public new string GUVRS_GAME_ID { get; set; } = "-1";
        public new string GUVRS_PLAYER_NAME { get; set; } = "...";
        public new string GUVRS_PLAYER_GUID { get; set; } = "-1";
        public new string GUVRS_PLAYER_RANK { get; set; } = "Unknown";
        public new string GUVRS_PLAYER_RANK_PROGRESS { get; set; } = "0";
        public new string GUVRS_PLAYER_DECKCODE { get; set; } = "No deck code found yet";

        public new string GUVRS_OPPONENT_NAME { get; set; } = "...";
        public new string GUVRS_OPPONENT_GUID { get; set; } = "-1";
        public new string GUVRS_OPPONENT_RANK { get; set; } = "Unknown";
        public new string GUVRS_OPPONENT_RANK_PROGRESS { get; set; } = "0";
        public new string GUVRS_OPPONENT_DECKCODE { get; set; } = "No deck code found yet";
    }
}
