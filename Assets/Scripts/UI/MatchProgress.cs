using UnityEngine;
using UnityEngine.UIElements;

using UnitWarfare.Core.Enums;

namespace UnitWarfare.UI
{
    public class MatchProgress : DataBindedUIComponent<MatchProgress.Data>
    {
        private System.Func<float> timer;

        private readonly Label l_name;
        private readonly Label l_timer;
        private readonly Button b_skip;
        private readonly VisualElement ve_progressBar;

        private const string NAME_LABEL_NAME = "name_label";
        private const string TIMER_LABEL_NAME = "timer";
        private const string SKIP_BUTTON_NAME = "skip";
        private const string PROGRESS_NAME = "progress";

        public delegate void Event();
        public event Event OnSkip;

        private float startTime;

        public MatchProgress(Config config) : base(config)
        {
            l_name = root.Q<Label>(NAME_LABEL_NAME);
            l_timer = root.Q<Label>(TIMER_LABEL_NAME);
            b_skip = root.Q<Button>(SKIP_BUTTON_NAME);
            ve_progressBar = root.Q<VisualElement>(PROGRESS_NAME);

            b_skip.clicked += () => { OnSkip?.Invoke(); };

            emb.OnUpdate += Update;
        }

        public record Data(string PlayerName, PlayerIdentification Player, System.Func<float> Timer);

        protected override string documentPath => "UI/Match/match_timer";

        protected override void OnShow(Data data)
        {
            timer = data.Timer;
            startTime = timer.Invoke();
            l_name.text = data.PlayerName;
            if (data.Player.Equals(PlayerIdentification.PLAYER))
                b_skip.style.display = DisplayStyle.Flex;
            else
                b_skip.style.display = DisplayStyle.None;
            ve_progressBar.style.width = Length.Percent(100);
            l_timer.text = GetTime(timer.Invoke());
        }

        private void Update()
        {
            if (!Showing)
                return;
            float time = timer.Invoke();
            ve_progressBar.style.width = Length.Percent((time / startTime) * 100);
            l_timer.text = GetTime(time);
        }

        private string GetTime(float time)
        {
            int minutes;
            try
            {
                minutes = (int)(time / 60);
            }
            catch
            {
                minutes = 0;
            }

            time -= (minutes * 60f);

            if (minutes == 0)
                return $"{(int)time}s";
            else
                return $"{minutes}m{(int)time}s";
        }
    }
}
