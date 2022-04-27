namespace MJTradier
{
    public partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkMyHoldingsButton = new System.Windows.Forms.Button();
            this.checkMyAccountInfoButton = new System.Windows.Forms.Button();
            this.myNameLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.myDepositLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.accountComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.axKHOpenAPI1 = new AxKHOpenAPILib.AxKHOpenAPI();
            this.testTextBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axKHOpenAPI1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkMyHoldingsButton);
            this.groupBox1.Controls.Add(this.checkMyAccountInfoButton);
            this.groupBox1.Controls.Add(this.myNameLabel);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.myDepositLabel);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.accountComboBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(7, 29);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBox1.Size = new System.Drawing.Size(362, 237);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "내 정보";
            // 
            // checkMyHoldingsButton
            // 
            this.checkMyHoldingsButton.Location = new System.Drawing.Point(229, 170);
            this.checkMyHoldingsButton.Name = "checkMyHoldingsButton";
            this.checkMyHoldingsButton.Size = new System.Drawing.Size(127, 29);
            this.checkMyHoldingsButton.TabIndex = 12;
            this.checkMyHoldingsButton.Text = "보유종목확인";
            this.checkMyHoldingsButton.UseVisualStyleBackColor = true;
            // 
            // checkMyAccountInfoButton
            // 
            this.checkMyAccountInfoButton.Location = new System.Drawing.Point(81, 170);
            this.checkMyAccountInfoButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkMyAccountInfoButton.Name = "checkMyAccountInfoButton";
            this.checkMyAccountInfoButton.Size = new System.Drawing.Size(125, 29);
            this.checkMyAccountInfoButton.TabIndex = 11;
            this.checkMyAccountInfoButton.Text = "계좌정보확인";
            this.checkMyAccountInfoButton.UseVisualStyleBackColor = true;
            // 
            // myNameLabel
            // 
            this.myNameLabel.AutoSize = true;
            this.myNameLabel.Location = new System.Drawing.Point(111, 79);
            this.myNameLabel.Name = "myNameLabel";
            this.myNameLabel.Size = new System.Drawing.Size(52, 15);
            this.myNameLabel.TabIndex = 10;
            this.myNameLabel.Text = "아무개";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "예금주";
            // 
            // myDepositLabel
            // 
            this.myDepositLabel.AutoSize = true;
            this.myDepositLabel.Location = new System.Drawing.Point(111, 120);
            this.myDepositLabel.Name = "myDepositLabel";
            this.myDepositLabel.Size = new System.Drawing.Size(42, 15);
            this.myDepositLabel.TabIndex = 8;
            this.myDepositLabel.Text = "0(원)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 7;
            this.label2.Text = "예수금";
            // 
            // accountComboBox
            // 
            this.accountComboBox.FormattingEnabled = true;
            this.accountComboBox.Location = new System.Drawing.Point(113, 36);
            this.accountComboBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.accountComboBox.Name = "accountComboBox";
            this.accountComboBox.Size = new System.Drawing.Size(138, 23);
            this.accountComboBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "계좌번호";
            // 
            // axKHOpenAPI1
            // 
            this.axKHOpenAPI1.Enabled = true;
            this.axKHOpenAPI1.Location = new System.Drawing.Point(88, 442);
            this.axKHOpenAPI1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.axKHOpenAPI1.Name = "axKHOpenAPI1";
            this.axKHOpenAPI1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axKHOpenAPI1.OcxState")));
            this.axKHOpenAPI1.Size = new System.Drawing.Size(125, 63);
            this.axKHOpenAPI1.TabIndex = 3;
            // 
            // testTextBox
            // 
            this.testTextBox.Location = new System.Drawing.Point(375, 42);
            this.testTextBox.Multiline = true;
            this.testTextBox.Name = "testTextBox";
            this.testTextBox.ReadOnly = true;
            this.testTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.testTextBox.Size = new System.Drawing.Size(700, 563);
            this.testTextBox.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1087, 620);
            this.Controls.Add(this.testTextBox);
            this.Controls.Add(this.axKHOpenAPI1);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axKHOpenAPI1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox accountComboBox;
        private System.Windows.Forms.Label label1;
        private AxKHOpenAPILib.AxKHOpenAPI axKHOpenAPI1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label myNameLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label myDepositLabel;
        private System.Windows.Forms.Button checkMyAccountInfoButton;
        private System.Windows.Forms.Button checkMyHoldingsButton;
        private System.Windows.Forms.TextBox testTextBox;
    }
}

