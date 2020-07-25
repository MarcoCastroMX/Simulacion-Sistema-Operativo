namespace Programa_8
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ListBoxDatos = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // ListBoxDatos
            // 
            this.ListBoxDatos.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ListBoxDatos.FormattingEnabled = true;
            this.ListBoxDatos.ItemHeight = 20;
            this.ListBoxDatos.Location = new System.Drawing.Point(13, 13);
            this.ListBoxDatos.Name = "ListBoxDatos";
            this.ListBoxDatos.Size = new System.Drawing.Size(406, 424);
            this.ListBoxDatos.TabIndex = 0;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(431, 450);
            this.Controls.Add(this.ListBoxDatos);
            this.Name = "Form2";
            this.Text = "Datos Procesos";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox ListBoxDatos;
    }
}