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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.메뉴ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.testTextBox = new System.Windows.Forms.TextBox();
            this.marketTrueButton = new System.Windows.Forms.Button();
            this.sellButton = new System.Windows.Forms.Button();
            this.codeToSellTextBox = new System.Windows.Forms.TextBox();
            this.numToSellTextBox = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.setShuDownButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buyButton = new System.Windows.Forms.Button();
            this.buyCodeTextBox = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axKHOpenAPI1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.메뉴ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1087, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 메뉴ToolStripMenuItem
            // 
            this.메뉴ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loginToolStripMenuItem});
            this.메뉴ToolStripMenuItem.Name = "메뉴ToolStripMenuItem";
            this.메뉴ToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.메뉴ToolStripMenuItem.Text = "메뉴";
            // 
            // loginToolStripMenuItem
            // 
            this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            this.loginToolStripMenuItem.Size = new System.Drawing.Size(137, 26);
            this.loginToolStripMenuItem.Text = "로그인";
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
            this.groupBox1.Location = new System.Drawing.Point(7, 42);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBox1.Size = new System.Drawing.Size(362, 224);
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
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(7, 376);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(479, 229);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Size = new System.Drawing.Size(471, 200);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "보유종목";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Size = new System.Drawing.Size(471, 200);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "미체결";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(128, 127);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 36);
            this.button1.TabIndex = 6;
            this.button1.Text = "자동화";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(128, 27);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(98, 40);
            this.button2.TabIndex = 6;
            this.button2.Text = "체결로그";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // testTextBox
            // 
            this.testTextBox.Location = new System.Drawing.Point(488, 435);
            this.testTextBox.Multiline = true;
            this.testTextBox.Name = "testTextBox";
            this.testTextBox.Size = new System.Drawing.Size(577, 170);
            this.testTextBox.TabIndex = 9;
            // 
            // marketTrueButton
            // 
            this.marketTrueButton.Location = new System.Drawing.Point(38, 40);
            this.marketTrueButton.Name = "marketTrueButton";
            this.marketTrueButton.Size = new System.Drawing.Size(96, 40);
            this.marketTrueButton.TabIndex = 10;
            this.marketTrueButton.Text = "강제장시작";
            this.marketTrueButton.UseVisualStyleBackColor = true;
            // 
            // sellButton
            // 
            this.sellButton.Location = new System.Drawing.Point(236, 136);
            this.sellButton.Name = "sellButton";
            this.sellButton.Size = new System.Drawing.Size(96, 40);
            this.sellButton.TabIndex = 11;
            this.sellButton.Text = "판매";
            this.sellButton.UseVisualStyleBackColor = true;
            // 
            // codeToSellTextBox
            // 
            this.codeToSellTextBox.Location = new System.Drawing.Point(110, 127);
            this.codeToSellTextBox.Name = "codeToSellTextBox";
            this.codeToSellTextBox.Size = new System.Drawing.Size(100, 25);
            this.codeToSellTextBox.TabIndex = 13;
            // 
            // numToSellTextBox
            // 
            this.numToSellTextBox.Location = new System.Drawing.Point(110, 166);
            this.numToSellTextBox.Name = "numToSellTextBox";
            this.numToSellTextBox.Size = new System.Drawing.Size(100, 25);
            this.numToSellTextBox.TabIndex = 14;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buyCodeTextBox);
            this.groupBox2.Controls.Add(this.buyButton);
            this.groupBox2.Controls.Add(this.setShuDownButton);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.marketTrueButton);
            this.groupBox2.Controls.Add(this.sellButton);
            this.groupBox2.Controls.Add(this.numToSellTextBox);
            this.groupBox2.Controls.Add(this.codeToSellTextBox);
            this.groupBox2.Location = new System.Drawing.Point(389, 42);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(338, 352);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "테스트용(삭제예정)";
            // 
            // setShuDownButton
            // 
            this.setShuDownButton.Location = new System.Drawing.Point(191, 40);
            this.setShuDownButton.Name = "setShuDownButton";
            this.setShuDownButton.Size = new System.Drawing.Size(100, 40);
            this.setShuDownButton.TabIndex = 17;
            this.setShuDownButton.Text = "강제장마감";
            this.setShuDownButton.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 169);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 15);
            this.label5.TabIndex = 16;
            this.label5.Text = "매도수량";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 15);
            this.label3.TabIndex = 15;
            this.label3.Text = "매도종목코드";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Location = new System.Drawing.Point(751, 42);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(314, 224);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "미구현";
            // 
            // buyButton
            // 
            this.buyButton.Location = new System.Drawing.Point(212, 245);
            this.buyButton.Name = "buyButton";
            this.buyButton.Size = new System.Drawing.Size(96, 52);
            this.buyButton.TabIndex = 18;
            this.buyButton.Text = "매수버튼";
            this.buyButton.UseVisualStyleBackColor = true;
            // 
            // buyCodeTextBox
            // 
            this.buyCodeTextBox.Location = new System.Drawing.Point(38, 245);
            this.buyCodeTextBox.Name = "buyCodeTextBox";
            this.buyCodeTextBox.Size = new System.Drawing.Size(100, 25);
            this.buyCodeTextBox.TabIndex = 19;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1087, 620);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.testTextBox);
            this.Controls.Add(this.axKHOpenAPI1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axKHOpenAPI1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 메뉴ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loginToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox accountComboBox;
        private System.Windows.Forms.Label label1;
        private AxKHOpenAPILib.AxKHOpenAPI axKHOpenAPI1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label myNameLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label myDepositLabel;
        private System.Windows.Forms.Button checkMyAccountInfoButton;
        private System.Windows.Forms.Button checkMyHoldingsButton;
        private System.Windows.Forms.TextBox testTextBox;
        private System.Windows.Forms.Button marketTrueButton;
        private System.Windows.Forms.Button sellButton;
        private System.Windows.Forms.TextBox codeToSellTextBox;
        private System.Windows.Forms.TextBox numToSellTextBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button setShuDownButton;
        private System.Windows.Forms.Button buyButton;
        private System.Windows.Forms.TextBox buyCodeTextBox;
    }
}

