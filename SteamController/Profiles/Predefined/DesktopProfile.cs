using WindowsInput;

namespace SteamController.Profiles.Predefined
{
    public sealed class DesktopProfile : Default.BackPanelShortcutsProfile
    {
        private const String Consumed = "DesktopProfileOwner";

        public DesktopProfile()
        {
            IsDesktop = true;
        }

        public override System.Drawing.Icon Icon
        {
            get
            {
                if (CommonHelpers.WindowsDarkMode.IsDarkModeEnabled)
                    return Resources.monitor_white;
                else
                    return Resources.monitor;
            }
        }

        internal override ProfilesSettings.BackPanelSettings BackPanelSettings
        {
            get { return ProfilesSettings.DesktopPanelSettings.Default; }
        }

        public override bool Selected(Context context)
        {
            return context.Enabled;
        }

        public override Status Run(Context c)
        {
            if (base.Run(c).IsDone)
            {
                return Status.Done;
            }

            /*
            if (c.KeyboardMouseValid)
            {
                c.Steam.LizardButtons = SettingsDebug.Default.LizardButtons;
                c.Steam.LizardMouse = SettingsDebug.Default.LizardMouse;
            }
            else
            {
                // Failed to acquire secure context
                // Enable emergency Lizard
                c.Steam.LizardButtons = true;
                c.Steam.LizardMouse = true;
            }
            */
            c.Steam.LizardButtons = false;
            c.Steam.LizardMouse = false;

            EmulateScrollOnLPad(c);
            EmulateScrollOnLStick(c);
            EmulateMouseOnRPad(c);
            EmulateMouseOnRStick(c);
            EmulateDPadArrows(c);

            c.Keyboard[VirtualKeyCode.RETURN] = c.Steam.BtnA;
            c.Keyboard[VirtualKeyCode.BACK] = c.Steam.BtnB;
            c.Keyboard[VirtualKeyCode.ESCAPE] = c.Steam.BtnX;
            // c.Keyboard[VirtualKeyCode.SPACE] = c.Steam.BtnY;

            return Status.Continue;
        }

        private void EmulateScrollOnLStick(Context c)
        {
            if (c.Steam.LeftThumbX)
            {
                c.Mouse.HorizontalScroll(
                    c.Steam.LeftThumbX.GetDeltaValue(
                        Context.ThumbToWhellSensitivity,
                        Devices.DeltaValueMode.AbsoluteTime,
                        Settings.Default.DesktopJoystickDeadzone
                    )
                );
            }
            if (c.Steam.LeftThumbY)
            {
                c.Mouse.VerticalScroll(
                    c.Steam.LeftThumbY.GetDeltaValue(
                        Context.ThumbToWhellSensitivity * (double)Settings.Default.ScrollDirection,
                        Devices.DeltaValueMode.AbsoluteTime,
                        Settings.Default.DesktopJoystickDeadzone
                    )
                );
            }
        }

        private void EmulateDPadArrows(Context c)
        {
            c.Keyboard[VirtualKeyCode.LEFT] = c.Steam.BtnDpadLeft;
            c.Keyboard[VirtualKeyCode.RIGHT] = c.Steam.BtnDpadRight;
            c.Keyboard[VirtualKeyCode.UP] = c.Steam.BtnDpadUp;
            c.Keyboard[VirtualKeyCode.DOWN] = c.Steam.BtnDpadDown;
        }
    }
}
