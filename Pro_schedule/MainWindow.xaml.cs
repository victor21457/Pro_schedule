using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pro_schedule
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool logined = false;
        public Constants _const = new Constants();
        System.Timers.Timer timer;
        public MainWindow _this;
        ObservableCollection<ProList> collect { get; set; }
        List<ProList> lProlist { get; set; }

        Detail_Form detailForm = null;
        public MainWindow()
        {
            collect = new ObservableCollection<ProList>();
            InitializeComponent();
            ShowLoginForm();
        }

        private void ShowLoginForm()
        {
            this.Visibility = Visibility.Hidden;
            SignInWindow sign = new SignInWindow(_const );
            sign.ShowDialog();
            if ( _const.logined == true)
            {
                lblEmpID.Content = $"{_const.empName} ({_const.empID.ToUpper()})";
                this.Visibility = Visibility.Visible;
                if (_const.proLevel < 90)          
                {
                    stackSession.Visibility = Visibility.Visible;
                    lblSessionTime.Content = _const.proLevel.ToString() + " Mins";
                    timer = new System.Timers.Timer(1000 * 60);     
                    timer.Elapsed += AutoSendArpProc;
                    timer.AutoReset = true;
                    timer.Enabled = true;
                    
                } else
                {
                    stackSession.Visibility = Visibility.Hidden;
                }
                   
                GetListData();
            }
        }

        private void GetListData()
        {
            using (SqlConnection conn = new SqlConnection(_const.conStringList))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("dbo.DLI_WOT_PROD_SCHEDULE_LIST", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();

                //dt_ListView.Items.Clear();
                lProlist = new List<ProList>();
                while (reader.Read())
                {
                    //dt_ListView.Items.Add(new ProList() {
                    //    Image = @"./Resource/arrow.png",
                    //    WO_ID = reader["WO_ID"].ToString(),
                    //    PART_ID = reader["PART_ID"].ToString(),
                    //    PART_DESC = reader["PART_DESC"].ToString(),
                    //    DUE_DATE = DateTime.Parse(reader["DUE_DATE"].ToString()).ToString("yyyy-MM-dd"),
                    //    STATUS = reader["STATUS"].ToString(),
                    //});

                    lProlist.Add(new ProList()
                    {
                        Image = @"./Resource/arrow.png",
                        WO_ID = reader["WO_ID"].ToString(),
                        PART_ID = reader["PART_ID"].ToString(),
                        PART_DESC = reader["PART_DESC"].ToString(),
                        DUE_DATE = DateTime.Parse(reader["DUE_DATE"].ToString()).ToString("yyyy-MM-dd"),
                        STATUS = reader["STATUS"].ToString(),
                    });
                }
                dt_ListView.ItemsSource = lProlist;
                lblTotal.Content = "Totals: " + lProlist.Count.ToString();
            }
        }
        
        private void AutoSendArpProc(object sender, ElapsedEventArgs e)
        {
            _const.proLevel -= 1;
            if (_const.proLevel < 0)
            {
                timer.Dispose();
                _const.logined = false;
                _const.logError = "Your session time was Expired.";
                
                this.Dispatcher.Invoke(() => {
                    CustomMsgWindow message = new CustomMsgWindow("Your session time was Expired.");
                    message.ShowDialog();
                    if (detailForm != null)
                    {
                        detailForm.timer.Dispose();
                        detailForm.Close();
                    }
                    ShowLoginForm();
                });
            }

            this.Dispatcher.Invoke(() => lblSessionTime.Content = _const.proLevel.ToString() + " Mins");
        }

        
        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            ProList obj = ((FrameworkElement)sender).DataContext as ProList;
            this.Visibility = Visibility.Hidden;
            _const.WO_ID = obj.WO_ID;
            detailForm = new Detail_Form(_const);
            detailForm.ShowDialog();
            this.Visibility = Visibility.Visible;
            detailForm = null;
        }
        private void WinClose(object sender, RoutedEventArgs e)
        {
            Close();
            Environment.Exit(0);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }


        string filterWO = "", filterPart = "", filterDesc = "";

        public static readonly DependencyProperty txtWOContent = DependencyProperty.Register("txtWOTxt", typeof(string), typeof(Pro_schedule.MainWindow));
        public static readonly DependencyProperty txtPartContent = DependencyProperty.Register("txtPartTxt", typeof(string), typeof(Pro_schedule.MainWindow));
        public static readonly DependencyProperty txtDescContent = DependencyProperty.Register("txtDescTxt", typeof(string), typeof(Pro_schedule.MainWindow));
        public string txtWOTxt
        {
            get { return (string)GetValue(txtWOContent); }
            set { SetValue(txtWOContent, value); }
        }
        public string txtPartTxt
        {
            get { return (string)GetValue(txtPartContent); }
            set { SetValue(txtPartContent, value); }
        }
        public string txtDescTxt
        {
            get { return (string)GetValue(txtDescContent); }
            set { SetValue(txtDescContent, value); }
        }
        private void btnClear_Clieck(object sender, RoutedEventArgs e)
        {
            txtWOTxt = "";
            txtPartTxt = "";
            txtDescTxt = "";
        }

        private void testWindow_MouseMove(object sender, MouseEventArgs e)
        {
            //var point = e.GetPosition(this);
            if (_const.proLevel != _const.session)
            {
                _const.proLevel = _const.session;
                lblSessionTime.Content = _const.proLevel.ToString() + " Mins";
            }
            
        }

        private void btnLogout(object sender, RoutedEventArgs e)
        {
            if ( timer != null ) timer.Dispose();
            ShowLoginForm();
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            string filter = t.Text;
            FilterAction(filter, t.Name);
        }

        private void FilterAction(string filter, string Obj)
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(dt_ListView.ItemsSource);
            cv.Filter = o =>
            {
                ProList p = o as ProList;
                if (Obj == "txtWO") filterWO = filter;
                if (Obj == "txtPART") filterPart = filter;
                if (Obj == "txtDESC") filterDesc = filter;

                return p.WO_ID.ToUpper().IndexOf(filterWO.ToUpper()) > -1
                        && p.PART_ID.ToUpper().IndexOf(filterPart.ToUpper()) > -1
                        && p.PART_DESC.ToUpper().IndexOf(filterDesc.ToUpper()) > -1;
            };

            lblTotal.Content = "Totals: " + dt_ListView.Items.Count.ToString();
        }
    }

    public class ProList: ObservableCollection<ProList>
    {
        public string Image { get; set; }
        public string WO_ID { get; set; }
        public string PART_ID { get; set; }
        public string PART_DESC { get; set; }
        public string DUE_DATE { get; set; }
        public string STATUS { get; set; }
    }
}


