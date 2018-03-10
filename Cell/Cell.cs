using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS {

    /// <summary>
    /// A Cell is an individual Cell reference consisiting
    /// of it's name and any dependencies.
    /// </summary>
    public class Cell {

        /// <summary>
        /// Name of Cell
        /// </summary>
        private String cellName;

        /// <summary>
        /// List of all dependants
        /// </summary>
        private Dictionary<String, Cell> dependants;

        /// <summary>
        /// Contents of cell
        /// Formula, Double, or String
        /// </summary>
        private object Contents;

        /// <summary>
        /// Value of Cell
        /// Double, String, or Formula Error
        /// </summary>
        private object Value;

        //Constructs a Cell given a cellname
        //and creates list of dependants.
        public Cell(String cellName) {
            this.cellName = cellName;
            dependants = new Dictionary<String, Cell>();
            Contents = String.Empty;
            Value = String.Empty;
        }

        /// <summary>
        /// Returns a list of dependant cells.
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, Cell> getDep() {
            return dependants;
        }

        /// <summary>
        /// Returns contents of a cell
        /// </summary>
        /// <returns></returns>
        public object GetContents() {
            return Contents;
        }

        /// <summary>
        /// Sets contents of cell
        /// </summary>
        /// <param name="o"></param>
        public void SetContents(Object o) {
            this.Contents = o;
        }

        /// <summary>
        /// Gets value of cell
        /// </summary>
        /// <returns></returns>
        public object GetValue() {
            return Value;
        }

        /// <summary>
        /// Sets Value of Cell
        /// </summary>
        /// <param name="o"></param>
        public void SetValue(Object o) {
            this.Value = o;
        }

        /// <summary>
        /// Returns the name of the Cell
        /// </summary>
        /// <returns></returns>
        public String GetName() {
            return cellName;
        }

    }
}
