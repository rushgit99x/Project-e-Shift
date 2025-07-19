using e_Shift.Business.Interface;
using e_Shift.Business.Services;
using e_Shift.Repository.Interface;
using e_Shift.Repository.Service;
using e_Shift.Repository.Services;
using FontAwesome.Sharp;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace e_Shift.Forms
{
    public partial class AdminDashboard : Form
    {
        // Fields
        private IconButton currentBtn;
        private Panel leftBorderBtn;
        private Form currentChildForm;

        // Constructor
        public AdminDashboard()
        {
            InitializeComponent();
            leftBorderBtn = new Panel();
            leftBorderBtn.Size = new Size(7, 60);
            panelMenu.Controls.Add(leftBorderBtn);

            // Form
            this.Text = string.Empty;
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
        }

        // Structs
        private struct RGBColors
        {
            public static Color color1 = Color.FromArgb(172, 126, 241);
            public static Color color2 = Color.FromArgb(249, 118, 176);
            public static Color color3 = Color.FromArgb(253, 138, 114);
            public static Color color4 = Color.FromArgb(95, 77, 221);
            public static Color color5 = Color.FromArgb(249, 88, 155);
            public static Color color6 = Color.FromArgb(24, 161, 251);
        }

        // Methods
        private void ActivateButton(object senderBtn, Color color)
        {
            if (senderBtn != null)
            {
                DisableButton();
                // Button
                currentBtn = (IconButton)senderBtn;
                currentBtn.BackColor = Color.FromArgb(37, 36, 81);
                currentBtn.ForeColor = color;
                currentBtn.TextAlign = ContentAlignment.MiddleCenter;
                currentBtn.IconColor = color;
                currentBtn.TextImageRelation = TextImageRelation.TextBeforeImage;
                currentBtn.ImageAlign = ContentAlignment.MiddleRight;
                // Left border button
                leftBorderBtn.BackColor = color;
                leftBorderBtn.Location = new Point(0, currentBtn.Location.Y);
                leftBorderBtn.Visible = true;
                leftBorderBtn.BringToFront();
                // Current Child Form Icon
                iconCurrentChildForm.IconChar = currentBtn.IconChar;
                iconCurrentChildForm.IconColor = color;
            }
        }

        private void DisableButton()
        {
            if (currentBtn != null)
            {
                currentBtn.BackColor = Color.FromArgb(31, 30, 68);
                currentBtn.ForeColor = Color.Gainsboro;
                currentBtn.TextAlign = ContentAlignment.MiddleLeft;
                currentBtn.IconColor = Color.Gainsboro;
                currentBtn.TextImageRelation = TextImageRelation.ImageBeforeText;
                currentBtn.ImageAlign = ContentAlignment.MiddleLeft;
            }
        }

        private void OpenChildForm(Form childForm)
        {
            if (currentChildForm != null)
            {
                currentChildForm.Close();
            }
            currentChildForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelDesktop.Controls.Add(childForm);
            panelDesktop.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            lblTitleChildForm.Text = childForm.Text;
        }

        private void Reset()
        {
            DisableButton();
            leftBorderBtn.Visible = false;
            iconCurrentChildForm.IconChar = IconChar.Home;
            iconCurrentChildForm.IconColor = Color.MediumPurple;
            lblTitleChildForm.Text = "Home";
        }

        // Events
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            // Instead of opening AdminDashboard, reset to home state
            //Reset();
            ActivateButton(sender, RGBColors.color2);
            OpenChildForm(new AdminSummeryDashboard());
        }

        private void btnJobs_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color2);
            OpenChildForm(new AdminJobs());
        }

        private void btnLoadManagement_Click(object sender, EventArgs e)
        {
            //ActivateButton(sender, RGBColors.color3);
            //OpenChildForm(new AdminLoadManagement());
            ActivateButton(sender, RGBColors.color3);
            ILoadRepository loadRepository = new LoadRepository();
            ILoadService loadService = new LoadService(loadRepository);
            var loadManagementForm = new AdminLoadManagement(loadService);
            OpenChildForm(loadManagementForm); // Replace the parameterless call with the injected instance
        }

        private void btnUserManagement_Click(object sender, EventArgs e)
        {
            //ActivateButton(sender, RGBColors.color4);
            //OpenChildForm(new AdminUserManagement());
            ActivateButton(sender, RGBColors.color4);
            IAdminManageRepository adminRepository = new AdminManageRepository();
            IAdminManageService adminService = new AdminManageService(adminRepository);
            var adminUserManagementForm = new AdminUserManagement(adminService);
            OpenChildForm(adminUserManagementForm);
        }

        private void btnTransportUnits_Click(object sender, EventArgs e)
        {
            ActivateButton(sender, RGBColors.color5);
            OpenChildForm(new AdminTransportUnits());
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            //ActivateButton(sender, RGBColors.color6);
            //OpenChildForm(new AdminProducts());
            ActivateButton(sender, RGBColors.color6);
            IProductRepository productRepository = new ProductRepository();
            IProductService productService = new ProductService(productRepository);
            var adminProductsForm = new AdminProducts(productService);
            OpenChildForm(adminProductsForm); // Replace the parameterless call with the injected instance
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            //ActivateButton(sender, RGBColors.color1);
            //OpenChildForm(new AdminReports());
            ActivateButton(sender, RGBColors.color1);
            IReportRepository reportRepository = new ReportRepository();
            IReportService reportService = new ReportService(reportRepository);
            var adminReportsForm = new AdminReports(reportService);
            OpenChildForm(adminReportsForm); // Replace the parameterless call with the injected instance
        }

        //private void btnNotifications_Click(object sender, EventArgs e)
        //{
        //    ActivateButton(sender, RGBColors.color2);
        //    OpenChildForm(new AdminNotifications());
        //}

        //private void btnSettings_Click(object sender, EventArgs e)
        //{
        //    ActivateButton(sender, RGBColors.color3);
        //    OpenChildForm(new AdminSettings());
        //}

        private void btnHome_Click(object sender, EventArgs e)
        {
            if (currentChildForm != null)
            {
                currentChildForm.Close();
            }
            Reset();
        }

        // Drag Form
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void panelTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}