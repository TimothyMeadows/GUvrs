﻿using System;
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
        public new string GUVRS_PLAYER_RATING { get; set; } = "-1";

        public new string GUVRS_OPPONENT_NAME { get; set; } = "...";
        public new string GUVRS_OPPONENT_GUID { get; set; } = "-1";
        public new string GUVRS_OPPONENT_RATING { get; set; } = "-1";
    }
}
