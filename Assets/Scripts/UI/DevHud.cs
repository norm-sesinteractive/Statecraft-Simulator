using System.Collections.Generic;
using System.Globalization;
using Statecraft.Game;
using Statecraft.Sim.Commands;
using Statecraft.Sim.Core;
using Statecraft.Sim.Diagnostics;
using Statecraft.Sim.Institutions;
using Statecraft.Sim.Nations;
using Statecraft.Sim.Politics;
using Statecraft.Sim.Society;
using UnityEngine;
using UnityEngine.UIElements;

namespace Statecraft.UI
{
    /// <summary>
    /// Developer HUD, not the player UI: date and speed, the selected nation's levers, its vice
    /// table and outcomes, a "why" breakdown from the ledger, and recent events. Labels are plain
    /// technical ASCII (dev tooling); the shipping UI will carry Norm's text.
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public sealed class DevHud : MonoBehaviour
    {
        /// <summary>Which lever group the left panel shows.</summary>
        enum LeverTab
        {
            Media,
            Education,
            Law,
            Politics,
        }

        const int WhyRows = 6;
        const int EventRows = 5;

        [SerializeField] UIDocument document;
        [SerializeField] SimulationHost host;

        readonly SafeAreaPadding safeArea = new SafeAreaPadding();
        readonly List<Contribution> terms = new List<Contribution>();
        readonly List<Button> speedButtons = new List<Button>();
        readonly List<Button> tabButtons = new List<Button>();
        readonly List<Button> nationChips = new List<Button>();
        readonly Slider[] mediaSliders = new Slider[MediaMix.CategoryCount];
        readonly Label[] mediaShares = new Label[MediaMix.CategoryCount];
        readonly Slider[] curriculumSliders = new Slider[Curriculum.FocusCount];
        readonly Label[] curriculumShares = new Label[Curriculum.FocusCount];
        readonly Label[,] viceCells = new Label[ViceVector.Count, 3];
        readonly Label[] whyLabels = new Label[WhyRows];
        readonly Label[] eventLabels = new Label[EventRows];
        readonly Label[,] generationCells = new Label[3, 2];
        readonly Button[] fomentButtons = new Button[GovernmentState.TypeCount];
        readonly Button[] voteButtons = new Button[GovernmentState.TypeCount];
        Label politicsStatus;
        Meter suspicionMeter;
        Meter shadowMeter;

        VisualElement hudRoot;
        VisualElement nationsBar;
        VisualElement[] leverGroups;
        Label dateLabel;
        Label nationLabel;
        Label leversLocked;
        Slider lawSlider;
        Label lawValue;
        Meter contentmentMeter;
        Meter crimeMeter;
        Meter unrestMeter;
        Meter fearMeter;
        Meter aggressionMeter;
        LeverTab tab = LeverTab.Media;
        int selectedNation = -1;
        long eventsShown = -1;
        bool built;

        /// <summary>One labeled bar in the outcomes section.</summary>
        sealed class Meter
        {
            public Label Value;
            public VisualElement Fill;
            public double Min;
            public double Max;
        }

        void OnEnable()
        {
            if (document == null) document = GetComponent<UIDocument>();
            if (host == null) host = FindAnyObjectByType<SimulationHost>();
            Build();
            if (host != null)
            {
                host.Ticked += Refresh;
                host.SpeedChanged += RefreshSpeedButtons;
            }
        }

        void OnDisable()
        {
            if (host != null)
            {
                host.Ticked -= Refresh;
                host.SpeedChanged -= RefreshSpeedButtons;
            }
            built = false;
        }

        void Start()
        {
            if (host != null && host.Simulation != null) SelectNation(Mathf.Max(0, host.Simulation.World.PlayerHomeIndex));
            RefreshSpeedButtons();
        }

        void Update()
        {
            if (built) safeArea.Apply(hudRoot);
        }

        // ---- construction -------------------------------------------------------------------

        void Build()
        {
            var root = document != null ? document.rootVisualElement : null;
            hudRoot = root?.Q<VisualElement>("hud-root");
            if (hudRoot == null)
            {
                Debug.LogError("[Statecraft] DevHud: the UIDocument has no 'hud-root' element (expected DevHud.uxml).", this);
                return;
            }

            dateLabel = root.Q<Label>("date-label");
            nationLabel = root.Q<Label>("nation-label");
            leversLocked = root.Q<Label>("levers-locked");
            nationsBar = root.Q<VisualElement>("nations-bar");
            root.Q<Button>("prev-nation").clicked += () => SelectNation(selectedNation - 1);
            root.Q<Button>("next-nation").clicked += () => SelectNation(selectedNation + 1);

            BuildSpeedButtons(root.Q<VisualElement>("speed-buttons"));
            BuildLevers(root.Q<VisualElement>("lever-tabs"), root.Q<ScrollView>("levers-scroll"));
            BuildReadouts(root.Q<ScrollView>("readouts-scroll"));

            nationChips.Clear();
            nationsBar.Clear();
            built = true;
            SelectTab(tab);
        }

        void BuildSpeedButtons(VisualElement row)
        {
            row.Clear();
            speedButtons.Clear();
            int count = host != null ? host.SpeedCount : 5;
            for (int i = 0; i < count; i++)
            {
                int speed = i;
                float monthsPerSecond = SimulationHost.MonthsPerSecondAt(i);
                var button = new Button(() => { if (host != null) host.SetSpeed(speed); })
                {
                    text = i == 0 ? "||" : monthsPerSecond.ToString("0", CultureInfo.InvariantCulture) + "x",
                };
                button.AddToClassList("speed-button");
                row.Add(button);
                speedButtons.Add(button);
            }
        }

        void BuildLevers(VisualElement tabRow, ScrollView scroll)
        {
            tabRow.Clear();
            tabButtons.Clear();
            scroll.Clear();

            var tabs = new[] { LeverTab.Media, LeverTab.Education, LeverTab.Law, LeverTab.Politics };
            leverGroups = new VisualElement[tabs.Length];
            foreach (var t in tabs)
            {
                var button = new Button(() => SelectTab(t)) { text = t.ToString() };
                button.AddToClassList("tab-button");
                tabRow.Add(button);
                tabButtons.Add(button);
                leverGroups[(int)t] = new VisualElement();
                scroll.Add(leverGroups[(int)t]);
            }

            var media = leverGroups[(int)LeverTab.Media];
            media.Add(Note("Airtime weights; the % is the share actually aired. Only: all airtime to one row."));
            media.Add(SmallButton("Even", () => SetAll(mediaSliders, -1, OnMediaChanged)));
            for (int i = 0; i < MediaMix.CategoryCount; i++)
            {
                int row = i;
                string label = i < ViceVector.Count ? ((Vice)i).ToString() : "Neutral";
                mediaSliders[i] = AddSlider(media, label, out mediaShares[i], OnMediaChanged, () => SetAll(mediaSliders, row, OnMediaChanged));
            }

            var education = leverGroups[(int)LeverTab.Education];
            education.Add(Note("School weights. Youth train now; adults change a generation later. Only: one focus."));
            education.Add(SmallButton("Even", () => SetAll(curriculumSliders, -1, OnCurriculumChanged)));
            for (int i = 0; i < Curriculum.FocusCount; i++)
            {
                int row = i;
                curriculumSliders[i] = AddSlider(education, ((CurriculumFocus)i).ToString(), out curriculumShares[i], OnCurriculumChanged,
                    () => SetAll(curriculumSliders, row, OnCurriculumChanged));
            }

            var law = leverGroups[(int)LeverTab.Law];
            law.Add(Note("0 pardons all but the most guilty; 1 cracks down ruthlessly."));
            lawSlider = AddSlider(law, "Severity", out lawValue, OnLawChanged);

            var politics = leverGroups[(int)LeverTab.Politics];
            politics.Add(Note("Foment a revolution (months, can fail) or, in a democracy, call a vote. Offered only when possible."));
            politicsStatus = new Label("-");
            politicsStatus.AddToClassList("small");
            politics.Add(politicsStatus);
            for (int k = 0; k < GovernmentState.TypeCount; k++)
            {
                var type = (GovernmentType)k;
                var row = new VisualElement();
                row.AddToClassList("lever-row");
                var label = new Label(type.ToString());
                label.AddToClassList("lever-name");
                fomentButtons[k] = new Button(() => OnFoment(type)) { text = "Foment" };
                voteButtons[k] = new Button(() => OnVote(type)) { text = "Vote" };
                row.Add(label);
                row.Add(fomentButtons[k]);
                row.Add(voteButtons[k]);
                politics.Add(row);
            }
        }

        void BuildReadouts(ScrollView scroll)
        {
            scroll.Clear();

            scroll.Add(SectionTitle("Vices"));
            scroll.Add(TableRow("", "Expr", "Outlet", "Frust", header: true));
            for (int v = 0; v < ViceVector.Count; v++)
            {
                var row = TableRow(((Vice)v).ToString(), "-", "-", "-", header: false);
                for (int c = 0; c < 3; c++) viceCells[v, c] = (Label)row[c + 1];
                scroll.Add(row);
            }

            scroll.Add(SectionTitle("Generations"));
            scroll.Add(TableRow("", "Content", "Unrest", "", header: true));
            for (int c = 0; c < 3; c++)
            {
                var row = TableRow(((Cohort)c).ToString(), "-", "-", "", header: false);
                for (int k = 0; k < 2; k++) generationCells[c, k] = (Label)row[k + 1];
                scroll.Add(row);
            }

            scroll.Add(SectionTitle("Outcomes"));
            contentmentMeter = AddMeter(scroll, "Contentment", -1, 1);
            crimeMeter = AddMeter(scroll, "Crime", 0, 1);
            unrestMeter = AddMeter(scroll, "Unrest", 0, 1);
            fearMeter = AddMeter(scroll, "Fear", 0, 1);
            aggressionMeter = AddMeter(scroll, "Aggression", 0, 1);
            suspicionMeter = AddMeter(scroll, "Gov suspicion", 0, 1);
            shadowMeter = AddMeter(scroll, "Shadow susp.", 0, 1);

            scroll.Add(SectionTitle("Why adult contentment? (largest terms)"));
            for (int i = 0; i < WhyRows; i++)
            {
                whyLabels[i] = new Label(" ");
                whyLabels[i].AddToClassList("small");
                scroll.Add(whyLabels[i]);
            }

            scroll.Add(SectionTitle("Recent events"));
            for (int i = 0; i < EventRows; i++)
            {
                eventLabels[i] = new Label(" ");
                eventLabels[i].AddToClassList("small");
                scroll.Add(eventLabels[i]);
            }
        }

        static Slider AddSlider(VisualElement parent, string name, out Label valueLabel, EventCallback<ChangeEvent<float>> onChange,
            System.Action only = null)
        {
            var row = new VisualElement();
            row.AddToClassList("lever-row");
            var label = new Label(name);
            label.AddToClassList("lever-name");
            var slider = new Slider(0f, 1f);
            slider.AddToClassList("lever-slider");
            slider.RegisterValueChangedCallback(onChange);
            valueLabel = new Label("-");
            valueLabel.AddToClassList("lever-value");
            row.Add(label);
            row.Add(slider);
            row.Add(valueLabel);
            if (only != null) row.Add(SmallButton("Only", only));
            parent.Add(row);
            return slider;
        }

        static Button SmallButton(string text, System.Action onClick)
        {
            var button = new Button(onClick) { text = text };
            button.AddToClassList("small-button");
            return button;
        }

        // Sets one slider to 1 and the rest to 0 (or all to 1 when row is -1), then sends one command.
        void SetAll(Slider[] sliders, int row, EventCallback<ChangeEvent<float>> send)
        {
            if (!CanCommand() || !host.Simulation.World.Nations[selectedNation].PlayerControlled) return;
            for (int i = 0; i < sliders.Length; i++) sliders[i].SetValueWithoutNotify(row < 0 || i == row ? 1f : 0f);
            send(null);
        }

        static Meter AddMeter(VisualElement parent, string name, double min, double max)
        {
            var row = new VisualElement();
            row.AddToClassList("meter-row");
            var label = new Label(name);
            label.AddToClassList("meter-name");
            var track = new VisualElement();
            track.AddToClassList("meter-track");
            var fill = new VisualElement();
            fill.AddToClassList("meter-fill");
            track.Add(fill);
            var value = new Label("-");
            value.AddToClassList("meter-value");
            row.Add(label);
            row.Add(track);
            row.Add(value);
            parent.Add(row);
            return new Meter { Value = value, Fill = fill, Min = min, Max = max };
        }

        static VisualElement TableRow(string name, string a, string b, string c, bool header)
        {
            var row = new VisualElement();
            row.AddToClassList("table-row");
            foreach (string text in new[] { name, a, b, c })
            {
                var cell = new Label(text);
                cell.AddToClassList(row.childCount == 0 ? "cell-name" : "cell");
                if (header) cell.AddToClassList("cell-header");
                row.Add(cell);
            }
            return row;
        }

        static Label SectionTitle(string text)
        {
            var label = new Label(text);
            label.AddToClassList("section-title");
            return label;
        }

        static Label Note(string text)
        {
            var label = new Label(text);
            label.AddToClassList("note");
            return label;
        }

        // ---- selection ----------------------------------------------------------------------

        void SelectTab(LeverTab next)
        {
            tab = next;
            if (leverGroups == null) return;
            for (int i = 0; i < leverGroups.Length; i++)
            {
                leverGroups[i].style.display = i == (int)tab ? DisplayStyle.Flex : DisplayStyle.None;
                tabButtons[i].EnableInClassList("tab-button--active", i == (int)tab);
            }
        }

        void SelectNation(int index)
        {
            if (!built || host == null || host.Simulation == null) return;
            int count = host.Simulation.World.Nations.Count;
            selectedNation = ((index % count) + count) % count;
            SyncLeversFromSim(host.Simulation.World.Nations[selectedNation]);
            Refresh();
        }

        // Sliders hold the player's raw weights; they are only pushed from the sim when the selection changes.
        void SyncLeversFromSim(Nation nation)
        {
            var media = nation.Institutions.Media.ToArray();
            double mediaMax = Max(media);
            for (int i = 0; i < media.Length; i++)
                mediaSliders[i].SetValueWithoutNotify(mediaMax > 0 ? (float)(media[i] / mediaMax) : 0f);

            var curriculum = nation.Institutions.Curriculum.ToArray();
            double curriculumMax = Max(curriculum);
            for (int i = 0; i < curriculum.Length; i++)
                curriculumSliders[i].SetValueWithoutNotify(curriculumMax > 0 ? (float)(curriculum[i] / curriculumMax) : 0f);

            lawSlider.SetValueWithoutNotify((float)nation.Institutions.Law.Severity);

            ApplyEditable(nation.PlayerControlled);
        }

        // Levers lock when the hand does not hold the nation, including after it is overthrown mid-game.
        void ApplyEditable(bool editable)
        {
            foreach (var slider in mediaSliders) slider.SetEnabled(editable);
            foreach (var slider in curriculumSliders) slider.SetEnabled(editable);
            lawSlider.SetEnabled(editable);
            leversLocked.style.display = editable ? DisplayStyle.None : DisplayStyle.Flex;
        }

        // ---- lever input --------------------------------------------------------------------

        void OnMediaChanged(ChangeEvent<float> change)
        {
            if (!CanCommand()) return;
            var weights = new double[MediaMix.CategoryCount];
            for (int i = 0; i < weights.Length; i++) weights[i] = mediaSliders[i].value;
            host.Enqueue(new SetMediaMix(selectedNation, weights));
        }

        void OnCurriculumChanged(ChangeEvent<float> change)
        {
            if (!CanCommand()) return;
            var weights = new double[Curriculum.FocusCount];
            for (int i = 0; i < weights.Length; i++) weights[i] = curriculumSliders[i].value;
            host.Enqueue(new SetCurriculum(selectedNation, weights));
        }

        void OnLawChanged(ChangeEvent<float> change)
        {
            if (!CanCommand()) return;
            host.Enqueue(new SetLawSeverity(selectedNation, change.newValue));
        }

        void OnFoment(GovernmentType target)
        {
            if (CanCommand() && host.Simulation.CanFoment(selectedNation, target)) host.Enqueue(new FomentRevolution(selectedNation, target));
        }

        void OnVote(GovernmentType target)
        {
            if (CanCommand() && host.Simulation.CanCallVote(selectedNation, target)) host.Enqueue(new CallVote(selectedNation, target));
        }

        bool CanCommand() => built && host != null && host.Simulation != null && selectedNation >= 0;

        // ---- refresh ------------------------------------------------------------------------

        void RefreshSpeedButtons()
        {
            if (host == null) return;
            for (int i = 0; i < speedButtons.Count; i++)
                speedButtons[i].EnableInClassList("speed-button--active", i == host.SpeedIndex);
        }

        void Refresh()
        {
            if (!built || host == null || host.Simulation == null) return;
            var simulation = host.Simulation;
            var nations = simulation.World.Nations;
            if (selectedNation < 0 || selectedNation >= nations.Count)
            {
                SelectNation(Mathf.Max(0, simulation.World.PlayerHomeIndex));
                return;
            }

            var nation = nations[selectedNation];
            var society = nation.Society;
            dateLabel.text = (simulation.IsGameOver ? "GAME OVER: hand overthrown. " : "") +
                "Year " + simulation.Date.Year + ", Month " + simulation.Date.Month;
            nationLabel.text = nation.NameKey + (nation.PlayerControlled ? " (yours)" : "") + ", " + nation.Government.Type;
            ApplyEditable(nation.PlayerControlled);

            for (int c = 0; c < 3; c++)
            {
                var cohort = society.GetCohort((Cohort)c);
                generationCells[c, 0].text = Signed(cohort.Contentment);
                generationCells[c, 1].text = F(cohort.Unrest);
                generationCells[c, 1].EnableInClassList("cell--hot", cohort.Unrest >= simulation.Tuning.Politics.RevoltThreshold);
            }
            RefreshPolitics(simulation, nation);

            for (int v = 0; v < ViceVector.Count; v++)
            {
                viceCells[v, 0].text = F(society.Expressed[v]);
                viceCells[v, 1].text = F(society.Outlets[v]);
                viceCells[v, 2].text = F(society.Frustration[v]);
                viceCells[v, 2].EnableInClassList("cell--hot", society.Frustration[v] >= 0.1);
            }

            SetMeter(contentmentMeter, society.Contentment);
            SetMeter(crimeMeter, society.Crime);
            SetMeter(unrestMeter, society.Unrest);
            SetMeter(fearMeter, society.Fear);
            SetMeter(aggressionMeter, society.Aggression);
            SetMeter(suspicionMeter, nation.Government.Suspicion);
            SetMeter(shadowMeter, nation.Government.ShadowSuspicion);

            var institutions = nation.Institutions;
            for (int i = 0; i < MediaMix.CategoryCount; i++)
                mediaShares[i].text = Percent(i < ViceVector.Count ? institutions.Media.Share((Vice)i) : institutions.Media.NeutralShare);
            for (int i = 0; i < Curriculum.FocusCount; i++)
                curriculumShares[i].text = Percent(institutions.Curriculum.Share((CurriculumFocus)i));
            lawValue.text = F(institutions.Law.Severity);

            simulation.Ledger.Collect(selectedNation, Quantity.Contentment, -1, (int)Cohort.Adult, terms);
            for (int i = 0; i < WhyRows; i++)
                whyLabels[i].text = i < terms.Count ? Signed(terms[i].Amount) + "  " + terms[i].Source : " ";

            RefreshEvents(simulation);
            RefreshNationChips(nations);
        }

        void RefreshEvents(Simulation simulation)
        {
            if (simulation.Events.TotalRaised == eventsShown) return;
            eventsShown = simulation.Events.TotalRaised;
            for (int i = 0; i < EventRows; i++)
            {
                if (i >= simulation.Events.Count)
                {
                    eventLabels[i].text = i == 0 ? "(none yet)" : " ";
                    continue;
                }
                var e = simulation.Events.GetRecent(i);
                eventLabels[i].text = e.Date + " " + simulation.World.Nations[e.NationIndex].NameKey + " " + e.Kind + Detail(e) + " (" + F(e.Value) + ")";
            }
        }

        static string Detail(SimEvent e)
        {
            if (e.Detail < 0) return "";
            switch (e.Kind)
            {
                case SimEventKind.RevoltSucceeded: return " by " + (Cohort)e.Detail + " to " + (GovernmentType)(int)e.Value;
                case SimEventKind.RevoltCrushed:
                case SimEventKind.HiddenHandOverthrown: return " by " + (Cohort)e.Detail;
                default: return " " + (GovernmentType)e.Detail;
            }
        }

        // Buttons are shown enabled only when the sim would accept the command (never offer an input it would ignore).
        void RefreshPolitics(Simulation simulation, Nation nation)
        {
            var government = nation.Government;
            string status = government.Type + " for " + (government.MonthsInPower / 12) + " y";
            if (government.Pending != PendingChange.None)
                status += "; " + government.Pending + " toward " + government.PendingTarget + ", " + government.PendingMonths + " month(s) left";
            else if (government.Cooldown > 0)
                status += "; next change possible in " + government.Cooldown + " month(s)";
            if (government.HiddenHandOverthrown) status += "; the hand was overthrown here";
            politicsStatus.text = status;

            bool canVoteHere = simulation.Tuning.Politics.Types[(int)government.Type].CanVote;
            for (int k = 0; k < GovernmentState.TypeCount; k++)
            {
                var type = (GovernmentType)k;
                fomentButtons[k].SetEnabled(simulation.CanFoment(nation.Index, type));
                voteButtons[k].SetEnabled(simulation.CanCallVote(nation.Index, type));
                voteButtons[k].style.display = canVoteHere ? DisplayStyle.Flex : DisplayStyle.None;
            }
        }

        void RefreshNationChips(IReadOnlyList<Nation> nations)
        {
            if (nationChips.Count != nations.Count)
            {
                nationsBar.Clear();
                nationChips.Clear();
                for (int i = 0; i < nations.Count; i++)
                {
                    int index = i;
                    var chip = new Button(() => SelectNation(index));
                    chip.AddToClassList("nation-chip");
                    nationsBar.Add(chip);
                    nationChips.Add(chip);
                }
            }

            for (int i = 0; i < nations.Count; i++)
            {
                var s = nations[i].Society;
                nationChips[i].text = nations[i].NameKey + "  C " + Signed(s.Contentment) + "  Cr " + F(s.Crime) + "  U " + F(s.Unrest);
                nationChips[i].EnableInClassList("nation-chip--selected", i == selectedNation);
            }
        }

        static void SetMeter(Meter meter, double value)
        {
            meter.Value.text = meter.Min < 0 ? Signed(value) : F(value);
            double fraction = (value - meter.Min) / (meter.Max - meter.Min);
            meter.Fill.style.width = Length.Percent(Mathf.Clamp01((float)fraction) * 100f);
        }

        static double Max(double[] values)
        {
            double max = 0;
            foreach (double v in values)
                if (v > max) max = v;
            return max;
        }

        static string F(double value) => value.ToString("0.00", CultureInfo.InvariantCulture);

        static string Signed(double value) => value.ToString("+0.00;-0.00;0.00", CultureInfo.InvariantCulture);

        static string Percent(double share) => (share * 100).ToString("0", CultureInfo.InvariantCulture) + "%";
    }
}
