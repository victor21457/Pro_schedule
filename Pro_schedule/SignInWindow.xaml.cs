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

namespace Pro_schedule
{
    /// <summary>
    /// Interaction logic for SignInWindow.xaml
    /// </summary>
    public partial class SignInWindow : Window
    {
        public Constants _const;
        public SignInWindow(Constants _const)
        {
            InitializeComponent();
            this._const = _const;
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

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginProc();
        }

        private void LoginProc()
        {
            if (UserName.Text.Length == 0 || Password.Password.Length == 0)
            {
                CustomMsgWindow message = new CustomMsgWindow("Username and Password Error, Try Again."); 
                message.ShowDialog();
                return;
            }
            if (Authur())
            {
                _const.logined = true;
                Close();
            }
            else
            {
                CustomMsgWindow message = new CustomMsgWindow(_const.logError);
                message.ShowDialog();
                return;
            }
        }

        private bool Authur()
        {
            using (SqlConnection conn = new SqlConnection(_const.conStringAuth))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.uspLogin", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@pEMPID", UserName.Text.ToString().ToUpper());
                cmd.Parameters.AddWithValue("@pPassword", Password.Password.ToString());

                var pEmpname = cmd.Parameters.Add("@EMPNAME", SqlDbType.NVarChar, 60);
                pEmpname.Direction = ParameterDirection.Output;

                var pProAccess = cmd.Parameters.Add("@PRODUCTION_SCHED_ACCESS", SqlDbType.NChar, 1);
                pProAccess.Direction = ParameterDirection.Output;

                var pProLevel = cmd.Parameters.Add("@PRODUCTION_SCHED_ACCESS_LEVEL", SqlDbType.Int);
                pProLevel.Direction = ParameterDirection.Output;

                var pOkGo = cmd.Parameters.Add("@OK_TO_GO", SqlDbType.Int);
                pOkGo.Direction = ParameterDirection.Output;

                var pResponse = cmd.Parameters.Add("@responseMessage", SqlDbType.NVarChar, 250);
                pResponse.Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();
                int proOk = (int)pOkGo.Value;
                string proRes = (string)pResponse.Value;
                if (proOk > 0)
                {
                    _const.logError = proRes;
                    return false;
                }

                string empName = pEmpname.Value.ToString().ToUpper();
                string proAccess = pProAccess.Value.ToString();
                int proLevel = (int)pProLevel.Value;
                if (proAccess != "Y") return false;

                _const.empID = UserName.Text.ToUpper();
                _const.empName = empName;
                _const.proLevel = _const.session = proLevel;


            }
            return true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ( e.Key == Key.Return)
            {
                LoginProc();
            }
        }
    }
}
