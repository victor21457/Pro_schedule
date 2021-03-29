using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Timers;


namespace Pro_schedule
{
    /// <summary>
    /// Interaction logic for Detail_Form.xaml
    /// </summary>
    public partial class Detail_Form : Window
    {
        public Constants _const;
        public string spType = "";
        public string lacking = "";
        public string lockedBy = "", lockedByName = "";

        public bool Locked = false;

        List<cmbData> actions, status, room, department;
        string depID, roomID, statusID, actionID, prioID;

        public Timer timer;
        public Detail_Form(Constants _const)
        {
            InitializeComponent();
            this._const = _const;
            lblEmpID.Content = $"{_const.empName} ({_const.empID.ToUpper()})";


            GetDetailData();
            SetCmbDepartment();
            //SetCmbRoom();
            SetCmbStatus();
            SetCmbAction();
            SetCmbPriority();
            EnableControllers();



            timer = new System.Timers.Timer(1000 * 60);
            timer.Elapsed += AutoSendArpProc;
            timer.AutoReset = true;
            timer.Enabled = true;

            _const.curTime = DateTime.Now.ToString("HH:mm");
            lblLink.Content = "RECORD LOCKED BY: " + lockedByName + " @ " + _const.curTime;

            if (_const.proLevel < 90)
            {
                stackSession.Visibility = Visibility.Visible;
                lblSessionTime.Content = _const.proLevel.ToString() + " Mins";
            }
            else
            {
                stackSession.Visibility = Visibility.Hidden;
            }

        }

        private void AutoSendArpProc(object sender, ElapsedEventArgs e)
        {
            _const.curTime = DateTime.Now.ToString("HH:mm");
            this.Dispatcher.Invoke(() => lblLink.Content = "RECORD LOCKED BY: " + _const.empName + " @ " + _const.curTime);

            if (_const.proLevel < 90)
                this.Dispatcher.Invoke(() => lblSessionTime.Content = _const.proLevel.ToString() + " Mins");
        }

        private void SetCmbDepartment()
        {
            using (SqlConnection conn = new SqlConnection(_const.conStringList))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[DLI_WOT_PROD_SCHED_DDL]", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SP_TYPE", "D");
                SqlDataReader reader = cmd.ExecuteReader();

