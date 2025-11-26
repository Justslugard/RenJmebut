using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ESEMKAH
{
    public partial class FormService : Form
    {
        DataClasses1DataContext db = new DataClasses1DataContext();
        public FormService()
        {
            InitializeComponent();
        }
        private void FormService_Load(object sender, EventArgs e)
        {
            LoadDGV();
        }
        private void LoadDGV()
        {
            dgv.Rows.Clear();
            DataClasses1DataContext dbdgv = new DataClasses1DataContext();
            IQueryable<Service> data = dbdgv.Services.Where(x => x.Name.Contains(textBoxSearch.Text)
            || x.Category.Name.Contains(textBoxSearch.Text)
            || x.Unit.Name.Contains(textBoxSearch.Text)
            || x.PriceUnit.ToString().Contains(textBoxSearch.Text));
            foreach (var item in data)
                dgv.Rows.Add(item.Id, item.Name, item.Category.Name, item.Unit.Name,item.PriceUnit, item.EstimationDuration);
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            LoadDGV();
        }
    }
}
