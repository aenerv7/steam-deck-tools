namespace PowerControl.Menu
{
    public class MenuItemWithOptions : MenuItem
    {
        public delegate object CurrentValueDelegate();
        public delegate object[] OptionsValueDelegate();
        public delegate object ApplyValueDelegate(object selected);

        public IList<Object> Options { get; set; } = new List<Object>();
        public Object SelectedOption { get; set; }
        public Object ActiveOption { get; set; }
        public int ApplyDelay { get; set; }
        public bool CycleOptions { get; set; } = true;

        public CurrentValueDelegate CurrentValue { get; set; }
        public OptionsValueDelegate OptionsValues { get; set; }
        public ApplyValueDelegate ApplyValue { get; set; }
        public CurrentValueDelegate ResetValue { get; set; }

        private System.Windows.Forms.Timer delayTimer;
        private ToolStripMenuItem toolStripItem;

        public MenuItemWithOptions()
        {
            this.Selectable = true;
        }

        public override void Reset()
        {
            if (ResetValue == null)
                return;

            var resetOption = ResetValue();
            if (resetOption == null || resetOption.Equals(ActiveOption))
                return;

            SelectedOption = resetOption;
            onApply();
        }

        public override void Update()
        {
            if (CurrentValue != null)
            {
                var result = CurrentValue();
                if (result != null)
                {
                    ActiveOption = result;
                    Visible = true;
                }
                else
                {
                    Visible = false;
                }
            }

            if (OptionsValues != null)
            {
                var result = OptionsValues();
                if (result != null)
                {
                    Options = result.ToList();
                    updateOptions();
                }
                else
                {
                    Visible = false;
                }
            }

            if (ActiveOption == null && Options.Count > 0)
                ActiveOption = Options.First();

            onUpdateToolStrip();
        }

        private void scheduleApply()
        {
            if (delayTimer != null)
                delayTimer.Stop();

            if (ApplyDelay == 0)
            {
                onApply();
                return;
            }

            delayTimer = new System.Windows.Forms.Timer();
            delayTimer.Interval = ApplyDelay > 0 ? ApplyDelay : 1;
            delayTimer.Tick += delegate (object? sender, EventArgs e)
            {
                if (delayTimer != null)
                    delayTimer.Stop();

                onApply();
            };
            delayTimer.Enabled = true;
        }

        private void onApply()
        {
            if (ApplyValue != null)
                ActiveOption = ApplyValue(SelectedOption);
            else
                ActiveOption = SelectedOption;

            SelectedOption = null;

            onUpdateToolStrip();
        }

        private void onUpdateToolStrip()
        {
            if (toolStripItem == null)
                return;

            foreach (ToolStripMenuItem item in toolStripItem.DropDownItems)
                item.Checked = Object.Equals(item.Tag, SelectedOption ?? ActiveOption);

            toolStripItem.Visible = Visible && Options.Count > 0;
        }

        private void updateOptions()
        {
            if (toolStripItem == null)
                return;

            toolStripItem.DropDownItems.Clear();

            foreach (var option in Options)
            {
                var optionItem = new ToolStripMenuItem(option.ToString());
                optionItem.Tag = option;
                optionItem.Click += delegate (object? sender, EventArgs e)
                {
                    SelectedOption = option;
                    onApply();
                };
                toolStripItem.DropDownItems.Add(optionItem);
            }
        }

        public override void CreateMenu(ToolStripItemCollection collection)
        {
            if (toolStripItem != null)
                return;

            toolStripItem = new ToolStripMenuItem();
            toolStripItem.Text = Name;
            updateOptions();
            collection.Add(toolStripItem);
        }

        private void SelectIndex(int index)
        {
            if (Options.Count == 0)
                return;

            SelectedOption = Options[Math.Clamp(index, 0, Options.Count - 1)];
            scheduleApply();
        }

        public override void SelectNext()
        {
            int index = Options.IndexOf(SelectedOption ?? ActiveOption);
            if (index < 0)
                SelectIndex(0); // select first
            else if (CycleOptions)
                SelectIndex((index + 1) % Options.Count);
            else
                SelectIndex(index + 1);
        }

        public override void SelectPrev()
        {
            int index = Options.IndexOf(SelectedOption ?? ActiveOption);
            if (index < 0)
                SelectIndex(Options.Count - 1); // select last
            else if (CycleOptions)
                SelectIndex((index - 1 + Options.Count) % Options.Count);
            else
                SelectIndex(index - 1);
        }

        private String optionText(Object option)
        {
            String text;

            if (option == null)
                text = Color("?", Colors.White);
            else if (Object.Equals(option, SelectedOption ?? ActiveOption))
                text = Color(option.ToString(), Colors.Red);
            else if (Object.Equals(option, ActiveOption))
                text = Color(option.ToString(), Colors.White);
            else
                text = Color(option.ToString(), Colors.Green);

            return text;
        }

        public override string Render(MenuItem selected)
        {
            string output = "";

            if (selected == this)
                output += Color(Name + ":", Colors.White).PadRight(30);
            else
                output += Color(Name + ":", Colors.Blue).PadRight(30);

            output += optionText(SelectedOption ?? ActiveOption);

            if (SelectedOption != null && !Object.Equals(ActiveOption, SelectedOption))
                output += " (active: " + optionText(ActiveOption) + ")";

            return output;
        }
    }
}