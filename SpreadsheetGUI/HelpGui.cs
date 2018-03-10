using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI {
    public partial class HelpGui : Form {

        /// <summary>
        /// Initializes Help Form
        /// </summary>
        public HelpGui() {
            InitializeComponent();
        }

        /// <summary>
        /// Updates text of menu items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Change(object sender, EventArgs e) {
            RadioButton btn = sender as RadioButton;
            switch (btn.Name) {
                case "radioButton1":
                    //New
                    textBox1.Text = "Clicking New, or Pressing Ctrl + N, will open";
                    textBox1.Text += " a new blank spreadsheet. This will appear in a new window.";

                    break;
                    //Save
                case "radioButton2":
                    textBox1.Text = "Need to save your spreadsheet? Clicking Save, or Pressing Ctrl + S, will ";
                    textBox1.Text += "open a file window, so that you may save your work.";
                    textBox1.Text += " Simply name your spreadsheet and click save.";

                    break;
                    //Open
                case "radioButton3":
                    textBox1.Text = "Clicking Open, or Pressing Ctrl + O, will ";
                    textBox1.Text += "open a file window, so that you may choose a file ";
                    textBox1.Text += "to open. Don't worry, you can continue working in your ";
                    textBox1.Text += "current spreadsheet and a new window will appear.";

                    break;
                case "radioButton4":
                    textBox1.Text = "Done with your spreadheet? Clicking Close, or Pressing Ctrl+Shift+C, ";
                    textBox1.Text += "will close the current spreadsheet.";
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// When Help Menu loads, sets the text for Textbox 2 and 3.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpGui_Load(object sender, EventArgs e) {
            textBox2.Text = "Select the cell you would like to edit either by clicking on it with your mouse, ";
            textBox2.Text += "or highlighting the cell using the arrow keys. The Content TextBox will ";
            textBox2.Text += "automatically activate, so you may begin typing when ready. ";
            textBox2.Text += "When your done typing press \"Enter\".";


            textBox3.Text = "When you select a cell, it's contents will be displayed in the Cell Content Box. ";
            textBox3.Text += " This texbox is editibale so you can change the contents of the cell.";
            textBox3.AppendText(Environment.NewLine);
            textBox3.AppendText(Environment.NewLine);
            textBox3.Text += "The cell address will be displayed in the Cell Name box.";
            textBox3.AppendText(Environment.NewLine);
            textBox3.AppendText(Environment.NewLine);
            textBox3.Text += "The Value of the cell will be dsplayed in the Cell Value.";

        }


    }
}
