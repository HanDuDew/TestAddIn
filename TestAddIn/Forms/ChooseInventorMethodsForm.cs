using System;
using System.Windows.Forms;

namespace AutoBeau.Forms
{
    public partial class ChooseInventorMethodsForm : Form
    {
        // Properties to store the selected methods
        public bool AutoArrangeSelected { get; private set; }
        public bool HoleTableSelected { get; private set; }
        public bool CentermarksSelected { get; private set; }
        public bool RetrieveDimensionsSelected { get; private set; }
        
        // Event to notify when selection changes
        public event EventHandler SelectionChanged;

        public ChooseInventorMethodsForm()
        {
            InitializeComponent();
            LoadCurrentSelections();
        }

        /// <summary>
        /// Load current selections from the main window
        /// </summary>
        public void LoadCurrentSelections()
        {
            // These will be set from the calling code
            autoArrangeCheckBox.Checked = AutoArrangeSelected;
            checkBox2.Checked = HoleTableSelected;
            checkBox3.Checked = CentermarksSelected;
            retrieveDimsCheckBox.Checked = RetrieveDimensionsSelected;
        }

        /// <summary>
        /// Set the current selections (called from main window)
        /// </summary>
        public void SetCurrentSelections(bool autoArrange, bool holeTable, bool centermarks, bool retrieveDims)
        {
            AutoArrangeSelected = autoArrange;
            HoleTableSelected = holeTable;
            CentermarksSelected = centermarks;
            RetrieveDimensionsSelected = retrieveDims;
            LoadCurrentSelections();
        }

        /// <summary>
        /// Get the current selections from the form
        /// </summary>
        public void GetSelections(out bool autoArrange, out bool holeTable, out bool centermarks, out bool retrieveDims)
        {
            autoArrange = autoArrangeCheckBox.Checked;
            holeTable = checkBox2.Checked;
            centermarks = checkBox3.Checked;
            retrieveDims = retrieveDimsCheckBox.Checked;
        }

        private void ChooseInventorMethodsForm_Load(object sender, EventArgs e)
        {
            // Set form properties
            this.Text = "Choose Inventor Methods";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ShowInTaskbar = false;

            // Add event handlers for checkbox changes
            autoArrangeCheckBox.CheckedChanged += CheckBox_CheckedChanged;
            checkBox2.CheckedChanged += CheckBox_CheckedChanged;
            checkBox3.CheckedChanged += CheckBox_CheckedChanged;
            retrieveDimsCheckBox.CheckedChanged += CheckBox_CheckedChanged;
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Update properties when checkboxes change
            AutoArrangeSelected = autoArrangeCheckBox.Checked;
            HoleTableSelected = checkBox2.Checked;
            CentermarksSelected = checkBox3.Checked;
            RetrieveDimensionsSelected = retrieveDimsCheckBox.Checked;

            // Notify that selection has changed
            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Apply selections and close the form
        /// </summary>
        private void ApplySelections()
        {
            AutoArrangeSelected = autoArrangeCheckBox.Checked;
            HoleTableSelected = checkBox2.Checked;
            CentermarksSelected = checkBox3.Checked;
            RetrieveDimensionsSelected = retrieveDimsCheckBox.Checked;
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Cancel without applying changes
        /// </summary>
        private void CancelChanges()
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            ApplySelections();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            CancelChanges();
        }
    }
}