                department = new List<cmbData>();
                List<string> texts = new List<string>();
                int depIndex = -1;
                while (reader.Read())
                {
                    cmbData cmbitem = new cmbData();
                    cmbitem.value = reader["VALUE"].ToString();
                    cmbitem.text = reader["TEXT"].ToString();
                    department.Add(cmbitem);
                    texts.Add(cmbitem.text);
                    if (cmbitem.value == depID) depIndex = texts.Count - 1;
                }
                cmbDEPARTMENT.ItemsSource = texts;
                cmbDEPARTMENT.SelectedIndex = depIndex;
            }
        }
        private void SetCmbRoom()
        {
            if (cmbDEPARTMENT.SelectedIndex == -1) return;
            using (SqlConnection conn = new SqlConnection(_const.conStringList))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[DLI_WOT_PROD_SCHED_DDL]", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SP_TYPE", "R");
                cmd.Parameters.AddWithValue("@DEPT", department[cmbDEPARTMENT.SelectedIndex].value);
                SqlDataReader reader = cmd.ExecuteReader();

                room = new List<cmbData>();
                List<string> texts = new List<string>();
                int roomIndex = -1;
                while (reader.Read())
                {
                    cmbData cmbitem = new cmbData();
                    cmbitem.value = reader["VALUE"].ToString();
                    cmbitem.text = reader["TEXT"].ToString();
                    room.Add(cmbitem);
                    texts.Add(cmbitem.text);
                    if (cmbitem.value == roomID) roomIndex = texts.Count - 1;
                }
                cmbROOM.ItemsSource = texts;
                cmbROOM.SelectedIndex = roomIndex;
            }
        }

        private void SetCmbStatus()
        {
            using (SqlConnection conn = new SqlConnection(_const.conStringList))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[DLI_WOT_PROD_SCHED_DDL]", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SP_TYPE", "S");
                SqlDataReader reader = cmd.ExecuteReader();

                status = new List<cmbData>();
                List<string> texts = new List<string>();
                int statusIndex = -1;
                while (reader.Read())
                {
                    cmbData cmbitem = new cmbData();
                    cmbitem.value = reader["VALUE"].ToString();
                    cmbitem.text = reader["TEXT"].ToString();
                    status.Add(cmbitem);
                    texts.Add(cmbitem.text);
                    if (cmbitem.value == statusID) statusIndex = texts.Count - 1;
                }
                cmbSTATUS.ItemsSource = texts;
                cmbSTATUS.SelectedIndex = statusIndex;
            }
        }
        private void SetCmbAction()
        {
            using (SqlConnection conn = new SqlConnection(_const.conStringList))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[DLI_WOT_PROD_SCHED_DDL]", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SP_TYPE", "A");
                SqlDataReader reader = cmd.ExecuteReader();

                actions = new List<cmbData>();
                List<string> texts = new List<string>();
                int actionIndex = -1;
                while (reader.Read())
                {
                    cmbData cmbitem = new cmbData();
                    cmbitem.value = reader["VALUE"].ToString();
                    cmbitem.text = reader["TEXT"].ToString();
                    actions.Add(cmbitem);
                    texts.Add(cmbitem.text);
                    if (cmbitem.value == actionID) actionIndex = texts.Count - 1;
                }
                cmbACTION.ItemsSource = texts;
                cmbACTION.SelectedIndex = actionIndex;
            }
        }
        private void SetCmbPriority()
        {
            List<string> priority = new List<string>() { "HOT", "HIGH", "NORMAL", "LOW" };
            cmbPRIORITY.ItemsSource = priority;
        }

        private void GetDetailData()
        {
            using (SqlConnection conn = new SqlConnection(_const.conStringList))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[DLI_WOT_PROD_SCHED_IUD]", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SP_TYPE", "S");
                cmd.Parameters.AddWithValue("@WO_ID", _const.WO_ID);
                cmd.Parameters.AddWithValue("@EMP_ID", _const.empID);
                cmd.Parameters.Add("@SP_MESSAGE", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@OK2GO", SqlDbType.Int).Direction = ParameterDirection.Output;


                cmd.ExecuteNonQuery();

                int proOk = (int)cmd.Parameters["@OK2GO"].Value;
                if (proOk > 0)
                {
                    _const.logError = cmd.Parameters["@SP_MESSAGE"].Value.ToString();
                    CustomMsgWindow message = new CustomMsgWindow(_const.logError);
                    message.ShowDialog();
                    lblLogError.Content = _const.logError;
                    Close();
                }

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    WO_ID.Content = reader["WO_ID"].ToString();
                    PART_ID.Content = reader["PART_ID"].ToString();
                    STATUS.Content = reader["WO_STATUS"].ToString();
                    DUE_DATE.Content = DateTime.Parse(reader["DUE_DATE"].ToString()).ToString("MM/dd/yyyy");
                    QTY.Content = reader["QTY"].ToString();
                    PICK.Content = reader["PICK"].ToString();
                    lockedByName = reader["RECORD_LOCK_BY"].ToString();
                    lockedBy = reader["RECORD_LOCKED_BY_ID"].ToString();
                    lacking = reader["LACKING"].ToString();

                    depID = reader["DEPARTMENT"].ToString();
                    roomID = reader["ROOM_RECORD_ID"].ToString();
                    statusID = reader["STATUS_ROW_ID"].ToString();
                    actionID = reader["ACTION_ROW_ID"].ToString();
                    lblPartDesc.Content = reader["PART_DESC"].ToString();
                    txtNote.Text = reader["NOTES"].ToString();
                    string priority = reader["PRIORITY"].ToString();
                    if (!string.IsNullOrEmpty(priority))
                    {
                        cmbPRIORITY.SelectedIndex = int.Parse(priority) - 1;
                    }
                    else
                    {
                        cmbPRIORITY.SelectedIndex = 2;
                    }
                    break;
                }

                if (!string.IsNullOrEmpty(lockedBy) && lockedBy.ToUpper() == _const.empID.ToUpper()) Locked = true;
                else Locked = false;

                if (lockedBy.ToUpper() != _const.empID.ToUpper() || _const.proLevel < 90)
                {
                    btnLink.Visibility = Visibility.Hidden;

                }
                else
                {
                    btnLink.Visibility = Visibility.Visible;

                }

                if (lacking != "YES")
                {
                    btnLacking.Content = "NO LACKING";
                    btnLacking.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x3C, 0xB3, 0x71));

                }
                else
                {
                    btnLacking.Content = "LACKING";
                    btnLacking.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xB2, 0x22, 0x22));
                }


            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {

            depID = cmbDEPARTMENT.SelectedIndex > -1 ? department[cmbDEPARTMENT.SelectedIndex].value : null;
            roomID = cmbROOM.SelectedIndex > -1 ? room[cmbROOM.SelectedIndex].value : null;
            statusID = cmbSTATUS.SelectedIndex > -1 ? status[cmbSTATUS.SelectedIndex].value : null;
            actionID = cmbSTATUS.SelectedIndex > -1 ? actions[cmbACTION.SelectedIndex].value : null;
            prioID = cmbPRIORITY.SelectedIndex > -1 ? (cmbPRIORITY.SelectedIndex + 1).ToString() : null;

            using (SqlConnection conn = new SqlConnection(_const.conStringList))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[DLI_WOT_PROD_SCHED_IUD]", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SP_TYPE", "U");
                cmd.Parameters.AddWithValue("@WO_ID", _const.WO_ID);
                cmd.Parameters.AddWithValue("@EMP_ID", _const.empID);

                cmd.Parameters.AddWithValue("@DEPT", depID);

                if (string.IsNullOrEmpty(prioID)) cmd.Parameters.AddWithValue("@PRIORITY", null);
                else cmd.Parameters.AddWithValue("@PRIORITY", int.Parse(prioID));

                if (string.IsNullOrEmpty(roomID)) cmd.Parameters.AddWithValue("@ROOM_RECORD_ID", null);
                else cmd.Parameters.AddWithValue("@ROOM_RECORD_ID", int.Parse(roomID));

                if (string.IsNullOrEmpty(MANUAL_QTY.Text)) cmd.Parameters.AddWithValue("@BATCHSHEET_QTY", null);
                else cmd.Parameters.AddWithValue("@BATCHSHEET_QTY", float.Parse(MANUAL_QTY.Text));

                if (string.IsNullOrEmpty(statusID)) cmd.Parameters.AddWithValue("@STATUS_ROW_ID", null);
                else cmd.Parameters.AddWithValue("@STATUS_ROW_ID", int.Parse(statusID));

                if (string.IsNullOrEmpty(actionID)) cmd.Parameters.AddWithValue("@ACTION_ROW_ID", null);
                else cmd.Parameters.AddWithValue("@ACTION_ROW_ID", int.Parse(actionID));

                cmd.Parameters.AddWithValue("@NOTES", txtNote.Text);

                cmd.Parameters.Add("@SP_MESSAGE", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@SP_ERROR_NUMBER", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@OK2GO", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                int proOk = (int)cmd.Parameters["@OK2GO"].Value;
                if (proOk == 0)
                {
                    lblLogError.Foreground = Brushes.Green;
                    lblLogError.Content = cmd.Parameters["@SP_MESSAGE"].Value;
                }
                else if (proOk > 0)
                {
                    lblLogError.Foreground = Brushes.Red;
                    lblLogError.Content = cmd.Parameters["@SP_MESSAGE"].Value;
                }
            }
        }
        private void btnLink_Click(object sender, RoutedEventArgs e)
        {
            if (_const.proLevel >= 90)
            {
                using (SqlConnection conn = new SqlConnection(_const.conStringList))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("[dbo].[DLI_WOT_PROD_SCHED_IUD]", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SP_TYPE", "UR");
                    cmd.Parameters.AddWithValue("@WO_ID", _const.WO_ID);
                    cmd.Parameters.AddWithValue("@EMP_ID", _const.empID);
                    cmd.ExecuteNonQuery();
                }
            }

        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            //lblLink.Content = "RECORD LOCKED BY: " + _const.empName + " @ " + _const.curTime
            if (_const.proLevel != _const.session)
            {
                _const.proLevel = _const.session;
                lblSessionTime.Content = _const.proLevel.ToString() + " Mins";
            }
        }

        private void cmbDEPARTMENT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            SetCmbRoom();

        }

        private void btnLacking_Click(object sender, RoutedEventArgs e)
        {

            string url = @"https://dlihome.deseretlabs.com/vmfg/wo_tracking_rm_requirements.asp?strWO_ID=" + _const.WO_ID;
            System.Diagnostics.Process.Start("cmd", "/c start " + url);

        }
        private void CloseWin()
        {
            using (SqlConnection conn = new SqlConnection(_const.conStringList))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("[dbo].[DLI_WOT_PROD_SCHED_IUD]", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SP_TYPE", "UL");
                cmd.Parameters.AddWithValue("@WO_ID", _const.WO_ID);
                cmd.Parameters.AddWithValue("@EMP_ID", _const.empID);
                cmd.ExecuteNonQuery();
            }
            Close();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            CloseWin();
        }
        private void WinClose(object sender, RoutedEventArgs e)
        {
            CloseWin();
        }
        private void EnableControllers()
        {
            Submit.IsEnabled = txtNote.IsEnabled = MANUAL_QTY.IsEnabled = cmbDEPARTMENT.IsEnabled = cmbROOM.IsEnabled
                = cmbSTATUS.IsEnabled = cmbACTION.IsEnabled = cmbPRIORITY.IsEnabled = btnLacking.IsEnabled = Locked;
            //if ( Locked == false)
            //{
            //    if (lacking != "YES")
            //    {
            //        btnLacking.Background = new SolidColorBrush(Color.FromArgb(0x88, 0x3C, 0xB3, 0x71));
            //        btnLacking.Foreground = Brushes.Gray;
            //    }
            //    else
            //    {
            //        btnLacking.Background = new SolidColorBrush(Color.FromArgb(0x88, 0xB2, 0x22, 0x22));
            //        btnLacking.Foreground = Brushes.Gray;
            //    }
            //}
        }
        private void cmbACTION_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //cmbData selected = (cmbData)cmbACTION.SelectedItem;
            //MessageBox.Show(selected.value + "  " + selected.text);
        }
    }

    public class cmbData
    {
        public string value { get; set; }
        public string text { get; set; }
    }
}
