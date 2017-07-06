using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Perceptron {
    public partial class FormDataEntry : Form {
        public FormDataEntry() {
            InitializeComponent();

            buttonOk.Click += (o, s) => {
                this.DialogResult = DialogResult.OK;
            };
        }
    }
}
