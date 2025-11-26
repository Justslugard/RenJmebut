using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace ESEMKAH
{
    public partial class FormEmployee : Form
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        int dgvSelectedRow = -1;
        string status;
        public FormEmployee()
        {
            InitializeComponent();
        }

        private void FormEmployee_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'sMKLaundry2018DataSet.Job' table. You can move, or remove it, as needed.
            this.jobTableAdapter.Fill(this.sMKLaundry2018DataSet.Job);
            loadDGV();
        }
        private void loadDGV()
        {
            dgv.Rows.Clear();
            DataClasses1DataContext dbdgv = new DataClasses1DataContext();
            IQueryable<Employee> data = dbdgv.Employees.Where(x => x.Name.Contains(textBoxSearch.Text)
            || x.Email.Contains(textBoxSearch.Text)
            || x.PhoneNumber.Contains(textBoxSearch.Text));
            foreach (Employee item in data)
                dgv.Rows.Add(item.Id, item.Name, item.Email, item.PhoneNumber,
                    item.Address, item.DateofBirth, item.Job.Name, item.Salary, item.Password);
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            loadDGV();
        }

        private void dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (status != null) return;
            dgvSelectedRow = e.RowIndex;
            if (dgvSelectedRow < 0) return;
            textBoxEmployeeID.Text = dgv.Rows[dgvSelectedRow].Cells[0].Value.ToString();
            textBoxName.Text = dgv.Rows[dgvSelectedRow].Cells[1].Value.ToString();
            textBoxEmail.Text = dgv.Rows[dgvSelectedRow].Cells[2].Value.ToString();
            textBoxPhoneNumber.Text = dgv.Rows[dgvSelectedRow].Cells[3].Value.ToString();
            textBoxAddress.Text = dgv.Rows[dgvSelectedRow].Cells[4].Value.ToString();
            dateTimePickerDOB.Value = Convert.ToDateTime(dgv.Rows[dgvSelectedRow].Cells[5].Value);
            comboBoxJob.Text = dgv.Rows[dgvSelectedRow].Cells[6].Value.ToString();
            numericUpDownSalary.Text = dgv.Rows[dgvSelectedRow].Cells[7].Value.ToString();
            textBoxPassword.Text = dgv.Rows[dgvSelectedRow].Cells[8].Value.ToString();
        }
        private void clearTextBox()
        {
            textBoxEmployeeID.Clear();
            textBoxName.Clear();
            textBoxEmail.Clear();
            textBoxPhoneNumber.Clear();
            textBoxAddress.Clear();
            dateTimePickerDOB.Value = DateTime.Now;
            comboBoxJob.Text = "";
            numericUpDownSalary.Value = 0;
            textBoxPassword.Clear();
            textBoxConfirmPassword.Clear();
        }
        private void enableField(bool a)
        {
            textBoxName.Enabled = a;
            textBoxEmail.Enabled = a;
            textBoxPhoneNumber.Enabled = a;
            textBoxAddress.Enabled = a;
            dateTimePickerDOB.Enabled = a;
            comboBoxJob.Enabled = a;
            numericUpDownSalary.Enabled = a;
            textBoxPassword.Enabled = a;
            textBoxConfirmPassword.Enabled = a;
        }
        private void enableButton(bool a)
        {
            buttonInsert.Enabled = a;
            buttonUpdate.Enabled = a;
            buttonDelete.Enabled = a;
            buttonSave.Enabled = !a;
            buttonCancel.Enabled = !a;
        }
        private void buttonInsert_Click(object sender, EventArgs e)
        {
            clearTextBox(); enableField(true); enableButton(false); textBoxEmployeeID.Text = AutoINT().ToString();
            status = "insert";
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (dgvSelectedRow < 0)
            {
                MessageBox.Show("Select Data First", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            status = "update";
            enableField(true); enableButton(false);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dgvSelectedRow < 0)
            {
                MessageBox.Show("Select Data First", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult dr = MessageBox.Show("Are you sure to delete this data?",
                "Warning" ,MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.Yes)
            {
                Employee emp = db.Employees.FirstOrDefault(x => x.Id.Equals(textBoxEmployeeID.Text));
                db.Employees.DeleteOnSubmit(emp);
                db.SubmitChanges();
                loadDGV();
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (!Validation()) return;
            if (status == "insert")
            {
                db.Employees.InsertOnSubmit(new Employee()
                {
                    Name = textBoxName.Text,
                    Email = textBoxEmail.Text,
                    PhoneNumber = textBoxPhoneNumber.Text,
                    Address = textBoxAddress.Text,
                    DateofBirth = dateTimePickerDOB.Value,
                    IdJob = comboBoxJob.SelectedIndex + 1,
                    Salary = numericUpDownSalary.Value,
                    Password = textBoxPassword.Text
                });
            }
            else if (status == "update")
            {
                Employee emp = db.Employees.FirstOrDefault(x => x.Id.Equals(textBoxEmployeeID.Text));
                emp.Name = textBoxName.Text;
                emp.Email = textBoxEmail.Text;
                emp.PhoneNumber = textBoxPhoneNumber.Text;
                emp.Address = textBoxAddress.Text;
                emp.DateofBirth = dateTimePickerDOB.Value;
                emp.IdJob = comboBoxJob.SelectedIndex + 1;
                emp.Salary = numericUpDownSalary.Value;
            }
            db.SubmitChanges();
            loadDGV();
            enableField(false); enableButton(true); status = null;
            clearTextBox();

        }
        private bool Validation()
        {
            Regex email = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Regex phonenumber = new Regex("\\+\\d+");
            if (string.IsNullOrWhiteSpace(textBoxEmployeeID.Text) ||
                string.IsNullOrWhiteSpace(textBoxName.Text) ||
                string.IsNullOrWhiteSpace(textBoxPhoneNumber.Text) ||
                string.IsNullOrWhiteSpace(textBoxAddress.Text) ||
                string.IsNullOrWhiteSpace(comboBoxJob.Text))
            {
                MessageBox.Show("Field Must Be Fill", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else if (!email.IsMatch(textBoxEmail.Text))
            {
                MessageBox.Show("Invalid Email Format", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else if (!phonenumber.IsMatch(textBoxPhoneNumber.Text))
            {
                MessageBox.Show("Phone Number must start with +");
                return false;
            }
            else if (textBoxConfirmPassword.Text != textBoxPassword.Text)
            {
                MessageBox.Show("Pasword not match","Warning",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            else return true;
        }
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            enableField(false); enableButton(true); clearTextBox();
            status = null;
        }
        private int AutoINT()
        {
            var ID = db.Employees.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            return ID;
        }
    }
}
