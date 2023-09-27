using ExternalHelpers;
using WindowsInput;

namespace SteamController.Profiles.Default
{
    public abstract class ShortcutsProfile : Profile
    {
        public const String ShortcutConsumed = "ShortcutsProfile";
        protected readonly TimeSpan HoldShort = TimeSpan.FromMilliseconds(300);
        protected readonly TimeSpan Hold = TimeSpan.FromSeconds(1);
        protected readonly TimeSpan HoldLong = TimeSpan.FromSeconds(3);

        public override Status Run(Context c)
        {
            /*
            // Steam + 3 dots simulate CTRL+SHIFT+ESCAPE
            if (c.Steam.BtnSteam.Hold(Hold, ShortcutConsumed) && c.Steam.BtnQuickAccess.HoldOnce(Hold, ShortcutConsumed))
            {
                // Simulate CTRL+ALT+DELETE behavior (not working)
                // c.Keyboard.KeyPress(new VirtualKeyCode[] { VirtualKeyCode.LCONTROL, VirtualKeyCode.LMENU }, VirtualKeyCode.DELETE);
                // We can send CTRL+SHIFT+ESCAPE to bring up Task Manager at least
                c.Keyboard.KeyPress(new VirtualKeyCode[] { VirtualKeyCode.LCONTROL, VirtualKeyCode.SHIFT }, VirtualKeyCode.ESCAPE);
                return Status.Done;
            }
            */

            if (c.Steam.BtnSteam.Hold(HoldShort,
                                      ShortcutConsumed))
            {
                if (SteamShortcuts(c))
                {
                    return Status.Done;
                }
            }

            // if(c.Steam.BtnQuickAccess.Hold(Press, ShortcutConsumed))
            if (c.Steam.BtnQuickAccess.Pressed())
            {
                switch (Settings.Default.KeyboardStyle)
                {
                    case Settings.KeyboardStyles.OnScreenKeyboard:

                        c.Keyboard.KeyPress(new VirtualKeyCode[]
                                            {
                                                VirtualKeyCode.LCONTROL,
                                                VirtualKeyCode.LWIN
                                            },
                                            VirtualKeyCode.VK_O);

                        break;

                    case Settings.KeyboardStyles.TouchKeyboard:

                        // Toggle on screen keyboard
                        if (!OnScreenKeyboard.Toggle())
                        {
                            // Fallback to CTRL+WIN+O
                            c.Keyboard.KeyPress(new VirtualKeyCode[]
                                                {
                                                    VirtualKeyCode.LCONTROL,
                                                    VirtualKeyCode.LWIN
                                                },
                                                VirtualKeyCode.VK_O);
                        }

                        break;
                }

                return Status.Done;
            }

            /*

            // Always consume 3 dots
            if(c.Steam.BtnQuickAccess.Hold(Press, ShortcutConsumed))
            {
                return Status.Done;
            }

            // Hold options for 1s to use next profile, or 3 seconds to switch between desktop-mode
            if(c.Steam.BtnOptions.HoldOnce(PressLong, ShortcutConsumed))
            {
                if (!c.SelectNext())
                    c.BackToDefault();
                return Status.Done;
            }
            else if (c.Steam.BtnOptions.HoldChain(PressExtraLong, ShortcutConsumed, "SwitchToDesktop"))
            {
                c.BackToDefault();
                return Status.Done;
            }
            */

            return Status.Continue;
        }

        protected virtual bool SteamShortcuts(Context c)
        {
            if (c.Steam.BtnQuickAccess.Pressed())
            {
                c.Keyboard.KeyPress(WindowsInput.VirtualKeyCode.LWIN,
                                    WindowsInput.VirtualKeyCode.VK_G);

                return true;
            }

            if (c.Steam.BtnQuickAccess.HoldOnce(Hold,
                                                ShortcutConsumed))
            {
                c.Keyboard.KeyPress(new VirtualKeyCode[]
                                    {
                                        VirtualKeyCode.LCONTROL,
                                        VirtualKeyCode.SHIFT
                                    },
                                    VirtualKeyCode.ESCAPE);

                return true;
            }
            
            if (c.Steam.BtnOptions.Pressed())
            {
                c.Keyboard.KeyPress(VirtualKeyCode.APPS);

                return true;
            }

            if (c.Steam.BtnMenu.Pressed())
            {
                c.Keyboard.KeyPress(VirtualKeyCode.LWIN,
                                    VirtualKeyCode.TAB);

                return true;
            }

            return false;
        }
    }
}