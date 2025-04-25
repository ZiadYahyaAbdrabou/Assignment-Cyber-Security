using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CV_Parser_App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnValidate_Click(object sender, EventArgs e)
        {
            bool allValid = true;
            ResetFieldColors();

            // Validate Name
            if (!Regex.IsMatch(txtName.Text, @"^[\p{L}\s'-]+$"))
            {
                HighlightInvalidField(txtName);
                allValid = false;
            }

            // Validate Email
            if (!Regex.IsMatch(txtEmail.Text, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                HighlightInvalidField(txtEmail);
                allValid = false;
            }

            // Validate Phone
            if (!Regex.IsMatch(txtPhone.Text, @"^\+?[0-9\s\-\(\)]{10,}$"))
            {
                HighlightInvalidField(txtPhone);
                allValid = false;
            }

            // Validate Password
            if (!Regex.IsMatch(txtPassword.Text, @"^(?=.*[A-Za-z])(?=.*\d).{8,}$"))
            {
                HighlightInvalidField(txtPassword);
                allValid = false;
            }

            // Validate Address
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                HighlightInvalidField(txtAddress);
                allValid = false;
            }

            // Validate Postal Code
            if (!Regex.IsMatch(txtPostalCode.Text, @"^[a-zA-Z0-9\s\-]{3,10}$"))
            {
                HighlightInvalidField(txtPostalCode);
                allValid = false;
            }

            if (allValid)
            {
                MessageBox.Show("All fields are valid!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSaveManual.Enabled = true;
            }
            else
            {
                MessageBox.Show("Please correct the highlighted fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnSaveManual.Enabled = false;
            }
        }

        private void HighlightInvalidField(Control control)
        {
            control.BackColor = Color.LightPink;
        }

        private void ResetFieldColors()
        {
            txtName.BackColor = SystemColors.Window;
            txtEmail.BackColor = SystemColors.Window;
            txtPhone.BackColor = SystemColors.Window;
            txtPassword.BackColor = SystemColors.Window;
            txtAddress.BackColor = SystemColors.Window;
            txtPostalCode.BackColor = SystemColors.Window;
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCVContent.Text))
            {
                MessageBox.Show("Please paste or enter CV content first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string cvText = txtCVContent.Text;

            // Extract name
            Match nameMatch = Regex.Match(cvText, @"^(?<name>[A-Z][a-z]+(\s[A-Z][a-z]+)+)", RegexOptions.Multiline);
            lblName.Text = nameMatch.Success ? nameMatch.Groups["name"].Value : "Not found";

            // Extract email
            Match emailMatch = Regex.Match(cvText, @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b");
            lblEmail.Text = emailMatch.Success ? emailMatch.Value : "Not found";

            // Extract phone
            Match phoneMatch = Regex.Match(cvText, @"(\+?\d[\d\s\-\(\)]{8,}\d)");
            lblPhone.Text = phoneMatch.Success ? phoneMatch.Value : "Not found";

            // Extract skills
            MatchCollection skillMatches = Regex.Matches(cvText, @"\b(C#|Java|Python|JavaScript|SQL|HTML|CSS|\.NET|ASP|PHP|Ruby|Swift|Kotlin|React|Angular|Vue|Node\.?js)\b", RegexOptions.IgnoreCase);
            string skills = "";
            foreach (Match match in skillMatches)
            {
                if (!skills.Contains(match.Value, StringComparison.OrdinalIgnoreCase))
                {
                    skills += (skills == "" ? "" : ", ") + match.Value;
                }
            }
            lblSkills.Text = string.IsNullOrEmpty(skills) ? "Not found" : skills;

            // Extract years of experience
            Match expMatch = Regex.Match(cvText, @"(\d+)\s+(years?|yrs?\.?)\s+experience", RegexOptions.IgnoreCase);
            lblExperience.Text = expMatch.Success ? $"{expMatch.Groups[1].Value} years" : "Not found";

            btnSaveCV.Enabled = true;
        }

        private void btnSaveManual_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv";
            saveDialog.Title = "Save Form Data";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                string content = saveDialog.FileName.EndsWith(".csv")
                    ? $"Name,Email,Phone,Address,PostalCode\n{txtName.Text},{txtEmail.Text},{txtPhone.Text},{txtAddress.Text},{txtPostalCode.Text}"
                    : $"Name: {txtName.Text}\nEmail: {txtEmail.Text}\nPhone: {txtPhone.Text}\nAddress: {txtAddress.Text}\nPostal Code: {txtPostalCode.Text}";

                System.IO.File.WriteAllText(saveDialog.FileName, content);
                MessageBox.Show("Data saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSaveCV_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv";
            saveDialog.Title = "Save CV Data";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                string content = saveDialog.FileName.EndsWith(".csv")
                    ? $"Name,Email,Phone,Skills,Experience\n{lblName.Text},{lblEmail.Text},{lblPhone.Text},\"{lblSkills.Text}\",{lblExperience.Text}"
                    : $"Name: {lblName.Text}\nEmail: {lblEmail.Text}\nPhone: {lblPhone.Text}\nSkills: {lblSkills.Text}\nExperience: {lblExperience.Text}";

                System.IO.File.WriteAllText(saveDialog.FileName, content);
                MessageBox.Show("Data saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLoadCV_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openDialog.Title = "Load CV File";

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                txtCVContent.Text = System.IO.File.ReadAllText(openDialog.FileName);
            }
        }
    }
}