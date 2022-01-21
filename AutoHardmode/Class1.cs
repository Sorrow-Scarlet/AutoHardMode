using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
/// <summary>
/// the detailed thing can be seen in readme.md
/// 详见readme_zh.md
/// </summary>
namespace Plugin
{
    #region info
    [ApiVersion(2, 1)]
    public class Plugin : TerrariaPlugin
    {

        public override string Author => "Sorrow";

        public override string Description => "本插件可在打通肉山后，经过四天自动开肉";

        public override string Name => "AutoHardmode";
        public override Version Version => new Version(1, 0, 0, 0);

        public Plugin(Main game) : base(game)
        {
        }
        #endregion 
        
        
        /// <summary>
        /// in the following sections,you will see how the plugin will work
        /// </summary>
        public static bool judger = false;
        public static int countdown = 4;//96 hours == 4 days
        /// <summary>
        /// initialize 2 commands and no hook
        /// </summary>
        public override void Initialize()
        {
            Commands.ChatCommands.Add(new Command(
                permissions: new List<string> { "check.hm" },
                cmd: this.Cmd,
                "check"));
            Commands.ChatCommands.Add(new Command(
                permissions: new List<string> { "athm.hm" },
                cmd: this.CmdAthm,
                "athm"
                ));
        }
        #region Dispose
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Deregister hooks here
            }
            base.Dispose(disposing);
        }
        #endregion
        private void CmdAthm(CommandArgs Args)
        {
            Args.Player.SendSuccessMessage("Athm bashed,{0} seconds left", countdown);
            Console.WriteLine("Now start the plugin");
            
            Timer WofCd = new Timer()
            {
                Interval = 86400000,//86400000ms=1 day
                Enabled = true, 
                AutoReset = false
            };

            //elasped the timer with private method
            WofCd.Elapsed += new ElapsedEventHandler(OntimedEvent);
            WofCd.Start();

          

        }
        
        private static void OntimedEvent(object source, ElapsedEventArgs e)
        {
            if (countdown > 0)
            {
                countdown--;
            }
            else if (countdown == 0)
            {
              
                string path = @"tshock\config.json";

                string tempContent = File.ReadAllText(path);
                tempContent = Regex.Replace(tempContent, "\"DisableHardmode\": true,", "\"DisableHardmode\": false,");
                File.WriteAllText(path, tempContent);

              
                Commands.HandleCommand(TSPlayer.Server, "/reload");

                Console.WriteLine("WoF is unlocked");
                judger = true;


                string tempContent2 = File.ReadAllText(path);
                tempContent2 = Regex.Replace(tempContent2, "\"DisableHardmode\": false,", "\"DisableHardmode\": true,");
                File.WriteAllText(path, tempContent2);
            }

        }
        /// <summary>
        /// to check WoF states
        /// </summary>
        /// <param name="args"></param>
        private void Cmd(CommandArgs args)
        {
            if (judger == false)
            {
                args.Player.SendSuccessMessage("剩余{0}天解锁肉山,先别开袋子",countdown);
            }
            else if (judger == true)
            {
                args.Player.SendSuccessMessage("肉山已解锁,再打一次肉山可开袋子");
            }
        }

    }
}