using ExternalHelpers;

using WindowsInput;

namespace SteamController.Profiles.Default
{
    public abstract class ShortcutsProfile : Profile
    {
        public const String ShortcutConsumed = "ShortcutsProfile";
        public readonly TimeSpan HoldForShorcuts = TimeSpan.FromMilliseconds(200);
        private readonly TimeSpan HoldForLongPress = TimeSpan.FromSeconds(1);
        private readonly TimeSpan HoldToSwitchProfile = TimeSpan.FromSeconds(1);
        private readonly TimeSpan HoldToSwitchDesktop = TimeSpan.FromSeconds(2);

        public override Status Run(Context c)
        {
            // Steam + 3 dots simulate CTRL+SHIFT+ESCAPE
            if (c.Steam.BtnSteam.Hold(HoldForShorcuts, ShortcutConsumed) && c.Steam.BtnQuickAccess.HoldOnce(HoldForShorcuts, ShortcutConsumed))
            {
                // Simulate CTRL+ALT+DELETE behavior (not working)
                // c.Keyboard.KeyPress(new VirtualKeyCode[] { VirtualKeyCode.LCONTROL, VirtualKeyCode.LMENU }, VirtualKeyCode.DELETE);
                // We can send CTRL+SHIFT+ESCAPE to bring up Task Manager at least
                c.Keyboard.KeyPress(new VirtualKeyCode[] { VirtualKeyCode.LCONTROL, VirtualKeyCode.SHIFT }, VirtualKeyCode.ESCAPE);
                return Status.Done;
            }

            /*
            // Hold options for 1s to use next profile, or 3 seconds to switch between desktop-mode
            if (c.Steam.BtnOptions.HoldOnce(HoldToSwitchProfile, ShortcutConsumed))
            {
                if (!c.SelectNext())
                    c.BackToDefault();
                return Status.Done;
            }
            else if (c.Steam.BtnOptions.HoldChain(HoldToSwitchDesktop, ShortcutConsumed, "SwitchToDesktop"))
            {
                c.BackToDefault();
                return Status.Done;
            }

            // Always consume 3 dots
            if (c.Steam.BtnQuickAccess.Hold(HoldForShorcuts, ShortcutConsumed))
            {
                return Status.Done;
            }
            */

            if(c.Steam.BtnQuickAccess.Hold(HoldForLongPress, ShortcutConsumed))
            {
                switch(Settings.Default.KeyboardStyle)
                {
                    case Settings.KeyboardStyles.CTRL_WIN_O:
                        c.Keyboard.KeyPress(new VirtualKeyCode[] { VirtualKeyCode.LCONTROL, VirtualKeyCode.LWIN }, VirtualKeyCode.VK_O);
                        break;

                    case Settings.KeyboardStyles.WindowsTouch:
                        if(!OnScreenKeyboard.Toggle())
                        {
                            // Fallback to CTRL+WIN+O
                            c.Keyboard.KeyPress(new VirtualKeyCode[] { VirtualKeyCode.LCONTROL, VirtualKeyCode.LWIN }, VirtualKeyCode.VK_O);
                        }
                        break;
                }

                return Status.Done;
            }

            if(c.Steam.BtnSteam.Hold(HoldForShorcuts, ShortcutConsumed))
            {
                if(SteamShortcuts(c))
                {
                    return Status.Done;
                }
            }

            return Status.Continue;
        }

        protected virtual bool SteamShortcuts(Context c)
        {
            if(c.Steam.BtnOptions.Pressed())
            {
                // c.Keyboard.KeyPress(VirtualKeyCode.LWIN, VirtualKeyCode.TAB);
                c.Keyboard.KeyPress(VirtualKeyCode.SHIFT, VirtualKeyCode.F10);
                return true;
            }

            
            if(c.Steam.BtnMenu.Pressed())
            {
                // c.Keyboard.KeyPress(VirtualKeyCode.F11);
                c.Keyboard.KeyPress(VirtualKeyCode.LWIN, VirtualKeyCode.TAB);
                return true;
            }

            return false;
        }
    }
}
