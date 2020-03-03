using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Discord;
using Discord.WebSocket;
using IslaBot.Discord;
using IslaBot.Discord.Entities;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using MySql.Data.MySqlClient;

namespace IslaBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public int _timer = 0;
        public static string connectionString;

        public static ListBox _memlist;
        public static ListBox _output;
        public static ListBox _channels;
        public static TextBox _input;
        public static Label _mcount;
        public static Label _latency;
        public static Dictionary<string, ulong> _chandata = new Dictionary<string, ulong>();

        public dynamic streamTextBoxWriter;
        public MainWindow()
        {
            connectionString = ConfigurationManager.ConnectionStrings["ODIN.Properties.Settings.Database1ConnectionString"].ConnectionString;

            Unity.RegisterTypes();
            InitializeComponent();
            _memlist = MemberList;
            _output = ChatLog;
            _mcount = MembersCount;
            _latency = LatencyLabel;
            _input = TextBox;

            _chandata.Add("General", 521492937931227136);
            _chandata.Add("Offtopic", 589513825653489692);
            _chandata.Add("Processing", 557061019848015882);
            _chandata.Add("Officer", 556330051126427649);
            _chandata.Add("News", 556329436396650529);

            _channels = Channels;
            _channels.ItemsSource = _chandata;
          
            #region Redirect Text Output
            streamTextBoxWriter = new TextBoxStreamWriter(_memlist);
            System.Console.SetOut(streamTextBoxWriter);
            #endregion
            Console.WriteLine("");

            var result = Main();
            
        }
        public async Task Main()
        {

            ApplTitle.Content = "ODIN Version 1.0.0";
            var Config = new IslaConfig
            {
                Token = "NDg1NzUxNDM5NDM2ODA4MTky.Dm1HLQ.1wJItNnxpQBVc_dNdoG674XWrSI",
                SocketConfig = SocketConfig.GetDefault()
                
            };

            var _connection = Unity.Resolve<Connection>();
            await _connection.ConnectAsync(Config);
            
            while (true)
            {
                _timer++;
                TimeSpan t = TimeSpan.FromSeconds(_timer);

                string answer = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}",
                                t.Days,
                                t.Hours,
                                t.Minutes,
                                t.Seconds);
                Timer.Content = "Bot Uptime: " + answer;
                
                await Task.Delay(1000);
            };
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(1);
        }

        public void LeftMouseDown_Event(object sender, EventArgs e)

        {
            this.DragMove();
        }

        public static void SetPop(string e)
        {
            _mcount.Dispatcher.BeginInvoke((Action)(() => { _mcount.Content = "Server Pop: " + e; }));
        }
        public static void SetLat(string e)
        {
            _latency.Dispatcher.BeginInvoke((Action)(() => { _latency.Content = "Latency: " + e; }));
        }
        public static void ChatLogUpd(string e)
        {
            _output.Dispatcher.BeginInvoke((Action)(() => { _output.Items.Insert(0, e); }));
 
        }
        public static void ChanUpd(string e, string e2)
        {
        

        }


        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }



        private void Test_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (_channels.SelectedItem is KeyValuePair<string, ulong>)
            {
                
                KeyValuePair<string, ulong> temp1 = (KeyValuePair<string, ulong>)_channels.SelectedItem;
                //Console.WriteLine(temp1.Value + "|" +temp1.Key);
                Connection._chan = temp1.Value;
            }
            
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            Connection.SendRemoteMsg(_input.Text);
        }

        private void loadData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(MainWindow.connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand("select DiscId,RobloxId, Exp, Rank from Profiles", connection))
                    {
                        SqlDataAdapter adp = new SqlDataAdapter(cmd);
                        DataSet ds = new DataSet();
                        adp.Fill(ds, "DatabaseLink");
                        dataGrid.DataContext = ds;

                    }
                }
                
            }
            catch (Exception err)
            {
                MessageBox.Show("Error: " + err);
            }

        }
    }
}
